using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RestrictedItem : DirectoryItem
	{
		public RestrictedItem(MailRecipient recipient) : base(recipient)
		{
		}

		public IList<ADObjectId> AcceptMessagesOnlyFrom
		{
			get
			{
				return base.GetListProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.AcceptMessagesOnlyFrom");
			}
		}

		public IList<ADObjectId> AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return base.GetListProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.AcceptMessagesOnlyFromDLMembers");
			}
		}

		public IList<ADObjectId> RejectMessagesFrom
		{
			get
			{
				return base.GetListProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.RejectMessagesFrom");
			}
		}

		public IList<ADObjectId> RejectMessagesFromDLMembers
		{
			get
			{
				return base.GetListProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.RejectMessagesFromDLMembers");
			}
		}

		public bool RequireAllSendersAreAuthenticated
		{
			get
			{
				return base.GetProperty<bool>("Microsoft.Exchange.Transport.DirectoryData.RequireAllSendersAreAuthenticated", false);
			}
		}

		public bool ModerationEnabled
		{
			get
			{
				return base.GetProperty<bool>("Microsoft.Exchange.Transport.DirectoryData.ModerationEnabled", false);
			}
		}

		public IList<ADObjectId> ModeratedBy
		{
			get
			{
				return base.GetListProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.ModeratedBy");
			}
		}

		public IList<ADObjectId> BypassModerationFrom
		{
			get
			{
				return base.GetListProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.BypassModerationFrom");
			}
		}

		public IList<ADObjectId> BypassModerationFromDLMembers
		{
			get
			{
				return base.GetListProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.BypassModerationFromDLMembers");
			}
		}

		public ADObjectId ArbitrationMailbox
		{
			get
			{
				return base.GetProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.ArbitrationMailbox");
			}
		}

		public TransportModerationNotificationFlags SendModerationNotifications
		{
			get
			{
				return (TransportModerationNotificationFlags)base.GetProperty<int>("Microsoft.Exchange.Transport.DirectoryData.SendModerationNotifications", 3);
			}
		}

		public bool BypassNestedModerationEnabled
		{
			get
			{
				return base.GetProperty<bool>("Microsoft.Exchange.Transport.DirectoryData.BypassNestedModerationEnabled", false);
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxReceiveSize
		{
			get
			{
				ulong bytesValue;
				if (base.Recipient.ExtendedProperties.TryGetValue<ulong>("Microsoft.Exchange.Transport.DirectoryData.MaxReceiveSize", out bytesValue))
				{
					ByteQuantifiedSize limitedValue = ByteQuantifiedSize.FromBytes(bytesValue);
					return new Unlimited<ByteQuantifiedSize>(limitedValue);
				}
				return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			}
		}

		public override void PreProcess(Expansion expansion)
		{
			if (this.CheckDeliveryRestrictions(expansion))
			{
				this.Allow(expansion);
			}
		}

		public virtual void Allow(Expansion expansion)
		{
		}

		protected override bool CheckDeliveryRestrictions(Expansion expansion)
		{
			long num;
			RestrictionCheckResult restrictionCheckResult = DeliveryRestriction.CheckRestriction(this, expansion.Sender, expansion.Resolver.IsAuthenticated, expansion.Resolver.MailItem.IsJournalReport(), expansion.Message.OriginalMessageSize, expansion.Configuration.MaxReceiveSize, expansion.Configuration.PrivilegedSenders, expansion.Resolver.ResolverCache, expansion.MailItem.OrganizationId, out num);
			ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "Restriction Check returns {0}: recipient {1} sender {2} authenticated {3} stream size {4}", new object[]
			{
				(int)restrictionCheckResult,
				base.Recipient,
				expansion.Sender,
				expansion.Resolver.IsAuthenticated,
				expansion.Message.OriginalMessageSize
			});
			if (ADRecipientRestriction.Failed(restrictionCheckResult))
			{
				if (restrictionCheckResult == (RestrictionCheckResult)2147483649U)
				{
					base.Recipient.AddDsnParameters("MaxRecipMessageSizeInKB", num);
					base.Recipient.AddDsnParameters("CurrentMessageSizeInKB", expansion.Message.OriginalMessageSize >> 10);
				}
				base.FailRecipient(DeliveryRestriction.GetResponseForResult(restrictionCheckResult));
				return false;
			}
			if (ADRecipientRestriction.Moderated(restrictionCheckResult) && !this.ShouldSkipModerationBasedOnMessage(expansion))
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<MailRecipient>((long)this.GetHashCode(), "Recipient {0} requires moderation", base.Recipient);
				this.ModerateRecipient(expansion);
				return false;
			}
			return true;
		}

		public bool IsDeliveryToGroupRestricted()
		{
			return this.ModerationEnabled || !RestrictedItem.IsNullOrEmptyCollection<ADObjectId>(this.AcceptMessagesOnlyFrom) || !RestrictedItem.IsNullOrEmptyCollection<ADObjectId>(this.AcceptMessagesOnlyFromDLMembers);
		}

		private static bool IsNullOrEmptyCollection<T>(ICollection<T> collection)
		{
			return collection == null || collection.Count == 0;
		}

		protected void ModerateRecipient(Expansion expansion)
		{
			TransportMailItem transportMailItem = TransportMailItem.NewSideEffectMailItem(expansion.MailItem, expansion.MailItem.ADRecipientCache.OrganizationId, LatencyComponent.Categorizer, MailDirectionality.Originating, expansion.MailItem.ExternalOrganizationId);
			ADRecipientCache<TransportMiniRecipient> adrecipientCache = transportMailItem.ADRecipientCache;
			RoutingAddress moderatedRecipientArbitrationMailbox = this.GetModeratedRecipientArbitrationMailbox(adrecipientCache);
			string value = null;
			if (expansion.Sender.ObjectId != null)
			{
				TransportMiniRecipient data = adrecipientCache.FindAndCacheRecipient(expansion.Sender.ObjectId).Data;
				if (data != null)
				{
					value = data.DisplayName;
				}
			}
			if (string.IsNullOrEmpty(value) && expansion.Sender.P2Address != null)
			{
				value = expansion.Sender.P2Address.AddressString;
			}
			if (string.IsNullOrEmpty(value))
			{
				value = (string)expansion.MailItem.From;
			}
			string text;
			if (!this.TryGetFormattedModeratorAddresses(adrecipientCache, out text))
			{
				ExTraceGlobals.ResolverTracer.TraceError((long)this.GetHashCode(), "No moderator addresses.");
				base.FailRecipient(AckReason.NoModeratorAddresses);
				return;
			}
			try
			{
				bool flag = this.SendModerationNotifications == TransportModerationNotificationFlags.Always || (this.SendModerationNotifications == TransportModerationNotificationFlags.Internal && expansion.Configuration.AcceptedDomains.CheckInternal(SmtpDomain.GetDomainPart(expansion.MailItem.From)));
				if (flag && expansion.Message != null && expansion.Message.AutoResponseSuppress != (AutoResponseSuppress)0)
				{
					flag = false;
				}
				ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "Create initiation message. arbMbx='{0}'; recipient='{1}'; sender='{2}'; moderators='{3}'; notification={4}", new object[]
				{
					moderatedRecipientArbitrationMailbox,
					base.Recipient,
					expansion.MailItem.From,
					text,
					flag
				});
				ApprovalInitiation.CreateAndSubmitApprovalInitiation(transportMailItem, expansion.MailItem, base.Recipient, (string)expansion.MailItem.From, text, moderatedRecipientArbitrationMailbox, flag);
				base.Recipient.Ack(AckStatus.SuccessNoDsn, AckReason.ModerationStarted);
			}
			catch (ApprovalInitiation.ApprovalInitiationFailedException ex)
			{
				ExTraceGlobals.ResolverTracer.TraceError<ApprovalInitiation.ApprovalInitiationFailedException>((long)this.GetHashCode(), "Failing recipient because generating approval initiation caused an exception {0}", ex);
				if (ex.ExceptionType == ApprovalInitiation.ApprovalInitiationFailedException.FailureType.ModerationLoop)
				{
					base.Recipient.DsnRequested = DsnRequestedFlags.Never;
				}
				base.FailRecipient(ex.ExceptionSmtpResponse);
			}
			catch (ExchangeDataException arg)
			{
				ExTraceGlobals.ResolverTracer.TraceError<ExchangeDataException>((long)this.GetHashCode(), "Failing recipient because generating approval initiation caused an exception {0}", arg);
				base.FailRecipient(AckReason.ModerationInitFailed);
			}
		}

		protected bool ShouldSkipModerationBasedOnMessage(Expansion expansion)
		{
			if (expansion.BypassChildModeration || expansion.Message.BypassChildModeration)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<string>((long)this.GetHashCode(), "Skipping child moderation with BypassChildModeration: id={0}", expansion.MailItem.InternetMessageId);
				return true;
			}
			HeaderList headers = expansion.MailItem.RootPart.Headers;
			Header header = headers.FindFirst("X-MS-Exchange-Organization-Approval-Approved");
			string address;
			if (header != null && header.TryGetValue(out address) && base.Recipient.Email.Equals((RoutingAddress)address))
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<string>((long)this.GetHashCode(), "Already approved: id={0}", expansion.MailItem.InternetMessageId);
				return true;
			}
			Header header2 = headers.FindFirst("X-MS-Exchange-Organization-Unjournal-Processed");
			if (header2 != null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<string>((long)this.GetHashCode(), "By pass moderation for traffic going to DLs via eha migration or live journaling: id={0}", expansion.MailItem.InternetMessageId);
				return true;
			}
			if (ObjectClass.IsOfClass(expansion.MailItem.Message.MapiMessageClass, "IPM.Note.Microsoft.Approval.Request") && headers.FindFirst("X-MS-Exchange-Organization-Mapi-Admin-Submission") != null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<string>((long)this.GetHashCode(), "Not moderating an approval request id={0}", expansion.MailItem.InternetMessageId);
				return true;
			}
			return false;
		}

		private RoutingAddress GetModeratedRecipientArbitrationMailbox(ADRecipientCache<TransportMiniRecipient> cache)
		{
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.IgnoreArbitrationMailboxForModeratedRecipient.Enabled || this.ArbitrationMailbox == null)
			{
				return RoutingAddress.Empty;
			}
			if (this.ArbitrationMailbox.IsDeleted)
			{
				ExEventLog exEventLog = new ExEventLog(ExTraceGlobals.ResolverTracer.Category, TransportEventLog.GetEventSource());
				exEventLog.LogEvent(TransportEventLogConstants.Tuple_RecipientStampedWithDeletedArbitrationMailbox, null, new object[]
				{
					base.Recipient,
					this.ArbitrationMailbox.Name
				});
				return RoutingAddress.Empty;
			}
			TransportMiniRecipient data = cache.FindAndCacheRecipient(this.ArbitrationMailbox).Data;
			if (data == null)
			{
				return RoutingAddress.Empty;
			}
			return (RoutingAddress)data.PrimarySmtpAddress.ToString();
		}

		private bool TryGetFormattedModeratorAddresses(ADRecipientCache<TransportMiniRecipient> cache, out string moderators)
		{
			moderators = null;
			IList<ADObjectId> list;
			if (this.ModeratedBy == null || this.ModeratedBy.Count == 0)
			{
				GroupItem groupItem = this as GroupItem;
				if (groupItem == null || groupItem.ManagedBy == null || groupItem.ManagedBy.Count == 0)
				{
					return false;
				}
				int num = Math.Min(groupItem.ManagedBy.Count, 25);
				list = new List<ADObjectId>(num);
				for (int i = 0; i < num; i++)
				{
					list.Add(groupItem.ManagedBy[i]);
				}
			}
			else
			{
				list = this.ModeratedBy;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ADObjectId objectId in list)
			{
				TransportMiniRecipient data = cache.FindAndCacheRecipient(objectId).Data;
				if (data != null)
				{
					SmtpAddress primarySmtpAddress = data.PrimarySmtpAddress;
					if (primarySmtpAddress.IsValidAddress && !primarySmtpAddress.Equals(SmtpAddress.NullReversePath))
					{
						stringBuilder.Append((string)primarySmtpAddress);
						stringBuilder.Append(';');
					}
				}
			}
			moderators = stringBuilder.ToString();
			return !string.IsNullOrEmpty(moderators);
		}
	}
}
