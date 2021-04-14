using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfCursorsException : EsentMemoryException
	{
		public EsentOutOfCursorsException() : base("Out of table cursors", JET_err.OutOfCursors)
		{
		}

		private EsentOutOfCursorsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
