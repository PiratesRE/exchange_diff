using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadCheckpointSignatureException : EsentInconsistentException
	{
		public EsentBadCheckpointSignatureException() : base("Bad signature for a checkpoint file", JET_err.BadCheckpointSignature)
		{
		}

		private EsentBadCheckpointSignatureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
