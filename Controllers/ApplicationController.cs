using Microsoft.AspNetCore.Mvc;
using WorkHive.Models;
using WorkHive.Models.Repositories;
using WorkHive.Models.ViewModels;

namespace WorkHive.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ApplicationRepository _applicationRepository= new ApplicationRepository();
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SubmitApplication(int jobID)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "User");
            else if (HttpContext.Session.GetString("UserRole") != "freelancer")
                return RedirectToAction("CreateFreelancer", "Freelancer");
            else
                return View(jobID);
            
        }
        [HttpPost]
        public IActionResult SubmitApplication(Application app)
        {
            app.freelancer_id = (int) HttpContext.Session.GetInt32("UserId");
            _applicationRepository.CreateApplication(app);
            return RedirectToAction("ViewApplicationsByFreelancer"); 
        }

        public IActionResult ViewApplicationsByFreelancer()
        {
            List<ApplicationDetails> applications = new List<ApplicationDetails>();
            List<Application> apps= new List<Application>();

            int freelancer_id = (int)HttpContext.Session.GetInt32("UserId");
            apps = _applicationRepository.GetAppsByFreelancerId(freelancer_id);
            JobRepository jobRepository = new JobRepository();
           
            foreach(Application app in apps)
            {
                Job job=jobRepository.GetJobById(app.job_id);
                ApplicationDetails applicationDetails = new ApplicationDetails()
                {
                    application = app,
                    Job=job
                };
                applications.Add(applicationDetails);
            }

            return View(applications);
        }
    }
}
