using System.Diagnostics;

namespace DemonsRunner.Domain.Models
{
    public class PHPScriptExecutor : IDisposable
    {
        public event EventHandler? ScriptExitedByUser;

        public event DataReceivedEventHandler? ScriptOutputMessageReceived;

        private bool _disposed = false;

        private readonly Process _executableConsole;

        public PHPScript ExecutableScript { get; }

        public bool IsRunning { get; private set; }

        public PHPScriptExecutor(PHPScript executableScript, bool showExecutingWindow)
        {
            ExecutableScript = executableScript;
            _executableConsole = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/k chcp 65001",      // set UTF8 endcoding to cmd output.
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WorkingDirectory = ExecutableScript.ExecutableFile.FullPath.TrimEnd(ExecutableScript.ExecutableFile.Name.ToCharArray()),
                    CreateNoWindow = !showExecutingWindow,
                },
                EnableRaisingEvents = true,
            };
            _executableConsole.Exited += OnScriptExited;
            _executableConsole.ErrorDataReceived += OnScriptOutputErrorReceived;
            _executableConsole.OutputDataReceived += OnScriptOutputDataReceived;
        }

        private void OnScriptOutputErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                ScriptOutputMessageReceived?.Invoke(this, e);
            }
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

        public void Start()
        {
            if (_disposed) 
                throw new ObjectDisposedException(nameof(PHPScriptExecutor));
            _executableConsole.Start();
            _executableConsole.StandardInput.WriteLine(ExecutableScript.Command);
            _executableConsole.StandardInput.Flush();
            _executableConsole.BeginOutputReadLine();
            _executableConsole.BeginErrorReadLine();
            IsRunning = true;
        }

        public void Stop()
        {
            if (_disposed) 
                throw new ObjectDisposedException(nameof(PHPScriptExecutor));
            _executableConsole.Kill();
            IsRunning = false;
        }

        public void Dispose()
        {
            IsRunning = false;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // managed resources 
                    _executableConsole.Exited -= OnScriptExited;
                    _executableConsole.OutputDataReceived -= OnScriptOutputDataReceived;
                    _executableConsole.Dispose();
                }

                // unmanaged resourses
                _disposed = true;
            }
        }
    }
}
