using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentVersionStoreOutOfMemoryAndCleanupTimedOutException : EsentUsageException
	{
		public EsentVersionStoreOutOfMemoryAndCleanupTimedOutException() : base("Version store out of memory (and cleanup attempt failed to complete)", JET_err.VersionStoreOutOfMemoryAndCleanupTimedOut)
		{
		}

		private EsentVersionStoreOutOfMemoryAndCleanupTimedOutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
