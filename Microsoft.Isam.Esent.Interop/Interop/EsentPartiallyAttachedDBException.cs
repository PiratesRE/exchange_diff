using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentPartiallyAttachedDBException : EsentUsageException
	{
		public EsentPartiallyAttachedDBException() : base("Database is partially attached. Cannot complete attach operation", JET_err.PartiallyAttachedDB)
		{
		}

		private EsentPartiallyAttachedDBException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
