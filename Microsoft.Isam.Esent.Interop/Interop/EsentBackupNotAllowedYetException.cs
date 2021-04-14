using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBackupNotAllowedYetException : EsentStateException
	{
		public EsentBackupNotAllowedYetException() : base("Cannot do backup now", JET_err.BackupNotAllowedYet)
		{
		}

		private EsentBackupNotAllowedYetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
