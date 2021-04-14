using System;
using System.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal class OwaPingStrategy : ProtocolPingStrategyBase
	{
		protected override void PrepareRequest(HttpWebRequest request)
		{
			base.PrepareRequest(request);
			request.Method = "GET";
		}
	}
}
