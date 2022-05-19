using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public partial class MIPSSimulator
    {
        public Form1 gui;
        public List<Register> Registers;
        string[] InstructionSet;
        public List<StackData> Stack;
        public List<string> InputText;
        public List<string> InputData;
        string CurrentInstruction;
        int CurrentLine;
        public int LastLine;
        public List<MemoryData> MemoryTable = new List<MemoryData>();
        List<LabelData> LabelTable = new List<LabelData>();
        public bool isHalted = false;
        public bool DoneFlag = false;
        bool PreProcessFlag = false;
        public int[] args = new int[3]; //register names,values... for the instruction.
        bool FPA = false;
        public int MFLO = 0;
        public int MFHI = 0;
        public void InputChanged() => PreProcessFlag = true;
        public MIPSSimulator(string[] text,string[] data, Form1 sender)
        {
            this.gui = sender;
            Random rand = new Random(Environment.TickCount);

            Registers = new List<Register>(32);
            string[] tempRegisters = new string[]{"$zero","$at","$v0","$v1","$a0","$a1","$a2","$a3","$t0","$t1","$t2",
                "$t3","$t4","$t5","$t6","$t7","$s0","$s1","$s2","$s3","$s4","$s5","$s6","$s7","$t8","$t9","$k0","$k1",
                "$gp","$sp","$fp","$ra"};
            for (int i = 0; i < tempRegisters.Length; i++)
            {
                Registers.Add(new Register(tempRegisters[i], "0"));
            }
            for(int i = 0; i< 32; i++)
            {
                Registers.Add(new Register("$"+ (i < 10 ? "0" : "") + i, "0"));
            }

            string[] tempInstructionSet = new string[] { "add", "sub", "and", "or", "slt",                                                                     "add.d","sub.d","mul.d","div.d","c.eq.d","c.lt.d","mult","div"
                                                        ,"addi", "andi", "ori", "slti"
                                                        ,"lw", "sw"                                                                                              ,"ldc1","sdc1"
                                                        , "beq", "blt"
                                                        , "j" };
            InstructionSet = new string[tempInstructionSet.Length];
            for (int i = 0; i < tempInstructionSet.Length; i++)
            {
                InstructionSet[i] = tempInstructionSet[i];
            }

            Stack = new List<StackData>(100);
            for (int i = 0; i < 100; i++)
            {
                Stack.Add(new StackData("0"));
            }
            Registers[28].Value = "10000000"; //gp
            Registers[29].Value = "40396"; //sp
            Registers[30].Value = "40396"; //fp


            InputText = new List<string>(text);
            InputData = new List<string>(data);

        }
        
    }
}
