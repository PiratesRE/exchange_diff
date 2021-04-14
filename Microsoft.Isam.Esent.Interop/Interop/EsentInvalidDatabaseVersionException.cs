using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidDatabaseVersionException : EsentInconsistentException
	{
		public EsentInvalidDatabaseVersionException() : base("Database engine is incompatible with database", JET_err.InvalidDatabaseVersion)
		{
		}

		private EsentInvalidDatabaseVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
