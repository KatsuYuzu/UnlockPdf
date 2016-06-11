using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;

namespace UnlockPdf
{
    public static class PdfUnlocker
    {
        /// <summary>
        /// パスワードを解除します。
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="password"></param>
        public static void Unlock(string sourcePath, string destinationPath, string password)
        {
            using (var reader = new PdfReader(sourcePath, Encoding.UTF8.GetBytes(password)))
            using (var document = new Document())
            using (var writer = PdfWriter.GetInstance(document, File.OpenWrite(destinationPath)))
            {
                document.Open();

                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    writer.NewPage();

                    var page = writer.GetImportedPage(reader, i);

                    writer.DirectContent.AddTemplate(page, 0, 0);
                }

                document.Close();
            }
        }
    }
}
