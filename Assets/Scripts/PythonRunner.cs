using System.Diagnostics;
using System.IO;  // For path management

public class PythonRunner
{
    public static void RunPythonScript(string organization, string repository, string gitHubToken)
    {
        // If gitHubToken is not null or empty, use it; otherwise, use "None"
        string token = !string.IsNullOrEmpty(gitHubToken) ? gitHubToken : "None";

        // Get the absolute path to the Python script
        string scriptPath = Path.Combine(UnityEngine.Application.dataPath, "Scripts/stats.py");

        if (!File.Exists(scriptPath))
        {
            UnityEngine.Debug.LogError($"Python script not found at path: {scriptPath}");
            return;
        }

        // Wrap the script path in quotes to handle spaces
        string arguments = $"\"{scriptPath}\" {organization} {repository} {token}";

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = "python",  // Ensure 'python' is in your system's PATH
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,  // Capture error messages for debugging
            CreateNoWindow = true
        };

        using (Process process = Process.Start(start))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            UnityEngine.Debug.Log("Python script executed.");
            if (!string.IsNullOrEmpty(output))
                UnityEngine.Debug.Log($"Output: {output}");
            if (!string.IsNullOrEmpty(error))
                UnityEngine.Debug.LogError($"Error: {error}");
        }
    }
}
