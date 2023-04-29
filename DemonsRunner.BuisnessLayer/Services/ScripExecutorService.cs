﻿using DemonsRunner.BuisnessLayer.Responses;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Services;
using System.Diagnostics;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScripExecutorService : IScriptExecutorService
    {
        public IDataResponse<PHPScriptExecutor> Start(IEnumerable<PHPScript> scripts, bool showExecutingWindow)
        {
            try
            {
                var executors = new List<PHPScriptExecutor>();
                foreach (var script in scripts)
                {
                    var executor = new PHPScriptExecutor(script, showExecutingWindow);
                    if (executor.Start())
                    {
                        executors.Add(executor);
                    }
                }

                return new DataResponse<PHPScriptExecutor>
                {
                    Description = "Scripts were successfully started!",
                    Data = executors,
                    OperationStatus = StatusCode.Success
                };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataResponse<PHPScriptExecutor>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                };
            }
        }

        public IDataResponse<PHPScriptExecutor> Stop(IEnumerable<PHPScriptExecutor> executors)
        {
            try
            {
                if (!IsScriptsRunning(executors))
                {
                    throw new InvalidOperationException("Some script is not running!");
                }

                foreach (var executor in executors)
                {
                    executor.Stop();
                    executor.Dispose();
                }

                return new DataResponse<PHPScriptExecutor>
                {
                    OperationStatus = StatusCode.Success,
                    Description = "Runners were killed and disposed successfully!"
                };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataResponse<PHPScriptExecutor>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                };
            }
        }

        private bool IsScriptsRunning(IEnumerable<PHPScriptExecutor> executors) => executors.ToList().TrueForAll(executor => executor.IsRunning);
    }
}
