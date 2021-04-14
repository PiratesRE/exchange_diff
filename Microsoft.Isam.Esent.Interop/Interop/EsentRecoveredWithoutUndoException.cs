using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecoveredWithoutUndoException : EsentStateException
	{
		public EsentRecoveredWithoutUndoException() : base("Soft recovery successfully replayed all operations, but the Undo phase of recovery was skipped", JET_err.RecoveredWithoutUndo)
		{
		}

		private EsentRecoveredWithoutUndoException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
