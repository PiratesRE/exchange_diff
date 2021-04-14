using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecoveredWithErrorsException : EsentStateException
	{
		public EsentRecoveredWithErrorsException() : base("Restored with errors", JET_err.RecoveredWithErrors)
		{
		}

		private EsentRecoveredWithErrorsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
