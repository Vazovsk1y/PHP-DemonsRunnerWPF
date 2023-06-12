namespace DaemonsRunner.Domain.Tests.Infrastructure
{
    public class TestStorageFixture : IDisposable
    {
        private readonly string _testDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Test");

        private readonly List<string> _testFilesNames = new();

        public string TestDirectoryPath => _testDirectoryPath;

        public TestStorageFixture()
        {
            var directoryInfo = new DirectoryInfo(TestDirectoryPath);
            directoryInfo.Create();
            for (int i = 0; i < 10; i++)
            {
                var fileInfo = new FileInfo(Path.Combine(TestDirectoryPath, $"file{i}.php"));
                using var stream = fileInfo.Create();
                _testFilesNames.Add(fileInfo.Name);
            }
        }

        public string GetRandomFileName() => _testFilesNames[new Random().Next(_testFilesNames.Count)];

        public void Dispose()
        {
            Directory.Delete(_testDirectoryPath, true);
        }
    }
}
