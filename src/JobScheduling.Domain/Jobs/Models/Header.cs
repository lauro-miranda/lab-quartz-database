using JobScheduling.Messages.Requests;
using LM.Domain.Valuables;
using LM.Responses;
using LM.Responses.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace JobScheduling.Domain.Jobs.Models
{
    public class Header : ValueObject
    {
        Header() { }
        Header(string value) { Value = value; }

        public string Value { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public static Response<Header> Create(List<JobRequestMessage.HeaderMessage> messages)
        {
            var response = Response<Header>.Create();

            if (messages == null || !messages.Any())
                response.WithBusinessError(nameof(messages), "O header não foi informado.");

            if (response.HasError) return response;

            return response.SetValue(new Header(JsonConvert.SerializeObject(messages)));
        }

        public static implicit operator Header(Maybe<Header> entity) => entity.Value;

        public static implicit operator Header(Response<Header> entity) => entity.Data;
    }
}