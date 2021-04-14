using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDiskReadVerificationFailureException : EsentCorruptionException
	{
		public EsentDiskReadVerificationFailureException() : base("The OS returned ERROR_CRC from file IO", JET_err.DiskReadVerificationFailure)
		{
		}

		private EsentDiskReadVerificationFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
