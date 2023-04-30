using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemonsRunner.BuisnessLayer.Responses
{
    internal class CollectionDataResponse<T> : ICollectionDataResponse<T>
    {
        public IEnumerable<T> Data { get; set; }

        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
