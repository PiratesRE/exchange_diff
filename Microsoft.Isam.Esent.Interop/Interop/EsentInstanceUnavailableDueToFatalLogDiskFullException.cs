using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInstanceUnavailableDueToFatalLogDiskFullException : EsentFatalException
	{
		public EsentInstanceUnavailableDueToFatalLogDiskFullException() : base("This instance cannot be used because it encountered a log-disk-full error performing an operation (likely transaction rollback) that could not tolerate failure", JET_err.InstanceUnavailableDueToFatalLogDiskFull)
		{
		}

		private EsentInstanceUnavailableDueToFatalLogDiskFullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
