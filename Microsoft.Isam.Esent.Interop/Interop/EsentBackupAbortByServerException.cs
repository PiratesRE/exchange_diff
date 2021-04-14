using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBackupAbortByServerException : EsentOperationException
	{
		public EsentBackupAbortByServerException() : base("Backup was aborted by server by calling JetTerm with JET_bitTermStopBackup or by calling JetStopBackup", JET_err.BackupAbortByServer)
		{
		}

		private EsentBackupAbortByServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
