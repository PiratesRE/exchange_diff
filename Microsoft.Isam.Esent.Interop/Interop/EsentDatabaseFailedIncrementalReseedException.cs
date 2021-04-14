using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseFailedIncrementalReseedException : EsentStateException
	{
		public EsentDatabaseFailedIncrementalReseedException() : base("The incremental reseed being performed on the specified database cannot be completed due to a fatal error.  A full reseed is required to recover this database.", JET_err.DatabaseFailedIncrementalReseed)
		{
		}

		private EsentDatabaseFailedIncrementalReseedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
