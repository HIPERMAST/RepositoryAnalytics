using System.Diagnostics;

public class PythonRunner
{
    public static void RunPythonScript(string organization, string repository, bool useToken)
    {
        string token = useToken ? "your-github-token" : "None";
        string script = "path_to_your_python_script/stats.py"; // Update with your path

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"{script} {organization} {repository} {token}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(start))
        {
            process.WaitForExit();
            UnityEngine.Debug.Log("Python script executed.");
        }
    }
}
