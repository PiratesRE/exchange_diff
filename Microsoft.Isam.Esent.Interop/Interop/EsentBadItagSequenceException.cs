using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadItagSequenceException : EsentStateException
	{
		public EsentBadItagSequenceException() : base("Bad itagSequence for tagged column", JET_err.BadItagSequence)
		{
		}

		private EsentBadItagSequenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
