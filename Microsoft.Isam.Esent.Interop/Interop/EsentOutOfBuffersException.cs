using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfBuffersException : EsentMemoryException
	{
		public EsentOutOfBuffersException() : base("Out of database page buffers", JET_err.OutOfBuffers)
		{
		}

		private EsentOutOfBuffersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
