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

        public async Task<List<Project>> LoadProjectsAsync()
        {
            var file = Path.Combine(_repoPath, "projects.json");
            if (!File.Exists(file)) return new List<Project>();

            var json = await File.ReadAllTextAsync(file);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            return JsonSerializer.Deserialize<List<Project>>(json, options) ?? new List<Project>();
        }

        private static string GenerateId()
        {
            return $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6]}";
        }

        public async Task AddProjectAsync(Project project)
        {
            var projects = await LoadProjectsAsync();

            if (string.IsNullOrWhiteSpace(project.Id))
            {
                project.Id = GenerateId();
            }

            projects.Add(project);
            await SaveProjectsAsync(projects);
        }

        public async Task UpdateProjectAsync(Project project)
        {
            var projects = await LoadProjectsAsync();
            var existing = projects.FirstOrDefault(p => p.Id == project.Id);
            if (existing != null)
            {
                existing.Title = project.Title;
                existing.Description = project.Description;
                existing.Link = project.Link;
                existing.Image = project.Image;
                await SaveProjectsAsync(projects);
            }
        }

        public async Task DeleteProjectAsync(string id)
        {
            var projects = await LoadProjectsAsync();
            var toRemove = projects.FirstOrDefault(p => p.Id == id);
            if (toRemove != null)
            {
                projects.Remove(toRemove);
                await SaveProjectsAsync(projects);
            }
        }


        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public async Task SaveProjectsAsync(List<Project> projects)
        {
            var file = Path.Combine(_repoPath, "projects.json");
            var json = JsonSerializer.Serialize(projects, JsonOptions);
            await File.WriteAllTextAsync(file, json, Encoding.UTF8);
        }
    }
}
