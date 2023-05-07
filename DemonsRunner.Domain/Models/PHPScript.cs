namespace DemonsRunner.Domain.Models
{
    /// <summary>
    /// Configured php-script model.
    /// </summary>
    public class PHPScript
    {
        /// <summary>
        /// Script title.
        /// </summary>
        public string Name => $"{ExecutableFile.Name} script";

        /// <summary>
        /// Command to execute in command line.
        /// </summary>
        public string Command => $"demon .php {ExecutableFile.Name} start";

        /// <summary>
        /// File that will be executed in the command.
        /// </summary>
        public PHPDemon ExecutableFile { get; }

        public PHPScript(PHPDemon executableFile)
        {
            ExecutableFile = executableFile;
        }
    }
}
