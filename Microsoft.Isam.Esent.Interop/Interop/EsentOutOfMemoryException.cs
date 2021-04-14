using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfMemoryException : EsentMemoryException
	{
		public EsentOutOfMemoryException() : base("Out of Memory", JET_err.OutOfMemory)
		{
		}

		private EsentOutOfMemoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
