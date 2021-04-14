using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSoftRecoveryOnSnapshotException : EsentObsoleteException
	{
		public EsentSoftRecoveryOnSnapshotException() : base("Soft recovery on a database from a shadow copy backup set", JET_err.SoftRecoveryOnSnapshot)
		{
		}

		private EsentSoftRecoveryOnSnapshotException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
