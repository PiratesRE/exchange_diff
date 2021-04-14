using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport
{
	internal class MailRecipientWrapper : EnvelopeRecipient, IMailRecipientWrapperFacade
	{
		public MailRecipientWrapper(MailRecipient mailRecipient, IReadOnlyMailItem mailItem)
		{
			if (mailRecipient == null)
			{
				throw new ArgumentNullException("mailRecipient");
			}
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			this.mailRecipient = mailRecipient;
			this.mailItem = mailItem;
		}

		public override RoutingAddress Address
		{
			get
			{
				this.DisposeValidation();
				return this.mailRecipient.Email;
			}
			set
			{
				this.DisposeValidation();
				this.mailRecipient.Email = value;
			}
		}

		[Obsolete("Use ResolvedMessageEventSource.GetRoutingOverride() instead")]
		public override RoutingDomain RoutingOverride
		{
			get
			{
				this.DisposeValidation();
				if (this.mailRecipient.RoutingOverride == null)
				{
					return RoutingDomain.Empty;
				}
				return this.mailRecipient.RoutingOverride.RoutingDomain;
			}
		}

		public override string OriginalRecipient
		{
			get
			{
				this.DisposeValidation();
				return this.mailRecipient.ORcpt;
			}
			set
			{
				this.DisposeValidation();
				this.mailRecipient.ORcpt = value;
			}
		}

		public override DsnTypeRequested RequestedReports
		{
			get
			{
				this.DisposeValidation();
				return EnumConverter.InternalToPublic(this.mailRecipient.DsnRequested);
			}
			set
			{
				this.DisposeValidation();
				this.mailRecipient.DsnRequested = EnumConverter.PublicToInternal(value);
			}
		}

		public override IDictionary<string, object> Properties
		{
			get
			{
				this.DisposeValidation();
				return this.mailRecipient.ExtendedPropertyDictionary;
			}
		}

		public override DeliveryMethod OutboundDeliveryMethod
		{
			get
			{
				this.DisposeValidation();
				switch (this.mailRecipient.NextHop.NextHopType.DeliveryType)
				{
				case DeliveryType.DnsConnectorDelivery:
				case DeliveryType.SmartHostConnectorDelivery:
				case DeliveryType.SmtpRelayToRemoteAdSite:
				case DeliveryType.SmtpRelayToTiRg:
				case DeliveryType.SmtpRelayWithinAdSite:
				case DeliveryType.SmtpRelayWithinAdSiteToEdge:
				case DeliveryType.SmtpRelayToDag:
				case DeliveryType.SmtpRelayToMailboxDeliveryGroup:
				case DeliveryType.SmtpRelayToConnectorSourceServers:
				case DeliveryType.SmtpRelayToServers:
					return DeliveryMethod.Smtp;
				case DeliveryType.MapiDelivery:
				case DeliveryType.SmtpDeliveryToMailbox:
					return DeliveryMethod.Mailbox;
				case DeliveryType.NonSmtpGatewayDelivery:
					return DeliveryMethod.File;
				case DeliveryType.DeliveryAgent:
					return DeliveryMethod.DeliveryAgent;
				}
				return DeliveryMethod.Unknown;
			}
		}

		public override RecipientCategory RecipientCategory
		{
			get
			{
				this.DisposeValidation();
				switch (this.mailItem.Directionality)
				{
				case MailDirectionality.Originating:
				{
					PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
					if (!Components.Configuration.TryGetAcceptedDomainTable(this.mailItem.ADRecipientCache.OrganizationId, out perTenantAcceptedDomainTable))
					{
						return RecipientCategory.Unknown;
					}
					if (perTenantAcceptedDomainTable.AcceptedDomainTable.GetDomainEntry(SmtpDomain.GetDomainPart(this.Address)) != null)
					{
						return RecipientCategory.InSameOrganization;
					}
					return RecipientCategory.InDifferentOrganization;
				}
				case MailDirectionality.Incoming:
					return RecipientCategory.Incoming;
				}
				return RecipientCategory.Unknown;
			}
		}

		IMailRecipientFacade IMailRecipientWrapperFacade.MailRecipient
		{
			get
			{
				return this.mailRecipient;
			}
		}

		internal MailRecipient MailRecipient
		{
			get
			{
				return this.mailRecipient;
			}
		}

		[Obsolete("Use ResolvedMessageEventSource.SetRoutingOverride() instead")]
		public override void SetRoutingOverride(RoutingDomain routingDomain)
		{
			this.DisposeValidation();
			RoutingOverride routingOverride = (routingDomain == RoutingDomain.Empty) ? null : new RoutingOverride(routingDomain, DeliveryQueueDomain.UseOverrideDomain);
			this.mailRecipient.SetRoutingOverride(routingOverride, null, null);
		}

		internal override bool IsPublicFolderRecipient()
		{
			if (this.mailItem.ADRecipientCache == null)
			{
				return false;
			}
			SmtpProxyAddress proxyAddress = new SmtpProxyAddress(this.Address.ToString(), true);
			Result<TransportMiniRecipient> result = this.mailItem.ADRecipientCache.FindAndCacheRecipient(proxyAddress);
			if (result.Data != null)
			{
				Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType = result.Data.RecipientType;
				if (Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicFolder == recipientType || Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicDatabase == recipientType)
				{
					return true;
				}
			}
			return false;
		}

		internal void DisposeValidation()
		{
			if (!this.mailRecipient.IsActive || this.mailRecipient.IsProcessed)
			{
				throw new ObjectDisposedException(Strings.EnvelopRecipientDisposed);
			}
		}

		private MailRecipient mailRecipient;

		private IReadOnlyMailItem mailItem;
	}
}
