using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentNoBackupException : EsentStateException
	{
		public EsentNoBackupException() : base("No backup in progress", JET_err.NoBackup)
		{
		}

		private EsentNoBackupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
