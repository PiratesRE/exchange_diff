using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseLockedException : EsentUsageException
	{
		public EsentDatabaseLockedException() : base("Database exclusively locked", JET_err.DatabaseLocked)
		{
		}

		private EsentDatabaseLockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
