using JobScheduling.Messages.Requests;
using LM.Domain.Entities;
using LM.Responses;
using LM.Responses.Extensions;
using System;

namespace JobScheduling.Domain.Groups.Models
{
    public class Group : Entity
    {
        [Obsolete(ConstructorObsoleteMessage, true)]
        Group() { }

        public Group(string name
            , string description)
            : base(Guid.NewGuid())
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public Response Update(GroupRequestMessage message)
        {
            var response = Response.Create();

            if (string.IsNullOrEmpty(message.Name))
                response.WithBusinessError(nameof(message.Name), "O nome não foi informado.");

            if (string.IsNullOrEmpty(message.Description))
                response.WithBusinessError(nameof(message.Description), "A descrição não foi informada.");

            if (response.HasError) return response;

            Name = message.Name;
            Description = message.Description;
            UpdateLastUpdatedDate();

            return response;
        }

        public static Response<Group> Create(GroupRequestMessage message)
        {
            var response = Response<Group>.Create();

            if (string.IsNullOrEmpty(message.Name))
                response.WithBusinessError(nameof(message.Name), "O nome não foi informado.");

            if (string.IsNullOrEmpty(message.Description))
                response.WithBusinessError(nameof(message.Description), "A descrição não foi informada.");

            if (response.HasError) return response;

            return response.SetValue(new Group(message.Name, message.Description));
        }

        public static implicit operator Group(Maybe<Group> maybe) => maybe.Value;

        public static implicit operator Group(Response<Group> response) => response.Data.Value;
    }
}