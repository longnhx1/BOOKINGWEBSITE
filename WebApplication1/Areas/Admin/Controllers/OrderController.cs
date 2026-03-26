using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class OrderController : Controller
{
    // NOTE: Hiện dự án chưa có model/repository Order. Controller này chỉ tạo khung CRUD
    // để bạn gắn logic thực tế ở bài sau.

    public IActionResult Index() => View();

    public IActionResult Display(int id) => View();

    public IActionResult Add() => View();

    [HttpPost]
    public IActionResult Add(object _)
        => RedirectToAction(nameof(Index));

    public IActionResult Update(int id) => View();

    [HttpPost]
    public IActionResult Update(int id, object _)
        => RedirectToAction(nameof(Index));

    public IActionResult Delete(int id) => View();

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
        => RedirectToAction(nameof(Index));
}

