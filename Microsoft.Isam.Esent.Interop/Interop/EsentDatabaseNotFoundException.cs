using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseNotFoundException : EsentUsageException
	{
		public EsentDatabaseNotFoundException() : base("No such database", JET_err.DatabaseNotFound)
		{
		}

		private EsentDatabaseNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
