using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;
using DataAccess.DataContext;

namespace DataAccess.Repositories
{
    public class PollRepository : IPollRepository
    {
        private readonly PollDbContext _context;

        // Constructor Injection
        public PollRepository(PollDbContext context)
        {
            _context = context;
        }

        public void CreatePoll(Poll poll)
        {
            _context.Polls.Add(poll);
            _context.SaveChanges();
        }

        public IEnumerable<Poll> GetPolls()
        {
            return _context.Polls.OrderByDescending(p => p.DateCreated).ToList();
        }

        public void Vote(int pollId, int optionNumber)
        {
            var poll = _context.Polls.Find(pollId);
            if (poll == null) return;

            switch (optionNumber)
            {
                case 1: poll.Option1VotesCount++; break;
                case 2: poll.Option2VotesCount++; break;
                case 3: poll.Option3VotesCount++; break;
            }

            _context.SaveChanges();

            //UploadToCloud(poll);
        }

        //private void UploadToCloud(Poll poll)
        //{
        //    Console.WriteLine($"upload: {poll.Title}");
        //}
    }
}

