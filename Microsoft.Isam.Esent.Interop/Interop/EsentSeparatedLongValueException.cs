using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSeparatedLongValueException : EsentStateException
	{
		public EsentSeparatedLongValueException() : base("Operation not supported on separated long-value", JET_err.SeparatedLongValue)
		{
		}

		private EsentSeparatedLongValueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
