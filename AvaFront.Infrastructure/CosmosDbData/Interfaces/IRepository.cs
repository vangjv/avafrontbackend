﻿using AvaFront.Shared.Entities.Base;

namespace AvaFront.Infrastructure.CosmosDbData.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        ///     Get items given a string SQL query directly.
        ///     Likely in production, you may want to use alternatives like Parameterized Query or LINQ to avoid SQL Injection and avoid having to work with strings directly.
        ///     This is kept here for demonstration purpose.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetItemsAsync(string query);

        /// <summary>
        ///     Get one item by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetItemAsync(string id);
        Task<T> AddItemAsync(T item);
        Task<T> UpdateItemAsync(string id, T item);
        Task DeleteItemAsync(string id);

    }
}
