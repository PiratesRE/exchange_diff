using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMissingFullBackupException : EsentStateException
	{
		public EsentMissingFullBackupException() : base("The database missed a previous full backup before incremental backup", JET_err.MissingFullBackup)
		{
		}

		private EsentMissingFullBackupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
