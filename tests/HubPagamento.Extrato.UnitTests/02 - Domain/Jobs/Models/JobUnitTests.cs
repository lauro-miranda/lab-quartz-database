using AutoFixture;
using FluentAssertions;
using JobScheduling.Domain.Groups.Models;
using JobScheduling.Domain.Jobs.Models;
using JobScheduling.Messages.Requests;
using LM.Domain.Entities.Extensions;

namespace HubPagamento.Extrato.UnitTests._02___Domain.Jobs.Models
{
    public class JobUnitTests
    {
        Fixture Fixture { get; }

        public JobUnitTests()
        {
            Fixture = new Fixture();
        }

        [Fact]
        public void Create_ShouldReturnsSuccess()
        {
            var group = Group.Create(Fixture.Create<GroupRequestMessage>());
            group.Data.Value.SetId(1);

            var response = Job.Create(group, Fixture.Create<JobRequestMessage>());

            response.HasError.Should().BeFalse();
            response.Data.HasValue.Should().BeTrue();
        }

        [Fact]
        public void Create_WithInvalidFielsShouldReturnsError()
        {
            var group = Group.Create(Fixture.Create<GroupRequestMessage>());
            group.Data.Value.SetId(1);

            var response = Job.Create(group, new JobRequestMessage());

            response.HasError.Should().BeTrue();
            response.Data.HasValue.Should().BeFalse();
        }

        [Fact]
        public void Update_ShouldReturnsSuccess()
        {
            var group = Group.Create(Fixture.Create<GroupRequestMessage>());
            group.Data.Value.SetId(1);

            var job = Job.Create(group, Fixture.Create<JobRequestMessage>());

            var message = Fixture.Create<JobRequestMessage>();
            var response = job.Data.Value.Update(message);

            response.HasError.Should().BeFalse();

            job.Data.Value.Name.Should().Be(message.Name);
            job.Data.Value.Description.Should().Be(message.Description);
            job.Data.Value.CronExpression.Should().Be(message.CronExpression);
            job.Data.Value.Url.Should().Be(message.Url);
        }

        [Fact]
        public void Update_WithInvalidFielsShouldReturnsError()
        {
            var group = Group.Create(Fixture.Create<GroupRequestMessage>());
            group.Data.Value.SetId(1);

            var job = Job.Create(group, Fixture.Create<JobRequestMessage>());

            var response = job.Data.Value.Update(new JobRequestMessage());

            response.HasError.Should().BeTrue();
        }

        [Fact]
        public void UpdateGroup_ShouldReturnsSuccess()
        {
            var group = Group.Create(Fixture.Create<GroupRequestMessage>());
            group.Data.Value.SetId(1);

            var job = Job.Create(group, Fixture.Create<JobRequestMessage>());

            var message = Fixture.Create<JobRequestMessage>();
            var groupUpdate = Group.Create(Fixture.Create<GroupRequestMessage>());
            groupUpdate.Data.Value.SetId(2);
            var response = job.Data.Value.Update(groupUpdate, message);

            response.HasError.Should().BeFalse();

            job.Data.Value.Name.Should().Be(message.Name);
            job.Data.Value.Description.Should().Be(message.Description);
            job.Data.Value.CronExpression.Should().Be(message.CronExpression);
            job.Data.Value.Url.Should().Be(message.Url);
            job.Data.Value.JobGroup.GroupId.Should().Be(groupUpdate.Data.Value.Id);
        }

        [Fact]
        public void UpdateGroup_WithInvalidFielsShouldReturnsError()
        {
            var group = Group.Create(Fixture.Create<GroupRequestMessage>());
            group.Data.Value.SetId(1);

            var job = Job.Create(group, Fixture.Create<JobRequestMessage>());

            var response = job.Data.Value.Update(group, new JobRequestMessage());

            response.HasError.Should().BeTrue();
        }
    }
}