using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Data.SqlClient.Criptography
{
    /// <summary>
    /// Utility class for getting certificates from Key Store
    /// </summary>
    public static class CertStoreUtils
    {
        private static readonly StoreLocation[] Locations = new StoreLocation[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine };
        
        /// <summary>
        /// Get certificates by type of match and keyword
        /// </summary>
        /// <param name="findType"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static X509Certificate2[] GetCertificates(X509FindType findType, string keyword)
        {
            if (findType == X509FindType.FindByThumbprint)
                keyword = keyword.ToUpper(); //X509Certificates encodes thumbprints as uppercase hex
            var collection = new List<X509Certificate2>();
            foreach (var location in Locations)
            {
                try
                {
                    var result = GetCertificates(StoreName.My, location, findType, keyword);
                    collection.AddRange(result);
                }
                catch
                {
                }
            }

            return collection
                .DistinctAndPrioritizePrivateCertificates()
                .ToArray();
        }

        private static IEnumerable<X509Certificate2> GetCertificates(StoreName storeName, StoreLocation storeLocation, X509FindType findType , string keyword)
        {
            var store = new X509Store(storeName, storeLocation);
            try
            {
                store.Open(OpenFlags.ReadOnly);

                var certCollection = store.Certificates;
                var signingCert = certCollection.Find(findType, keyword, true); //TODO: Confirm if result can be null
                return signingCert.OfType<X509Certificate2>();
            }
            finally
            {
                store.Close();
            }
        }

        //TODO: Confirm that it behaves as intended
        private static IEnumerable<X509Certificate2> DistinctAndPrioritizePrivateCertificates(this IEnumerable<X509Certificate2> collection)
        {
            return collection
                .OrderByDescending(c => c.HasPrivateKey) // In case if we have certificate with private key and without with same thumbprint we want to preffer one with private key
                .Distinct(CertificateThumbprintEquatable.Instance);
        }

        private class CertificateThumbprintEquatable : IEqualityComparer<X509Certificate2>
        {
            public static CertificateThumbprintEquatable Instance = new CertificateThumbprintEquatable();
            public bool Equals(X509Certificate2 x, X509Certificate2 y)
            {
                return x.Thumbprint.SequenceEqual(y.Thumbprint);
            }

            public int GetHashCode(X509Certificate2 obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
