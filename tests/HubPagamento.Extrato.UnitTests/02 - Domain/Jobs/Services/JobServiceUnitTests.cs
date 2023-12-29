using AutoFixture;
using FluentAssertions;
using JobScheduling.Domain.Groups.Models;
using JobScheduling.Domain.Groups.Repositories.Contracts;
using JobScheduling.Domain.Jobs.Models;
using JobScheduling.Domain.Jobs.Repositories.Contracts;
using JobScheduling.Domain.Jobs.Services;
using JobScheduling.Messages.Requests;
using LM.Domain.Entities.Extensions;
using LM.Domain.UnitOfWork;
using Moq;

namespace HubPagamento.Extrato.UnitTests._02___Domain.Jobs.Services
{
    public class JobServiceUnitTests
    {
        Mock<IJobRepository> JobRepository { get; }

        Mock<IGroupRepository> GroupRepository { get; }

        Mock<IUnitOfWork> Uow { get; }

        JobService JobService { get; }

        Fixture Fixture { get; }

        public JobServiceUnitTests()
        {
            JobRepository = new Mock<IJobRepository>();
            GroupRepository = new Mock<IGroupRepository>();
            Uow = new Mock<IUnitOfWork>();

            Fixture = new Fixture();

            JobService = new JobService(JobRepository.Object
                , GroupRepository.Object
                , Uow.Object);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnsSuccess()
        {
            var group = Group.Create(Fixture.Create<GroupRequestMessage>());
            group.Data.Value.SetId(1);

            var job = Job.Create(group, Fixture.Create<JobRequestMessage>());

            JobRepository.Setup(x => x.FindAsNoTrackingAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => job.Data.Value);

            var response = await JobService.GetAsync(Guid.NewGuid());

            response.HasError.Should().BeFalse();
            response.Data.HasValue.Should().BeTrue();
        }
    }
}