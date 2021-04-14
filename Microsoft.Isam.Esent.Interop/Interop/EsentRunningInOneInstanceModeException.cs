using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRunningInOneInstanceModeException : EsentUsageException
	{
		public EsentRunningInOneInstanceModeException() : base("Multi-instance call with single-instance mode enabled", JET_err.RunningInOneInstanceMode)
		{
		}

		private EsentRunningInOneInstanceModeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
