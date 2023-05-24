using System.Diagnostics;

namespace DemonsRunner.Domain.Models
{
    /// <summary>
    /// Abstraction over .NET Process class that represent the script execution model in cmd.
    /// </summary>
    public class PHPScriptExecutor : IDisposable
    {
        #region --Events--

        /// <summary>
        /// Occurs when user closed cmd window manualy or stop the script process in task manager.
        /// Messages receiving if it was started will be stop.
        /// </summary>
        public event EventHandler? ScriptExitedByUserOutsideApp;

        /// <summary>
        /// Occurs when data have received from cmd output, such as errors or messages.
        /// </summary>
        public event Func<object, string, Task>? ScriptOutputMessageReceived;

        #endregion

        #region --Fields--

        private bool _disposed = false;

        private bool _isExitedByTaskManager = true;

        private readonly Process _executableConsole;

        #endregion

        #region --Properties--

        /// <summary>
        /// Script that executed in cmd.
        /// </summary>
        public PHPScript ExecutableScript { get; }

        public bool IsRunning { get; private set; }

        public bool IsMessagesReceiving { get; private set; }

        #endregion

        #region --Constructors--

        public PHPScriptExecutor(PHPScript executableScript, bool showExecutingWindow)
        {
            ExecutableScript = executableScript;
            _executableConsole = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = "/k chcp 65001",      // set UTF8 endcoding to cmd output.
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    //WorkingDirectory = ExecutableScript.ExecutableFile.FullPath.TrimEnd(ExecutableScript.ExecutableFile.Name.ToCharArray()),
                    WorkingDirectory = "D:\\IDE\\MyTelegramBot\\TelegramBot\\bin\\Release\\net7.0",   // for testing 
                    CreateNoWindow = !showExecutingWindow,
                },
                EnableRaisingEvents = true,
            };
            _executableConsole.Exited += OnScriptExited;
            _executableConsole.ErrorDataReceived += OnScriptOutputDataReceived;
            _executableConsole.OutputDataReceived += OnScriptOutputDataReceived;
        }

        #endregion

        #region --Methods--

        /// <summary>
        /// Starts cmd process.
        /// </summary>
        public Task<bool> StartAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PHPScriptExecutor));
            }
            if (IsRunning)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} is already started");
            }

            bool startingResult = _executableConsole.Start();
            IsRunning = startingResult;
            return Task.FromResult(IsRunning);
        }

        /// <summary>
        /// Begin receiving data from output.
        /// </summary>
        public Task StartMessagesReceivingAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PHPScriptExecutor));
            }
            if (!IsRunning)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} wasn't starting");
            }
            if (IsMessagesReceiving)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} messages receiving is already started");
            }

            _executableConsole.BeginOutputReadLine();
            _executableConsole.BeginErrorReadLine();
            IsMessagesReceiving = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the command of the script to be executed at the command line.
        /// </summary>
        public Task ExecuteCommandAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PHPScriptExecutor));
            }
            if (!IsRunning)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} wasn't starting");
            }

            //_executableConsole.StandardInput.WriteLine(ExecutableScript.Command);
            _executableConsole.StandardInput.WriteLine("TelegramBot.exe start");  // for test
            _executableConsole.StandardInput.Flush();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Kills runnig cmd process immediately.
        /// </summary>
        public Task StopAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PHPScriptExecutor));
            }
            if (IsMessagesReceiving)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} messages receiving is active");
            }
            if (!IsRunning)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} wasn't starting");
            }

            _executableConsole.Kill();
            _isExitedByTaskManager = false;
            IsRunning = false;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Breaks receiveng messages from output.
        /// </summary>
        public Task StopMessagesReceivingAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PHPScriptExecutor));
            }
            if (!IsRunning)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} wasn't starting");
            }
            if (!IsMessagesReceiving)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} messages receiving wasnt started");
            }

            _executableConsole.CancelErrorRead();
            _executableConsole.CancelOutputRead();
            IsMessagesReceiving = false;
            return Task.CompletedTask;
        }

        public void Dispose() => CleanUp();

        private void OnScriptOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string endcodingMessage = "Active code page: 65001";
            if (!string.IsNullOrEmpty(e.Data) && e.Data != endcodingMessage)
            {
                ScriptOutputMessageReceived?.Invoke(this, e.Data);
            }
        }

        private void OnScriptExited(object? sender, EventArgs e) 
        {
            if (!_isExitedByTaskManager)
            {
                return;
            }

            if (IsMessagesReceiving)
            {
                _executableConsole.CancelOutputRead();
                _executableConsole.CancelErrorRead();
                IsMessagesReceiving = false;
            }
            IsRunning = false;
            IsMessagesReceiving = false;
            ScriptExitedByUserOutsideApp?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void CleanUp()
        {
            if (_disposed)
            {
                return;
            }

            _executableConsole.Exited -= OnScriptExited;
            _executableConsole.OutputDataReceived -= OnScriptOutputDataReceived;
            _executableConsole.ErrorDataReceived -= OnScriptOutputDataReceived;
            _executableConsole.Dispose();
            _disposed = true;
        }

        #endregion
    }
}
