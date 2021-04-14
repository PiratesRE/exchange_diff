using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SBinary
	{
		public SBinary(byte[] ba)
		{
			this.ba = ba;
		}

		internal int GetBytesToMarshal()
		{
			int num = _SBinary.SizeOf + 7 & -8;
			return num + (this.ba.Length + 7 & -8);
		}

		internal unsafe void MarshalToNative(_SBinary* psbin, ref byte* pbExtra)
		{
			psbin->cb = this.ba.Length;
			psbin->lpb = pbExtra;
			byte* ptr = pbExtra;
			pbExtra += (IntPtr)(this.ba.Length + 7 & -8);
			for (int i = 0; i < this.ba.Length; i++)
			{
				*ptr = this.ba[i];
				ptr++;
			}
		}

		internal byte[] ba;
	}
}
