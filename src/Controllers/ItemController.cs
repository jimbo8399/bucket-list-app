using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bucketlist.Models;
using bucketlist.Services;

namespace bucketlist.Controllers;

public class ItemController : Controller
{
    private readonly ICosmosDbService _cosmosDbService;
    private readonly IRedisClient _redisClient;

    public ItemController(ICosmosDbService cosmosDbService, IRedisClient redisClient)
    {
        _cosmosDbService = cosmosDbService;
        _redisClient = redisClient;
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
    [ActionName("Create")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateAsync([Bind("Id,Name,Description,Distance,Price,IsPicked")] Item item)
    {
        if (ModelState.IsValid)
        {
            item.Id = Guid.NewGuid().ToString();
            await _cosmosDbService.AddItemAsync(item);

            if (item.IsPicked)
            {
                return await PickItemAsync(item);
            }

            return RedirectToAction("Index");
        }
        return View(item);
    }

    [HttpPost]
    [ActionName("Pick")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> PickAsync([Bind("Id,Name,Description,Distance,Price,IsPicked")] Item item)
    {
        if (ModelState.IsValid)
        {
            return await PickItemAsync(item);
        }
        return View(item);
    }

    private async Task<ActionResult> PickItemAsync(Item item)
    {
        var current = await _redisClient.GetPickedEntryAsync();
        if (current != null)
        {
            current.IsPicked = false;
            await _cosmosDbService.UpdateItemAsync(current.Id, current);

            item.IsPicked = true;
            await UpdateItem(item);

            return RedirectToAction("Index");
        }

        var pickedItem = await _cosmosDbService.FindPickedItemAsync();
        if (pickedItem == null)
        {
            item.IsPicked = true;
            await UpdateItem(item);

            return RedirectToAction("Index");
        }

        pickedItem.IsPicked = false;
        await UpdateItem(pickedItem);

        return RedirectToAction("Index");
    }

    private async Task UpdateItem(Item item)
    {
        await _cosmosDbService.UpdateItemAsync(item.Id, item);
        if (item.IsPicked)
        {
            await _redisClient.UpdatePickedEntryAsync(item);
        }
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

    [HttpPost]
    [ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditAsync([Bind("Id,Name,Description,Distance,Price")] Item item)
    {
        if (ModelState.IsValid)
        {
            await UpdateItem(item);

            return RedirectToAction("Index");
        }

        return View(item);
    }

    [ActionName("Complete")]
    public async Task<ActionResult> CompleteAsync(string id)
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
    [ActionName("Complete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CompleteConfirmedAsync([Bind("Id,IsPicked")] string id, bool isPicked)
    {
        if (isPicked)
        {
            await _redisClient.DeletePickedEntryAsync();
        }
        await _cosmosDbService.DeleteItemAsync(id);

        // Recalculate items

        return RedirectToAction("Index");
    }

    [ActionName("Details")]
    public async Task<ActionResult> DetailsAsync(string id)
    {
        return View(await _cosmosDbService.GetItemAsync(id));
    }
}
