using AutoFixture;
using FluentAssertions;
using JobScheduling.Domain.Groups.Models;
using JobScheduling.Domain.Groups.Repositories.Contracts;
using JobScheduling.Domain.Groups.Services;
using JobScheduling.Messages.Requests;
using LM.Domain.UnitOfWork;
using LM.Responses;
using Moq;

namespace HubPagamento.Extrato.UnitTests._02___Domain.Groups.Services
{
    public class GroupServiceUnitTests
    {
        Mock<IGroupRepository> GroupRepository { get; }

        Mock<IUnitOfWork> Uow { get; }

        GroupService GroupService { get; }

        GroupRequestMessage GroupRequestMessage { get; }

        Fixture Fixture { get; }

        public GroupServiceUnitTests()
        {
            GroupRepository = new Mock<IGroupRepository>();
            Uow = new Mock<IUnitOfWork>();

            Fixture = new Fixture();

            GroupRequestMessage = Fixture.Create<GroupRequestMessage>();

            GroupService = new GroupService(GroupRepository.Object
                , Uow.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnsSuccess()
        {
            GroupRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(() => Fixture.CreateMany<Group>().ToList());

            var response = await GroupService.GetAllAsync();

            response.HasError.Should().BeFalse();
            response.Data.Value.Should().HaveCountGreaterThan(1);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnsSuccess()
        {
            GroupRepository.Setup(x => x.FindAsync(It.IsAny<Guid>())).ReturnsAsync(() => Fixture.Create<Group>());

            var response = await GroupService.GetAsync(Guid.NewGuid());

            response.HasError.Should().BeFalse();
            response.Data.HasValue.Should().BeTrue();
        }

        [Fact]
        public async Task GetAsync_WithNotFound_ShouldReturnsError()
        {
            GroupRepository.Setup(x => x.FindAsync(It.IsAny<Guid>())).ReturnsAsync(() => Maybe<Group>.Create());

            var code = Guid.NewGuid();
            var response = await GroupService.GetAsync(code);

            response.HasError.Should().BeTrue();
            response.Data.HasValue.Should().BeFalse();
            response.Messages.Should().Contain(x => x.Text.Equals($"Não foi possível encontrar o grupo com o código '{code}'."));
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnsSuccess()
        {
            Uow.Setup(x => x.CommitAsync()).ReturnsAsync(() => true);

            var response = await GroupService.CreateAsync(GroupRequestMessage);

            response.HasError.Should().BeFalse();
            response.Data.HasValue.Should().BeTrue();
        }

        [Fact]
        public async Task CreateAsync_WithExistingGroup_ShouldReturnsError()
        {
            GroupRepository.Setup(x => x.AnyAsync(GroupRequestMessage.Name)).ReturnsAsync(() => true);

            var response = await GroupService.CreateAsync(GroupRequestMessage);

            response.HasError.Should().BeTrue();
            response.Data.HasValue.Should().BeFalse();
            response.Messages.Should().Contain(x => x.Text.Equals("Já existe um grupo com esse nome."));
        }

        [Fact]
        public async Task CreateAsync_WithInvalidFiels_ShouldReturnsError()
        {
            var response = await GroupService.CreateAsync(new GroupRequestMessage());

            response.HasError.Should().BeTrue();
            response.Data.HasValue.Should().BeFalse();
        }

        [Fact]
        public async Task CreateAsync_WithCommitAsyncError_ShouldReturnsError()
        {
            var response = await GroupService.CreateAsync(GroupRequestMessage);

            response.HasError.Should().BeTrue();
            response.Data.HasValue.Should().BeFalse();
            response.Messages.Should().Contain(x => x.Text.Equals("Falha ao tentar salvar o grupo."));
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnsSuccess()
        {
            var code = Guid.NewGuid();
            GroupRepository.Setup(x => x.FindAsync(code)).ReturnsAsync(() => Fixture.Create<Group>());
            Uow.Setup(x => x.CommitAsync()).ReturnsAsync(() => true);

            var response = await GroupService.UpdateAsync(code, GroupRequestMessage);

            response.HasError.Should().BeFalse();
            response.Data.HasValue.Should().BeTrue();

            response.Data.Value.Name.Should().Be(GroupRequestMessage.Name);
            response.Data.Value.Description.Should().Be(GroupRequestMessage.Description);
        }

        [Fact]
        public async Task UpdateAsync_WithWithExistingNameInOtherGroup_ShouldReturnsError()
        {
            var code = Guid.NewGuid();
            GroupRepository.Setup(x => x.AnyAsync(code, GroupRequestMessage.Name)).ReturnsAsync(() => true);

            var response = await GroupService.UpdateAsync(code, GroupRequestMessage);

            response.HasError.Should().BeTrue();
            response.Data.HasValue.Should().BeFalse();
            response.Messages.Should().Contain(x => x.Text.Equals("Já existe um grupo com esse nome."));
        }

        [Fact]
        public async Task UpdateAsync_WithNotFound_ShouldReturnsError()
        {
            var code = Guid.NewGuid();
            GroupRepository.Setup(x => x.FindAsync(code)).ReturnsAsync(() => Maybe<Group>.Create());

            var response = await GroupService.UpdateAsync(code, GroupRequestMessage);

            response.HasError.Should().BeTrue();
            response.Data.HasValue.Should().BeFalse();
            response.Messages.Should().Contain(x => x.Text.Equals($"Não foi possível encontrar o grupo com o código '{code}'."));
        }

        [Fact]
        public async Task UpdateAsync_WithCommitAsyncError_ShouldReturnsError()
        {
            var code = Guid.NewGuid();
            GroupRepository.Setup(x => x.FindAsync(code)).ReturnsAsync(() => Fixture.Create<Group>());

            var response = await GroupService.UpdateAsync(code, GroupRequestMessage);

            response.HasError.Should().BeTrue();
            response.Data.HasValue.Should().BeFalse();
            response.Messages.Should().Contain(x => x.Text.Equals("Falha ao tentar atualizar o grupo."));
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnsSuccess()
        {
            var code = Guid.NewGuid();
            GroupRepository.Setup(x => x.FindAsync(code)).ReturnsAsync(() => Fixture.Create<Group>());
            Uow.Setup(x => x.CommitAsync()).ReturnsAsync(() => true);

            var response = await GroupService.DeleteAsync(code);

            response.HasError.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_WithNotFound_ShouldReturnsSuccess()
        {
            var code = Guid.NewGuid();
            GroupRepository.Setup(x => x.FindAsync(code)).ReturnsAsync(() => Maybe<Group>.Create());

            var response = await GroupService.DeleteAsync(code);

            response.HasError.Should().BeTrue();
            response.Messages.Should().Contain(x => x.Text.Equals($"Não foi possível encontrar o grupo com o código '{code}'."));
        }

        [Fact]
        public async Task DeleteAsync_WithCommitAsyncError_ShouldReturnsSuccess()
        {
            var code = Guid.NewGuid();
            GroupRepository.Setup(x => x.FindAsync(code)).ReturnsAsync(() => Fixture.Create<Group>());

            var response = await GroupService.DeleteAsync(code);

            response.HasError.Should().BeTrue();
            response.Messages.Should().Contain(x => x.Text.Equals("Falha ao tentar remover o grupo."));
        }
    }
}