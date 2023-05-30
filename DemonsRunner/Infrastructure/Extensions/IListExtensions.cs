using DemonsRunner.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemonsRunner.Infrastructure.Extensions
{
    internal static class IListExtensions
    {
        public static void RemoveAll<T> (this IList<T> collection, IEnumerable<T> deletingCollection)
        {
            foreach (var deletingItem in deletingCollection)
            {
                collection.Remove(deletingItem);
            }
        }

        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> addingCollection)
        {
            if (collection is null) 
                throw new NullReferenceException(nameof(collection));

            foreach (var item in addingCollection)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Add demon if it not exist in collection.
        /// </summary>
        /// <returns>
        /// true - if any item was added, false - if collection wasn't modified.
        /// </returns>
        public static bool AddIfNotExist(this IList<PHPFile> demons, IEnumerable<PHPFile> demonsToAdd)
        {
            bool isCollectionModified = false;
            foreach (var demon in demonsToAdd)
            {
                if (demons.FirstOrDefault(d => d.Name == demon.Name && d.FullPath == demon.FullPath) is null)
                {
                    isCollectionModified = true;
                    demons.Add(demon);
                }
            }
            return isCollectionModified;
        }
    }
}
