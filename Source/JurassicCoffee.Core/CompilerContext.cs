using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JurassicCoffee.Core
{
    public class CompilerContext
    {
        public string WorkingDirectory { get; set; }
        public Guid Id { get; private set; }

        public CompilerContext()
        {
            Id = Guid.NewGuid();
        }
    }
}
