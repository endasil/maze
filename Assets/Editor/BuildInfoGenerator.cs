using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildInfoGenerator : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string gitDescribe = GetGitOutput("describe --always --dirty") ?? "unknown";
        string commitCount = GetGitOutput("rev-list --count HEAD") ?? "0";
        string utcTime = DateTime.UtcNow.ToString("yy.MM.dd HH:mm");

        var sb = new StringBuilder();
        sb.AppendLine("public static class BuildInfo");
        sb.AppendLine("{");
        sb.AppendLine($"    public const string VERSION = \"{gitDescribe}\";");
        sb.AppendLine($"    public const int COMMIT_COUNT = {commitCount};");
        sb.AppendLine($"    public const string BUILD_TIME = \"{utcTime}\";");
        sb.AppendLine("}");

        File.WriteAllText("Assets/BuildInfo.cs", sb.ToString());
        UnityEngine.Debug.Log($"BuildInfo.cs generated: version {gitDescribe}, commit count {commitCount}, time {utcTime}");
    }

    private static string GetGitOutput(string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        try
        {
            process.Start();
            string output = process.StandardOutput.ReadLine();
            process.WaitForExit();
            return output?.Trim();
        }
        catch
        {
            return null;
        }
    }
}