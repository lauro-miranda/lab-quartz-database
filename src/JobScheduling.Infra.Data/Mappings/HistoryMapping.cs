using JobScheduling.Domain.Histories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScheduling.Infra.Data.Mappings
{
    public class HistoryMapping : IEntityTypeConfiguration<History>
    {
        public void Configure(EntityTypeBuilder<History> builder)
        {
            builder.ToTable(nameof(History));

            builder.Property(x => x.Url).HasMaxLength(256);

            builder.HasOne(x => x.HistoryJob).WithOne().HasForeignKey<History>(x => x.HistoryJobId);
        }
    }
}