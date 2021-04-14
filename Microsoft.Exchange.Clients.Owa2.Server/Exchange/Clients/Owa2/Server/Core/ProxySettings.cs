using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ProxySettings
	{
		public ProxySettings()
		{
		}

		public ProxySettings(string userPrincipalName, string[] proxyAddresses)
		{
			this.UserPrincipalName = userPrincipalName;
			this.ProxyAddresses = proxyAddresses;
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string UserPrincipalName { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string[] ProxyAddresses { get; set; }
	}
}
