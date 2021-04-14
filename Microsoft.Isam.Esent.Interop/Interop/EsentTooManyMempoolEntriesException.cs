using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyMempoolEntriesException : EsentMemoryException
	{
		public EsentTooManyMempoolEntriesException() : base("Too many mempool entries requested", JET_err.TooManyMempoolEntries)
		{
		}

		private EsentTooManyMempoolEntriesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
