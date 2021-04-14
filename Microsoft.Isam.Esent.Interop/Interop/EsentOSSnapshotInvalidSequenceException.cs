using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOSSnapshotInvalidSequenceException : EsentUsageException
	{
		public EsentOSSnapshotInvalidSequenceException() : base("OS Shadow copy API used in an invalid sequence", JET_err.OSSnapshotInvalidSequence)
		{
		}

		private EsentOSSnapshotInvalidSequenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
