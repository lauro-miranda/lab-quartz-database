using JobScheduling.Domain.Jobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScheduling.Infra.Data.Mappings
{
    public class JobGroupMapping : IEntityTypeConfiguration<JobGroup>
    {
        public void Configure(EntityTypeBuilder<JobGroup> builder)
        {
            builder.ToTable(nameof(JobGroup));

            builder.HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId);
        }
    }
}