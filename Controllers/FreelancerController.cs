using Microsoft.AspNetCore.Mvc;
using WorkHive.Models.Repositories;
using WorkHive.Models;

namespace WorkHive.Controllers
{
    public class FreelancerController : Controller
    {
        private readonly FreelancerRepository _freelancerRepository= new FreelancerRepository();    
        public IActionResult Index()
        {
            return View();
        } 
        public IActionResult CreateFreelancer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateFreelancer(Freelancer model)
        {
            if (model.skills == null || model.skills.Count < 2 || model.skills.Count > 5)
            {
                ViewBag.Error = "Please add between 2 and 5 skills.";
                return View("CreateFreelancer", model);
            }

            foreach (var skill in model.skills)
            {
                skill.freelancer_id = model.FreelancerId; // Assign the freelancer ID to each skill
            }

            _freelancerRepository.CreateFreelancer(model);
            ViewBag.Success = "Freelancer profile created successfully!";
            return RedirectToAction("Index","Home");
        }


        [HttpGet]
        public IActionResult SearchFreelancers(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
               
                return View(new List<Freelancer>());
            }
           var freelancers = _freelancerRepository.GetFreelancersBySkill(searchQuery);

            return View(freelancers);
        }

    }
}
