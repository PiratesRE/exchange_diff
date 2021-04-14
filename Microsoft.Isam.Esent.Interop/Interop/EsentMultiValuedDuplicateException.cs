using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMultiValuedDuplicateException : EsentStateException
	{
		public EsentMultiValuedDuplicateException() : base("Duplicate detected on a unique multi-valued column", JET_err.MultiValuedDuplicate)
		{
		}

		private EsentMultiValuedDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
