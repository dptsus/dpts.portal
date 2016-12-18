using DPTS.Domain.Entities;
using System.Data.Entity;

namespace DPTS.Data.Context
{
    public class DPTSDbContext : DbContext
    {
        public DPTSDbContext() : base("DPTS")
        {
        }
        public virtual DbSet<Doctor> Doctor { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
