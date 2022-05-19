using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public class StackData
    {
        private static int StaticMemory = 40000;
        private string value;
        private int memory;

        public StackData(string value)
        {
            Value = value;
            Memory = StaticMemory;
            StaticMemory += 4;
        }

        public int Memory { get => memory; set => memory = value; }
        public string Value { get => value; set => this.value = value; }
    }
}
