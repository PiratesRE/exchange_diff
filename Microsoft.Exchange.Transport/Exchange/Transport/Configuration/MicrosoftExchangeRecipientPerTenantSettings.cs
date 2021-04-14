using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal sealed class MicrosoftExchangeRecipientPerTenantSettings : TenantConfigurationCacheableItem<MicrosoftExchangeRecipient>
	{
		public MicrosoftExchangeRecipientPerTenantSettings()
		{
		}

		public MicrosoftExchangeRecipientPerTenantSettings(MicrosoftExchangeRecipient microsoftExchangeRecipient, OrganizationId orgId) : base(true)
		{
			this.SetInternalData(microsoftExchangeRecipient, orgId);
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.primarySmtpAddress;
			}
		}

		public bool UsingDefault
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.usingDefault;
			}
		}

		public override long ItemSize
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return (long)this.estimatedSize;
			}
		}

		public override void ReadData(IConfigurationSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			MicrosoftExchangeRecipient[] results = null;
			MicrosoftExchangeRecipient microsoftExchangeRecipient = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				results = session.Find<MicrosoftExchangeRecipient>(null, QueryScope.SubTree, null, null, 1);
			});
			if (adoperationResult.Succeeded && results != null && results.Length != 0)
			{
				microsoftExchangeRecipient = results[0];
			}
			this.SetInternalData(microsoftExchangeRecipient, session.SessionSettings.CurrentOrganizationId);
		}

		private void SetInternalData(MicrosoftExchangeRecipient microsoftExchangeRecipient, OrganizationId orgId)
		{
			if (microsoftExchangeRecipient != null && this.ParseMicrosoftExchangeRecipientAndSetAddress(microsoftExchangeRecipient))
			{
				this.usingDefault = false;
			}
			else
			{
				this.usingDefault = true;
				this.SetPostmasterSettings(orgId);
			}
			this.estimatedSize = ((this.primarySmtpAddress == SmtpAddress.Empty) ? 1 : (1 + this.primarySmtpAddress.Length * 2));
		}

		private void SetPostmasterSettings(OrganizationId orgId)
		{
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
			AcceptedDomainEntry defaultDomain;
			if (Components.Configuration.TryGetAcceptedDomainTable(orgId, out perTenantAcceptedDomainTable))
			{
				defaultDomain = perTenantAcceptedDomainTable.AcceptedDomainTable.DefaultDomain;
			}
			else
			{
				defaultDomain = Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomain;
			}
			string domain;
			if (defaultDomain != null && !string.IsNullOrEmpty(defaultDomain.DomainName.Domain))
			{
				domain = defaultDomain.DomainName.Domain;
			}
			else
			{
				domain = Components.Configuration.LocalServer.TransportServer.GetDomainOrComputerName();
			}
			this.primarySmtpAddress = new SmtpAddress(ADMicrosoftExchangeRecipient.DefaultName, domain);
			ExTraceGlobals.ConfigurationTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "Using '{0}' as MicrosoftExchangeRecipient address.", this.primarySmtpAddress);
		}

		private bool ParseMicrosoftExchangeRecipientAndSetAddress(MicrosoftExchangeRecipient mer)
		{
			if (mer == null)
			{
				throw new ArgumentNullException("mer");
			}
			if (mer.PrimarySmtpAddress.IsValidAddress)
			{
				this.primarySmtpAddress = mer.PrimarySmtpAddress;
				return true;
			}
			foreach (ProxyAddress proxyAddress in mer.EmailAddresses)
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp && SmtpAddress.IsValidSmtpAddress(proxyAddress.AddressString))
				{
					this.primarySmtpAddress = new SmtpAddress(proxyAddress.AddressString);
					ExTraceGlobals.ConfigurationTracer.TraceDebug<string>((long)this.GetHashCode(), "Using '{0}' as MicrosoftExchangeRecipient address because primary SMTP proxy not found.", proxyAddress.AddressString);
					return true;
				}
			}
			return false;
		}

		private SmtpAddress primarySmtpAddress;

		private bool usingDefault;

		private int estimatedSize;
	}
}
