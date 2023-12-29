using JobScheduling.Domain.Jobs.Models;
using JobScheduling.Messages.Responses;
using System.Collections.Generic;
using System.Linq;

namespace JobScheduling.Domain.Jobs.Mappers
{
    public static class JobMapperExtensions
    {
        public static JobResponseMessage ToJobResponseMessage(this Job job)
        {
            if (job == null) return new JobResponseMessage();

            return new JobResponseMessage
            {
                Code = job.Code,
                Name = job.Name,
                CronExpression = job.CronExpression,
                Data = job.Data,
                Description = job.Description
            };
        }

        public static List<JobResponseMessage> ToJobResponseMessage(this List<Job> jobs)
        {
            if (jobs == null) return new List<JobResponseMessage>();

            return jobs.Select(j => j.ToJobResponseMessage()).ToList();
        }
    }
}