using DemonsRunner.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace DemonsRunner.Infrastructure.Extensions
{
    internal static class IListExtensions
    {
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
        public static bool AddIfNotExist(this IList<PHPDemon> demons, IEnumerable<PHPDemon> demonsToAdd)
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
