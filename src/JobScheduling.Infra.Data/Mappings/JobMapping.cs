using JobScheduling.Domain.Jobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScheduling.Infra.Data.Mappings
{
    public class JobMapping : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable(nameof(Job));

            builder.Property(x => x.Name).HasMaxLength(50);
            builder.Property(x => x.Url).HasMaxLength(256);
            builder.Property(x => x.CronExpression).HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(100);

            builder.Ignore(x => x.KeyName);

            builder.OwnsOne(x => x.Headers)
                .Property(x => x.Value)
                .HasColumnName("Header")
                .IsRequired(false);

            builder.HasOne(x => x.JobGroup).WithOne().HasForeignKey<Job>(x => x.JobGroupId);
        }
    }
}