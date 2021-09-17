using System;
using System.Buffers;
using System.Security.Cryptography;

namespace Microsoft.Data.SqlClient.Criptography.PEM
{
	internal static class CryptoPool
	{
		public static byte[] Rent(int minimumLength)
		{
			return ArrayPool<byte>.Shared.Rent(minimumLength);
		}

		public static void Return(byte[] array, int clearSize = -1)
		{
			bool flag = clearSize < 0;
			if (!flag && clearSize != 0)
			{
				CryptographicOperations.ZeroMemory(array.AsSpan(0, clearSize));
			}
			ArrayPool<byte>.Shared.Return(array, flag);
		}
	}
}
