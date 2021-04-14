using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseFileReadOnlyException : EsentUsageException
	{
		public EsentDatabaseFileReadOnlyException() : base("Tried to attach a read-only database file for read/write operations", JET_err.DatabaseFileReadOnly)
		{
		}

		private EsentDatabaseFileReadOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
