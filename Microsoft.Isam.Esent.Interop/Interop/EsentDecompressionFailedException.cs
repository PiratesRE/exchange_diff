using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDecompressionFailedException : EsentCorruptionException
	{
		public EsentDecompressionFailedException() : base("Internal error: data could not be decompressed", JET_err.DecompressionFailed)
		{
		}

		private EsentDecompressionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
