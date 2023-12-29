using JobScheduling.Domain.Groups.Models;
using JobScheduling.Messages.Requests;
using LM.Domain.Entities;
using LM.Responses;
using LM.Responses.Extensions;
using System;
using System.Linq;

namespace JobScheduling.Domain.Jobs.Models
{
    public class Job : Entity
    {
        [Obsolete(ConstructorObsoleteMessage, true)]
        Job() { }
        Job(string name
            , string cronExpression
            , string url
            , string? data
            , string? description
            , JobGroup jobGroup)
            : base(Guid.NewGuid())
        {
            Name = name;
            CronExpression = cronExpression;
            Url = url;
            Data = data;
            Description = description;
            UpdateGroup(jobGroup);
        }

        public string Name { get; private set; } = "";

        public string? Description { get; private set; } = "";

        public string CronExpression { get; private set; } = "0/30 * * ? * * *";

        public string Url { get; private set; }

        public Header Headers { get; private set; }

        public string? Data { get; private set; } = "";

        public long JobGroupId { get; private set; }

        public JobGroup JobGroup { get; private set; }

        public string KeyName => $"{Name}-{Code}";

        void UpdateGroup(JobGroup jobGroup)
        {
            JobGroup = jobGroup;
            JobGroupId = jobGroup.Id;
        }

        public Response Update(Group group, JobRequestMessage message)
        {
            if (group.Code != JobGroup.Group.Code)
                UpdateGroup(new JobGroup(group));

            return Update(message);
        }

        public Response Update(JobRequestMessage message)
        {
            var response = Response.Create();

            if (string.IsNullOrEmpty(message.Name))
                response.WithBusinessError(nameof(message.Name)
                    , "O nome do job não foi informado.");

            if (string.IsNullOrEmpty(message.CronExpression))
                response.WithBusinessError(nameof(message.CronExpression)
                    , "A expressão do job não foi informada.");

            if (string.IsNullOrEmpty(message.Url))
                response.WithBusinessError(nameof(message.Url)
                    , "A url do job não foi informada.");

            if (response.HasError) return response;

            Name = message.Name;
            CronExpression = message.CronExpression;
            Url = message.Url;
            Data = message.Data;
            Description = message.Description;

            if (message.Headers.Any())
            {
                Headers = Header.Create(message.Headers);
            }

            UpdateLastUpdatedDate();

            return response;
        }

        public static Response<Job> Create(Group group, JobRequestMessage message)
        {
            var response = Response<Job>.Create();

            if (group == null || group.Id <= 0)
                response.WithBusinessError($"O grupo informado não é válido.");

            if (string.IsNullOrEmpty(message.Name))
                response.WithBusinessError(nameof(message.Name)
                    , "O nome do job não foi informado.");

            if (string.IsNullOrEmpty(message.CronExpression))
                response.WithBusinessError(nameof(message.CronExpression)
                    , "A expressão do job não foi informada.");

            if (string.IsNullOrEmpty(message.Url))
                response.WithBusinessError(nameof(message.Url)
                    , "A url do job não foi informada.");

            if (response.HasError) return response;

            var job = new Job(message.Name
                , message.CronExpression
                , message.Url
                , message.Data
                , message.Description
                , new JobGroup(group));

            if (message.Headers.Any())
            {
                job.Headers = Header.Create(message.Headers);
            }

            return response.SetValue(job);
        }


        public static implicit operator Job(Maybe<Job> maybe) => maybe.Value;

        public static implicit operator Job(Response<Job> response) => response.Data.Value;
    }
}