using JobScheduling.Domain.Groups.Models;
using LM.Domain.Entities;
using LM.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobScheduling.Domain.Jobs.Models
{
    public class JobGroup : Entity
    {
        [Obsolete(ConstructorObsoleteMessage, true)]
        JobGroup() { }
        public JobGroup(Group group) 
            : base(Guid.NewGuid())
        {
            Group = group;
            GroupId = group.Id;
        }

        public long GroupId { get; private set; }

        public Group Group { get; private set; }


        public static implicit operator JobGroup(Maybe<JobGroup> maybe) => maybe.Value;

        public static implicit operator JobGroup(Response<JobGroup> response) => response.Data.Value;
    }
}