using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogReadVerifyFailureException : EsentCorruptionException
	{
		public EsentLogReadVerifyFailureException() : base("Checksum error in log file during backup", JET_err.LogReadVerifyFailure)
		{
		}

		private EsentLogReadVerifyFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
