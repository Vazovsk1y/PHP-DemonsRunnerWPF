using Microsoft.VisualBasic;

namespace DaemonsRunner.Domain.Models
{
    /// <summary>
    /// Represents a script that references a PHP file and includes a command to be executed in the console.
    /// </summary>
    public class PHPScript
    {
        // represents php-script model that might be start in console.

        /// <summary>
        /// Script name.
        /// </summary>
        public string Name => $"{ExecutableFile.Name} script";

        /// <summary>
        /// Command to execute in command line for php-daemon starting.
        /// </summary>
        public string Command => $"php {ExecutableFile.Name} start";

        /// <summary>
        /// File containing the code to be executed by the daemon.
        /// </summary>
        public PHPFile ExecutableFile { get; }

        private PHPScript(PHPFile executableFile)
        {
            ExecutableFile = executableFile;
        }

        public static PHPScript Create(PHPFile executableFile)
        {
            return new PHPScript(executableFile);
        }
    }
}
