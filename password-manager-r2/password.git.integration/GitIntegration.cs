using CliWrap;
using CliWrap.Buffered;
using System.Threading;
using CliWrap.EventStream;

namespace password.git.integration
{
    public static class GitIntegration
    {
        public static async Task<int> InvokeGit(IEnumerable<string> commands, string path = ".")
        {
            var cmd = Cli.Wrap("git")
                .WithArguments(commands)
                .WithWorkingDirectory(workingDirPath: path);

            int exitCode = 255;

            await foreach (var cmdEvt in cmd.ListenAsync())
            {
                switch (cmdEvt)
                {
                    case ExitedCommandEvent exited:
                        {
                            exitCode = exited.ExitCode;
                            break;
                        }
                }
            }
            return exitCode;
        }

        public static async Task<int> UpdateGitHub(string updateMessage, string path = ".")
        {
            int pull = await InvokeGit(new[] { "pull" }, path);
            if (pull == 0)
            {
                int add = await InvokeGit(new[] { "add", "Logins.xml" }, path);
                if (add == 0)
                {
                    int commit = await InvokeGit(new[] { "commit", "-m", $"{updateMessage}" }, path);
                    if (commit == 0)
                    {
                        return await InvokeGit(new[] { "push" }, path);
                    }
                }
            }
            return 255;
        }
    }
}