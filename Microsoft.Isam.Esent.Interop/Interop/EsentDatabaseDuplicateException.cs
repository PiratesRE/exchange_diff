using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseDuplicateException : EsentUsageException
	{
		public EsentDatabaseDuplicateException() : base("Database already exists", JET_err.DatabaseDuplicate)
		{
		}

		private EsentDatabaseDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
