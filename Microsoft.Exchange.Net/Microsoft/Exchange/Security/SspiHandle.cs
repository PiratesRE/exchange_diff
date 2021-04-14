using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct SspiHandle
	{
		public bool IsZero
		{
			get
			{
				return this.HandleHi == IntPtr.Zero && this.HandleLo == IntPtr.Zero;
			}
		}

		public override string ToString()
		{
			return this.HandleHi.ToString("x") + ":" + this.HandleLo.ToString("x");
		}

		private IntPtr HandleHi;

		private IntPtr HandleLo;
	}
}
