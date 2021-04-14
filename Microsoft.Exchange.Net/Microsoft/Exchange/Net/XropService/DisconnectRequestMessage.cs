using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Net.XropService
{
	[CLSCompliant(false)]
	[MessageContract]
	public sealed class DisconnectRequestMessage
	{
		[MessageBodyMember]
		public DisconnectRequest Request { get; set; }
	}
}
