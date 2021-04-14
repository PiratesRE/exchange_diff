using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogDisabledDueToRecoveryFailureException : EsentFatalException
	{
		public EsentLogDisabledDueToRecoveryFailureException() : base("Try to log something after recovery faild", JET_err.LogDisabledDueToRecoveryFailure)
		{
		}

		private EsentLogDisabledDueToRecoveryFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
