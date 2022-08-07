using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NpfRatexTest3.Models
{
    public class NpfRatexTest3Context:DbContext
    {
        public DbSet<TestData> TestDatas { get; set; }

        public string connectionString;

        public NpfRatexTest3Context(string connectionString)
        {
            this.connectionString = (connectionString is null) == false ? connectionString : "Data Source=TestData.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}
