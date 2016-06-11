using System.IO;

namespace UnlockPdf
{
    public static class PathHelper
    {
        private static readonly string NumberPattern = " ({0})";

        /// <summary>
        /// 固有のパスを生成します。
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CreateUniquePath(string directoryPath, string fileName)
        {
            return CreateUniquePath(directoryPath, fileName, 0);
        }

        /// <summary>
        /// 固有のパスを生成します。
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="fileName"></param>
        /// <param name="uniqueNumber"></param>
        /// <returns></returns>
        private static string CreateUniquePath(string directoryPath, string fileName, int uniqueNumber)
        {
            var result = uniqueNumber > 0
                ? Path.Combine(directoryPath, fileName.Insert(fileName.LastIndexOf(Path.GetExtension(fileName)), string.Format(NumberPattern, uniqueNumber)))
                : Path.Combine(directoryPath, fileName);

            return File.Exists(result)
                ? CreateUniquePath(directoryPath, fileName, uniqueNumber + 1)
                : result;
        }
    }
}
