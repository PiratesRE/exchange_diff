using System;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	[Serializable]
	internal struct NATIVE_COMMIT_ID
	{
		public NATIVE_SIGNATURE signLog;

		public int reserved;

		public long commitId;
	}
}
