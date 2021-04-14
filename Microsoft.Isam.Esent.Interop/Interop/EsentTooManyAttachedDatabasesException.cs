using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyAttachedDatabasesException : EsentUsageException
	{
		public EsentTooManyAttachedDatabasesException() : base("Too many open databases", JET_err.TooManyAttachedDatabases)
		{
		}

		private EsentTooManyAttachedDatabasesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
