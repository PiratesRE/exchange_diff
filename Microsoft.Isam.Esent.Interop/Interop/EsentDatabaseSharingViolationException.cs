using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseSharingViolationException : EsentUsageException
	{
		public EsentDatabaseSharingViolationException() : base("A different database instance is using this database", JET_err.DatabaseSharingViolation)
		{
		}

		private EsentDatabaseSharingViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
