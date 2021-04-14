using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadBackupDatabaseSizeException : EsentObsoleteException
	{
		public EsentBadBackupDatabaseSizeException() : base("The backup database size is not in 4k", JET_err.BadBackupDatabaseSize)
		{
		}

		private EsentBadBackupDatabaseSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
