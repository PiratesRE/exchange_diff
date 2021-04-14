using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Net.XropService
{
	[CLSCompliant(false)]
	[MessageContract]
	public sealed class ConnectResponseMessage
	{
		[MessageBodyMember]
		public ConnectResponse Response { get; set; }
	}
}
