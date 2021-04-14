using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidBufferSizeException : EsentStateException
	{
		public EsentInvalidBufferSizeException() : base("Data buffer doesn't match column size", JET_err.InvalidBufferSize)
		{
		}

		private EsentInvalidBufferSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
