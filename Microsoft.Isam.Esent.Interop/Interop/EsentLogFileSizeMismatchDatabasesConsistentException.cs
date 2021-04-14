using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogFileSizeMismatchDatabasesConsistentException : EsentStateException
	{
		public EsentLogFileSizeMismatchDatabasesConsistentException() : base("databases have been recovered, but the log file size used during recovery does not match JET_paramLogFileSize", JET_err.LogFileSizeMismatchDatabasesConsistent)
		{
		}

		private EsentLogFileSizeMismatchDatabasesConsistentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
