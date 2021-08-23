using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace VIBES
{
	public class Memory
	{
		public const uint PROCESS_VM_READ = 16;

		public const uint PROCESS_VM_WRITE = 32;

		public const uint PROCESS_VM_OPERATION = 8;

		public const uint PAGE_READWRITE = 4;

		private Process CurProcess;

		private IntPtr ProcessHandle;

		private string ProcessName;

		private int ProcessID;

		public IntPtr BaseModule;

		public Memory()
		{
		}

		public bool AttackProcess(string _ProcessName)
		{
			Process[] processesByName = Process.GetProcessesByName(_ProcessName);
			if (processesByName.Length == 0)
			{
				return false;
			}
			this.BaseModule = processesByName[0].MainModule.BaseAddress;
			this.CurProcess = processesByName[0];
			this.ProcessID = processesByName[0].Id;
			this.ProcessName = _ProcessName;
			this.ProcessHandle = Memory.OpenProcess(56, false, this.ProcessID);
			return this.ProcessHandle != IntPtr.Zero;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

		~Memory()
		{
			if (this.ProcessHandle != IntPtr.Zero)
			{
				Memory.CloseHandle(this.ProcessHandle);
			}
		}

		internal static IntPtr GetBaseAddress(string ProcessName)
		{
			IntPtr baseAddress;
			try
			{
				baseAddress = Process.GetProcessesByName(ProcessName)[0].MainModule.BaseAddress;
			}
			catch
			{
				baseAddress = IntPtr.Zero;
			}
			return baseAddress;
		}

		public static IntPtr GetModuleBaseAddress(Process proc, string modName)
		{
			IntPtr zero = IntPtr.Zero;
			foreach (ProcessModule module in proc.Modules)
			{
				if (module.ModuleName != modName)
				{
					continue;
				}
				zero = module.BaseAddress;
				return zero;
			}
			return zero;
		}

		public ulong GetPointer(params ulong[] args)
		{
			ulong num = (ulong)0;
			for (int i = 0; i <= (int)args.Length - 1; i++)
			{
				if (i == (int)args.Length - 1)
				{
					num += args[i];
				}
				else
				{
					num = this.ReadInt64(num + args[i]);
				}
			}
			return num;
		}

		public ulong GetPointerInt(ulong add, ulong[] offsets, int level)
		{
			ulong num = add;
			for (int i = 0; i < level; i++)
			{
				num = this.ReadInt64(num) + offsets[i];
			}
			return num;
		}

		public bool IsOpen()
		{
			if (this.ProcessName == string.Empty)
			{
				return false;
			}
			return this.AttackProcess(this.ProcessName);
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr OpenProcess(uint dwAccess, bool inherit, int pid);

		public ulong Read12Byte(ulong _lpBaseAddress, byte[] array)
		{
			IntPtr intPtr;
			Memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, array, (ulong)12, out intPtr);
			return BitConverter.ToUInt64(array, 0);
		}

		public byte[] ReadBytes(ulong _lpBaseAddress, int Length)
		{
			IntPtr intPtr;
			byte[] numArray = new byte[Length];
			Memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, numArray, (ulong)12, out intPtr);
			return numArray;
		}

		public float ReadFloat(ulong _lpBaseAddress)
		{
			IntPtr intPtr;
			byte[] numArray = new byte[4];
			Memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, numArray, (ulong)4, out intPtr);
			return BitConverter.ToSingle(numArray, 0);
		}

		public int ReadInt32(ulong _lpBaseAddress)
		{
			IntPtr intPtr;
			byte[] numArray = new byte[4];
			Memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, numArray, (ulong)4, out intPtr);
			return BitConverter.ToInt32(numArray, 0);
		}

		public ulong ReadInt64(ulong _lpBaseAddress)
		{
			IntPtr intPtr;
			byte[] numArray = new byte[8];
			Memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, numArray, (ulong)8, out intPtr);
			return BitConverter.ToUInt64(numArray, 0);
		}

		public ulong ReadPointerInt(ulong add, ulong[] offsets, int level)
		{
			ulong num = add;
			for (int i = 0; i < level; i++)
			{
				num = this.ReadInt64(num) + offsets[i];
			}
			return this.ReadInt64(num);
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern bool ReadProcessMemory(IntPtr hProcess, ulong lpBaseAddress, [In][Out] byte[] lpBuffer, ulong dwSize, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", CharSet=CharSet.None, EntryPoint="ReadProcessMemory", ExactSpelling=false)]
		protected static extern bool ReadProcessMemory2(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesRead);

		public string ReadString(ulong _lpBaseAddress)
		{
			IntPtr intPtr;
			byte[] numArray = new byte[1280];
			if (!Memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, numArray, (ulong)1280, out intPtr))
			{
				return "";
			}
			string str = "";
			for (int i = 0; numArray[i] != 0; i++)
			{
				char chr = (char)numArray[i];
				str = string.Concat(str, chr.ToString());
			}
			return str;
		}

		public uint ReadUInt32(ulong _lpBaseAddress)
		{
			IntPtr intPtr;
			byte[] numArray = new byte[4];
			Memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, numArray, (ulong)4, out intPtr);
			return BitConverter.ToUInt32(numArray, 0);
		}

		public ulong ReadUInt64(ulong _lpBaseAddress)
		{
			IntPtr intPtr;
			byte[] numArray = new byte[8];
			Memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, numArray, (ulong)8, out intPtr);
			return BitConverter.ToUInt64(numArray, 0);
		}

		public int ReadXInt32(ulong _lpBaseAddress, byte[] array)
		{
			IntPtr intPtr;
			Memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, array, (ulong)4, out intPtr);
			return BitConverter.ToInt32(array, 0);
		}

		[DllImport("kernel32", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=true, SetLastError=true)]
		private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

		public void WriteBool(ulong pAddress, bool value)
		{
			IntPtr intPtr;
			try
			{
				byte[] numArray = new byte[] { (byte)(value ? 1 : 0) };
				Memory.WriteProcessMemory(this.ProcessHandle, pAddress, numArray, (ulong)((int)numArray.Length), out intPtr);
			}
			catch (Exception exception)
			{
			}
		}

		public void WriteByte(ulong _lpBaseAddress, byte _Value)
		{
			byte[] bytes = BitConverter.GetBytes((short)_Value);
			IntPtr zero = IntPtr.Zero;
			Memory.WriteProcessMemory(this.ProcessHandle, _lpBaseAddress, bytes, (ulong)((int)bytes.Length), out zero);
		}

		public void WriteBytes(ulong _lpBaseAddress, byte[] buffer)
		{
			IntPtr intPtr;
			Memory.WriteProcessMemory(this.ProcessHandle, _lpBaseAddress, buffer, (ulong)((int)buffer.Length), out intPtr);
		}

		public void WriteFloat(ulong _lpBaseAddress, float _Value)
		{
			this.WriteMemory(_lpBaseAddress, BitConverter.GetBytes(_Value));
		}

		public void WriteInt16(ulong _lpBaseAddress, short _Value)
		{
			this.WriteMemory(_lpBaseAddress, BitConverter.GetBytes(_Value));
		}

		public void WriteInt32(ulong _lpBaseAddress, int _Value)
		{
			this.WriteMemory(_lpBaseAddress, BitConverter.GetBytes(_Value));
		}

		public void WriteInt64(ulong _lpBaseAddress, long _Value)
		{
			this.WriteMemory(_lpBaseAddress, BitConverter.GetBytes(_Value));
		}

		public void WriteMemory(ulong MemoryAddress, byte[] Buffer)
		{
			uint num;
			IntPtr intPtr;
			Memory.VirtualProtectEx(this.ProcessHandle, (IntPtr)MemoryAddress, (uint)Buffer.Length, 4, out num);
			Memory.WriteProcessMemory(this.ProcessHandle, MemoryAddress, Buffer, (ulong)((int)Buffer.Length), out intPtr);
		}

		public void WriteNOP(ulong Address)
		{
			byte[] numArray = new byte[] { 144, 144, 144, 144, 144 };
			IntPtr zero = IntPtr.Zero;
			Memory.WriteProcessMemory(this.ProcessHandle, Address, numArray, (ulong)((int)numArray.Length), out zero);
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool WriteProcessMemory(IntPtr hProcess, ulong lpBaseAddress, [In][Out] byte[] lpBuffer, ulong dwSize, out IntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", CharSet=CharSet.None, EntryPoint="WriteProcessMemory", ExactSpelling=false)]
		private static extern bool WriteProcessMemory2(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, [Out] int lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern bool WriteProcessMemory3(IntPtr hProcess, IntPtr lpBaseAddress, object lpBuffer, long nSize, out IntPtr lpNumberOfBytesWritten);

		public void WriteString(ulong Address, string Text)
		{
			byte[] bytes = (new ASCIIEncoding()).GetBytes(Text);
			IntPtr zero = IntPtr.Zero;
			Memory.WriteProcessMemory(this.ProcessHandle, Address, bytes, (ulong)this.ReadString(Address).Length, out zero);
		}

		public void WriteUInt32(ulong _lpBaseAddress, uint _Value)
		{
			this.WriteMemory(_lpBaseAddress, BitConverter.GetBytes(_Value));
		}

		public void WriteXBytes(ulong _lpBaseAddress, byte[] _Value)
		{
			byte[] numArray = _Value;
			IntPtr zero = IntPtr.Zero;
			Memory.WriteProcessMemory(this.ProcessHandle, _lpBaseAddress, numArray, (ulong)((int)numArray.Length), out zero);
		}

		public void WriteXString(ulong pAddress, string pString)
		{
			try
			{
				IntPtr zero = IntPtr.Zero;
				if (Memory.WriteProcessMemory(this.ProcessHandle, pAddress, Encoding.UTF8.GetBytes(pString), (ulong)pString.Length, out zero))
				{
					byte[] numArray = new byte[1];
					Memory.WriteProcessMemory(this.ProcessHandle, pAddress + (ulong)pString.Length, numArray, (ulong)1, out zero);
				}
			}
			catch (Exception exception)
			{
			}
		}

		[Flags]
		public enum ProcessAccessFlags : uint
		{
			Terminate = 1,
			CreateThread = 2,
			VirtualMemoryOperation = 8,
			VirtualMemoryRead = 16,
			VirtualMemoryWrite = 32,
			DuplicateHandle = 64,
			CreateProcess = 128,
			SetQuota = 256,
			SetInformation = 512,
			QueryInformation = 1024,
			QueryLimitedInformation = 4096,
			Synchronize = 1048576,
			All = 2035711
		}
	}
}