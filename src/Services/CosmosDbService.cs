﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bucketlist.Models;
using Microsoft.Azure.Cosmos;

namespace bucketlist;

public class CosmosDbService : ICosmosDbService
{
    private Container _container;

    public CosmosDbService(
        CosmosClient dbClient,
        string databaseName,
        string containerName)
    {
        _container = dbClient.GetContainer(databaseName, containerName);
    }
    
    public async Task AddItemAsync(Item item)
    {
        await _container.CreateItemAsync<Item>(item, new PartitionKey(item.Id));
    }

    public async Task DeleteItemAsync(string id)
    {
        await _container.DeleteItemAsync<Item>(id, new PartitionKey(id));
    }

    public async Task<Item> GetItemAsync(string id)
    {
        try
        {
            ItemResponse<Item> response = await _container.ReadItemAsync<Item>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        { 
            return null;
        }
    }

    public async Task<Item> FindPickedItemAsync()
    {
        try
        {
            var response = await GetItemsAsync("SELECT * FROM c WHERE c.IsPicked = 'true'");
            if (response.Count() > 1)
            {
                throw new Exception("More than one item is marked as picked.");
            }
            return response.FirstOrDefault();
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Item>> GetItemsAsync(string queryString)
    {
        var query = _container.GetItemQueryIterator<Item>(new QueryDefinition(queryString));
        List<Item> results = new List<Item>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            
            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task UpdateItemAsync(string id, Item item)
    {
        await _container.UpsertItemAsync<Item>(item, new PartitionKey(id));
    }
}
