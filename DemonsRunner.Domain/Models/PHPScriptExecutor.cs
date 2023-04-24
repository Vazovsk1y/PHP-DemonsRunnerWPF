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

        public PHPScriptExecutor(PHPScript script, bool showExecutingWindow)
        {
            ExecutableScript = script;
            _executableConsole = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = ExecutableScript.ExecutableFile.FullPath.TrimEnd(ExecutableScript.ExecutableFile.Name.ToCharArray()),
                    CreateNoWindow = !showExecutingWindow,
                },
                EnableRaisingEvents = true,
            };
            _executableConsole.Exited += OnScriptExited;
            _executableConsole.OutputDataReceived += OnScriptOutputDataReceived;
        }

        private void OnScriptOutputDataReceived(object sender, DataReceivedEventArgs e) => ScriptOutputMessageReceived?.Invoke(this, e);

        private void OnScriptExited(object? sender, EventArgs e) => ScriptExitedByUser?.Invoke(this, EventArgs.Empty);

        public void Start()
        {
            try
            {
                _executableConsole.Start();
                _executableConsole.StandardInput.WriteLine(ExecutableScript.Command);
                _executableConsole.StandardInput.Flush();
                IsRunning = true;
            }
            catch
            {
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                _executableConsole.Kill();
                IsRunning = false;
            }
            catch
            {
                throw;
            }
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
                }

                // unmanaged resourses
                _executableConsole.Exited -= OnScriptExited;
                _executableConsole.OutputDataReceived -= OnScriptOutputDataReceived;
                _executableConsole.Dispose();
                _disposed = true;
            }
        }
    }
}
