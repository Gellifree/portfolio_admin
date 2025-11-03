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

            using var process = Process.Start(psi);
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                throw new Exception($"Git command failed: {args}\n{error}");
            }
        }
    }
}
