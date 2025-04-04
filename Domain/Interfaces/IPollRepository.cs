using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IPollRepository
    {
        void CreatePoll(Poll poll);
        IEnumerable<Poll> GetPolls();
        void Vote(int pollId, int optionNumber);
    }
}

