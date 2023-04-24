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
    internal class ScripExecutorService : IScriptExecutorService
    {
        public IResponse<PHPScriptExecutor> Start(IEnumerable<PHPScript> scripts, bool showExecutingWindow)
        {
            try
            {
                var executors = new List<PHPScriptExecutor>();
                foreach (var script in scripts)
                {
                    var executor = new PHPScriptExecutor(script, showExecutingWindow);
                    executors.Add(executor);
                    executor.Start();
                }

                return new Response<PHPScriptExecutor>
                {
                    Description = "Scripts were successfully started!",
                    Data = executors,
                    OperationStatus = StatusCode.Success
                };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Response<PHPScriptExecutor>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                };
            }
        }

        public IResponse<PHPScriptExecutor> Stop(IEnumerable<PHPScriptExecutor> executors)
        {
            try
            {
                foreach (var executor in executors)
                {
                    if (!executor.IsRunning)
                    {
                        throw new InvalidOperationException("Some script is not running!");
                    }
                    executor.Stop();
                    executor.Dispose();
                }

                return new Response<PHPScriptExecutor>
                {
                    OperationStatus = StatusCode.Success,
                    Description = "Runners were killed and disposed successfully!"
                };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Response<PHPScriptExecutor>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                };
            }
        }
    }
}
