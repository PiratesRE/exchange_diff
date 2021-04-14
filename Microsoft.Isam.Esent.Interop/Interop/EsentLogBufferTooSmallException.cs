using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogBufferTooSmallException : EsentObsoleteException
	{
		public EsentLogBufferTooSmallException() : base("Log buffer is too small for recovery", JET_err.LogBufferTooSmall)
		{
		}

		private EsentLogBufferTooSmallException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
