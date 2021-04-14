using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDTCMissingCallbackException : EsentObsoleteException
	{
		public EsentDTCMissingCallbackException() : base("Attempted to begin a distributed transaction but no callback for DTC coordination was specified on initialisation", JET_err.DTCMissingCallback)
		{
		}

		private EsentDTCMissingCallbackException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
