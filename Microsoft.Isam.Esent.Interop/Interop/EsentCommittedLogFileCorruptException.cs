using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCommittedLogFileCorruptException : EsentCorruptionException
	{
		public EsentCommittedLogFileCorruptException() : base("One or more logs were found to be corrupt during recovery.  These log files are required to maintain durable ACID semantics, but not required to maintain consistency if the JET_bitIgnoreLostLogs bit and JET_paramDeleteOutOfRangeLogs is specified during recovery.", JET_err.CommittedLogFileCorrupt)
		{
		}

		private EsentCommittedLogFileCorruptException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
