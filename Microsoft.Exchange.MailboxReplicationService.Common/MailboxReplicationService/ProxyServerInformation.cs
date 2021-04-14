using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class ProxyServerInformation
	{
		public ProxyServerInformation()
		{
		}

		public ProxyServerInformation(string fqdn, bool isProxyLocal)
		{
			this.ServerFqdn = fqdn;
			this.IsProxyLocal = isProxyLocal;
		}

		[DataMember(Name = "IsProxyLocal")]
		public bool IsProxyLocal { get; set; }

		[DataMember(Name = "ServerFqdn")]
		public string ServerFqdn { get; set; }
	}
}
