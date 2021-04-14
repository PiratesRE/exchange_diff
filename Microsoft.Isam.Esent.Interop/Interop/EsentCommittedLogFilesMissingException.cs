using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCommittedLogFilesMissingException : EsentCorruptionException
	{
		public EsentCommittedLogFilesMissingException() : base("One or more logs that were committed to this database, are missing.  These log files are required to maintain durable ACID semantics, but not required to maintain consistency if the JET_bitReplayIgnoreLostLogs bit is specified during recovery.", JET_err.CommittedLogFilesMissing)
		{
		}

		private EsentCommittedLogFilesMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
