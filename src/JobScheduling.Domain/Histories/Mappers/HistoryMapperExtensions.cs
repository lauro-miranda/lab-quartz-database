using JobScheduling.Domain.Histories.Config;
using JobScheduling.Domain.Histories.Models;
using JobScheduling.Domain.Jobs.Models;
using JobScheduling.ExternalServices.Dtos;
using JobScheduling.Messages.Responses;
using LM.Domain.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobScheduling.Domain.Histories.Mappers
{
    public static class HistoryMapperExtensions
    {
        public static List<HistoryResponseMessage> ToHistoryResponseMessage(this List<History> histories)
        {
            if (histories == null) return new List<HistoryResponseMessage>();

            return histories.Select(h => h.ToHistoryResponseMessage()).ToList();
        }

        public static HistoryResponseMessage ToHistoryResponseMessage(this History history)
        {
            if (history == null) return new HistoryResponseMessage();

            return new HistoryResponseMessage
            {
                Code = history.Code,
                Url = history.Url,
                Date = history.CreatedAt,
                Body = history.Body,
                Job = history.HistoryJob == null || history.HistoryJob.Job == null ? new HistoryResponseMessage.JobResponseMessage()
                    : new HistoryResponseMessage.JobResponseMessage
                    {
                        Code = history.HistoryJob.Job.Code,
                        CronExpression = history.HistoryJob.Job.CronExpression,
                        Name = history.HistoryJob.Job.Name
                    }
            };
        }

        public static MessageDto ToMessageDto(this Job job)
        {
            if (job == null) return new MessageDto();

            var headers = job.Headers == null || string.IsNullOrEmpty(job.Headers.Value)
                ? new List<MessageDto.HeaderDto>()
                : JsonConvert.DeserializeObject<List<MessageDto.HeaderDto>>(job.Headers.Value);

            var dto = new MessageDto
            {
                Url = job.Url,
                Body = job.Data ?? "",
                Headers = headers ?? new List<MessageDto.HeaderDto>()
            };

            if (!string.IsNullOrEmpty(dto.Body))
            {
                var current = DateTimeHelper.GetCurrentDate();
                var start = new DateTime(current.Year, current.Month, current.Day, 0, 0, 0);
                var end = new DateTime(current.Year, current.Month, current.Day, 23, 59, 59);

                dto.Body = dto.Body
                    .Replace(ReplaceConfig.StartDate, start.ToString())
                    .Replace(ReplaceConfig.EndDate, end.ToString());
            }

            return dto;
        }
    }
}