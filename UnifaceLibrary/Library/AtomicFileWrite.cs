using System;
using System.IO;

namespace UnifaceLibrary
{
    public class AtomicFileWrite : IDisposable
    {
        private FileInfo _tempFilePath;
        private readonly FileInfo _filePath;
        private bool committed = false;

        /// <summary>
        /// Creates a file to specified path, and only on successful commit is the file actually written.
        /// This is achieved by writing to a temporary file location and then deleting the existing file
        /// and renaming on Dispose.
        /// Should be used within a using statment.
        /// Will rollback if Commit() is not called before Dispose().
        /// </summary>
        /// <param name="filePath">The file to create</param>
        public AtomicFileWrite(FileInfo filePath)
        {
            _filePath = filePath ?? throw new System.ArgumentNullException(nameof(filePath));
            _tempFilePath = new FileInfo(filePath.FullName + ".tmp");
        }

        public FileStream OpenWrite()
        {
            return _tempFilePath.OpenWrite();
        }

        /// <summary>
        /// Flag to commit the change on Dispose.
        /// Cannot commit change immediately because the file stream may be open.
        /// </summary>
        public void Commit()
        {
            committed = true;
        }

        public void Dispose()
        {
            if (_tempFilePath != null)
            {
                if (committed)
                {
                    _filePath.Delete();
                    _tempFilePath.Rename(_filePath.Name);
                }
                else    // Rollback
                {
                    _tempFilePath.Delete();
                }
            }

            _tempFilePath = null;
        }
    }
}