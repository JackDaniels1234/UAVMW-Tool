using System;
using System.Diagnostics;
using System.Threading;

namespace VIBES
{
	public class Threads
	{
		public Memory m = new Memory();

		public Form1 main = Form1.main;

		public Threads()
		{
		}

		public void PointerThread()
		{
			while (true)
			{
				try
				{
					if (this.m.IsOpen())
					{
						Offsets.gameProc = Process.GetProcessesByName("ModernWarfare")[0];
						Offsets.Proc = Memory.OpenProcess(2035711, false, Offsets.gameProc.Id);
						Offsets.BaseAddress = (ulong)Memory.GetBaseAddress("ModernWarfare");
						this.m.AttackProcess("ModernWarfare");
						Offsets.UAVPtr = this.m.ReadInt64(Offsets.BaseAddress + Offsets.UAV_Offset1);
					}
					else
					{
						this.m.AttackProcess("ModernWarfare");
					}
					Thread.Sleep(1000);
				}
				catch
				{
				}
			}
		}
	}
}