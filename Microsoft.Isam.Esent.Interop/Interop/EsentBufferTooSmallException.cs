using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBufferTooSmallException : EsentStateException
	{
		public EsentBufferTooSmallException() : base("Buffer is too small", JET_err.BufferTooSmall)
		{
		}

		private EsentBufferTooSmallException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
