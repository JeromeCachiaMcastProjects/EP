﻿using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Presentation.ActionFilters
{
    public class OneVoteOnlyFilter : ActionFilterAttribute
    {
        private readonly IPollRepository _repo;

        public OneVoteOnlyFilter(IPollRepository repo)
        {
            _repo = repo;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new RedirectToPageResult("/Account/Login", null, "/Identity");
                return;
            }

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (context.ActionArguments.TryGetValue("pollId", out var pollIdObj) && pollIdObj is int pollId)
            {
                var poll = _repo.GetPolls().FirstOrDefault(p => p.Id == pollId);
                if (poll != null && poll.VotedUserIds.Contains(userId))
                {
                    context.HttpContext.Items["AlreadyVoted"] = true;
                    context.HttpContext.Items["PollMessage"] = "You have already voted in this poll.";
                    context.Result = new RedirectToActionResult("Index", "Poll", null);
                }
            }
        }
    }
}
