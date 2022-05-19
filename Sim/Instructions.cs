using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
	public partial class MIPSSimulator
	{
		private void OpError()
        {
			gui.ReportError("Error: invalid usage of instruction");
			return;
		}
		void add()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) + int.Parse(Registers[args[1]].Value)))
					return;
			} 
			if(args[0] != 0 && args[0] != 1 &&  args[1] != 1 && args[2] != 1)
            {
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) + int.Parse(Registers[args[2]].Value)).ToString();
            }
            else
            {
				OpError();
            }
		}
		void addi()
		{
			bool spFlag = false;
			if(args[0] == 29)
            {
				spFlag = true;
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) + args[2]*4))
					return;
            }
			if(args[0] != 0 && args[0] != 1 && args[1] != 1)
            {
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) + (spFlag?args[2]*4:args[2])).ToString();
            }
            else
            {
				OpError();
				return;
            }
		}
		void sub()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) - int.Parse(Registers[args[1]].Value)))
					return;
			}
			if (args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) - int.Parse(Registers[args[2]].Value)).ToString();
			}
			else
			{
				OpError();
			}
		}
		void and()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) & int.Parse(Registers[args[1]].Value)))
					return;
			}
			if (args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) & int.Parse(Registers[args[2]].Value)).ToString();
			}
			else
			{
				OpError();
			}
		}
		void andi()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) & args[2]))
					return;
			}
			if (args[0] != 0 && args[0] != 1 && args[1] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) & args[2]).ToString();
			}
			else
			{
				OpError();
				return;
			}

		}
		void or()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) | int.Parse(Registers[args[1]].Value)))
					return;
			}
			if (args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) | int.Parse(Registers[args[2]].Value)).ToString();
			}
			else
			{
				OpError();
			}
		}
		void ori()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) | args[2]))
					return;
			}
			if (args[0] != 0 && args[0] != 1 && args[1] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) | args[2]).ToString();
			}
			else
			{
				OpError();
				return;
			}
		}
		void slt()
		{
			if(args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
            {
				Registers[args[0]].Value =(int.Parse(Registers[args[1]].Value) < int.Parse(Registers[args[2]].Value)) ? "1":"0";
            }
            else
            {
				OpError();
				return;
            }
		}
		void slti()
		{
			if (args[0] != 0 && args[0] != 1 && args[1] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) < args[2]) ? "1" : "0";
			}
			else
			{
				OpError();
				return;
			}
		}
		void lw()
		{
			if(args[0] == 29)
            {
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value + args[2]*4)))
					return;
            }
			if(args[0] != 0 && args[0] != 1 && args[2] == -1)
            {
				Registers[args[0]].Value = MemoryTable[args[1]].Value;
            }
			else if(args[0] != 0 && args[0] != 1) 
			{
				int offsetPos = ((int.Parse(Registers[args[1]].Value) - 40000) / 4 + args[2] + 1);
				if (offsetPos > 99 || offsetPos < 0)
				{
					OpError();
					return;
				}
				Registers[args[0]].Value = Stack[offsetPos].Value;
			}
            else
            {
				OpError();
				return;
            }
		}
        void sw()
		{
			if(args[0] != 1 && args[2] == -1)
            {
				MemoryTable[args[1]].Value = Registers[args[0]].Value.ToString();
            }else if(args[0] != 1)
            {
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) + args[2] * 4))
                {
					OpError();
					return;
                }
				int offsetPos = ((int.Parse(Registers[args[1]].Value) -40000)/4 + args[2] +1);
				if (offsetPos > 99 || offsetPos < 0)
                {
					OpError();
					return;
                }
				Stack[offsetPos].Value = Registers[args[0]].Value;
            }
            else
            {
				OpError();
				return;
            }
		}
		void beq()
		{
			if (args[0] != 1 && args[1] != 1)
			{
				LastLine = CurrentLine;
				if (int.Parse(Registers[args[0]].Value) == int.Parse(Registers[args[1]].Value))
				{
					CurrentLine = args[2];
				}
				else
					CurrentLine++;
            }
            else
            {
				OpError();
            }
		}
		void blt()
		{
			if(args[0] != 1 && args[1] != 1)
            {
				LastLine = CurrentLine;
				if ((int.Parse((Registers[args[0]].Value))) < (int.Parse(Registers[args[1]].Value)))
				{
					CurrentLine = args[2];
				}
				else
					CurrentLine++;
            }
			else
			{
				OpError();
			}
		}
		void j()
		{
			LastLine = CurrentLine;
			CurrentLine = args[0];
		}
	}
}
