using System;
using System.Collections.Generic;
using System.IO;

namespace JurassicCoffee.Core.Diagnostics
{
    public class CompilationRecord
    {
        public DateTime CompilationTime { get; private set; }
        public TimeSpan CompilationDuration { get; private set; }
        public IEnumerable<CompilationRecordEntry> Entries { get; private set; }
        
        public CompilationRecord(IEnumerable<CompilationRecordEntry> entries, DateTime compilationTime, TimeSpan compilationDuration)
        {
            Entries = entries;
            CompilationDuration = compilationDuration;
            CompilationTime = compilationTime;
        }
    }
}
