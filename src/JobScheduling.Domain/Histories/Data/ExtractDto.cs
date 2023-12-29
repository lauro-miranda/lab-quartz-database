using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace JobScheduling.Domain.Histories.Data
{
    [ExcludeFromCodeCoverage]
    public class ExtractDto
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