using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFileIOAbortException : EsentObsoleteException
	{
		public EsentFileIOAbortException() : base("instructs the JET_ABORTRETRYFAILCALLBACK caller to abort the specified I/O", JET_err.FileIOAbort)
		{
		}

		private EsentFileIOAbortException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
