using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRunningInMultiInstanceModeException : EsentUsageException
	{
		public EsentRunningInMultiInstanceModeException() : base("Single-instance call with multi-instance mode enabled", JET_err.RunningInMultiInstanceMode)
		{
		}

		private EsentRunningInMultiInstanceModeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
