using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFileIORetryException : EsentObsoleteException
	{
		public EsentFileIORetryException() : base("instructs the JET_ABORTRETRYFAILCALLBACK caller to retry the specified I/O", JET_err.FileIORetry)
		{
		}

		private EsentFileIORetryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
