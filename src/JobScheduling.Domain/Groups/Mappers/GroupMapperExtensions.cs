using JobScheduling.Domain.Groups.Models;
using JobScheduling.Messages.Responses;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace JobScheduling.Domain.Groups.Mappers
{
    [ExcludeFromCodeCoverage]
    public static class GroupMapperExtensions
    {
        public static GroupResponseMessage ToGroupResponseMessage(this Group group)
        {
            if (group == null) return new GroupResponseMessage();

            return new GroupResponseMessage
            {
                Code = group.Code,
                Name = group.Name,
                Description = group.Description
            };
        }

        public static List<GroupResponseMessage> ToGroupResponseMessage(this List<Group> groups)
        {
            if (groups == null) return new List<GroupResponseMessage>();

            return groups.Select(g => g.ToGroupResponseMessage()).ToList();
        }
    }
}