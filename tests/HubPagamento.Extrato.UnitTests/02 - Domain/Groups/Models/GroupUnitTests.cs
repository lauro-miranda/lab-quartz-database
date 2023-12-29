using AutoFixture;
using FluentAssertions;
using JobScheduling.Domain.Groups.Models;
using JobScheduling.Messages.Requests;

namespace HubPagamento.Extrato.UnitTests._02___Domain.Groups.Models
{
    public class GroupUnitTests
    {
        GroupRequestMessage GroupRequestMessage { get; }

        public GroupUnitTests()
        {
            var fixture = new Fixture();

            GroupRequestMessage = fixture.Create<GroupRequestMessage>();
        }

        [Fact]
        public void Create_ShouldReturnsSuccess()
        {
            var response = Group.Create(GroupRequestMessage);

            response.HasError.Should().BeFalse();
            response.Data.HasValue.Should().BeTrue();

            response.Data.Value.Name.Should().Be(GroupRequestMessage.Name);
            response.Data.Value.Description.Should().Be(GroupRequestMessage.Description);
        }

        [Fact]
        public void Create_WithInvalidFields_ShouldReturnsError()
        {
            var group = Group.Create(new GroupRequestMessage());

            group.HasError.Should().BeTrue();
            group.Data.HasValue.Should().BeFalse();

            group.Messages.Should().Contain(m => m.Text == "O nome não foi informado.");
            group.Messages.Should().Contain(m => m.Text == "A descrição não foi informada.");
        }

        [Fact]
        public void Update_ShouldReturnsSuccess()
        {
            var group = Group.Create(GroupRequestMessage);

            var name = "Nome";
            var description = "Descrição";
            var response = group.Data.Value.Update(new GroupRequestMessage 
            {
                Name = name,
                Description = description
            });

            response.HasError.Should().BeFalse();

            group.Data.Value.Name.Should().Be(name);
            group.Data.Value.Description.Should().Be(description);
        }

        [Fact]
        public void Update_WithInvalidFields_ShouldReturnsError()
        {
            var group = Group.Create(GroupRequestMessage);

            var response = group.Data.Value.Update(new GroupRequestMessage());

            response.HasError.Should().BeTrue();

            response.Messages.Should().Contain(m => m.Text == "O nome não foi informado.");
            response.Messages.Should().Contain(m => m.Text == "A descrição não foi informada.");
        }
    }
}