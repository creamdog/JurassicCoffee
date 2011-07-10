
using System;
using System.IO;

namespace JurassicCoffee.Core.Diagnostics
{
    public class FileRecordEntry : CompilationRecordEntry
    {
        public FileRecordEntry(FileInfo fileInfo, FileInsertionMode fileInsertionMode, string description)
        {
            FileInfo = fileInfo;
            InsertionMode = fileInsertionMode;
            Description = description;
        }

        public FileInfo FileInfo { get; private set; }
        public FileInsertionMode InsertionMode { get; private set; }
        
        public enum FileInsertionMode
        {
            Embedded,
            Compiled
        }

        
    }
}
