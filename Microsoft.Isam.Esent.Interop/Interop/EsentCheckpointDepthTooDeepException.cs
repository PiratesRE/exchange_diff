using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCheckpointDepthTooDeepException : EsentQuotaException
	{
		public EsentCheckpointDepthTooDeepException() : base("too many outstanding generations between checkpoint and current generation", JET_err.CheckpointDepthTooDeep)
		{
		}

		private EsentCheckpointDepthTooDeepException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
