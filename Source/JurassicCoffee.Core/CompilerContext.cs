using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JurassicCoffee.Core.Diagnostics;

namespace JurassicCoffee.Core
{
    public class CompilerContext
    {
        public string WorkingDirectory { get; set; }
        public Guid Id { get; private set; }
        public CompilationRecorder CompilationRecorder { get; private set; }

        public CompilerContext(CompilationRecorder compilationRecorder)
        {
            CompilationRecorder = compilationRecorder;
            Id = Guid.NewGuid();
        }
    }
}
