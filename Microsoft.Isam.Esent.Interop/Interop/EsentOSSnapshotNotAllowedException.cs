using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOSSnapshotNotAllowedException : EsentStateException
	{
		public EsentOSSnapshotNotAllowedException() : base("OS Shadow copy not allowed (backup or recovery in progress)", JET_err.OSSnapshotNotAllowed)
		{
		}

		private EsentOSSnapshotNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
