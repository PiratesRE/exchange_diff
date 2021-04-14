using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class OneOffItem : RecipientItem
	{
		public OneOffItem(MailRecipient recipient) : base(recipient)
		{
		}

		public static bool IsLocalAddress(RoutingAddress smtpAddress, AcceptedDomainTable orgDomains, out ProxyAddressPrefix addressType)
		{
			ProxyAddress proxyAddress;
			if (Resolver.TryDeencapsulate(smtpAddress, out proxyAddress))
			{
				addressType = proxyAddress.Prefix;
				return proxyAddress.Prefix == ProxyAddressPrefix.LegacyDN || (proxyAddress.Prefix == ProxyAddressPrefix.X400 && OneOffItem.IsLocalX400Address(proxyAddress));
			}
			addressType = ProxyAddressPrefix.Smtp;
			return OneOffItem.IsLocalSmtpAddress(smtpAddress, orgDomains);
		}

		public static bool IsLocalX400Address(ProxyAddress x400Proxy)
		{
			RoutingX400Address address;
			return RoutingX400Address.TryParse(x400Proxy.AddressString, out address) && Components.Configuration.X400AuthoritativeDomainTable.CheckAccepted(address);
		}

		public static bool IsLocalAddress(ProxyAddress proxyAddress, AcceptedDomainTable orgDomains)
		{
			if (proxyAddress.Prefix == ProxyAddressPrefix.LegacyDN)
			{
				return true;
			}
			if (proxyAddress.Prefix == ProxyAddressPrefix.X400)
			{
				return OneOffItem.IsLocalX400Address(proxyAddress);
			}
			return proxyAddress.Prefix == ProxyAddressPrefix.Smtp && OneOffItem.IsLocalSmtpAddress(new RoutingAddress(proxyAddress.AddressString), orgDomains);
		}

		public static bool IsLocalSmtpAddress(RoutingAddress address, AcceptedDomainTable orgDomains)
		{
			if (orgDomains == null)
			{
				throw new ArgumentNullException("orgDomains");
			}
			return orgDomains.CheckAuthoritative(SmtpDomain.GetDomainPart(address));
		}

		public static bool IsAuthoritativeOrInternalRelaySmtpAddress(RoutingAddress smtpAddress, OrganizationId orgId)
		{
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
			return Components.Configuration.TryGetAcceptedDomainTable(orgId, out perTenantAcceptedDomainTable) && perTenantAcceptedDomainTable.AcceptedDomainTable.CheckInternal(SmtpDomain.GetDomainPart(smtpAddress));
		}

		public override void PreProcess(Expansion expansion)
		{
			AcceptedDomainTable acceptedDomains = expansion.Configuration.AcceptedDomains;
			ProxyAddressPrefix smtp = ProxyAddressPrefix.Smtp;
			if (OneOffItem.IsLocalAddress(base.Email, acceptedDomains, out smtp))
			{
				if (smtp == ProxyAddressPrefix.LegacyDN)
				{
					base.FailRecipient(AckReason.LocalRecipientExAddressUnknown);
				}
				else if (smtp == ProxyAddressPrefix.X400)
				{
					base.FailRecipient(AckReason.LocalRecipientX400AddressUnknown);
				}
				else
				{
					base.FailRecipient(AckReason.LocalRecipientAddressUnknown);
				}
				if (Resolver.PerfCounters != null)
				{
					Resolver.PerfCounters.UnresolvedOrgRecipientsTotal.Increment();
				}
			}
		}

		public override void PostProcess(Expansion expansion)
		{
			ExTraceGlobals.ResolverTracer.TraceDebug<ResolverMessageType, RoutingAddress>(0L, "OneOffItem:Msg Type={0}, recipientAddress={1}", expansion.Message.Type, base.Email);
			OofRestriction.ExternalUserOofCheck(expansion, base.Recipient);
			MsgTypeRestriction.ExternalRecipientMessageTypeCheck(expansion, base.Recipient);
			if (!base.Recipient.IsProcessed)
			{
				this.CheckDeliveryRestrictions(expansion);
			}
		}
	}
}
