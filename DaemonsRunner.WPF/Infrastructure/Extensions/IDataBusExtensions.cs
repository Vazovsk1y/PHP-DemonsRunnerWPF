﻿using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using System.Collections.Generic;

namespace DaemonsRunner.Infrastructure.Extensions
{
    internal static class IDataBusExtensions
    {
        #region --Generic Extensions--

        public static void SendAll<T>(this IDataBus dataBus, IEnumerable<T> messages)
        {
            foreach(var message in messages) 
            {
                dataBus.Send(message);
            }
        }

        #endregion
    }
}
