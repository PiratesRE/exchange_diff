using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLoggingDisabledException : EsentUsageException
	{
		public EsentLoggingDisabledException() : base("Log is not active", JET_err.LoggingDisabled)
		{
		}

		private EsentLoggingDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
