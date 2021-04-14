using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecoveryVerifyFailureException : EsentCorruptionException
	{
		public EsentRecoveryVerifyFailureException() : base("One or more database pages read from disk during recovery do not match the expected state.", JET_err.RecoveryVerifyFailure)
		{
		}

		private EsentRecoveryVerifyFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
