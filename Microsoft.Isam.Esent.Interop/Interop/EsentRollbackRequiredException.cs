using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRollbackRequiredException : EsentObsoleteException
	{
		public EsentRollbackRequiredException() : base("Must rollback current transaction -- cannot commit or begin a new one", JET_err.RollbackRequired)
		{
		}

		private EsentRollbackRequiredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
