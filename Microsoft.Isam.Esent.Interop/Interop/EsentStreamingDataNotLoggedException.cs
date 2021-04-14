using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentStreamingDataNotLoggedException : EsentObsoleteException
	{
		public EsentStreamingDataNotLoggedException() : base("Illegal attempt to replay a streaming file operation where the data wasn't logged. Probably caused by an attempt to roll-forward with circular logging enabled", JET_err.StreamingDataNotLogged)
		{
		}

		private EsentStreamingDataNotLoggedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
