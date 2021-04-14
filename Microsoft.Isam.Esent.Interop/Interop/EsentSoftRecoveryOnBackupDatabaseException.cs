using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSoftRecoveryOnBackupDatabaseException : EsentUsageException
	{
		public EsentSoftRecoveryOnBackupDatabaseException() : base("Soft recovery is intended on a backup database. Restore should be used instead", JET_err.SoftRecoveryOnBackupDatabase)
		{
		}

		private EsentSoftRecoveryOnBackupDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
