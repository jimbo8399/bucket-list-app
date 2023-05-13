using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bucketlist.Models;
using bucketlist.Services;

namespace bucketlist.Controllers;

public class ItemController : Controller
{
    private readonly ICosmosDbService _cosmosDbService;
    private readonly ITopFiveCalculator _calculator;
    private readonly IItemsHandler _itemHandler;

    public ItemController(ICosmosDbService cosmosDbService, ITopFiveCalculator calculator, IItemsHandler itemHandler)
    {
        _cosmosDbService = cosmosDbService;
        _calculator = calculator;
        _itemHandler = itemHandler;
    }

    [ActionName("Index")]
    public async Task<IActionResult> Index()
    {

        var model = new BucketListModel()
        {
            TopFiveCheapestItems = await _calculator.GetTopFiveCheapest(),
            TopFiveClosestItems = await _calculator.GetTopFiveClosest(),
            AllCompletedItems = await _itemHandler.GetAllCompletedItemsAsync()
        };
        return View(model);
    }

    [ActionName("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ActionName("Create")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateAsync([Bind("Id,Name,Description,Distance,Price")] Item item)
    {
        if (ModelState.IsValid)
        {
            await _itemHandler.CreateNewItem(item);

            await _calculator.CalculateAndStoreTopFives();

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

        Item item = await _itemHandler.GetItem(id);
        if (item == null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost]
    [ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditAsync([Bind("Id,Name,Description,Distance,Price,IsCompleted")] Item item)
    {
        if (ModelState.IsValid)
        {
            await _itemHandler.UpdateItem(item);

            await _calculator.CalculateAndStoreTopFives();

            return RedirectToAction("Index");
        }

        return View(item);
    }

    [ActionName("Complete")]
    public async Task<ActionResult> CompleteAsync([Bind("Id")] string id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        Item item = await _itemHandler.GetItem(id);
        if (item == null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost]
    [ActionName("Complete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CompleteConfirmedAsync([Bind("Id,Name,Description,Distance,Price,IsCompleted")] Item item)
    {
        if (ModelState.IsValid)
        {
            await _itemHandler.MarkAsCompletedItem(item);

            await _calculator.CalculateAndStoreTopFives();

            return RedirectToAction("Index");
        }

        return View(item);
    }

    [ActionName("Details")]
    public async Task<ActionResult> DetailsAsync(string id)
    {
        return View(await _itemHandler.GetItem(id));
    }
}
