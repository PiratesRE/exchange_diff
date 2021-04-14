using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentClientRequestToStopJetServiceException : EsentOperationException
	{
		public EsentClientRequestToStopJetServiceException() : base("Client has requested stop service", JET_err.ClientRequestToStopJetService)
		{
		}

		private EsentClientRequestToStopJetServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
