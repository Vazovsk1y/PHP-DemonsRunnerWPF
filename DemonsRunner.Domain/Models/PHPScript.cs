namespace DemonsRunner.Domain.Models
{
    public class PHPScript
    {
        public string Name => $"{ExecutableFile.Name} script";

        public string Command => $"demon .php {ExecutableFile.Name} start";

        public PHPDemon ExecutableFile { get; }

        public PHPScript(PHPDemon executableFile)
        {
            ExecutableFile = executableFile;
        }
    }
}
