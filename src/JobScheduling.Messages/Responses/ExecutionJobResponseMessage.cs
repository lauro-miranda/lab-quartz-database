using System;
using System.Collections.Generic;

namespace JobScheduling.Messages.Responses
{
    public class ExecutionJobResponseMessage
    {
        public Guid Code { get; set; }

        public string Name { get; set; } = "";

        public List<JobResponseMessage> Jobs { get; set; } = new List<JobResponseMessage>();

        public class JobResponseMessage
        {
            public Guid Code { get; set; }

            public string Name { get; set; } = "";

            public string CronExpression { get; set; } = "";

            public string? Data { get; set; }

            public string? Description { get; set; }

            public DateTimeOffset? NextValidTimeAfter { get; set; }
        }
    }
}