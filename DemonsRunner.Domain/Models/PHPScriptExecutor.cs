using System.Diagnostics;

namespace DemonsRunner.Domain.Models
{
    /// <summary>
    /// Abstraction over .NET Process class that represents the php-script execution model in cmd.
    /// </summary>
    public class PHPScriptExecutor : IDisposable
    {
        // represents class to interract with php-demon* execution in command line. The script execution model.

        /* 
        A PHP daemon is a background process that runs on a server without active user interaction.
        It runs forever or according to a given schedule, serving to perform certain tasks or process data.
        A PHP daemon can perform a variety of tasks, such as periodic request processing, scheduling tasks,
        maintaining background processes, or interacting with other services and server components. 
        */

        #region --Events--

        /// <summary>
        /// Occurs when user stop the script process in task manager manualy.
        /// Messages receiving if it was started will be stop.
        /// </summary>
        public event EventHandler? ScriptExitedByTaskManager;

        /// <summary>
        /// Occurs when data have received from script cmd output, such as errors or messages.
        /// </summary>
        public event Func<object, string, Task>? ScriptOutputMessageReceived;

        #endregion

        #region --Fields--

        private bool _isDisposed = false;

        private bool _isClosedByTaskManager = true;

        private readonly Process _executableConsole;

        #endregion

        #region --Properties--

        /// <summary>
        /// Php-script model that executed/will be execute in cmd.
        /// </summary>
        public PHPScript ExecutableScript { get; }

        public bool IsRunning { get; private set; }

        public bool IsMessagesReceivingEnable { get; private set; }

        #endregion

        #region --Constructors--

        public PHPScriptExecutor(PHPScript executableScript)
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
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true,
            };
            _executableConsole.Exited += OnProcessExited;
            _executableConsole.ErrorDataReceived += OnProcessOutputDataReceived;
            _executableConsole.OutputDataReceived += OnProcessOutputDataReceived;
        }

        #endregion

        #region --Methods--

        /// <summary>
        /// Starts script cmd process.
        /// </summary>
        /// <returns>
        /// true - if successfully started, otherwise - false.
        /// </returns>
        public Task<bool> StartAsync()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (IsRunning)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} is already started.");
            }

            bool startingResult = _executableConsole.Start();
            IsRunning = startingResult;
            return Task.FromResult(IsRunning);
        }

        /// <summary>
        /// Begins receiving data from output.
        /// </summary>
        public Task StartMessagesReceivingAsync()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (!IsRunning)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} wasn't started.");
            }
            if (IsMessagesReceivingEnable)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} messages receiving is already started.");
            }

            _executableConsole.BeginOutputReadLine();
            _executableConsole.BeginErrorReadLine();
            IsMessagesReceivingEnable = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the script command in the command line.
        /// </summary>
        public Task ExecuteCommandAsync()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (!IsRunning)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} wasn't started.");
            }

            //_executableConsole.StandardInput.WriteLine(ExecutableScript.Command);
            _executableConsole.StandardInput.WriteLine("TelegramBot.exe start");  // for test
            _executableConsole.StandardInput.Flush();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Kills running script cmd process immediately.
        /// </summary>
        public Task StopAsync()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (IsMessagesReceivingEnable)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} messages receiving is active.");
            }
            if (!IsRunning)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} wasn't started.");
            }

            _executableConsole.Kill();
            _isClosedByTaskManager = false;
            IsRunning = false;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops receiving messages from script cmd output.
        /// </summary>
        public Task StopMessagesReceivingAsync()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if (!IsRunning)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} wasn't started.");
            }
            if (!IsMessagesReceivingEnable)
            {
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} messages receiving wasn't started.");
            }

            _executableConsole.CancelErrorRead();
            _executableConsole.CancelOutputRead();
            IsMessagesReceivingEnable = false;
            return Task.CompletedTask;
        }

        public void Dispose() => CleanUp();

        #region --EventHandlers--

        private void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string endcodingMessage = "Active code page: 65001";
            if (!string.IsNullOrEmpty(e.Data) && e.Data != endcodingMessage)
            {
                ScriptOutputMessageReceived?.Invoke(this, e.Data);
            }
        }

        private void OnProcessExited(object? sender, EventArgs e) 
        {
            if (!_isClosedByTaskManager)
            {
                return;
            }

            if (IsMessagesReceivingEnable)
            {
                _executableConsole.CancelOutputRead();
                _executableConsole.CancelErrorRead();
                IsMessagesReceivingEnable = false;
            }

            IsRunning = false;
            ScriptExitedByTaskManager?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        private void CleanUp()
        {
            if (_isDisposed)
            {
                return;
            }

            _executableConsole.Exited -= OnProcessExited;
            _executableConsole.OutputDataReceived -= OnProcessOutputDataReceived;
            _executableConsole.ErrorDataReceived -= OnProcessOutputDataReceived;
            _executableConsole.Dispose();
            _isDisposed = true;
        }

        #endregion
    }
}
