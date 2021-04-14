using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseIdInUseException : EsentObsoleteException
	{
		public EsentDatabaseIdInUseException() : base("A database is being assigned an id already in use", JET_err.DatabaseIdInUse)
		{
		}

		private EsentDatabaseIdInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
