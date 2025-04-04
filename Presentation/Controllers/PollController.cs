using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Presentation.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    public class PollController : Controller
    {
        private readonly IPollRepository _pollRepository;

        // Constructor Injection
        public PollController(IPollRepository pollRepository)
        {
            _pollRepository = pollRepository;
        }

        public IActionResult Index()
        {
            var polls = _pollRepository.GetPolls();
            return View(polls);
        }

        public IActionResult Create()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                TempData["Message"] = "You must be logged in for this functionality.";
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(PollCreateViewModel model)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                TempData["Message"] = "You must be logged in for this functionality.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
                return View(model);

            var poll = new Poll
            {
                Title = model.Title,
                Option1Text = model.Option1Text,
                Option2Text = model.Option2Text,
                Option3Text = model.Option3Text,
                DateCreated = DateTime.UtcNow
            };

            _pollRepository.CreatePoll(poll);
            return RedirectToAction("Index");
        }

        public IActionResult Vote(int id)
        {
            var poll = _pollRepository.GetPolls().FirstOrDefault(p => p.Id == id);
            if (poll == null) return NotFound();

            return View(poll);
        }

        [HttpPost]
        [ServiceFilter(typeof(OneVoteOnlyFilter))]
        public IActionResult Vote(int pollId, int option, [FromServices] IPollRepository repo)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                TempData["Message"] = "You must be logged in for this functionality.";
                return RedirectToAction("Index");
            }

            if (HttpContext.Items["AlreadyVoted"] is true && HttpContext.Items["PollMessage"] is string msg)
            {
                TempData["Message"] = msg;
                return RedirectToAction("Index");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var poll = repo.GetPolls().FirstOrDefault(p => p.Id == pollId);

            if (poll == null) return NotFound();

            poll.VotedUserIds.Add(userId); // Mark user as voted
            repo.Vote(pollId, option);

            return RedirectToAction("Results", new { id = pollId });
        }


        public IActionResult Results(int id)
        {
            var poll = _pollRepository.GetPolls().FirstOrDefault(p => p.Id == id);
            if (poll == null) return NotFound();

            return View(poll);
        }
        public IActionResult AllResults()
        {
            var polls = _pollRepository.GetPolls();
            return View(polls);
        }

    }
}
