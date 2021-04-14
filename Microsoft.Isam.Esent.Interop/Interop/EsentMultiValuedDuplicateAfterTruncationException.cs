using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMultiValuedDuplicateAfterTruncationException : EsentStateException
	{
		public EsentMultiValuedDuplicateAfterTruncationException() : base("Duplicate detected on a unique multi-valued column after data was normalized, and normalizing truncated the data before comparison", JET_err.MultiValuedDuplicateAfterTruncation)
		{
		}

		private EsentMultiValuedDuplicateAfterTruncationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
