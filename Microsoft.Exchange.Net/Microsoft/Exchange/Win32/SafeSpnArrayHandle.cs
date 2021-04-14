using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Win32
{
	internal sealed class SafeSpnArrayHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeSpnArrayHandle() : base(true)
		{
		}

		public void SetCount(uint count)
		{
			if (this.IsInvalid)
			{
				throw new InvalidOperationException("SetCount() called on an invalid handle");
			}
			this.count = new uint?(count);
		}

		public string[] GetSpnStrings(uint count)
		{
			if (this.IsInvalid)
			{
				throw new InvalidOperationException("GetSpnStrings() called on an invalid handle");
			}
			this.SetCount(count);
			string[] array = new string[count];
			int num = 0;
			while ((long)num < (long)((ulong)count))
			{
				array[num] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(this.handle, num * SafeSpnArrayHandle.SizeOfIntPtr));
				num++;
			}
			return array;
		}

		protected override bool ReleaseHandle()
		{
			if (this.count == null)
			{
				throw new InvalidOperationException("Unknown Spn elements count.  SetCount() or GetSpnStrings() must be called.");
			}
			SafeSpnArrayHandle.DsFreeSpnArray(this.count.Value, this.handle);
			return true;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("NTDSAPI.DLL", CharSet = CharSet.Unicode, EntryPoint = "DsFreeSpnArrayW")]
		private static extern void DsFreeSpnArray([In] uint spnCount, [In] IntPtr spnArray);

		private static readonly int SizeOfIntPtr = Marshal.SizeOf(typeof(IntPtr));

		private uint? count;
	}
}
