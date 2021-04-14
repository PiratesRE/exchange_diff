using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadDbSignatureException : EsentObsoleteException
	{
		public EsentBadDbSignatureException() : base("Bad signature for a db file", JET_err.BadDbSignature)
		{
		}

		private EsentBadDbSignatureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
