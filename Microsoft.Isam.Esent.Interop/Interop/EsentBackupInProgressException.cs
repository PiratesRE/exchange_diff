using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBackupInProgressException : EsentStateException
	{
		public EsentBackupInProgressException() : base("Backup is active already", JET_err.BackupInProgress)
		{
		}

		private EsentBackupInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
