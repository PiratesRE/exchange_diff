using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMultiValuedColumnMustBeTaggedException : EsentUsageException
	{
		public EsentMultiValuedColumnMustBeTaggedException() : base("Attempted to create a multi-valued column, but column was not Tagged", JET_err.MultiValuedColumnMustBeTagged)
		{
		}

		private EsentMultiValuedColumnMustBeTaggedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
