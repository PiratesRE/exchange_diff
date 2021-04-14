using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMultiValuedIndexViolationException : EsentUsageException
	{
		public EsentMultiValuedIndexViolationException() : base("Non-unique inter-record index keys generated for a multivalued index", JET_err.MultiValuedIndexViolation)
		{
		}

		private EsentMultiValuedIndexViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
