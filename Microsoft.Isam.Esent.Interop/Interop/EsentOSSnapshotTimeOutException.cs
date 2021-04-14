using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOSSnapshotTimeOutException : EsentOperationException
	{
		public EsentOSSnapshotTimeOutException() : base("OS Shadow copy ended with time-out", JET_err.OSSnapshotTimeOut)
		{
		}

		private EsentOSSnapshotTimeOutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
