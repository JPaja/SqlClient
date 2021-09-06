using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.SqlClient;

namespace CertificateTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateSqlConnectionWithClientCertificatePem();
            CreateSqlConnectionWithClientCertificatePFX();
            CreateSqlConnectionWithClientCertificatePFXConnectionString();
        }

       /* public static void CreateSqlConnectionWithClientCertificatePem()
        {
            Console.WriteLine();
            Console.WriteLine(nameof(CreateSqlConnectionWithClientCertificatePem));
            try
            {
                var cert = X509Certificate2.CreateFromEncryptedPemFile("cert.pem", "testpass123", "key.pem");
                var csb = new SqlConnectionStringBuilder("Server=tcp:MDCSSQL-JOVPAV2,1453;Database=master"); //Trusted_Connection=False;MultipleActiveResultSets=true;
                //csb.Authentication = SqlAuthenticationMethod.SqlCertificate;
                using (var conn = new SqlConnection(csb.ConnectionString, cert))
                using (var cmd = new SqlCommand("SELECT 1;", conn))
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    if ((int)result == 1)
                    {
                        Console.WriteLine("OK");
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Crash");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
*/
        public static void CreateSqlConnectionWithClientCertificatePFXConnectionString()
        {
            Console.WriteLine();
            Console.WriteLine(nameof(CreateSqlConnectionWithClientCertificatePFXConnectionString));
            try
            {
                var certPath = @"F:\Sources\SqlClient-jovan\CertificateTester\bin\Debug\net5.0\hybrid-cert-a-1.pfx";
                var key = "Yukon900";
                var csb = new SqlConnectionStringBuilder($"Server=tcp:MDCSSQL-JOVPAV2,1453;Database=master;ClientCertificate={certPath};ClientKeyPassword={key};Authentication=SqlCertificate"); 
                using (var conn = new SqlConnection(csb.ConnectionString))
                using (var cmd = new SqlCommand("SELECT 1;", conn))
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    if ((int)result == 1)
                    {
                        Console.WriteLine("OK");
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Crash");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static void CreateSqlConnectionWithClientCertificatePFX()
        {
            Console.WriteLine();
            Console.WriteLine(nameof(CreateSqlConnectionWithClientCertificatePFX));
            try
            {
                var certPath = @"F:\Sources\SqlClient-jovan\CertificateTester\bin\Debug\net5.0\hybrid-cert-a-1.pfx";
                var key = "Yukon900";
                var cert = new X509Certificate2(certPath, key);
                var csb = new SqlConnectionStringBuilder($"Server=tcp:MDCSSQL-JOVPAV2,1453;Database=master;");
                using (var conn = new SqlConnection(csb.ConnectionString, cert))
                using (var cmd = new SqlCommand("SELECT 1;", conn))
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    if ((int)result == 1)
                    {
                        Console.WriteLine("OK");
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Crash");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
