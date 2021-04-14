using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentGivenLogFileHasBadSignatureException : EsentInconsistentException
	{
		public EsentGivenLogFileHasBadSignatureException() : base("Restore log file has bad signature", JET_err.GivenLogFileHasBadSignature)
		{
		}

		private EsentGivenLogFileHasBadSignatureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
