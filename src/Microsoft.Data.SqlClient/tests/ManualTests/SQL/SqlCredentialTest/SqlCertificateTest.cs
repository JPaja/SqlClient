// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Security;
using System.Threading;
using Xunit;

namespace Microsoft.Data.SqlClient.ManualTesting.Tests
{
    public static class SqlCertificateTest
    {

        [Fact]
        public static void CreateSqlConnectionWithClientCertificateStringPFX()
        {
            try
            {
                var csb = new SqlConnectionStringBuilder("Server=tcp:MDCSSQL-JOVPAV2, 1453;Database=master;User Id=sa;password=Yukon900;"); //Trusted_Connection=False;MultipleActiveResultSets=true;
                using (var conn = new SqlConnection(csb.ConnectionString))
                using (var cmd = new SqlCommand("SELECT 1;", conn))
                {
                    conn.Open();
                    Assert.Equal(1, cmd.ExecuteScalar());
                }
            }
            finally
            {
            }
        }
    }
}
