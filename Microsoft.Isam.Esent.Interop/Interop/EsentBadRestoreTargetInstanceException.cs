using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadRestoreTargetInstanceException : EsentUsageException
	{
		public EsentBadRestoreTargetInstanceException() : base("TargetInstance specified for restore is not found or log files don't match", JET_err.BadRestoreTargetInstance)
		{
		}

		private EsentBadRestoreTargetInstanceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
