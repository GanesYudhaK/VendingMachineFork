using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VendingMachineApp.Models;
using VendingMachineApp.ModelViewModel;
using System.Threading.Tasks;

// using System.Security.Claims;
// using Microsoft.AspNetCore.Authentication.Cookies;  // Pastikan ini ada

namespace VendingMachineApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        
        public AccountController(UserManager<Account> userManager, SignInManager<Account> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Register
        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
        
            var user = new Account { 
                UserName = model.Email, 
                Email = model.Email, 
                FullName = model.FullName 
                };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["SuccessMessage"] = "Registration successfuly! Welcome!";
                return RedirectToAction("Index", "Product"); // Redirect ke halaman produk setelah login berhasil
            }

            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            TempData["ErrorMessage"] = "Registration failed." + string.Join(" ", result.Errors.Select(e => e.Description));
            return View(model);
            
        }

        // Login
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View("Index", model);

            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null) {
                TempData["ErrorMessage"] = "Invalid login attempt. User not found.";
                return View("Index", model);
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe,  lockoutOnFailure: false);
            
            
            if (result.Succeeded) {
                // var claims = new List<Claim>
                // {
                //     new Claim(ClaimTypes.NameIdentifier, user.Id),
                //     new Claim(ClaimTypes.Name, user.UserName),
                //     new Claim(ClaimTypes.Email, user.Email)
                // };
                // Membuat identity dengan klaim tersebut
                // var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                // var principal = new ClaimsPrincipal(identity);

                 // Sign-in menggunakan SignInManager
                await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
                HttpContext.Session.SetString("FullName", user.FullName);
                

                TempData["SuccessMessage"] = "Login successfuly!";
                return RedirectToAction("Index", "Product"); // Redirect ke halaman produk setelah login berhasil
            } else {
                TempData["ErrorMessage"] = "Invalid login attempt. Please check your credentials.";
                return View("Index", model);
            }
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Account");
        }
    }
}
