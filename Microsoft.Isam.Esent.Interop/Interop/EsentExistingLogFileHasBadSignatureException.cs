using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentExistingLogFileHasBadSignatureException : EsentInconsistentException
	{
		public EsentExistingLogFileHasBadSignatureException() : base("Existing log file has bad signature", JET_err.ExistingLogFileHasBadSignature)
		{
		}

		private EsentExistingLogFileHasBadSignatureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
