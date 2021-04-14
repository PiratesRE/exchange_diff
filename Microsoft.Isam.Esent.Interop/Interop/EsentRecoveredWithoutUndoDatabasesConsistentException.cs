using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecoveredWithoutUndoDatabasesConsistentException : EsentStateException
	{
		public EsentRecoveredWithoutUndoDatabasesConsistentException() : base("Soft recovery successfully replayed all operations and intended to skip the Undo phase of recovery, but the Undo phase was not required", JET_err.RecoveredWithoutUndoDatabasesConsistent)
		{
		}

		private EsentRecoveredWithoutUndoDatabasesConsistentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
