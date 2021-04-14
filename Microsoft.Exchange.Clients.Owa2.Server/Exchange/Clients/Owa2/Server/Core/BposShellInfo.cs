using System;
using System.Runtime.Serialization;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class BposShellInfo : BposNavBarInfo
	{
		public BposShellInfo(string version, NavBarData data, string allowedProxyOrigins, string proxyScriptUrl) : base(version, data)
		{
			this.AllowedProxyOrigins = allowedProxyOrigins;
			this.ProxyScriptUrl = proxyScriptUrl;
		}

		[DataMember]
		public string SuiteServiceProxyOriginAllowedList { get; set; }

		[DataMember]
		public string SuiteServiceProxyScriptUrl { get; set; }

		[DataMember]
		public string AllowedProxyOrigins { get; set; }

		[DataMember]
		public string ProxyScriptUrl { get; set; }
	}
}
