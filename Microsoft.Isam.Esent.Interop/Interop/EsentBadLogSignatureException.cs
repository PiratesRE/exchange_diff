using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadLogSignatureException : EsentInconsistentException
	{
		public EsentBadLogSignatureException() : base("Bad signature for a log file", JET_err.BadLogSignature)
		{
		}

		private EsentBadLogSignatureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
