#if !NETSTANDARD2_0
using Microsoft.Data.SqlClient.Criptography.PVK;
using Microsoft.Data.SqlClient.Criptography.PEM;
#endif
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Security.Cryptography;

namespace Microsoft.Data.SqlClient.Criptography
{
    /// <summary>
    /// 
    /// </summary>
    public static class CertificateUtilis
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificatePath"></param>
        /// <param name="privateKeyPath"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static X509Certificate2 ParseWithPrivateKey(string certificatePath, string privateKeyPath, string password = null)
        {
            try
            {
                return new X509Certificate2(certificatePath, password);
            }
            catch { }
#if !NETSTANDARD2_0
            try
            {
                var certificate = new X509Certificate2(certificatePath);
                return certificate.ImportPrivateKey(privateKeyPath, password);
            }
            catch
            {
                
            }
#endif
            return null;
        }
#if !NETSTANDARD2_0
        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="privateKeyPath"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static X509Certificate2 ImportPrivateKey(this X509Certificate2 certificate, string privateKeyPath, string password = null)
        {
            if (privateKeyPath is null || certificate.HasPrivateKey)
                return certificate;
            //certificate = certificate.ImportPEMPrivateKey(privateKeyPath, password); when PEM compatibility is fixed
            return certificate.HasPrivateKey
                ? certificate
                : certificate.ImportPvkPrivateKey(privateKeyPath, password);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="privateKeyPath"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static X509Certificate2 ImportPvkPrivateKey(this X509Certificate2 certificate, string privateKeyPath, string password = null)
        {
            if (privateKeyPath is null || certificate.HasPrivateKey)
                return certificate;
            try
            {
                byte[] privateKeyData = File.ReadAllBytes(privateKeyPath);
                byte[] passwordData = string.IsNullOrEmpty(password)
                    ? Array.Empty<byte>()
                    : Encoding.ASCII.GetBytes(password);
               if (PVKUtils.TryParse(privateKeyData, passwordData, out var privKey))
                    return certificate.CopyWithPrivateKey(privKey);
            }
            catch
            {
            }
            return certificate;
        }
        
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="privateKeyPath"></param>
        /// <param name="password2"></param>
        /// <returns></returns>
        public static X509Certificate2 ImportPemPrivateKey(this X509Certificate2 certificate, string privateKeyPath, string password2)
        {
            if (privateKeyPath is null || certificate.HasPrivateKey)
                return certificate;
            try
            {
                var privateKeyData = File.ReadAllText(privateKeyPath).AsSpan();
                var passwordData = password2.AsSpan();
                string keyAlgorithm = certificate.GetKeyAlgorithm();
                return certificate.GetKeyAlgorithm() switch
                {
                    "1.2.840.113549.1.1.1" => ExtractKeyFromEncryptedPem(privateKeyData, passwordData, RSA.Create, certificate.CopyWithPrivateKey),
                    "1.2.840.10040.4.1" => ExtractKeyFromEncryptedPem(privateKeyData, passwordData, DSA.Create, certificate.CopyWithPrivateKey),
                    "1.2.840.10045.2.1" => ExtractKeyFromEncryptedPem(privateKeyData, passwordData, ECDsa.Create, certificate.CopyWithPrivateKey),
                    _ => certificate
                };
            }
            catch
            {
                return certificate;
            }
        }

        private static X509Certificate2 ExtractKeyFromEncryptedPem<TAlg>(ReadOnlySpan<char> keyPem, ReadOnlySpan<char> password, Func<TAlg> factory, Func<TAlg, X509Certificate2> import) where TAlg : AsymmetricAlgorithm
        {
            foreach (PemEnumerator.Enumerator.PemFieldItem pemFieldItem in new PemEnumerator(keyPem))
            {
                pemFieldItem.Deconstruct(out var readOnlySpan, out var pemFields);
                ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
                readOnlySpan = readOnlySpan2;
                int length = readOnlySpan.Length;
                //Range range = pemFields.Label;
                int offset = pemFields.LabelStart + length;
                int length2 = pemFields.LabelEnd + length - offset;
                ReadOnlySpan<char> span = readOnlySpan.Slice(offset, length2);
                if (span.SequenceEqual("ENCRYPTED PRIVATE KEY"))
                {
                    TAlg talg = factory();
                    AsymmetricAlgorithm asymmetricAlgorithm = talg;
                    readOnlySpan = readOnlySpan2;
                    length2 = readOnlySpan.Length;
                    //range = pemFields.Location;
                    offset = pemFields.LocationStart + length2;
                    length = pemFields.LocationEnd + length2 - offset;

                    PemKeyImportHelpers.ImportEncryptedPem<char>(readOnlySpan.Slice(offset, length), password, new PemKeyImportHelpers.ImportEncryptedKeyAction<char>(asymmetricAlgorithm.ImportEncryptedPkcs8PrivateKey));
                    try
                    {
                        return import(talg);
                    }
                    catch (ArgumentException inner)
                    {
                        throw new CryptographicException("SR.Cryptography_X509_NoOrMismatchedPemKey", inner);
                    }
                }
            }
            throw new CryptographicException("SR.Cryptography_X509_NoOrMismatchedPemKey");
        }*/

#endif
    }
}
