using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseInUseException : EsentUsageException
	{
		public EsentDatabaseInUseException() : base("Database in use", JET_err.DatabaseInUse)
		{
		}

		private EsentDatabaseInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
