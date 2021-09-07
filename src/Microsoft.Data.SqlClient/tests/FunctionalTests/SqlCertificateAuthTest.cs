// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Data.SqlClient.Tests
{
    public partial class SqlCertificateAuthTest
    {

        [Fact]
        public void LoginWithConnectionString()
        {
            var certPath = @"F:\Sources\Certificates\hybrid-cert-a-1.pfx";
            var key = "Yukon900";
            var csb = new SqlConnectionStringBuilder($"Server=tcp:MDCSSQL-JOVPAV2,1453;" +
                $"Database=master;" +
                $"ClientCertificate={certPath};" +
                $"ClientKeyPassword={key};" +
                $"Authentication=SqlCertificate");
            var conn = new SqlConnection(csb.ConnectionString);
            var cmd = new SqlCommand("SELECT 1;", conn);
            conn.Open();
            var result = cmd.ExecuteScalar();
            Assert.Equal(1, result);
        }
    }
}
