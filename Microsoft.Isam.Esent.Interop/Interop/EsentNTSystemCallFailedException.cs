using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentNTSystemCallFailedException : EsentOperationException
	{
		public EsentNTSystemCallFailedException() : base("A call to the operating system failed", JET_err.NTSystemCallFailed)
		{
		}

		private EsentNTSystemCallFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
