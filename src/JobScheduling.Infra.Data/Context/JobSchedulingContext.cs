using JobScheduling.Infra.Data.Context.Extensions;
using LM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace JobScheduling.Infra.Data.Context
{
    public class JobSchedulingContext : DbContext
    {
        public JobSchedulingContext(DbContextOptions<JobSchedulingContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(Entity).IsAssignableFrom(type.ClrType) && (type.BaseType == null || !typeof(Entity).IsAssignableFrom(type.BaseType.ClrType)))
                    modelBuilder.SetSoftDeleteFilter(type.ClrType);
            }
        }

        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateSoftDeleteStatuses();

            return base.SaveChangesAsync(cancellationToken);
        }

        void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is not Entity entity)
                    continue;

                switch (entry.State)
                {
                    case EntityState.Deleted:
                        (entity).Delete();
                        entry.State = EntityState.Modified;
                        break;
                }
            }
        }
    }
}