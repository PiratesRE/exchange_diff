using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidLogSequenceException : EsentCorruptionException
	{
		public EsentInvalidLogSequenceException() : base("Timestamp in next log does not match expected", JET_err.InvalidLogSequence)
		{
		}

		private EsentInvalidLogSequenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
