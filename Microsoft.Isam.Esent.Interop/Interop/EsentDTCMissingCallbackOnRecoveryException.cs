using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDTCMissingCallbackOnRecoveryException : EsentObsoleteException
	{
		public EsentDTCMissingCallbackOnRecoveryException() : base("Attempted to recover a distributed transaction but no callback for DTC coordination was specified on initialisation", JET_err.DTCMissingCallbackOnRecovery)
		{
		}

		private EsentDTCMissingCallbackOnRecoveryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
