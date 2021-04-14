using System;

namespace Microsoft.Exchange.BITS
{
	internal struct _BG_JOB_PROGRESS
	{
		public ulong BytesTotal;

		public ulong BytesTransferred;

		public uint FilesTotal;

		public uint FilesTransferred;
	}
}
