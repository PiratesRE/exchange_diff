using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal sealed class MicrosoftExchangeRecipientConfiguration : GlobalConfigurationBase<MicrosoftExchangeRecipient, MicrosoftExchangeRecipientConfiguration>
	{
		public RoutingAddress Address
		{
			get
			{
				return this.address;
			}
		}

		public bool UsingDefaultAddress
		{
			get
			{
				return this.usingDefault;
			}
		}

		protected override string ConfigObjectName
		{
			get
			{
				return "MicrosoftExchangeRecipient";
			}
		}

		protected override string ReloadFailedString
		{
			get
			{
				return Strings.ReadMicrosoftExchangeRecipientFailed;
			}
		}

		protected override ADObjectId GetObjectId(IConfigurationSession session)
		{
			return ADMicrosoftExchangeRecipient.GetDefaultId(session);
		}

		protected override void HandleObjectLoaded()
		{
			if (base.ConfigObject.PrimarySmtpAddress.IsValidAddress)
			{
				this.usingDefault = false;
				this.address = new RoutingAddress(base.ConfigObject.PrimarySmtpAddress.ToString());
				return;
			}
			foreach (ProxyAddress proxyAddress in base.ConfigObject.EmailAddresses)
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp && SmtpAddress.IsValidSmtpAddress(proxyAddress.AddressString))
				{
					this.usingDefault = false;
					this.address = new RoutingAddress(proxyAddress.AddressString);
					ExTraceGlobals.ConfigurationTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Using '{0}' as MicrosoftExchangeRecipient address because primary SMTP proxy not found.", this.address);
					return;
				}
			}
			this.SetDefaultAddress();
		}

		protected override bool HandleObjectNotFound()
		{
			if (Components.IsBridgehead)
			{
				return false;
			}
			this.SetDefaultAddress();
			return true;
		}

		private void SetDefaultAddress()
		{
			string domain;
			if (!string.IsNullOrEmpty(Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName))
			{
				domain = Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName;
			}
			else
			{
				domain = Components.Configuration.LocalServer.TransportServer.GetDomainOrComputerName();
			}
			this.usingDefault = true;
			this.address = new RoutingAddress(ADMicrosoftExchangeRecipient.DefaultName, domain);
			ExTraceGlobals.ConfigurationTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Using '{0}' as MicrosoftExchangeRecipient address.", this.address);
		}

		private RoutingAddress address;

		private bool usingDefault;
	}
}
