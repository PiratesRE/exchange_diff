using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotSeparateIntrinsicLVException : EsentUsageException
	{
		public EsentCannotSeparateIntrinsicLVException() : base("illegal attempt to separate an LV which must be intrinsic", JET_err.CannotSeparateIntrinsicLV)
		{
		}

		private EsentCannotSeparateIntrinsicLVException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
