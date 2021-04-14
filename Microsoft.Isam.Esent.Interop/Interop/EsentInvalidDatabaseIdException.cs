using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidDatabaseIdException : EsentUsageException
	{
		public EsentInvalidDatabaseIdException() : base("Invalid database id", JET_err.InvalidDatabaseId)
		{
		}

		private EsentInvalidDatabaseIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
