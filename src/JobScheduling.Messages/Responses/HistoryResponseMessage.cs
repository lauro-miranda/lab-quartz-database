using System;

namespace JobScheduling.Messages.Responses
{
    public class HistoryResponseMessage
    {
        public Guid Code { get; set; }

        public DateTime Date { get; set; }

        public string Url { get; set; } = "";

        public string Body { get; set; } = "";

        public JobResponseMessage Job { get; set; } = new JobResponseMessage();

        public class JobResponseMessage
        {
            public Guid Code { get; set; }

            public string Name { get; set; } = "";

            public string CronExpression { get; set; } = "0/30 * * ? * * *";
        }
    }
}