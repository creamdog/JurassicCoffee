using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace JurassicCoffee.Core.Diagnostics
{
    public class CompilationRecorder
    {
        private DateTime _started;
        private Stopwatch _stopwatch;
        private List<CompilationRecordEntry> _compilationRecordEnties;

        public void AddRecordEntry(CompilationRecordEntry entry)
        {
            _compilationRecordEnties.Add(entry);
        }

        public CompilationRecorder()
        {
            _stopwatch = new Stopwatch();
            _compilationRecordEnties = new List<CompilationRecordEntry>();
        }
        
        public void Start()
        {
            _started = DateTime.Now;
            _stopwatch = new Stopwatch();
            _compilationRecordEnties = new List<CompilationRecordEntry>();
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public CompilationRecord GetRecord()
        {
            return new CompilationRecord(_compilationRecordEnties, _started, _stopwatch.Elapsed);
        }
    }
}
