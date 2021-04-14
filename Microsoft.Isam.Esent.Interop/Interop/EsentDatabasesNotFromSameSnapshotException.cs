using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabasesNotFromSameSnapshotException : EsentObsoleteException
	{
		public EsentDatabasesNotFromSameSnapshotException() : base("Databases to be restored are not from the same shadow copy backup", JET_err.DatabasesNotFromSameSnapshot)
		{
		}

		private EsentDatabasesNotFromSameSnapshotException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
