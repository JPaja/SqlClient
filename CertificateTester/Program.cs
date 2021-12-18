using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.SqlClient;

namespace CertificateTester
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionStringPKCS12PFX();
            ConnectionStringPKCS7CerPVK();
            ConnectionStringPKCS7CrtPVK();
            ConnectionStringThumbprint();
            ConnectionStringSubjectName();
            ExternalPem();
        }



        public static void TestConnectionString(string connectionString, X509Certificate2 certificate = null, [CallerMemberName]string name = "")
        {
            Console.WriteLine();
            Console.WriteLine(name);
            try
            {
                var csb = new SqlConnectionStringBuilder(connectionString);
                using var conn = new SqlConnection(csb.ConnectionString, certificate);
                using var cmd = new SqlCommand("SELECT 1;", conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                if ((int)result == 1)
                    Console.WriteLine("OK");
                else
                    Console.WriteLine("Error");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Crash");
                Console.WriteLine(ex.Message);
                //Console.WriteLine(ex.StackTrace);
            }
        }

        public const string ConnectionBase = "Server=tcp:MDCSSQL-JOVPAV2,1453;Database=master";
 
        public static void ConnectionStringPKCS12PFX()
        {
            var certPath = @"C:\Users\a-jpavlovic\Desktop\Demo\SNI1.pfx";
            var password = "Yukon900";
            var connection = $"{ConnectionBase};ClientCertificate=file:{certPath};ClientKeyPassword={password};Authentication=SqlCertificate";
            TestConnectionString(connection);
        }

        public static void ConnectionStringPKCS7CerPVK()
        {
            var certPath = @"C:\Users\a-jpavlovic\Desktop\Demo\SNI1.cer";
            var keyPath = @"C:\Users\a-jpavlovic\Desktop\Demo\SNI1.pvk";
            var password = "Yukon900";
            var connection = $"{ConnectionBase};ClientCertificate=file:{certPath};ClientKey={keyPath};ClientKeyPassword={password};Authentication=SqlCertificate";
            TestConnectionString(connection);
        }

        public static void ConnectionStringPKCS7CrtPVK()
        {
            var certPath = @"C:\Users\a-jpavlovic\Desktop\Demo\SNI1.crt";
            var keyPath = @"C:\Users\a-jpavlovic\Desktop\Demo\SNI1.pvk";
            var password = "Yukon900";
            var connection = $"{ConnectionBase};ClientCertificate=file:{certPath};ClientKey={keyPath};ClientKeyPassword={password};Authentication=SqlCertificate";
            TestConnectionString(connection);
        }

        public static void ConnectionStringThumbprint()
        {
            var certThumbprint = @"47c74ec00cdf109ccd58d431ea95ada34b514b55";
            var connection = $"{ConnectionBase};ClientCertificate=sha1:{certThumbprint};Authentication=SqlCertificate";
            TestConnectionString(connection);
        }

        public static void ConnectionStringSubjectName()
        {
            var certSubjectName = @"service-broker-a";
            var connection = $"{ConnectionBase};ClientCertificate=subject:{certSubjectName};Authentication=SqlCertificate";
            TestConnectionString(connection);
        }

        public static void ExternalPem()
        {
            var certPath = @"C:\Users\a-jpavlovic\Desktop\Demo\SNI1Cert.pem";
            var keyPath = @"C:\Users\a-jpavlovic\Desktop\Demo\SNI1Key.pem";
            var password = "Yukon900";
            var connection = ConnectionBase;
            var cert = X509Certificate2.CreateFromEncryptedPemFile(certPath, password, keyPath);
            TestConnectionString(connection, cert);
        }
    }
}
