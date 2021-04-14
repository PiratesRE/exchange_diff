using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOSSnapshotInvalidSnapIdException : EsentUsageException
	{
		public EsentOSSnapshotInvalidSnapIdException() : base("invalid JET_OSSNAPID", JET_err.OSSnapshotInvalidSnapId)
		{
		}

		private EsentOSSnapshotInvalidSnapIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
