using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseAlreadyRunningMaintenanceException : EsentUsageException
	{
		public EsentDatabaseAlreadyRunningMaintenanceException() : base("The operation did not complete successfully because the database is already running maintenance on specified database", JET_err.DatabaseAlreadyRunningMaintenance)
		{
		}

		private EsentDatabaseAlreadyRunningMaintenanceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
