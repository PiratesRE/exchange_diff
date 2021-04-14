using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[Serializable]
	public abstract class DiscoverySearchBase : EwsStoreObject
	{
		public DiscoverySearchBase()
		{
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Name
		{
			get
			{
				return base.AlternativeId;
			}
			set
			{
				base.AlternativeId = value;
			}
		}
	}
}
