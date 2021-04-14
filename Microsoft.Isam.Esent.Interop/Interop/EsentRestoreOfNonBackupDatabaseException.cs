using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRestoreOfNonBackupDatabaseException : EsentUsageException
	{
		public EsentRestoreOfNonBackupDatabaseException() : base("hard recovery attempted on a database that wasn't a backup database", JET_err.RestoreOfNonBackupDatabase)
		{
		}

		private EsentRestoreOfNonBackupDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
