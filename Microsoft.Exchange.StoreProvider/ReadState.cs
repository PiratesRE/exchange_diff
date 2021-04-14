using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct ReadState
	{
		public ReadState(PropValue sourceKey, bool read)
		{
			this.sourceKey = sourceKey;
			this.read = read;
		}

		public PropValue SourceKey
		{
			get
			{
				return this.sourceKey;
			}
			set
			{
				this.sourceKey = value;
			}
		}

		public bool Read
		{
			get
			{
				return this.read;
			}
			set
			{
				this.read = value;
			}
		}

		internal int GetBytesToMarshal()
		{
			int num = _ReadState.SizeOf + 7 & -8;
			return num + (this.sourceKey.GetBytes().Length + 7 & -8);
		}

		internal unsafe void MarshalToNative(_ReadState* pread, ref byte* pbExtra)
		{
			pread->cbSourceKey = this.sourceKey.GetBytes().Length;
			pread->pbSourceKey = pbExtra;
			pread->ulFlags = (this.read ? 1 : 0);
			byte* ptr = pbExtra;
			pbExtra += (IntPtr)(this.sourceKey.GetBytes().Length + 7 & -8);
			for (int i = 0; i < this.sourceKey.GetBytes().Length; i++)
			{
				*ptr = this.sourceKey.GetBytes()[i];
				ptr++;
			}
		}

		private PropValue sourceKey;

		private bool read;
	}
}
