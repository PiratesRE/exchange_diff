using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidDatabaseException : EsentUsageException
	{
		public EsentInvalidDatabaseException() : base("Not a database file", JET_err.InvalidDatabase)
		{
		}

		private EsentInvalidDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
