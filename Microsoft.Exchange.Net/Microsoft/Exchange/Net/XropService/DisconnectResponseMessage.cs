using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Net.XropService
{
	[CLSCompliant(false)]
	[MessageContract]
	public sealed class DisconnectResponseMessage
	{
		[MessageBodyMember]
		public DisconnectResponse Response { get; set; }
	}
}
