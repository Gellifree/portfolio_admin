using Microsoft.Extensions.Configuration;
using portfolio_admin.Models;
using System.Text;
using System.Text.Json;

namespace portfolio_admin.Services
{
    public class JsonRepository
    {
        private readonly string _repoPath;

        public JsonRepository(IConfiguration config)
        {
            _repoPath = config["DatabaseRepoPath"] ?? throw new Exception("DatabaseRepoPath missing in config.");
        }


        public IEnumerable<string> GetJsonFiles()
        {
            return Directory.EnumerateFiles(_repoPath, "*.json", SearchOption.TopDirectoryOnly).Select(Path.GetFileName);
        }

        public string ReadFile(string filename)
        {
            var path = Path.Combine(_repoPath, filename);
            return File.ReadAllText(path);
        }

        public void SaveFile(string filename, string content) 
        {
            var path = Path.Combine(_repoPath, filename);
            File.WriteAllText(path, content);
        }

        public void CreateFile(string filename)
        {
            if (!filename.EndsWith(".json")) filename += ".json";
            var path = Path.Combine(_repoPath, filename);
            File.WriteAllText(path, "{}");
        }

        public void DeleteFile(string filename)
        {
            var path = Path.Combine(_repoPath, filename);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
