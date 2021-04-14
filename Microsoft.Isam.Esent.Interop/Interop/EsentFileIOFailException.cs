using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFileIOFailException : EsentObsoleteException
	{
		public EsentFileIOFailException() : base("instructs the JET_ABORTRETRYFAILCALLBACK caller to fail the specified I/O", JET_err.FileIOFail)
		{
		}

		private EsentFileIOFailException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
