using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using WorkHive.Models;
using WorkHive.Models.Repositories;
using WorkHive.Models.ViewModels;
//using static System.Net.Mime.MediaTypeNames;

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
            int? fId = HttpContext.Session.GetInt32("UserId");
            if (fId == null)
                return RedirectToAction("Login", "User");
            else if (HttpContext.Session.GetString("UserRole") != "freelancer")
                return RedirectToAction("CreateFreelancer", "Freelancer");
            else
            {
                if(_applicationRepository.HasFreelancerApplied((int)fId,jobID))
                {
                    ViewBag.message = "You have already applied to this job";
                    return RedirectToAction("ViewApplicationsByFreelancer");
                }
                return View(jobID);
            }
                
            
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

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _applicationRepository.DeleteApplication(id);   
            return RedirectToAction("FreelancerDashboard","Freelancer");
        }
       
        [HttpGet]
        public IActionResult ViewApplicationsForJob(int jobId)
        { 
            List<FreelancerApplication> list= new List<FreelancerApplication>();
            FreelancerRepository fr= new FreelancerRepository();
            List<Application> apps= new List<Application>();
            apps=_applicationRepository.GetAppsByJobId(jobId);
            var openApps = apps.Where(app => app.status == "Submitted").ToList();
            foreach (Application app in openApps)
            {
                Freelancer freelancer=fr.GetFreelancerById(app.freelancer_id);
                FreelancerApplication proposal = new FreelancerApplication()
                {
                    app = app,
                    freelancer = freelancer
                };
                list.Add(proposal);
            }
            return View(list);
        }

        [HttpGet]
        public IActionResult Reject(int appId)
        {
            Application application= _applicationRepository.GetApplicationById(appId);
            _applicationRepository.UpdateStatus(appId, "Rejected");

            return RedirectToAction("ViewApplicationsForJob", new { jobId=application.job_id });

        }
        [HttpGet]
        public IActionResult Accept(int appId)
        {
            Application application = _applicationRepository.GetApplicationById(appId);
            _applicationRepository.UpdateStatus(appId, "Accepted");
            JobRepository jobRepository = new JobRepository();
            jobRepository.UpdateStatus(application.job_id, "Assigned");
            return RedirectToAction("ClientDashboard", "User");
           
        }

        [HttpGet]
        public IActionResult ViewApplicationDetails(int appId)
        {
            FreelancerRepository fr = new FreelancerRepository();
            Application application = _applicationRepository.GetApplicationById(appId);
            Freelancer freelancer = fr.GetFreelancerById(application.freelancer_id);
            FreelancerApplication proposal = new FreelancerApplication()
            {
                app = application,
                freelancer = freelancer
            };
            return View(proposal);
        }
    }
}
