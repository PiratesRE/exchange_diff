using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyOpenTablesAndCleanupTimedOutException : EsentUsageException
	{
		public EsentTooManyOpenTablesAndCleanupTimedOutException() : base("Cannot open any more tables (cleanup attempt failed to complete)", JET_err.TooManyOpenTablesAndCleanupTimedOut)
		{
		}

		private EsentTooManyOpenTablesAndCleanupTimedOutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
