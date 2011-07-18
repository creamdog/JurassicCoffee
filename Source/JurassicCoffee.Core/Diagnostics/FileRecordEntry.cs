
using System;
using System.IO;
using JurassicCoffee.Core.IO;

namespace JurassicCoffee.Core.Diagnostics
{
    public class FileRecordEntry : CompilationRecordEntry
    {
        public FileRecordEntry(string file, FileProtocol fileProtocol, FileInsertionMode fileInsertionMode, string description)
        {
            File = file;
            InsertionMode = fileInsertionMode;
            Description = description;
            Protocol = fileProtocol;
        }

        public string File { get; private set; }
        public FileInsertionMode InsertionMode { get; private set; }
        public FileProtocol Protocol { get; private set; }
        
        public enum FileInsertionMode
        {
            Embedded,
            Compiled
        }
    }
}
