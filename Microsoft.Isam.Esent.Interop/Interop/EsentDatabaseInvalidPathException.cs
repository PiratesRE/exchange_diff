using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseInvalidPathException : EsentUsageException
	{
		public EsentDatabaseInvalidPathException() : base("Specified path to database file is illegal", JET_err.DatabaseInvalidPath)
		{
		}

		private EsentDatabaseInvalidPathException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
