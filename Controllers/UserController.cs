using Microsoft.AspNetCore.Mvc;
using VendingMachineApp.Models;
using VendingMachineApp.Data;
using VendingMachineApp.ModelViewModel;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VendingMachineApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Index: Menampilkan daftar pengguna dengan pencarian
        public async Task<IActionResult> Index(string searchString)
        {
            var viewModel = new UserViewModel
            {
                SearchString = searchString,
                Users = await _context.Users
                    .Where(u => string.IsNullOrEmpty(searchString) || u.Name.Contains(searchString))
                    .Select(u => new UserViewModel
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        Balance = u.Balance
                    })
                    .ToListAsync()
            };
            return View(viewModel);
        }

        // Create GET: Menampilkan form untuk menambah user
        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserViewModel());
        }

        // Create POST: Menambahkan data user baru
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserId = model.UserId,
                    Name = model.Name,
                    Balance = model.Balance ?? 0
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Edit GET: Menampilkan form untuk mengedit user
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)  return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var model = new UserViewModel
            {
                UserId = user.UserId,
                Name = user.Name,
                Balance = user.Balance
            };

            return View(model);
        }

        // Edit POST: Menyimpan perubahan data user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(model.UserId);
                if (user == null) return NotFound();

                user.Name = model.Name;
                user.Balance = model.Balance;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Delete GET: Menampilkan konfirmasi hapus user
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var model = new UserViewModel
            {
                UserId = user.UserId,
                Name = user.Name,
                Balance = user.Balance
            };

            return View(model);
        }

        // Delete POST: Menghapus data user
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                 _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            return RedirectToAction("Index");
        }
    }
}
