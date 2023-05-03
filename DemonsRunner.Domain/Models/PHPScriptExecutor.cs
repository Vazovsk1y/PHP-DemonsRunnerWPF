using System.Diagnostics;

namespace DemonsRunner.Domain.Models
{
    public class PHPScriptExecutor : IDisposable
    {
        #region --Events--

        public event EventHandler? ScriptExitedByUser;

        public event DataReceivedEventHandler? ScriptOutputMessageReceived;

        #endregion

        #region --Fields--

        private bool _disposed = false;

        private readonly Process _executableConsole;

        #endregion

        #region --Properties--

        public PHPScript ExecutableScript { get; }

        public bool IsRunning { get; private set; }

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
                    WorkingDirectory = ExecutableScript.ExecutableFile.FullPath.TrimEnd(ExecutableScript.ExecutableFile.Name.ToCharArray()),
                    //WorkingDirectory = "D:\\IDE\\MyTelegramBot\\TelegramBot\\bin\\Release\\net7.0",   // for testing 
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

        public Task<bool> StartAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PHPScriptExecutor));
            if (IsRunning)
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} is already started");

            var startingResult = _executableConsole.Start();
            IsRunning = startingResult;
            return Task.FromResult(IsRunning);
        }

        public Task StartMessageReceivingAsync()
        {
            _executableConsole.BeginOutputReadLine();
            _executableConsole.BeginErrorReadLine();
            return Task.CompletedTask;
        }

        public Task ExecuteCommandAsync()
        {
            _executableConsole.StandardInput.WriteLine(ExecutableScript.Command);
            //await _executableConsole.StandardInput.WriteLineAsync("TelegramBot.exe start");  // for test
            _executableConsole.StandardInput.Flush();
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PHPScriptExecutor));
            if (!IsRunning)
                throw new InvalidOperationException($"{nameof(PHPScriptExecutor)} is not starting");

            _executableConsole.Kill();
            IsRunning = false;
            return Task.CompletedTask;
        }

        public Task StopMessageReceivingAsync()
        {
            _executableConsole.CancelErrorRead();
            _executableConsole.CancelOutputRead();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            IsRunning = false;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnScriptOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string endcodingMessage = "Active code page: 65001";
            if (!string.IsNullOrEmpty(e.Data) && e.Data != endcodingMessage)
            {
                ScriptOutputMessageReceived?.Invoke(this, e);
            }
        }

        private void OnScriptExited(object? sender, EventArgs e) => ScriptExitedByUser?.Invoke(this, EventArgs.Empty);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // managed resources 
                    _executableConsole.Exited -= OnScriptExited;
                    _executableConsole.OutputDataReceived -= OnScriptOutputDataReceived;
                    _executableConsole.ErrorDataReceived -= OnScriptOutputDataReceived;
                    _executableConsole.Dispose();
                }

                // unmanaged resourses
                _disposed = true;
            }
        }

        #endregion
    }
}
