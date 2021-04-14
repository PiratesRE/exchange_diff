using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentAttachedDatabaseMismatchException : EsentInconsistentException
	{
		public EsentAttachedDatabaseMismatchException() : base("An outstanding database attachment has been detected at the start or end of recovery, but database is missing or does not match attachment info", JET_err.AttachedDatabaseMismatch)
		{
		}

		private EsentAttachedDatabaseMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
