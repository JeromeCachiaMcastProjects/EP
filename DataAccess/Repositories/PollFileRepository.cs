using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace DataAccess.Repositories
{
    public class PollFileRepository : IPollRepository
    {
        private readonly string _filePath = "polls.json";

        public void CreatePoll(Poll poll)
        {
            var polls = GetPolls().ToList();
            poll.Id = polls.Count > 0 ? polls.Max(p => p.Id) + 1 : 1;
            poll.DateCreated = DateTime.UtcNow;

            polls.Add(poll);
            SavePolls(polls);
        }

        public IQueryable<Poll> GetPolls()
        {
            if (!File.Exists(_filePath))
                return new List<Poll>().AsQueryable();

            var json = File.ReadAllText(_filePath);
            return (JsonSerializer.Deserialize<List<Poll>>(json) ?? new List<Poll>()).AsQueryable();
        }

        public void Vote(int pollId, int optionNumber)
        {
            var polls = GetPolls().ToList();
            var poll = polls.FirstOrDefault(p => p.Id == pollId);
            if (poll == null) return;

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return;

            if (poll.VotedUserIds.Contains(userId)) return;

            switch (optionNumber)
            {
                case 1: poll.Option1VotesCount++; break;
                case 2: poll.Option2VotesCount++; break;
                case 3: poll.Option3VotesCount++; break;
            }
            poll.VotedUserIds.Add(userId);
            SavePolls(polls);
        }

        private void SavePolls(List<Poll> polls)
        {
            var json = JsonSerializer.Serialize(polls, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        private string GetCurrentUserId()
        {
            var httpContext = new HttpContextAccessor().HttpContext;
            return httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

    }
}

