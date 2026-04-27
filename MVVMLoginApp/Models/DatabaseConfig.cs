using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMLoginApp.Models
{
    public static class DatabaseConfig
    {
        // ── Change ONLY this when switching computers ──

        // For HOME PC:
        public static readonly string ConnectionString =
            @"Server=DESKTOP-4LDUQSS; Database=Mawlers Cinema;
              Integrated Security=True;
              TrustServerCertificate=True;";

        // For SCHOOL PC (comment out home, uncomment this):
        // public static readonly string ConnectionString =
        //     @"Server=CCL2-11\MSSQLSERVER01; Database=Mawlers Cinema;
        //       User Id=sa; Password=ccl2;
        //       TrustServerCertificate=True;";
    }
}
