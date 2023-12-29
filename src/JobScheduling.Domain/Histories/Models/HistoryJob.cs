using JobScheduling.Domain.Jobs.Models;
using LM.Domain.Entities;
using LM.Responses;
using System;

namespace JobScheduling.Domain.Histories.Models
{
    public class HistoryJob : Entity
    {
        [Obsolete(ConstructorObsoleteMessage, true)]
        HistoryJob() { }
        public HistoryJob(Job job) : base(Guid.NewGuid())
        {
            Job = job;
            JobId = job.Id;
        }

        public long JobId { get; private set; }

        public Job Job { get; private set; }


        public static implicit operator HistoryJob(Maybe<HistoryJob> maybe) => maybe.Value;

        public static implicit operator HistoryJob(Response<HistoryJob> response) => response.Data.Value;
    }
}