using QPdfSharp;
using QPdfSharp.Options;

namespace UnlockPdf;

public static class PdfUnlocker
{
    public static void Unlock(string sourcePath, string destinationPath, string password)
    {
        var temporaryPath = destinationPath + "." + Guid.NewGuid().ToString("N") + ".tmp";

        try
        {
            using var document = Open(sourcePath, password);
            try
            {
                document.WriteFile(
                    temporaryPath,
                    new QPdfWriteOptions
                    {
                        PreserveEncryption = false
                    });
            }
            catch (QPdfException exception)
            {
                throw new InvalidDataException("The PDF could not be written.", exception);
            }

            File.Move(temporaryPath, destinationPath);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private static QPdf Open(string sourcePath, string password)
    {
        try
        {
            return new QPdf(sourcePath, password, new QPdfReadOptions());
        }
        catch (QPdfException exception) when (
            exception.Message.Contains("invalid password", StringComparison.OrdinalIgnoreCase))
        {
            throw new PdfPasswordException("The PDF password is invalid.", exception);
        }
        catch (QPdfException exception)
        {
            throw new InvalidDataException("The PDF could not be opened.", exception);
        }
    }
}

public sealed class PdfPasswordException : Exception
{
    public PdfPasswordException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
