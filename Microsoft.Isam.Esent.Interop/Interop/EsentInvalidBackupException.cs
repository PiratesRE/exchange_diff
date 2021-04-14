using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidBackupException : EsentUsageException
	{
		public EsentInvalidBackupException() : base("Cannot perform incremental backup when circular logging enabled", JET_err.InvalidBackup)
		{
		}

		private EsentInvalidBackupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
