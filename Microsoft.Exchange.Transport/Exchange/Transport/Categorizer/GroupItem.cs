using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class GroupItem : ReroutableItem
	{
		public GroupItem(MailRecipient recipient) : base(recipient)
		{
		}

		public DeliveryReportsReceiver SendDeliveryReportsTo
		{
			get
			{
				return (DeliveryReportsReceiver)base.GetProperty<int>("Microsoft.Exchange.Transport.DirectoryData.SendDeliveryReportsTo", 0);
			}
		}

		public bool? SendOofMessageToOriginator
		{
			get
			{
				return base.GetProperty<bool?>("Microsoft.Exchange.Transport.DirectoryData.SendOofMessageToOriginator", null);
			}
		}

		public IList<ADObjectId> ManagedBy
		{
			get
			{
				return base.GetListProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.ManagedBy");
			}
		}

		public TransportMiniRecipient ReadManager(Expansion expansion)
		{
			if (this.ManagedBy == null || this.ManagedBy.Count == 0)
			{
				return null;
			}
			return expansion.MailItem.ADRecipientCache.FindAndCacheRecipient(this.ManagedBy[0]).Data;
		}

		protected override void ProcessLocally(Expansion expansion)
		{
			switch (expansion.Message.Type)
			{
			case ResolverMessageType.LegacyOOF:
			case ResolverMessageType.InternalOOF:
			case ResolverMessageType.ExternalOOF:
			case ResolverMessageType.RN:
			case ResolverMessageType.NRN:
			case ResolverMessageType.DR:
			case ResolverMessageType.DelayedDSN:
			case ResolverMessageType.RelayedDSN:
			case ResolverMessageType.ExpandedDSN:
			case ResolverMessageType.RecallReport:
				base.Recipient.Ack(AckStatus.SuccessNoDsn, AckReason.DLBlockedMessageType);
				return;
			case ResolverMessageType.NDR:
				if (this.SendDeliveryReportsTo != DeliveryReportsReceiver.Manager)
				{
					base.Recipient.Ack(AckStatus.SuccessNoDsn, AckReason.DLBlockedRedirectableMessageType);
					return;
				}
				this.RedirectToManager(expansion);
				return;
			}
			this.ExpandMembers(expansion);
		}

		protected override void ProcessRemotely()
		{
			base.ApplyTemplate(MessageTemplate.Default);
		}

		protected override bool CheckDeliveryRestrictions(Expansion expansion)
		{
			if (expansion.Resolver.Message.DlExpansionProhibited)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "The sender prohibited DL expansion, Recipient {0} will NDR", base.Recipient.Email);
				base.FailRecipient(AckReason.DLExpansionBlockedBySender);
				return false;
			}
			long num;
			RestrictionCheckResult restrictionCheckResult = DeliveryRestriction.CheckRestriction(this, expansion.Sender, expansion.Resolver.IsAuthenticated, expansion.Resolver.MailItem.IsJournalReport(), expansion.Message.OriginalMessageSize, expansion.Configuration.MaxReceiveSize, expansion.Configuration.PrivilegedSenders, expansion.Resolver.ResolverCache, expansion.MailItem.OrganizationId, out num);
			if (restrictionCheckResult == (RestrictionCheckResult)2147483654U)
			{
				restrictionCheckResult = (RestrictionCheckResult)2147483657U;
			}
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
				if (Components.TransportAppConfig.Resolver.ForceNdrForDlRestrictionError)
				{
					base.Recipient.DsnRequested = DsnRequestedFlags.Default;
				}
				base.FailRecipient(DeliveryRestriction.GetResponseForResult(restrictionCheckResult));
				return false;
			}
			if (ADRecipientRestriction.Moderated(restrictionCheckResult) && !base.ShouldSkipModerationBasedOnMessage(expansion))
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<MailRecipient>((long)this.GetHashCode(), "Recipient {0} requires moderation", base.Recipient);
				base.ModerateRecipient(expansion);
				return false;
			}
			return true;
		}

		private void AddDuplicateEntryForMessageTracking(HashSet<RoutingAddress> duplicatesTrackingSet, TransportMiniRecipient entry)
		{
			string primarySmtpAddress = DirectoryItem.GetPrimarySmtpAddress(entry);
			if (!string.IsNullOrEmpty(primarySmtpAddress))
			{
				duplicatesTrackingSet.Add(new RoutingAddress(primarySmtpAddress));
				ExTraceGlobals.ResolverTracer.TraceDebug<string>((long)this.GetHashCode(), "Duplicate expansion for item {0}.", primarySmtpAddress);
			}
		}

		private void RedirectToManager(Expansion expansion)
		{
			TransportMiniRecipient transportMiniRecipient = this.ReadManager(expansion);
			if (transportMiniRecipient == null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "manager missing");
				base.Recipient.Ack(AckStatus.SuccessNoDsn, AckReason.DLRedirectManagerNotFound);
				return;
			}
			Expansion expansion2 = base.Expand(expansion, GroupItem.nullReversePathTemplate, HistoryType.Forwarded);
			if (expansion2 == null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "manager redirection loop");
				return;
			}
			MailRecipient mailRecipient = expansion2.Add(transportMiniRecipient, DsnRequestedFlags.Never);
			if (mailRecipient == null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "manager has a bad or missing primary SMTP address");
				base.Recipient.Ack(AckStatus.SuccessNoDsn, AckReason.DLRedirectManagerNotValid);
				return;
			}
			MsgTrackRedirectInfo msgTrackInfo = new MsgTrackRedirectInfo(base.Recipient.Email, mailRecipient.Email, null);
			MessageTrackingLog.TrackRedirect(MessageTrackingSource.ROUTING, expansion.MailItem, msgTrackInfo);
			base.Recipient.Ack(AckStatus.SuccessNoDsn, AckReason.DLRedirectedToManager);
		}

		private void GetExpansionSettings(Expansion expansion, out MessageTemplate template, out DsnRequestedFlags dsnFlags)
		{
			string reversePath = null;
			AutoResponseSuppress autoResponseSuppress = AutoResponseSuppress.DR | AutoResponseSuppress.RN | AutoResponseSuppress.NRN | AutoResponseSuppress.OOF | AutoResponseSuppress.AutoReply;
			dsnFlags = DsnRequestedFlags.Never;
			switch (this.SendDeliveryReportsTo)
			{
			case DeliveryReportsReceiver.Manager:
			{
				TransportMiniRecipient transportMiniRecipient = this.ReadManager(expansion);
				if (transportMiniRecipient != null)
				{
					string primarySmtpAddress = DirectoryItem.GetPrimarySmtpAddress(transportMiniRecipient);
					if (primarySmtpAddress != null)
					{
						reversePath = primarySmtpAddress;
						dsnFlags = (DsnRequestedFlags.Failure | DsnRequestedFlags.Delay);
					}
				}
				break;
			}
			case DeliveryReportsReceiver.Originator:
				reversePath = null;
				dsnFlags = base.Recipient.DsnRequested;
				autoResponseSuppress &= ~(AutoResponseSuppress.RN | AutoResponseSuppress.NRN);
				break;
			}
			if (this.SendOofMessageToOriginator ?? false)
			{
				autoResponseSuppress &= ~(AutoResponseSuppress.OOF | AutoResponseSuppress.AutoReply);
			}
			bool suppressRecallReport = expansion.Message.Type == ResolverMessageType.Recall && this.SendDeliveryReportsTo != DeliveryReportsReceiver.Originator;
			template = new MessageTemplate(reversePath, autoResponseSuppress, null, false, false, suppressRecallReport, base.BypassNestedModerationEnabled);
		}

		private void ExpandMembers(Expansion expansion)
		{
			if (base.TopLevelRecipient)
			{
				this.SetDeliverySettings(expansion);
			}
			else if (expansion.Resolver.TopLevelGroupItem == null)
			{
				this.SetDeliverySettings(expansion);
			}
			GroupItem topLevelGroupItem = expansion.Resolver.TopLevelGroupItem;
			MessageTemplate template = topLevelGroupItem.topLevelTemplate;
			DsnRequestedFlags dsnFlags = topLevelGroupItem.topLevelDsnFlags;
			Expansion children = base.Expand(expansion, template, HistoryType.Expanded);
			if (children == null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "expansion loop detected at {0}", base.Recipient.Email);
				return;
			}
			IRecipientSession session = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				session = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(expansion.MailItem.ADRecipientCache.OrganizationId), 533, "ExpandMembers", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Categorizer\\Resolver\\GroupItem.cs");
			}, 0);
			IADDistributionList group = null;
			try
			{
				ADNotificationAdapter.RunADOperation(delegate()
				{
					group = (session.FindByObjectGuid(this.ObjectGuid) as IADDistributionList);
				});
			}
			catch (DataValidationException)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "invalid group object");
				base.FailRecipient(AckReason.InvalidGroupForExpansion);
				return;
			}
			List<RecipientItem> trackingList = new List<RecipientItem>();
			HashSet<RoutingAddress> duplicatesTrackingSet = new HashSet<RoutingAddress>();
			if (group != null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "Expanding group {0}. BypassNestedModerationEnabled = {1}; BypassChildModeration = {2}; Message.BypassChildModeration = {3}", new object[]
				{
					base.Email,
					base.BypassNestedModerationEnabled,
					expansion.BypassChildModeration,
					expansion.Message.BypassChildModeration
				});
				try
				{
					try
					{
						children.CacheSerializedHistory();
						int chipIndex = expansion.Resolver.PendingChipItemsCount;
						ADNotificationAdapter.RunADOperation(delegate()
						{
							expansion.Resolver.ClearAllPendingChipsStartingAtIndex(chipIndex + 1);
							trackingList.Clear();
							duplicatesTrackingSet.Clear();
							IEnumerable<TransportMiniRecipient> enumerable = expansion.MailItem.ADRecipientCache.ExpandGroup<TransportMiniRecipient>(group);
							bool flag = false;
							foreach (TransportMiniRecipient entry in enumerable)
							{
								if (this.ShouldExpandChild(expansion, entry))
								{
									RecipientItem recipientItem = children.AddGroupExpansionItem(entry, dsnFlags);
									if (recipientItem != null)
									{
										trackingList.Add(recipientItem);
									}
								}
								else
								{
									this.AddDuplicateEntryForMessageTracking(duplicatesTrackingSet, entry);
								}
								flag = true;
							}
							ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Finished expanding group {0}.", this.Email);
							foreach (RecipientItem recipientItem2 in trackingList)
							{
								recipientItem2.AddItemVisited(expansion);
							}
							if (!flag && this.RecipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup)
							{
								ExTraceGlobals.ResolverTracer.TraceWarning<Guid, string>((long)this.GetHashCode(), "Found no members for non-universal group {0} on GC {1}", this.ObjectGuid, session.Source);
							}
						});
					}
					catch (DataValidationException)
					{
						ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "DataValidationException from DL expansion");
						if (base.RecipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.DynamicDistributionGroup)
						{
							ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "DDL misconfiguration");
							base.FailRecipient(AckReason.DDLMisconfiguration);
							return;
						}
						throw;
					}
					goto IL_267;
				}
				finally
				{
					children.ClearSerializedHistory();
				}
			}
			ExTraceGlobals.ResolverTracer.TraceError<Guid>((long)this.GetHashCode(), "Group object {0} not found", base.ObjectGuid);
			IL_267:
			AckStatusAndResponse ackStatusAndResponse;
			if ((base.Recipient.DsnRequested & DsnRequestedFlags.Success) != DsnRequestedFlags.Default && (dsnFlags & DsnRequestedFlags.Success) == DsnRequestedFlags.Default)
			{
				ackStatusAndResponse.SmtpResponse = AckReason.DLExpanded;
				ackStatusAndResponse.AckStatus = AckStatus.Expand;
			}
			else
			{
				ackStatusAndResponse.SmtpResponse = AckReason.DLExpandedSilently;
				ackStatusAndResponse.AckStatus = AckStatus.SuccessNoDsn;
			}
			if (!base.TopLevelRecipient)
			{
				base.Recipient.Ack(ackStatusAndResponse.AckStatus, ackStatusAndResponse.SmtpResponse);
			}
			else
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress, AckStatus, SmtpResponse>((long)this.GetHashCode(), "Pend acking the top level recipient {0} until the group is completely expanded. AckStatus {1}, AckReason {2}.", base.Recipient.Email, ackStatusAndResponse.AckStatus, ackStatusAndResponse.SmtpResponse);
				expansion.Resolver.ParkCurrentTopLevelRecipientAck(ackStatusAndResponse);
			}
			MsgTrackExpandInfo msgTrackInfo = new MsgTrackExpandInfo(session.Source, base.Recipient.Email, null, ackStatusAndResponse.SmtpResponse.ToString());
			MessageTrackingLog.TrackExpand<RecipientItem>(MessageTrackingSource.ROUTING, expansion.MailItem, msgTrackInfo, trackingList);
			if (duplicatesTrackingSet.Count != 0)
			{
				MessageTrackingLog.TrackExpandEvent<RoutingAddress>(MessageTrackingSource.ROUTING, expansion.MailItem, msgTrackInfo, duplicatesTrackingSet, MessageTrackingEvent.DUPLICATEEXPAND);
			}
		}

		private void SetDeliverySettings(Expansion expansion)
		{
			MessageTemplate messageTemplate;
			DsnRequestedFlags dsnRequestedFlags;
			this.GetExpansionSettings(expansion, out messageTemplate, out dsnRequestedFlags);
			this.topLevelTemplate = messageTemplate;
			this.topLevelDsnFlags = dsnRequestedFlags;
			expansion.Resolver.TopLevelGroupItem = this;
		}

		private bool ShouldExpandChild(Expansion expansion, TransportMiniRecipient entry)
		{
			return base.BypassNestedModerationEnabled || expansion.BypassChildModeration || expansion.Message.BypassChildModeration || !expansion.Resolver.ResolverCache.RecipientExists(entry.Id.ObjectGuid);
		}

		private static MessageTemplate nullReversePathTemplate = new MessageTemplate((string)RoutingAddress.NullReversePath, (AutoResponseSuppress)0, null, false, false, false, false);

		private MessageTemplate topLevelTemplate;

		private DsnRequestedFlags topLevelDsnFlags;
	}
}
