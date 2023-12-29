using System;

namespace JobScheduling.Messages.Responses
{
    public class JobResponseMessage
    {
        public Guid Code { get; set; }

        public string Name { get; set; } = "";

        public string CronExpression { get; set; } = "";

        public string? Data { get; set; }

        public string? Description { get; set; }
    }
}