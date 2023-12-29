using AutoFixture;
using FluentAssertions;
using JobScheduling.Domain.Groups.Models;
using JobScheduling.Domain.Histories.Repositories.Contracts;
using JobScheduling.Domain.Histories.Services;
using JobScheduling.Domain.Jobs.Models;
using JobScheduling.ExternalServices.Dtos;
using JobScheduling.ExternalServices.Services.Contracts;
using JobScheduling.Messages.Requests;
using LM.Domain.Entities.Extensions;
using LM.Domain.UnitOfWork;
using LM.Responses;
using Moq;

namespace HubPagamento.Extrato.UnitTests._02___Domain.Histories.Services
{
    public class HistoryServiceUnitTests
    {
        Mock<IHistoryRepository> HistoryRepository { get; }

        Mock<IUnitOfWork> Uow { get; }

        Mock<IExternalService> ExternalService { get; }

        HistoryService HistoryService { get; }

        Fixture Fixture { get; }

        public HistoryServiceUnitTests()
        {
            HistoryRepository = new Mock<IHistoryRepository>();
            Uow = new Mock<IUnitOfWork>();
            ExternalService = new Mock<IExternalService>();

            Fixture = new Fixture();

            HistoryService = new HistoryService(HistoryRepository.Object
                , Uow.Object
                , ExternalService.Object);
        }

        [Fact]
        public async Task RunAsync_ShouldReturnsSuccess()
        {
            var group = Group.Create(Fixture.Create<GroupRequestMessage>());
            group.Data.Value.SetId(1);

            var job = Job.Create(group, Fixture.Create<JobRequestMessage>());

            ExternalService.Setup(x => x.SendAsync(It.IsAny<MessageDto>())).ReturnsAsync(() => Response.Create());
            Uow.Setup(x => x.CommitAsync()).ReturnsAsync(() => true);

            var response = await HistoryService.RunAsync(job);

            response.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task RunAsync_WithoutJob_ShouldReturnsError()
        {
            var response = await HistoryService.RunAsync(Maybe<Job>.Create());

            response.HasError.Should().BeTrue();
            response.Messages.Should().Contain(x => x.Text.Equals("O job não foi informado."));
        }
    }
}