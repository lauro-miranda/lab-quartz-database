using JobScheduling.Domain.Jobs.Models;
using LM.Domain.Entities;
using LM.Responses;
using LM.Responses.Extensions;
using System;

namespace JobScheduling.Domain.Histories.Models
{
    public class History : Entity
    {
        [Obsolete(ConstructorObsoleteMessage, true)]
        History() { }
        public History(string url
            , string body
            , HistoryJob historyJob)
            : base(Guid.NewGuid())
        {
            Url = url;
            Body = body;
            HistoryJob = historyJob;
            HistoryJobId = historyJob.Id;
        }

        public string Url { get; private set; } = "";

        public string Body { get; private set; } = "";

        public long HistoryJobId { get; private set; }

        public HistoryJob HistoryJob { get; private set; }

        public static Response<History> Create(Job job)
        {
            var response = Response<History>.Create();

            if (job == null) return response.WithBusinessError(nameof(job), "Job não informado.");

            if (string.IsNullOrEmpty(job.Url)) response.WithBusinessError(nameof(job.Url), "Url do Job não informada.");

            if (string.IsNullOrEmpty(job.Data)) response.WithBusinessError(nameof(job.Data), "Corpo do Job não informada.");

            if (response.HasError) return response;

            return response.SetValue(new History(job.Url
                , job.Data ?? ""
                , new HistoryJob(job)));
        }


        public static implicit operator History(Maybe<History> maybe) => maybe.Value;

        public static implicit operator History(Response<History> response) => response.Data.Value;
    }
}