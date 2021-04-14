using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInternalErrorException : EsentOperationException
	{
		public EsentInternalErrorException() : base("Fatal internal error", JET_err.InternalError)
		{
		}

		private EsentInternalErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
