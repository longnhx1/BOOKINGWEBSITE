using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers 
{     
    public class ProductController : Controller     
    {         
        private readonly IProductRepository _productRepository;         
        private readonly ICategoryRepository _categoryRepository;         
        private readonly IWebHostEnvironment _env;
        
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IWebHostEnvironment env)         
        {             
            _productRepository = productRepository;             
            _categoryRepository = categoryRepository;
            _env = env;
        }          
        
        // Hiển thị danh sách sản phẩm         
        public async Task<IActionResult> Index()         
        {             
            var products = await _productRepository.GetAllAsync();             
            return View(products);         
        }         
        
        // Hiển thị form thêm sản phẩm mới         
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Add()         
        {             
            var categories = await _categoryRepository.GetAllAsync();             
            ViewBag.Categories = new SelectList(categories, "Id", "Name");              
            return View();         
        }          
        
        // Xử lý thêm sản phẩm mới         
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]         
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)         
        {             
            if (ModelState.IsValid)             
            {                 
                if (imageUrl != null)                 
                {                     
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage                     
                    product.ImageUrl = await SaveImage(imageUrl);                  
                }                  
                await _productRepository.AddAsync(product);                 
                return RedirectToAction(nameof(Index));             
            }             
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập             
            var categories = await _categoryRepository.GetAllAsync();             
            ViewBag.Categories = new SelectList(categories, "Id", "Name");             
            return View(product);         
        }

        // Viết thêm hàm SaveImage (tham khảo bài 02)  
        private async Task<string> SaveImage(IFormFile image)  
        {
            var imagesDir = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(imagesDir);

            var originalName = Path.GetFileName(image.FileName);
            var ext = Path.GetExtension(originalName);
            var safeBaseName = Path.GetFileNameWithoutExtension(originalName);

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                safeBaseName = safeBaseName.Replace(c, '_');
            }

            if (string.IsNullOrWhiteSpace(safeBaseName))
            {
                safeBaseName = "upload";
            }

            var fileName = $"{safeBaseName}_{Guid.NewGuid():N}{ext}";
            var savePath = Path.Combine(imagesDir, fileName);

            await using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/" + fileName;
        }

        // Hiển thị thông tin chi tiết sản phẩm         
        public async Task<IActionResult> Display(int id)         
        {             
            var product = await _productRepository.GetByIdAsync(id);             
            if (product == null)             
            {                 
                return NotFound();             
            }             
            return View(product);         
        }         
        
        // Hiển thị form cập nhật sản phẩm         
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(int id)         
        {             
            var product = await _productRepository.GetByIdAsync(id);             
            if (product == null)             
            {                 
                return NotFound();             
            }              
            var categories = await _categoryRepository.GetAllAsync();             
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);             
            return View(product);         
        }         
        
        // Xử lý cập nhật sản phẩm         
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]         
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)         
        {             
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl             
            if (id != product.Id)             
            {                 
                return NotFound();             
            }              
            
            if (ModelState.IsValid)             
            {                  
                var existingProduct = await _productRepository.GetByIdAsync(id);                  
                if (existingProduct == null)
                {
                    return NotFound();
                }
                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên                 
                if (imageUrl == null)                 
                {                     
                    product.ImageUrl = existingProduct.ImageUrl;                 
                }                 
                else                 
                {                     
                    // Lưu hình ảnh mới                     
                    product.ImageUrl = await SaveImage(imageUrl);                 
                }
                
                // Cập nhật các thông tin khác của sản phẩm                 
                existingProduct.Name = product.Name;                 
                existingProduct.Price = product.Price;                 
                existingProduct.Description = product.Description;                 
                existingProduct.CategoryId = product.CategoryId;                 
                existingProduct.ImageUrl = product.ImageUrl;                  
                
                await _productRepository.UpdateAsync(existingProduct);                                  
                return RedirectToAction(nameof(Index));             
            }             
            var categories = await _categoryRepository.GetAllAsync();             
            ViewBag.Categories = new SelectList(categories, "Id", "Name");             
            return View(product);         
        }          
        
        // Hiển thị form xác nhận xóa sản phẩm         
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int id)         
        {             
            var product = await _productRepository.GetByIdAsync(id);             
            if (product == null)             
            {                 
                return NotFound();             
            }             
            return View(product);         
        }          
        
        // Xử lý xóa sản phẩm         
        [HttpPost, ActionName("DeleteConfirmed")]
        [Authorize(Roles = SD.Role_Admin)]         
        public async Task<IActionResult> DeleteConfirmed(int id)         
        {             
            await _productRepository.DeleteAsync(id);             
            return RedirectToAction(nameof(Index));         
        }     
    } 
}