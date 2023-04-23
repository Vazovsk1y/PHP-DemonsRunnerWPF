using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Services;
using DemonsRunner.Implementations.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DemonsRunner.Implementations.Services
{
    internal class ScriptConfigureService : IScriptConfigureService
    {
        public IResponse<PHPScript> ConfigureScripts(IEnumerable<PHPDemon> demons)
        {
            try
            {
                var configuredScripts = new List<PHPScript>();
                foreach (var demon in demons)
                {
                    configuredScripts.Add(new PHPScript(demon));
                }

                return new Response<PHPScript>
                {
                    Description = "Scripts were successfully configurated!",
                    OperationStatus = StatusCode.Success,
                    Data = configuredScripts
                }; 
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Response<PHPScript>
                {
                    Description = "Something go wrong!",
                    OperationStatus = StatusCode.Fail
                };
            }
        }
    }
}
