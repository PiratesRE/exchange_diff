using System;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class CmdletProxyInfo
	{
		public CmdletProxyInfo(string remoteServerFqdn, int remoteServerVersion, bool shouldAsyncProxy, LocalizedString confirmationMessage, CmdletProxyInfo.ChangeCmdletProxyParametersDelegate changeCmdletProxyParameters)
		{
			this.RemoteServerFqdn = remoteServerFqdn;
			this.RemoteServerVersion = remoteServerVersion;
			this.ShouldAsyncProxy = shouldAsyncProxy;
			this.ConfirmationMessage = confirmationMessage;
			this.ChangeCmdletProxyParameters = changeCmdletProxyParameters;
		}

		public string RemoteServerFqdn { get; private set; }

		public int RemoteServerVersion { get; private set; }

		public bool ShouldAsyncProxy { get; private set; }

		public LocalizedString ConfirmationMessage { get; private set; }

		public CmdletProxyInfo.ChangeCmdletProxyParametersDelegate ChangeCmdletProxyParameters { get; set; }

		public delegate void ChangeCmdletProxyParametersDelegate(PropertyBag parameters);
	}
}
