using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidLogDataSequenceException : EsentStateException
	{
		public EsentInvalidLogDataSequenceException() : base("Some how the log data provided got out of sequence with the current state of the instance", JET_err.InvalidLogDataSequence)
		{
		}

		private EsentInvalidLogDataSequenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
