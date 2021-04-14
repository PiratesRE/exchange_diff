using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseInvalidPagesException : EsentUsageException
	{
		public EsentDatabaseInvalidPagesException() : base("Invalid number of pages", JET_err.DatabaseInvalidPages)
		{
		}

		private EsentDatabaseInvalidPagesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
