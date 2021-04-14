using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Net.XropService
{
	[CLSCompliant(false)]
	[MessageContract]
	public sealed class ConnectRequestMessage
	{
		[MessageBodyMember]
		public ConnectRequest Request { get; set; }
	}
}
