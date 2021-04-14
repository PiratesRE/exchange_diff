using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseLogSetMismatchException : EsentInconsistentException
	{
		public EsentDatabaseLogSetMismatchException() : base("Database does not belong with the current set of log files", JET_err.DatabaseLogSetMismatch)
		{
		}

		private EsentDatabaseLogSetMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
