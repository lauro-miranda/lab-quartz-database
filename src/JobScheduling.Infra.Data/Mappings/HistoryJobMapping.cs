using JobScheduling.Domain.Histories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScheduling.Infra.Data.Mappings
{
    public class HistoryJobMapping : IEntityTypeConfiguration<HistoryJob>
    {
        public void Configure(EntityTypeBuilder<HistoryJob> builder)
        {
            builder.ToTable(nameof(HistoryJob));

            builder.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId);
        }
    }
}