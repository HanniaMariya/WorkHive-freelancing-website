using Microsoft.AspNetCore.Mvc;
using WorkHive.Models.Repositories;
using WorkHive.Models;
using System.Reflection;
using WorkHive.Models.ViewModels;

namespace WorkHive.Controllers
{
    public class FreelancerController : Controller
    {
        private readonly FreelancerRepository _freelancerRepository= new FreelancerRepository();
       private readonly IWebHostEnvironment Environment;

         public FreelancerController(IWebHostEnvironment environment)
         {
                 Environment = environment;
         }

        public IActionResult Index()
        {
            return View();
        }

        private List<ApplicationDetails> getApplicationsByFreelancer()
        {
            List<ApplicationDetails> applications = new List<ApplicationDetails>();
            List<Application> apps = new List<Application>();
            int freelancer_id = (int)HttpContext.Session.GetInt32("UserId");
            apps = new ApplicationRepository().GetAppsByFreelancerId(freelancer_id);
            JobRepository jobRepository = new JobRepository();

            foreach (Application app in apps)
            {
                Job job = jobRepository.GetJobById(app.job_id);
                ApplicationDetails applicationDetails = new ApplicationDetails()
                {
                    application = app,
                    Job = job
                };
                applications.Add(applicationDetails);
            }
            return applications;
        }
        private List<Job> getJobsForFreelancer()
        {
            int freelancer_id = (int)HttpContext.Session.GetInt32("UserId");
            List<string> freelancerSkills=_freelancerRepository.GetSkillNamesByFreelancerId(freelancer_id).ToList();
            JobRepository jobRepository = new JobRepository();
            List<Job> jobs=jobRepository.SearchJobsBySkills(freelancerSkills);
            return jobs;
        }
        public IActionResult FreelancerDashboard()
        {
            List<ApplicationDetails>applications=getApplicationsByFreelancer(); 
            ViewBag.Model1 = applications;
            List<Job> jobs=getJobsForFreelancer();
            ViewBag.Model2 = jobs;
            return View();
        }
         public IActionResult Profile()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null)
                return RedirectToAction("Login", "User");
            Freelancer freelancer = new Freelancer();
            freelancer = _freelancerRepository.GetFreelancerById((int)id);
            return View(freelancer);
        }
       
        public IActionResult CreateFreelancer()
        {
           if(HttpContext.Session.GetInt32("UserId")==null)
            return RedirectToAction("Login","User");
           else
            return View();
        }

        //[HttpPost]
        //public IActionResult CreateFreelancer(Freelancer model)
        //{
        //    if (model.skills == null || model.skills.Count < 2 || model.skills.Count > 5)
        //    {
        //        ViewBag.Error = "Please add between 2 and 5 skills.";
        //        return View("CreateFreelancer", model);
        //    }

        //    foreach (var skill in model.skills)
        //    {
        //        skill.freelancer_id = model.FreelancerId; // Assign the freelancer ID to each skill
        //    }

        //    _freelancerRepository.CreateFreelancer(model);
        //    ViewBag.Success = "Freelancer profile created successfully!";
        //    return RedirectToAction("Index","Home");
        //}

        [HttpPost]
        public IActionResult CreateFreelancer(IFormFile ProfilePic, string Name, string Description, string Language, string LanguageLevel, string PortfolioLink, string Phone, List<Skill> skills)
        {
            if (skills == null || skills.Count < 1 || skills.Count > 5)
            {
                ViewBag.Error = "Please add between 1 and 5 skills.";
                return View("CreateFreelancer");
            }
            string wwwPath = this.Environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");

            // Ensure the Uploads folder exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string uploadedFilePath = null;
            if (ProfilePic != null)
            {
                var fileName = Path.GetFileName(ProfilePic.FileName);
                uploadedFilePath = Path.Combine(path, fileName);

                using (var stream = new FileStream(uploadedFilePath, FileMode.Create))
                {
                    ProfilePic.CopyTo(stream);
                }
            }
            int? id = HttpContext.Session.GetInt32("UserId");
            if (id == null)
                return RedirectToAction("Login", "User");

            Freelancer model = new Freelancer();
            model.FreelancerId = (int)id;    
            model.Name = Name;
            model.Description = Description;
            model.Language = Language;
            model.LanguageLevel = LanguageLevel;
            model.PortfolioLink = PortfolioLink;
            model.Phone = Phone;
            model.ProfilePic = uploadedFilePath;
            foreach (Skill skill in skills)
            {
                model.skills.Add(skill);
            }
            _freelancerRepository.CreateFreelancer(model);
            ViewBag.Success = "Freelancer profile created successfully!";
            return RedirectToAction("Index", "Home");
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
        [HttpGet]
        public IActionResult FreelancerDetails(int freelancerId)
        {
           Freelancer freelancer = _freelancerRepository.GetFreelancerById(freelancerId);

            return View(freelancer);
        }
        public IActionResult ViewFreelancers()
        {
           List<Freelancer>frees= _freelancerRepository.GetAllFreelancers();
            return View(frees);
        }
        [HttpGet]
        public IActionResult UpdateProfile()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            Freelancer freelancer = new Freelancer();
            freelancer = _freelancerRepository.GetFreelancerById((int)id);
          
            return View(freelancer);
        } 
        [HttpPost]
        public IActionResult UpdateProfile(Freelancer freelancer)
        {
            
            _freelancerRepository.UpdateFreelancer(freelancer);
            return RedirectToAction("Profile");  
        }
    }
}
