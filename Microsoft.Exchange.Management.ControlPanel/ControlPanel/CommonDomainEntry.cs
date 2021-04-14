using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CommonDomainEntry
	{
		public CommonDomainEntry(SmartHost smartHost)
		{
			if (smartHost.IsIPAddress)
			{
				this.address = smartHost.Address.ToString();
				return;
			}
			this.address = smartHost.Domain.HostnameString;
		}

		[DataMember]
		public string Address
		{
			get
			{
				return this.address;
			}
			set
			{
				this.address = value;
			}
		}

		private string address;
	}
}
