using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogSectorSizeMismatchDatabasesConsistentException : EsentStateException
	{
		public EsentLogSectorSizeMismatchDatabasesConsistentException() : base("databases have been recovered, but the log file sector size (used during recovery) does not match the current volume's sector size", JET_err.LogSectorSizeMismatchDatabasesConsistent)
		{
		}

		private EsentLogSectorSizeMismatchDatabasesConsistentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
