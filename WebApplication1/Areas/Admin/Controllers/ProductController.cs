using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IWebHostEnvironment _env;

    public ProductController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IWebHostEnvironment env)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _env = env;
    }

    // GET: /Admin/Product
    public async Task<IActionResult> Index()
    {
        var products = await _productRepository.GetAllAsync();
        return View(products);
    }

    // GET: /Admin/Product/Add
    public async Task<IActionResult> Add()
    {
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    // POST: /Admin/Product/Add
    [HttpPost]
    public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
    {
        if (ModelState.IsValid)
        {
            if (imageUrl != null)
            {
                product.ImageUrl = await SaveImage(imageUrl);
            }

            await _productRepository.AddAsync(product);
            return RedirectToAction(nameof(Index));
        }

        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(product);
    }

    public async Task<IActionResult> Display(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    // GET: /Admin/Product/Update
    public async Task<IActionResult> Update(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return NotFound();

        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
        return View(product);
    }

    // POST: /Admin/Product/Update
    [HttpPost]
    public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
    {
        ModelState.Remove("ImageUrl");

        if (id != product.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null) return NotFound();

        if (imageUrl == null)
        {
            product.ImageUrl = existingProduct.ImageUrl;
        }
        else
        {
            product.ImageUrl = await SaveImage(imageUrl);
        }

        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;
        existingProduct.Description = product.Description;
        existingProduct.CategoryId = product.CategoryId;
        existingProduct.ImageUrl = product.ImageUrl;

        await _productRepository.UpdateAsync(existingProduct);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Product/Delete
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    // POST: /Admin/Product/DeleteConfirmed
    [HttpPost, ActionName("DeleteConfirmed")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

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
}
