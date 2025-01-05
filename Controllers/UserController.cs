using Microsoft.AspNetCore.Mvc;
using WorkHive.Models.Repositories;
using WorkHive.Models;

namespace WorkHive.Controllers
{
    public class UserController : Controller
    {
        private UserRepository _userRepository=new UserRepository();    
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _userRepository.GetUserByEmail(user.email);
                if (existingUser != null)
                {
                    ViewBag.Error = "Email already exists.";
                    return View();
                }

                _userRepository.AddUser(user);
                ViewBag.Success = "Registration successful";
                var insertedUser = _userRepository.GetUser(user.email, user.password);
                HttpContext.Session.SetInt32("UserId", insertedUser.user_id);
                HttpContext.Session.SetString("UserName", insertedUser.name);
                HttpContext.Session.SetString("UserRole", insertedUser.role);

                if (user.role.Normalize().Equals("freelancer", StringComparison.OrdinalIgnoreCase))
                    return RedirectToAction("CreateFreelancer", "Freelancer");

                return RedirectToAction("ClientDashboard","User");
            }

            return View(user);
        }
        public ActionResult Login()
        {
            return View();
        }

   
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }

            var user = _userRepository.GetUser(email, password);
            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.user_id);
            HttpContext.Session.SetString("UserName", user.name);
            HttpContext.Session.SetString("UserRole", user.role);
            if (HttpContext.Session.GetString("UserRole") == "freelancer")
                return RedirectToAction("FreelancerDashboard", "Freelancer");
            return RedirectToAction("ClientDashboard", "User");

            
        }

      
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();    
            return RedirectToAction("Login");
        }
        public IActionResult ClientDashboard()
        {
            JobRepository jobRepository = new JobRepository();
            List<Job> jobs = new List<Job>();
            int userID = (int)HttpContext.Session.GetInt32("UserId");
            jobs = jobRepository.GetJobsByClientId(userID);
            return View(jobs);
        }
    }
}
