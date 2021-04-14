using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfFileHandlesException : EsentMemoryException
	{
		public EsentOutOfFileHandlesException() : base("Out of file handles", JET_err.OutOfFileHandles)
		{
		}

		private EsentOutOfFileHandlesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
