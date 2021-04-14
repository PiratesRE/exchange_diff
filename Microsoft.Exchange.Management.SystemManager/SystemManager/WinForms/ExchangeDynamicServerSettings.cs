using System;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeDynamicServerSettings : ExchangeSettings
	{
		public ExchangeDynamicServerSettings(IComponent owner) : base(owner)
		{
		}

		[UserScopedSetting]
		public Fqdn RemotePSServer
		{
			get
			{
				return (Fqdn)this["RemotePSServer"];
			}
			set
			{
				this["RemotePSServer"] = value;
			}
		}
	}
}
