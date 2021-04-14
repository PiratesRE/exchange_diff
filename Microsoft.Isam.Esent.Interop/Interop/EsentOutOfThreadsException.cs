using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfThreadsException : EsentMemoryException
	{
		public EsentOutOfThreadsException() : base("Could not start thread", JET_err.OutOfThreads)
		{
		}

		private EsentOutOfThreadsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
