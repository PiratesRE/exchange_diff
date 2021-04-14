using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.MailboxTransport.Shared.Providers
{
	internal class ADConfigurationProvider : IConfigurationProvider
	{
		private ADConfigurationProvider()
		{
		}

		public static ADConfigurationProvider Instance
		{
			get
			{
				return ADConfigurationProvider.instance;
			}
		}

		public string GetDefaultDomainName()
		{
			return Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName;
		}

		public bool TryGetDefaultDomainName(OrganizationId organizationId, out string domainName)
		{
			domainName = null;
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
			if (Components.Configuration.TryGetAcceptedDomainTable(organizationId, out perTenantAcceptedDomainTable))
			{
				domainName = perTenantAcceptedDomainTable.AcceptedDomainTable.DefaultDomain.DomainName.Domain;
			}
			return domainName != null;
		}

		public OutboundConversionOptions GetGlobalConversionOptions()
		{
			return new OutboundConversionOptions(new EmptyRecipientCache(), this.GetDefaultDomainName())
			{
				DsnMdnOptions = DsnMdnOptions.PropagateUserSettings,
				DsnHumanReadableWriter = Components.DsnGenerator.DsnHumanReadableWriter,
				Limits = 
				{
					MimeLimits = MimeLimits.Unlimited
				},
				LogDirectoryPath = Components.Configuration.LocalServer.ContentConversionTracingPath
			};
		}

		public void SendNDRForInvalidAddresses(IReadOnlyMailItem mailItem, List<DsnRecipientInfo> invalidRecipients, DsnMailOutHandlerDelegate dsnMailOutHandler)
		{
			Components.DsnGenerator.GenerateNDRForInvalidAddresses(false, mailItem, invalidRecipients, dsnMailOutHandler);
		}

		public void SendNDRForFailedSubmission(IReadOnlyMailItem ndrMailItem, SmtpResponse ndrReason, DsnMailOutHandlerDelegate dsnMailOutHandler)
		{
			List<DsnRecipientInfo> list = new List<DsnRecipientInfo>();
			list.Add(DsnGenerator.CreateDsnRecipientInfo(null, (string)ndrMailItem.From, null, ndrReason));
			Components.DsnGenerator.GenerateNDRForInvalidAddresses(false, ndrMailItem, list, dsnMailOutHandler);
		}

		public string GetQuarantineMailbox()
		{
			if (Components.DsnGenerator.QuarantineConfig != null)
			{
				return Components.DsnGenerator.QuarantineConfig.Mailbox;
			}
			return null;
		}

		public bool GetForwardingProhibitedFeatureStatus()
		{
			return Components.TransportAppConfig.Resolver.EnableForwardingProhibitedFeature;
		}

		private static ADConfigurationProvider instance = new ADConfigurationProvider();
	}
}
