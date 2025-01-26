using Microsoft.AspNetCore.Mvc;
using WorkHive.Models;
using WorkHive.Models.Repositories;
using WorkHive.Models.ViewModels;

namespace WorkHive.Controllers
{
    public class JobController : Controller
    {
        private readonly JobRepository _jobRepository= new JobRepository();
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult PostJob()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "User");
            else
                return View();
          
        }
        [HttpPost]
        public IActionResult PostJob(Job job, string skills)
        {
            if (ModelState.IsValid)
            {
                try
                { 
                    if (string.IsNullOrEmpty(skills))
                    {
                        ViewBag.Error = "Please select at least one skill!";
                        return View(job);
                    }
                    job.client_id=HttpContext.Session.GetInt32("UserId");
                    // Split the comma-separated skills and add to the job's skillsRequired list
                    var skillList = skills.Split(',').Select(skill => new SkillRequired { skill_name = skill }).ToList();
                    job.skillsRequired = skillList;
                    _jobRepository.CreateJob(job);
                    
                    ViewBag.Success = "Job posted successfully!";
                    return RedirectToAction("ClientDashboard","User");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "An error occurred while posting the job: " + ex.Message;
                    return View(job);
                }
            }
            ViewBag.Error = "All fields are required!";
            return RedirectToAction("ClientDashboard", "User");
        }
       
        public IActionResult ViewJobs()
        {
            List<Job> jobs = new List<Job>();
            jobs=_jobRepository.GetAllJobs();
            return View(jobs);
        }
        [HttpGet]
        public IActionResult JobDetails(int id)
        {
            Job job1=_jobRepository.GetJobById(id);
            UserRepository userRepository = new UserRepository();
            int? userid = job1.client_id;
            User user1 = userRepository.GetUserById((int)userid);
            JobDetails model = new JobDetails()
            {
                job = job1,
                user = user1
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult SearchJobs(decimal? budget, List<string> skills)
        {
            var jobs = new List<Job>();
            if (skills != null && skills.Any())
            {
                jobs = _jobRepository.SearchJobsByAllOfSkills(skills);
            }
            else
            {
                jobs = _jobRepository.GetAllJobs();
            }

            if (budget.HasValue && budget > 0)
            {
                jobs = jobs.Where(job => job.budget <= budget.Value).ToList();
            }

            // Return the partial view instead of the full view
            return PartialView("_JobsList", jobs);
        }

        public IActionResult GetAllJobs()
        {
            var jobs = _jobRepository.GetAllJobs();
            return View(jobs);
        }

    }
}
