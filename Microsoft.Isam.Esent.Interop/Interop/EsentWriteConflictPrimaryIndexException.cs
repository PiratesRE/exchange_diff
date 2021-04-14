using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentWriteConflictPrimaryIndexException : EsentStateException
	{
		public EsentWriteConflictPrimaryIndexException() : base("Update attempted on uncommitted primary index", JET_err.WriteConflictPrimaryIndex)
		{
		}

		private EsentWriteConflictPrimaryIndexException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
