using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDTCCallbackUnexpectedErrorException : EsentObsoleteException
	{
		public EsentDTCCallbackUnexpectedErrorException() : base("Unexpected error code returned from DTC callback", JET_err.DTCCallbackUnexpectedError)
		{
		}

		private EsentDTCCallbackUnexpectedErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
