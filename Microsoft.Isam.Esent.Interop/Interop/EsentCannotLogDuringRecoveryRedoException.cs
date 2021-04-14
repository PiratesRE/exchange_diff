using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotLogDuringRecoveryRedoException : EsentErrorException
	{
		public EsentCannotLogDuringRecoveryRedoException() : base("Try to log something during recovery redo", JET_err.CannotLogDuringRecoveryRedo)
		{
		}

		private EsentCannotLogDuringRecoveryRedoException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
