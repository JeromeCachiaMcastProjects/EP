using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.ViewModels;

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
            return View();
        }

        [HttpPost]
        public IActionResult Create(PollCreateViewModel model)
        {
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
        public IActionResult Vote(int pollId, int option, [FromServices] IPollRepository repo)
        {
            repo.Vote(pollId, option);
            return RedirectToAction("Results", new { id = pollId });
        }

        public IActionResult Results(int id)
        {
            var poll = _pollRepository.GetPolls().FirstOrDefault(p => p.Id == id);
            if (poll == null) return NotFound();

            return View(poll);
        }


    }
}
