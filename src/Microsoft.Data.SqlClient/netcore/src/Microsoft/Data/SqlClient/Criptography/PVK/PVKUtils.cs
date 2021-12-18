using System;
using System.Buffers.Binary;
using System.Linq;
using System.Security.Cryptography;

namespace Microsoft.Data.SqlClient.Criptography.PVK
{
    internal static class PVKUtils
    {
        private static readonly byte[] RSA2_MAGIC = { 82, 83, 65, 50 };

        public static bool TryParse(Span<byte> privateKey, Span<byte> password, out RSA rsa)
        {
            rsa = null;
            //6 metadata integers of size 4
            if (privateKey.Length < 6 * 4)
                return false;
            var position = 0;
            var magic = BinaryPrimitives.ReadUInt32LittleEndian(privateKey.Slice(position));
            if (magic != 0xb0b5f11e)
                return false;
            //8 bytes reserved
            position += 12;
            var encrypted = BinaryPrimitives.ReadInt32LittleEndian(privateKey.Slice(position));
            position += 4;
            var saltLenght = BinaryPrimitives.ReadInt32LittleEndian(privateKey.Slice(position));
            position += 4;
            var keyLenght = BinaryPrimitives.ReadInt32LittleEndian(privateKey.Slice(position));
            position += 4;
            if (privateKey.Length - position < saltLenght + keyLenght)
                return false;
            var salt = privateKey.Slice(position, saltLenght);

            //We are skipping first 8 bytes of key for some reason same as in jdbc https://github.com/microsoft/mssql-jdbc/blob/89bb744675941113a0ca9bcd7dc3c8f1310bb23c/src/main/java/com/microsoft/sqlserver/jdbc/SQLServerCertificateUtils.java#L174
            position += saltLenght + 8;
            var key = privateKey.Slice(position, keyLenght - 8);
            Span<byte> decoded = stackalloc byte[key.Length];
            key.CopyTo(decoded);
            if (encrypted != 0)
            {
                using var sha1 = SHA1.Create();
                Span<byte> hashData = stackalloc byte[salt.Length + password.Length];
                salt.CopyTo(hashData);
                password.CopyTo(hashData.Slice(salt.Length));
                Span<byte> hash = stackalloc byte[20];
                if (!sha1.TryComputeHash(hashData, hash, out int hashLenght))
                    return false;
                if (hashLenght < 16)
                    return false;
                hash = hash.Slice(0,16);
                RC4Transform(hash, ref decoded);
                if (!decoded.Slice(0, RSA2_MAGIC.Length).SequenceEqual(RSA2_MAGIC))
                {
                    for (int i = 0; i < 16 - 5; i++)
                        hash[i] = 0;
                    key.CopyTo(decoded);
                    RC4Transform(hash, ref decoded);
                }
            }

            if (!decoded.Slice(0,RSA2_MAGIC.Length).SequenceEqual(RSA2_MAGIC))
                return false;
            decoded = decoded.Slice(RSA2_MAGIC.Length);
           
            rsa = ParseRSAPrivateKey(decoded);
            if (rsa == null)
                return false;
            return true;
        }

        private static RSA ParseRSAPrivateKey(Span<byte> decoded)
        {
            //TODO: Use span to store as ASN1 format so we can use import private key from span<byte> instead of generating
            //RSAParameters and allocating 8 arrays as object

            if (decoded.Length < 4)
                return null;
            var byteLenght = BinaryPrimitives.ReadInt32LittleEndian(decoded) / 8;
            //We should leave 4 * (byteLenght / 2) since it would be wrong if byteLenght is odd number
            if (decoded.Slice(4).Length < 4 + 2 * byteLenght + (4 * (byteLenght / 2)))
                return null;
            var rsaParams = new RSAParameters();
            int position = 4;
            var exponent = decoded.Slice(position, 4);
            exponent.Reverse();
            rsaParams.Exponent = exponent.ToArray();
            position += 4;
            var modulus = decoded.Slice(position, byteLenght);
            modulus.Reverse();
            rsaParams.Modulus = modulus.ToArray();
            position += byteLenght;
            var p = decoded.Slice(position, byteLenght /2);
            p.Reverse();
            rsaParams.P = p.ToArray();
            position += byteLenght / 2;
            var q = decoded.Slice(position, byteLenght / 2);
            q.Reverse();
            rsaParams.Q = q.ToArray();
            position += byteLenght / 2;
            var dp = decoded.Slice(position, byteLenght / 2);
            dp.Reverse();
            rsaParams.DP = dp.ToArray();
            position += byteLenght / 2;
            var dq = decoded.Slice(position, byteLenght / 2);
            dq.Reverse();
            rsaParams.DQ = dq.ToArray();
            position += byteLenght / 2;
            var inverseQ = decoded.Slice(position, byteLenght / 2);
            inverseQ.Reverse();
            rsaParams.InverseQ = inverseQ.ToArray();
            position += byteLenght / 2;
            var d = decoded.Slice(position, byteLenght);
            d.Reverse();
            rsaParams.D = d.ToArray();
            try
            {
                return RSA.Create(rsaParams);
            }catch
            {
                return null;
            }
        }

        private static void RC4Transform(ReadOnlySpan<byte> key, ref Span<byte> data)
        {
            Span<byte> s = stackalloc byte[256];
            int i;
            int j;
            for (i = 0; i < s.Length; i++)
                s[i] = (byte)i;

            for (i = 0, j = 0; i < 256; i++)
            {
                j = (j + key[i % key.Length] + s[i]) & 255;
                Swap(ref s, i, j);
            }

            i = 0;
            j = 0;

            for (int k = 0; k < data.Length; k++)
            {
                i = (i + 1) & 255;
                j = (j + s[i]) & 255;

                Swap(ref s, i, j);
                data[k] = (byte)(data[k] ^ s[(s[i] + s[j]) & 255]);
            }
        }

        private static void Swap(ref Span<byte> s, int i, int j)
        {
            byte c = s[i];
            s[i] = s[j];
            s[j] = c;
        }
    }
}
