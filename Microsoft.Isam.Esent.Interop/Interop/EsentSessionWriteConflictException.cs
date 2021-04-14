using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSessionWriteConflictException : EsentUsageException
	{
		public EsentSessionWriteConflictException() : base("Attempt to replace the same record by two diffrerent cursors in the same session", JET_err.SessionWriteConflict)
		{
		}

		private EsentSessionWriteConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
