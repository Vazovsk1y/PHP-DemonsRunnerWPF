using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Domain.Responses.Intefaces;
using System.Collections.Generic;

namespace DemonsRunner.Infrastructure.Extensions
{
    internal static class IDataBusExtensions
    {
        public static void SendDescriptions(this IDataBus dataBus, IEnumerable<IResponse> responses)
        {
            foreach (var response in responses)
            {
                dataBus.Send(response.Description);
            }
        }
    }
}
