using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentExclusiveTableLockRequiredException : EsentUsageException
	{
		public EsentExclusiveTableLockRequiredException() : base("Must have exclusive lock on table.", JET_err.ExclusiveTableLockRequired)
		{
		}

		private EsentExclusiveTableLockRequiredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
