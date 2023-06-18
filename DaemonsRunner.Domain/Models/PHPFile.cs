using System.Text.Json.Serialization;

namespace DaemonsRunner.Domain.Models
{
    /// <summary>
    /// Code file model.
    /// </summary>
    public class PHPFile
    {
        // represents a PHP file containing the code to be executed within a daemon.

        /// <summary>
        /// File extension.
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// File name includes extension.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// File full path.
        /// </summary>
        public string FullPath { get; }

        private PHPFile(string name, string fullPath)
        {
            Extension = Path.GetExtension(name);
            Name = name;
            FullPath = fullPath;
        }

        public static PHPFile Create(string name, string fullPath)
        {
            if (!name.EndsWith(".php"))
            {
                throw new ArgumentException("File extension wasn't correct.", name);
            }
            if (!fullPath.EndsWith(name))
            {
                throw new ArgumentException($"Incorrect path.\nName: {name}\nPath: {fullPath}", fullPath);
            }

            return new PHPFile(name, fullPath);
        }
    }
}
