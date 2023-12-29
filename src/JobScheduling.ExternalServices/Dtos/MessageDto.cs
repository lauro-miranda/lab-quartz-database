using System.Collections.Generic;

namespace JobScheduling.ExternalServices.Dtos
{
    public class MessageDto
    {
        public string Url { get; set; } = "";

        public List<HeaderDto> Headers { get; set; } = new List<HeaderDto>();

        public string Body { get; set; } = "";

        public class HeaderDto
        {
            public string Key { get; set; } = "";

            public string Value { get; set; } = "";
        }
    }
}