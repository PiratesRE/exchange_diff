using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseInvalidNameException : EsentUsageException
	{
		public EsentDatabaseInvalidNameException() : base("Invalid database name", JET_err.DatabaseInvalidName)
		{
		}

		private EsentDatabaseInvalidNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
