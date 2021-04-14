using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[MessageContract]
	public class AutodiscoverResponseMessage
	{
		public AutodiscoverResponseMessage()
		{
			this.ServerVersionInfo = ServerVersionInfo.CurrentVersion.Member;
		}

		[MessageHeader]
		public ServerVersionInfo ServerVersionInfo { get; set; }
	}
}
