using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseStreamingFileMismatchException : EsentObsoleteException
	{
		public EsentDatabaseStreamingFileMismatchException() : base("Database and streaming file do not match each other", JET_err.DatabaseStreamingFileMismatch)
		{
		}

		private EsentDatabaseStreamingFileMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
