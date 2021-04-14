using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseInvalidIncrementalReseedException : EsentUsageException
	{
		public EsentDatabaseInvalidIncrementalReseedException() : base("The database is not a valid state to perform an incremental reseed.", JET_err.DatabaseInvalidIncrementalReseed)
		{
		}

		private EsentDatabaseInvalidIncrementalReseedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
