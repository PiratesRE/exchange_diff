using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Net.XropService
{
	[MessageContract]
	[CLSCompliant(false)]
	public sealed class ExecuteResponseMessage
	{
		[MessageBodyMember]
		public ExecuteResponse Response { get; set; }
	}
}
