using System.Diagnostics;

namespace UnlockPdf;

public static class PathHelper
{
    public static string CreateUniquePath(string directoryPath, string fileName)
    {
        var filePath = Path.Combine(directoryPath, fileName);
        if (!File.Exists(filePath))
        {
            return filePath;
        }

        var name = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        for (var uniqueNumber = 1; ; uniqueNumber++)
        {
            var candidate = Path.Combine(
                directoryPath,
                name + " (" + uniqueNumber + ")" + extension);
            if (!File.Exists(candidate))
            {
                return candidate;
            }
        }
    }

    public static void OpenExplorerWithSelected(string path)
    {
        Process.Start(
            new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = "/select, \"" + path + "\"",
                UseShellExecute = true
            });
    }
}
