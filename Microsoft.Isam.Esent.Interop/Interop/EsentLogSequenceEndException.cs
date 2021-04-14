using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogSequenceEndException : EsentFragmentationException
	{
		public EsentLogSequenceEndException() : base("Maximum log file number exceeded", JET_err.LogSequenceEnd)
		{
		}

		private EsentLogSequenceEndException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
