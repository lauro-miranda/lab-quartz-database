using JobScheduling.Domain.Groups.Mappers;
using JobScheduling.Domain.Groups.Models;
using JobScheduling.Domain.Groups.Repositories.Contracts;
using JobScheduling.Domain.Groups.Services.Contracts;
using JobScheduling.Messages.Requests;
using JobScheduling.Messages.Responses;
using LM.Domain.UnitOfWork;
using LM.Responses;
using LM.Responses.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobScheduling.Domain.Groups.Services
{
    public class GroupService : IGroupService
    {
        IGroupRepository GroupRepository { get; }

        IUnitOfWork Uow { get; }

        public GroupService(IGroupRepository groupRepository, IUnitOfWork uow)
        {
            GroupRepository = groupRepository;
            Uow = uow;
        }

        public async Task<Response<List<GroupResponseMessage>>> GetAllAsync()
        {
            var groups = await GroupRepository.GetAllAsync();

            return groups.ToGroupResponseMessage();
        }

        public async Task<Response<GroupResponseMessage>> GetAsync(Guid code)
        {
            var response = Response<GroupResponseMessage>.Create();

            var group = await GroupRepository.FindAsync(code);

            if (!group.HasValue)
                return response.WithBusinessError(nameof(code)
                    , $"Não foi possível encontrar o grupo com o código '{code}'.");

            return group.Value.ToGroupResponseMessage();
        }

        public async Task<Response<GroupResponseMessage>> CreateAsync(GroupRequestMessage message)
        {
            var response = Response<GroupResponseMessage>.Create();

            if (await GroupRepository.AnyAsync(message.Name))
                return response.WithBusinessError("Já existe um grupo com esse nome.");

            var group = Group.Create(message);

            if (group.HasError) return response.WithMessages(group.Messages);

            await GroupRepository.AddAsync(group);

            if (!await Uow.CommitAsync())
                return response.WithCriticalError("Falha ao tentar salvar o grupo.");

            return group.Data.Value.ToGroupResponseMessage();
        }

        public async Task<Response<GroupResponseMessage>> UpdateAsync(Guid code, GroupRequestMessage message)
        {
            var response = Response<GroupResponseMessage>.Create();

            if (await GroupRepository.AnyAsync(code, message.Name))
                return response.WithBusinessError("Já existe um grupo com esse nome.");

            var group = await GroupRepository.FindAsync(code);

            if (!group.HasValue)
                return response.WithBusinessError(nameof(code)
                    , $"Não foi possível encontrar o grupo com o código '{code}'.");

            if (response.WithMessages(group.Value.Update(message).Messages).HasError)
                return response;

            await GroupRepository.UpdateAsync(group);

            if (!await Uow.CommitAsync())
                return response.WithCriticalError("Falha ao tentar atualizar o grupo.");

            return group.Value.ToGroupResponseMessage();
        }

        public async Task<Response> DeleteAsync(Guid code)
        {
            var response = Response<GroupResponseMessage>.Create();

            var group = await GroupRepository.FindAsync(code);

            if (!group.HasValue)
                return response.WithBusinessError(nameof(code)
                    , $"Não foi possível encontrar o grupo com o código '{code}'.");

            group.Value.Delete();

            await GroupRepository.UpdateAsync(group);

            if (!await Uow.CommitAsync())
                return response.WithCriticalError("Falha ao tentar remover o grupo.");

            return response;
        }
    }
}