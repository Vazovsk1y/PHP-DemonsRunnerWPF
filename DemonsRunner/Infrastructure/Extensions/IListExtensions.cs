using DemonsRunner.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static void AddIfNotExist(this IList<PHPDemon> demons, IEnumerable<PHPDemon> demonsToAdd)
        {
            foreach (var demon in demonsToAdd)
            {
                if (demons.FirstOrDefault(d => d.Name == demon.Name && d.FullPath == demon.FullPath) is null)
                {
                    demons.Add(demon);
                }
            }
        }
    }
}
