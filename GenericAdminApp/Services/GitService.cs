using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio_admin.Services
{
    public class GitService
    {
        private readonly string repoPath;
        private readonly string branch;

        public GitService(IConfiguration config)
        {
            repoPath = config["DatabaseRepoPath"];
            branch = config["DatabaseRepoBranch"];
        }

        public void CommitAndPush(string message)
        {
            RunGitCommand("add .");
            RunGitCommand($"commit -m \"{message}\"");
            RunGitCommand($"push origin {branch}");
        }

        public void Pull()
        {
            
            RunGitCommand($"pull origin {branch}");
        }

        private void RunGitCommand(string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                WorkingDirectory = repoPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi)!;
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Debug.WriteLine($"[GitService] git {args}");
            Debug.WriteLine(output);
            if (!string.IsNullOrEmpty(error))
                Debug.WriteLine($"[Git Error] {error}");

            if (process.ExitCode != 0)
                throw new Exception($"Git command failed: {args}\n{error}");
        }
    }
}
