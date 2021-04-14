using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseDirtyShutdownException : EsentInconsistentException
	{
		public EsentDatabaseDirtyShutdownException() : base("Database was not shutdown cleanly. Recovery must first be run to properly complete database operations for the previous shutdown.", JET_err.DatabaseDirtyShutdown)
		{
		}

		private EsentDatabaseDirtyShutdownException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
