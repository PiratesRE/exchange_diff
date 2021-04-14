using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class CmdletNeedsProxyException : Exception
	{
		public CmdletNeedsProxyException(CmdletProxyInfo cmdletProxyInfo)
		{
			this.CmdletProxyInfo = cmdletProxyInfo;
		}

		public CmdletProxyInfo CmdletProxyInfo { get; private set; }
	}
}
