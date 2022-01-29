using System.IO.Abstractions;
using Microsoft.Extensions.Configuration;

namespace WordleBot.Core.Utilities
{
    /// <summary>
    /// Retrieves all of the valid Wordle words
    /// </summary>
    public class WordList
    {
        private readonly IFileSystem fs;
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;

        public List<string> Current { get; private set; } = new();

        private static string DataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wordlebot");

        public WordList(IFileSystem fs, HttpClient httpClient, IConfiguration config)
        {
            this.fs = fs;
            this.httpClient = httpClient;
            this.config = config;
        }

        public async Task Load()
        {
            if (!fs.Directory.Exists(DataPath)) fs.Directory.CreateDirectory(DataPath);

            using var cache = fs.File.Open(
                Path.Combine(DataPath, "wordlist.txt"),
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite);

            if (cache.ReadByte() < 0)
            {
                using var stream = await httpClient.GetStreamAsync(config.GetValue<string>("WordListSource"));
                await stream.CopyToAsync(cache);
            }

            cache.Position = 0;
            using var reader = new StreamReader(cache);
            while (await reader.ReadLineAsync() is string word) Current.Add(word);
        }
    }
}