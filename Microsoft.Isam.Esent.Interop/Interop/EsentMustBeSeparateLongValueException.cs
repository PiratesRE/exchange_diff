using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMustBeSeparateLongValueException : EsentUsageException
	{
		public EsentMustBeSeparateLongValueException() : base("Can only preread long value columns that can be separate, e.g. not size constrained so that they are fixed or variable columns", JET_err.MustBeSeparateLongValue)
		{
		}

		private EsentMustBeSeparateLongValueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
