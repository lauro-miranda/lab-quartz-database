using AutoFixture;
using FluentAssertions;
using JobScheduling.Domain.Groups.Models;
using JobScheduling.Domain.Histories.Models;
using JobScheduling.Domain.Jobs.Models;
using JobScheduling.Messages.Requests;
using LM.Domain.Entities.Extensions;
using LM.Responses;

namespace HubPagamento.Extrato.UnitTests._02___Domain.Histories.Models
{
    public class HistoryUnitTests
    {
        Job Job { get; }

        Fixture Fixture { get; }

        public HistoryUnitTests()
        {
            Fixture = new Fixture();

            var group = Group.Create(Fixture.Create<GroupRequestMessage>());
            group.Data.Value.SetId(1);

            Job = Job.Create(group, Fixture.Create<JobRequestMessage>());
        }

        [Fact]
        public void Create_ShouldReturnsSuccess()
        {
            var response = History.Create(Job);

            response.HasError.Should().BeFalse();
            response.Data.HasValue.Should().BeTrue();
            response.Data.Value.Url.Should().Be(Job.Url);
            response.Data.Value.Body.Should().Be(Job.Data);
            response.Data.Value.HistoryJob.Should().NotBeNull();
            response.Data.Value.HistoryJob.JobId.Should().Be(Job.Id);
        }

        [Fact]
        public void Create_WithoutJob_ShouldReturnsError()
        {
            var response = History.Create(Maybe<Job>.Create());

            response.HasError.Should().BeTrue();
            response.Messages.Should().Contain(x => x.Text.Equals("Job não informado."));
        }
    }
}