using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Net.XropService
{
	[CLSCompliant(false)]
	[MessageContract]
	public sealed class ExecuteRequestMessage
	{
		[MessageBodyMember]
		public ExecuteRequest Request { get; set; }
	}
}
