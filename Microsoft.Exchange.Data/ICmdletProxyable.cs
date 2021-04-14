using System;

namespace Microsoft.Exchange.Data
{
	public interface ICmdletProxyable
	{
		object GetProxyInfo();

		void SetProxyInfo(object proxyInfo);
	}
}
