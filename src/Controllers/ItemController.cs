using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bucketlist.Models;

namespace bucketlist.Controllers;

public class ItemController : Controller
{
    private readonly ICosmosDbService _cosmosDbService;
    public ItemController(ICosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    [ActionName("Index")]
    public async Task<IActionResult> Index()
    {
        return View(await _cosmosDbService.GetItemsAsync("SELECT * FROM c"));
    }

    [ActionName("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ActionName("Add")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateAsync([Bind("Id,Name,Description,Completed")] Item item)
    {
        if (ModelState.IsValid)
        {
            item.Id = Guid.NewGuid().ToString();
            await _cosmosDbService.AddItemAsync(item);
            return RedirectToAction("Index");
        }

        return View(item);
    }

    [HttpPost]
    [ActionName("Pick")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditAsync([Bind("Id,Name,Description,Completed")] Item item)
    {
        if (ModelState.IsValid)
        {
            await _cosmosDbService.UpdateItemAsync(item.Id, item);
            return RedirectToAction("Index");
        }

        return View(item);
    }

    [ActionName("Edit")]
    public async Task<ActionResult> EditAsync(string id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        Item item = await _cosmosDbService.GetItemAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        return View(item);
    }

    [ActionName("Check")]
    public async Task<ActionResult> CheckAsync(string id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        Item item = await _cosmosDbService.GetItemAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost]
    [ActionName("Check")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CheckConfirmedAsync([Bind("Id")] string id)
    {
        await _cosmosDbService.DeleteItemAsync(id);
        return RedirectToAction("Index");
    }

    [ActionName("Details")]
    public async Task<ActionResult> DetailsAsync(string id)
    {
        return View(await _cosmosDbService.GetItemAsync(id));
    }
}
