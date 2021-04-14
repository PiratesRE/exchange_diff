using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentWriteConflictException : EsentStateException
	{
		public EsentWriteConflictException() : base("Write lock failed due to outstanding write lock", JET_err.WriteConflict)
		{
		}

		private EsentWriteConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
