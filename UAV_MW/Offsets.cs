using System;
using System.Diagnostics;

namespace VIBES
{
	public class Offsets
	{
		public static IntPtr Proc;

		public static Process gameProc;

		public static ulong BaseAddress;

		public static ulong UAVPtr;

		public static ulong UAV_Offset1;

		public static ulong UAV_Offset2;

		public static ulong UAV_Offset3;

		public static ulong UAV_Offset4;

		public static ulong UAV_Offset5;

		public static ulong UAV_Offset6;

		static Offsets()
		{
			Offsets.BaseAddress = (ulong)0;
			Offsets.UAV_Offset1 = (ulong)388192264;
			Offsets.UAV_Offset2 = Offsets.UAV_Offset1 + (long)32;
			Offsets.UAV_Offset3 = Offsets.UAV_Offset1 + (long)24;
			Offsets.UAV_Offset4 = Offsets.UAV_Offset1 + (long)28;
			Offsets.UAV_Offset5 = Offsets.UAV_Offset1 + (long)56;
			Offsets.UAV_Offset6 = Offsets.UAV_Offset1 + (long)60;
		}

		public Offsets()
		{
		}
	}
}