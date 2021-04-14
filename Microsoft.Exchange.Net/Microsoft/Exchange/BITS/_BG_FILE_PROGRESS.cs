using System;

namespace Microsoft.Exchange.BITS
{
	internal struct _BG_FILE_PROGRESS
	{
		public ulong BytesTotal;

		public ulong BytesTransferred;

		public int Completed;
	}
}
