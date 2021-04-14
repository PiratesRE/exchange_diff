using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal struct DeliveryRestriction
	{
		private DeliveryRestriction(RestrictedItem recipient, Sender sender, bool isAuthenticated, bool isJournalReport, long messageSize, Unlimited<ByteQuantifiedSize> defaultMaxReceiveSize, IList<RoutingAddress> privilegedSenders)
		{
			this.recipient = recipient;
			this.sender = sender;
			this.isAuthenticated = isAuthenticated;
			this.isJournalReport = isJournalReport;
			this.messageSize = messageSize;
			this.defaultMaxReceiveSize = defaultMaxReceiveSize;
			this.privilegedSenders = privilegedSenders;
			this.recipientSession = null;
		}

		public static RestrictionCheckResult CheckRestriction(RestrictedItem recipient, Sender sender, bool isAuthenticated, bool isJournalReport, long messageSize, Unlimited<ByteQuantifiedSize> defaultMaxReceiveSize, IList<RoutingAddress> privilegedSenders, ISimpleCache<ADObjectId, bool> memberOfGroupCache, OrganizationId orgId, out long maxRecipientMessageSize)
		{
			DeliveryRestriction deliveryRestriction = new DeliveryRestriction(recipient, sender, isAuthenticated, isJournalReport, messageSize, defaultMaxReceiveSize, privilegedSenders);
			return deliveryRestriction.CheckRestriction(orgId, memberOfGroupCache, out maxRecipientMessageSize);
		}

		public static RestrictionCheckResult CheckSenderSizeRestriction(Sender sender, long messageSize, bool isAuthenticatedSender, bool isJournalReport, Unlimited<ByteQuantifiedSize> defaultSendLimit, IList<RoutingAddress> privilegedSenders, out long maxMessageSizeSendLimitInKB, out long currentMessageSizeInKB)
		{
			maxMessageSizeSendLimitInKB = 0L;
			currentMessageSizeInKB = 0L;
			if (isJournalReport)
			{
				return RestrictionCheckResult.AcceptedJournalReport;
			}
			if (DeliveryRestriction.IsPrivilegedSender(sender, isAuthenticatedSender, privilegedSenders))
			{
				return RestrictionCheckResult.AcceptedSizeOK;
			}
			bool flag = false;
			Unlimited<ByteQuantifiedSize> unlimited;
			if (!isAuthenticatedSender || sender.MaxSendSize.IsUnlimited)
			{
				unlimited = defaultSendLimit;
				flag = true;
			}
			else
			{
				unlimited = sender.MaxSendSize;
			}
			if (!unlimited.IsUnlimited)
			{
				currentMessageSizeInKB = DeliveryRestriction.GetSizeInKiloBytes(messageSize);
				maxMessageSizeSendLimitInKB = (long)unlimited.Value.ToKB();
				if (currentMessageSizeInKB > maxMessageSizeSendLimitInKB)
				{
					if (flag)
					{
						return (RestrictionCheckResult)2147483651U;
					}
					return (RestrictionCheckResult)2147483650U;
				}
			}
			return RestrictionCheckResult.AcceptedSizeOK;
		}

		public static SmtpResponse GetResponseForResult(RestrictionCheckResult result)
		{
			switch (result)
			{
			case (RestrictionCheckResult)2147483649U:
				return AckReason.MessageTooLargeForReceiver;
			case (RestrictionCheckResult)2147483650U:
				return AckReason.MessageTooLargeForSender;
			case (RestrictionCheckResult)2147483651U:
				return AckReason.MessageTooLargeForOrganization;
			case (RestrictionCheckResult)2147483652U:
			case (RestrictionCheckResult)2147483653U:
			case (RestrictionCheckResult)2147483654U:
				return AckReason.RecipientPermissionRestricted;
			case (RestrictionCheckResult)2147483655U:
				return AckReason.NotAuthenticated;
			case (RestrictionCheckResult)2147483656U:
				return AckReason.InvalidDirectoryObjectForRestrictionCheck;
			case (RestrictionCheckResult)2147483657U:
				return AckReason.RecipientPermissionRestrictedToGroup;
			default:
				throw new InvalidOperationException("Should only call this method for rejected sender!");
			}
		}

		public static bool IsPrivilegedSender(Sender sender, bool isAuthenticatedSender, IList<RoutingAddress> privilegedSenders)
		{
			if (!isAuthenticatedSender)
			{
				return false;
			}
			if (sender.RecipientType != null)
			{
				if (sender.RecipientType.Value == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicDatabase)
				{
					return true;
				}
				if (sender.RecipientType.Value == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MicrosoftExchange)
				{
					return true;
				}
			}
			return sender.EmailAddress != null && privilegedSenders.IndexOf(sender.EmailAddress.Value) != -1;
		}

		private static long GetSizeInKiloBytes(long size)
		{
			return (size + 1023L) / 1024L;
		}

		private RestrictionCheckResult CheckRestriction(OrganizationId orgId, ISimpleCache<ADObjectId, bool> memberOfGroupCache, out long maxRecipientMessageSize)
		{
			DeliveryRestriction.<>c__DisplayClass1 CS$<>8__locals1 = new DeliveryRestriction.<>c__DisplayClass1();
			CS$<>8__locals1.orgId = orgId;
			maxRecipientMessageSize = 0L;
			if (this.isJournalReport)
			{
				return RestrictionCheckResult.AcceptedJournalReport;
			}
			if (DeliveryRestriction.IsPrivilegedSender(this.sender, this.isAuthenticated, this.privilegedSenders))
			{
				return RestrictionCheckResult.AcceptedPrivilegedSender;
			}
			if (this.IsMessageTooBig(out maxRecipientMessageSize))
			{
				return (RestrictionCheckResult)2147483649U;
			}
			if (this.recipient == null)
			{
				return ADRecipientRestriction.CheckDeliveryRestrictionForOneOffRecipient(this.sender.ObjectId, this.isAuthenticated);
			}
			GroupItem groupItem = this.recipient as GroupItem;
			IRecipientSession session = null;
			if (this.recipientSession == null)
			{
				ADNotificationAdapter.RunADOperation(delegate()
				{
					session = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(CS$<>8__locals1.orgId), 369, "CheckRestriction", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Categorizer\\Resolver\\DeliveryRestriction.cs");
				}, 0);
				this.recipientSession = session;
			}
			return ADRecipientRestriction.CheckDeliveryRestriction(this.sender.ObjectId, this.isAuthenticated, this.recipient.RejectMessagesFrom, this.recipient.RejectMessagesFromDLMembers, this.recipient.AcceptMessagesOnlyFrom, this.recipient.AcceptMessagesOnlyFromDLMembers, this.recipient.BypassModerationFrom, this.recipient.BypassModerationFromDLMembers, this.recipient.ModeratedBy, (groupItem != null) ? groupItem.ManagedBy : null, this.recipient.RequireAllSendersAreAuthenticated, this.recipient.ModerationEnabled, this.recipient.RecipientType, this.recipientSession, memberOfGroupCache);
		}

		private bool IsMessageTooBig(out long maxRecipientMessageSize)
		{
			maxRecipientMessageSize = 0L;
			Unlimited<ByteQuantifiedSize> maxReceiveSize;
			if (this.recipient == null || this.recipient.MaxReceiveSize.IsUnlimited)
			{
				maxReceiveSize = this.defaultMaxReceiveSize;
			}
			else
			{
				maxReceiveSize = this.recipient.MaxReceiveSize;
			}
			if (!maxReceiveSize.IsUnlimited)
			{
				maxRecipientMessageSize = (long)maxReceiveSize.Value.ToKB();
				return DeliveryRestriction.GetSizeInKiloBytes(this.messageSize) > (long)maxReceiveSize.Value.ToKB();
			}
			return false;
		}

		private RestrictedItem recipient;

		private Sender sender;

		private bool isAuthenticated;

		private bool isJournalReport;

		private long messageSize;

		private Unlimited<ByteQuantifiedSize> defaultMaxReceiveSize;

		private IList<RoutingAddress> privilegedSenders;

		private IRecipientSession recipientSession;
	}
}
