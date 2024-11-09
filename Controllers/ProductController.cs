using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineApp.Data;
using VendingMachineApp.Models;
using VendingMachineApp.ModelViewModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace VendingMachineApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Menampilkan daftar produk
        
        public async Task<IActionResult> Index(string searchString)
        {
            var viewModel = new ProductViewModels {
                SearchString = searchString
            };

            var products = from p in _context.Products
                            where p.Price > 0
                           select p;

            if (!string.IsNullOrEmpty(searchString)) {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            viewModel.FullName = _context.Accounts.Where(u => u.UserName == User.Identity.Name).Select(i => i.FullName).Single();

            //menggunakan ViewModel
            viewModel.Products = await products
                .Select(p => new ProductViewModels
                {
                    IdProduct = p.IdProduct,
                    Name = p.Name,
                    Price = p.Price,
                    // Quantity = p.Quantity
                })
                .ToListAsync();
            ViewData["SearchString"] = searchString;
            return View(viewModel);
        }

        // Menampilkan form untuk membuat produk baru
        public IActionResult Create()
        {
            return View();
        }

        // Menangani POST untuk membuat produk baru
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModels productViewModel)
        {
            if (ModelState.IsValid) //validasi ViewModel
            {
                var product = new Product
                {
                    Name = productViewModel.Name,
                    Price = productViewModel.Price,
                    Quantity = productViewModel.Quantity
                };
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        // Menampilkan form untuk mengedit produk
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var productViewModel = new ProductViewModels
            {
                IdProduct = product.IdProduct,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity
            };

            return View(productViewModel);
        }

        // Menangani POST untuk mengedit produk
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModels productViewModel)
        {
            if (id != productViewModel.IdProduct) return NotFound();

            if (ModelState.IsValid) {
                try {
                    var product = await _context.Products.FindAsync(id);
                    if (product == null) return NotFound();

                    product.Name = productViewModel.Name;
                    product.Price = productViewModel.Price;
                    product.Quantity = productViewModel.Quantity;

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!ProductExists(productViewModel.IdProduct)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        // Mengambil id produk yg akan didelete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.IdProduct == id);
            if (product == null) return NotFound();

            var viewModel = new ProductViewModels
            {
                IdProduct = product.IdProduct,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity
            };

            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null) {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.IdProduct == id);
        }
    }
}
