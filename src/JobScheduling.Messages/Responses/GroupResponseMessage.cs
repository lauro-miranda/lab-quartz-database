using System;

namespace JobScheduling.Messages.Responses
{
    public class GroupResponseMessage
    {
        public Guid Code { get; set; }

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";
    }
}