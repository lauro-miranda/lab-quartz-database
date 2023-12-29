using System;
using System.Collections.Generic;

namespace JobScheduling.Messages.Requests
{
    public class JobRequestMessage
    {
        public Guid GroupCode { get; set; }

        public string Name { get; set; } = "";

        public string CronExpression { get; set; } = "";

        public string Url { get; set; } = "";

        public string? Data { get; set; }

        public string? Description { get; set; }

        public List<HeaderMessage> Headers { get; set; } = new List<HeaderMessage>();

        public class HeaderMessage
        {
            public string Key { get; set; } = "";

            public string Value { get; set; } = "";
        }
    }
}