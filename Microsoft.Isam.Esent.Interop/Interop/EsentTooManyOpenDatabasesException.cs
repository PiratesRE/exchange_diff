using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyOpenDatabasesException : EsentObsoleteException
	{
		public EsentTooManyOpenDatabasesException() : base("Too many open databases", JET_err.TooManyOpenDatabases)
		{
		}

		private EsentTooManyOpenDatabasesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
