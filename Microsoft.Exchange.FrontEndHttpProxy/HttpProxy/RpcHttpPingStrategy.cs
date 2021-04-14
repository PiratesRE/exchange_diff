using System;
using System.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal class RpcHttpPingStrategy : ProtocolPingStrategyBase
	{
		public override Uri BuildUrl(string fqdn)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			return new UriBuilder
			{
				Scheme = Uri.UriSchemeHttps,
				Host = fqdn,
				Path = "rpc/rpcproxy.dll"
			}.Uri;
		}

		protected override void PrepareRequest(HttpWebRequest request)
		{
			base.PrepareRequest(request);
			request.Method = "RPC_IN_DATA";
		}

		protected override bool IsWebExceptionExpected(WebException exception)
		{
			HttpWebResponse httpWebResponse = exception.Response as HttpWebResponse;
			return httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.Unauthorized;
		}
	}
}
