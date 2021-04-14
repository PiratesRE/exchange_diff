using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseCorruptedException : EsentCorruptionException
	{
		public EsentDatabaseCorruptedException() : base("Non database file or corrupted db", JET_err.DatabaseCorrupted)
		{
		}

		private EsentDatabaseCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
