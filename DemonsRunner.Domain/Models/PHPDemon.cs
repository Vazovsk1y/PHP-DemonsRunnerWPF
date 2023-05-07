namespace DemonsRunner.Domain.Models
{
    /// <summary>
    /// Model that represent php-daemon file.
    /// </summary>
    public class PHPDemon
    {
        /// <summary>
        /// File extension.
        /// </summary>
        public const string EXTENSION = ".php";

        /// <summary>
        /// File name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File fullname(path).
        /// </summary>
        public string FullPath { get; set; }
    }
}
