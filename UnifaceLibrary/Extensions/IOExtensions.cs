using System.IO;

namespace UnifaceLibrary
{
    public static class IOExtensions
    {
        public static DirectoryInfo CombinePath(this DirectoryInfo directory, string path)
        {
            return new DirectoryInfo(Path.Combine(directory.FullName, path));
        }

        public static FileInfo CombineFile(this DirectoryInfo directory, string fileName)
        {
            return new FileInfo(Path.Combine(directory.FullName, fileName));
        }

        public static void Rename(this FileInfo file, string newName)
        {
            var dest = file.Directory.CombineFile(newName).FullName;
            File.Move(file.FullName, dest);
        }
    }
}
