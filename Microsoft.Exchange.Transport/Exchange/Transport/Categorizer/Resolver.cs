using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class Resolver
	{
		public Resolver(TransportMailItem mailItem, TaskContext taskContext, Resolver.DeferBifurcatedDelegate deferHandler) : this(mailItem, taskContext, deferHandler, new ResolverCache())
		{
		}

		public Resolver(TransportMailItem mailItem, TaskContext taskContext, Resolver.DeferBifurcatedDelegate deferHandler, ResolverCache cache)
		{
			this.mailItem = mailItem;
			this.taskContext = taskContext;
			this.deferHandler = deferHandler;
			this.configuration = new ResolverConfiguration(mailItem.OrganizationId, mailItem.TransportSettings);
			this.message = new ResolverMessage(this.mailItem.Message, this.mailItem.MimeSize);
			this.backupMailItem = new LimitedMailItem(this, taskContext);
			this.isAuthenticated = MultilevelAuth.IsAuthenticated(this.mailItem);
			this.resolverCache = cache;
		}

		public static ExEventLog EventLogger
		{
			get
			{
				return Resolver.eventLogger;
			}
		}

		public static ResolverPerfCountersInstance PerfCounters
		{
			get
			{
				return Resolver.perfCounters;
			}
		}

		public ResolverConfiguration Configuration
		{
			get
			{
				return this.configuration;
			}
		}

		public TransportMailItem MailItem
		{
			get
			{
				return this.mailItem;
			}
		}

		public TaskContext TaskContext
		{
			get
			{
				return this.taskContext;
			}
		}

		public ResolverMessage Message
		{
			get
			{
				return this.message;
			}
		}

		public Stack<RecipientItem> RecipientStack
		{
			get
			{
				return this.recipientStack;
			}
		}

		public Sender Sender
		{
			get
			{
				return this.sender;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return this.isAuthenticated;
			}
		}

		public ResolverCache ResolverCache
		{
			get
			{
				return this.resolverCache;
			}
		}

		public int PendingChipItemsCount
		{
			get
			{
				return this.backupMailItem.PendingChipItemsCount;
			}
		}

		public RecipientItem CurrentTopLevelRecipientInProcess
		{
			get
			{
				return this.currentTopLevelRecipientInProcess;
			}
		}

		public GroupItem TopLevelGroupItem
		{
			get
			{
				return this.topLevelGroupItem;
			}
			set
			{
				this.topLevelGroupItem = value;
			}
		}

		public static bool IsResolved(MailRecipient recipient)
		{
			return recipient.ExtendedProperties.GetValue<bool>("Microsoft.Exchange.Transport.Resolved", false);
		}

		public static void SetResolved(MailRecipient recipient)
		{
			recipient.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.Resolved", true);
		}

		public static void ClearResolverProperties(MailRecipient recipient)
		{
			recipient.ExtendedProperties.Remove("Microsoft.Exchange.Transport.Resolved");
			recipient.ExtendedProperties.Remove("Microsoft.Exchange.Transport.Processed");
			foreach (CachedProperty cachedProperty in DirectoryItem.AllCachedProperties)
			{
				recipient.ExtendedProperties.Remove(cachedProperty.ExtendedProperty);
			}
		}

		public static void ClearResolverAndTransportSettings(TransportMailItem mailItem)
		{
			if (mailItem.ADRecipientCache != null)
			{
				mailItem.ADRecipientCache.Clear();
			}
			mailItem.ExtendedProperties.Remove("Microsoft.Exchange.Transport.Sender.Resolved");
			foreach (CachedProperty cachedProperty in SenderSchema.CachedProperties)
			{
				mailItem.ExtendedProperties.Remove(cachedProperty.ExtendedProperty);
			}
			mailItem.ClearTransportSettings();
		}

		public static void InitializePerfCounters()
		{
			try
			{
				Resolver.perfCounters = ResolverPerfCounters.GetInstance("_total");
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.ResolverTracer.TraceError<InvalidOperationException>(0L, "Failed to initialize performance counters: {0}", ex);
				Resolver.EventLogger.LogEvent(TransportEventLogConstants.Tuple_ResolverPerfCountersLoadFailure, null, new object[]
				{
					ex.ToString()
				});
			}
		}

		public static bool TryEncapsulate(ProxyAddress address, out RoutingAddress outer)
		{
			outer = RoutingAddress.Empty;
			SmtpProxyAddress smtpProxyAddress = address as SmtpProxyAddress;
			if (smtpProxyAddress == null && !SmtpProxyAddress.TryEncapsulate(address, Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName, out smtpProxyAddress))
			{
				return false;
			}
			outer = new RoutingAddress(smtpProxyAddress.AddressString);
			return true;
		}

		public static bool TryDeencapsulate(RoutingAddress address, out ProxyAddress inner)
		{
			return Resolver.TryDeencapsulate(address, Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName, out inner);
		}

		public static bool TryDeencapsulate(RoutingAddress address, string firstOrgDefaultDomainName, out ProxyAddress inner)
		{
			inner = null;
			return string.Equals(address.DomainPart, firstOrgDefaultDomainName, StringComparison.OrdinalIgnoreCase) && SmtpProxyAddress.TryDeencapsulate((string)address, out inner);
		}

		public static bool IsEncapsulatedAddress(RoutingAddress address)
		{
			return Resolver.IsEncapsulatedAddress(address, Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName);
		}

		public void BifurcateAndDeferAmbigousRecipients(List<MailRecipient> recipientsToDefer, TransportMailItem mailItem, TaskContext taskContext, SmtpResponse ackReason)
		{
			this.BifurcateAndDeferRecipients(recipientsToDefer, mailItem, taskContext, ackReason, DeferReason.AmbiguousRecipient, Components.TransportAppConfig.Resolver.DeferralTimeForAmbiguousRecipients);
		}

		public void BifurcateAndDeferRecipients(List<MailRecipient> recipientsToDefer, TransportMailItem mailItem, TaskContext taskContext, SmtpResponse ackReason)
		{
			this.BifurcateAndDeferRecipients(recipientsToDefer, mailItem, taskContext, ackReason, DeferReason.TransientFailure, TimeSpan.FromMinutes(Components.TransportAppConfig.Resolver.ResolverRetryInterval));
		}

		private void BifurcateAndDeferRecipients(List<MailRecipient> recipientsToDefer, TransportMailItem mailItem, TaskContext taskContext, SmtpResponse ackReason, DeferReason deferReason, TimeSpan deferTime)
		{
			TransportMailItem transportMailItem = Resolver.ForkMailItem(recipientsToDefer, mailItem, deferReason);
			foreach (MailRecipient mailRecipient in transportMailItem.Recipients)
			{
				mailRecipient.Ack(AckStatus.Retry, ackReason);
				Resolver.ClearResolverProperties(mailRecipient);
			}
			this.deferHandler(transportMailItem, taskContext, deferReason, deferTime);
		}

		public bool RewriteEmail(MailRecipient recipient, ProxyAddress email, MessageTrackingSource messageTrackingSource)
		{
			RoutingAddress routingAddress;
			if (!Resolver.TryEncapsulate(email, out routingAddress))
			{
				return false;
			}
			if (recipient.Email != routingAddress)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress>(0L, "recipient address {0} to be rewritten", recipient.Email);
				string value = recipient.ExtendedProperties.GetValue<string>("Microsoft.Exchange.Transport.DirectoryData.LegacyExchangeDN", null);
				MsgTrackResolveInfo msgTrackInfo = new MsgTrackResolveInfo(value, recipient.Email, routingAddress);
				MessageTrackingLog.TrackResolve(messageTrackingSource, this.mailItem, msgTrackInfo);
				ProxyAddress proxyAddress;
				if (recipient.ORcpt == null && (!(email.Prefix == ProxyAddressPrefix.Smtp) || !Resolver.TryDeencapsulate(recipient.Email, out proxyAddress)))
				{
					recipient.ORcpt = "rfc822;" + recipient.Email;
				}
				recipient.Email = routingAddress;
				ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress, string>(0L, "recipient address rewritten to {0}, ORCPT = {1}", recipient.Email, recipient.ORcpt ?? "<null>");
			}
			return true;
		}

		public void ResolveSenderAndTopLevelRecipients()
		{
			this.sender = new Sender(this.mailItem);
			this.LookupRecipientsAndSender();
			Sender.Resolve(this.GetADRawEntryFromCache(this.sender.P1Address), this.GetADRawEntryFromCache(this.sender.P2Address), this.mailItem);
			foreach (MailRecipient resolved in this.mailItem.Recipients.AllUnprocessed)
			{
				Resolver.SetResolved(resolved);
			}
		}

		public void ResolveAll()
		{
			this.ResolveSenderAndTopLevelRecipients();
			if (!this.CheckSenderSizeRestriction())
			{
				return;
			}
			this.recipientStack = new Stack<RecipientItem>();
			Expansion expansion = new Expansion(this);
			foreach (MailRecipient mailRecipient in this.mailItem.Recipients.AllUnprocessed)
			{
				if (!mailRecipient.IsProcessed && !mailRecipient.ExtendedProperties.GetValue<bool>("Microsoft.Exchange.Transport.Processed", false))
				{
					RecipientItem recipientItem = RecipientItem.Create(mailRecipient, true);
					recipientItem.AddItemVisited(expansion);
					expansion.Add(recipientItem, true);
				}
			}
			if (!this.CheckRecipientLimit())
			{
				return;
			}
			while (this.recipientStack.Count > 0)
			{
				RecipientItem recipientItem2 = this.recipientStack.Pop();
				ExTraceGlobals.ResolverTracer.TracePfd<int, RoutingAddress, long>(0L, "PFD CAT {0} Resolving recipient {1} (msgId={2})", 17314, recipientItem2.Recipient.Email, this.mailItem.RecordId);
				Expansion expansion2 = Expansion.Resume(recipientItem2.Recipient, this);
				if (recipientItem2.TopLevelRecipient)
				{
					this.CheckDLLimitsAndAckRecipientIfPending();
					this.CommitPendingChips();
					this.ResetStateForNextTopLevelRecipient(recipientItem2);
				}
				try
				{
					recipientItem2.Process(expansion2);
					if (!recipientItem2.TopLevelRecipient || !this.ackPending)
					{
						recipientItem2.Recipient.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.Processed", true);
					}
				}
				catch (UnresolvedRecipientBifurcatedTransientException)
				{
					ExTraceGlobals.ResolverTracer.TraceWarning(0L, "Caught UnresolvedRecipientBifurcatedTransientException; leaving recipient unprocessed");
				}
			}
			this.CheckDLLimitsAndAckRecipientIfPending();
			this.CommitBackupMailItem();
		}

		public void CommitBackupMailItem()
		{
			this.CommitPendingChips();
			this.backupMailItem.CommitLogAndSubmit();
		}

		public MailRecipient AddRecipient(string primarySmtpAddress, TransportMiniRecipient entry, DsnRequestedFlags dsnRequested, string orcpt, bool commitIfChipIsFull, out bool addedToPrimary)
		{
			TransportMailItem transportMailItem = this.mailItem;
			bool flag = Resolver.IsExpandableEntry(entry);
			MailRecipient mailRecipient;
			if (this.mailItem.Recipients.Count > ResolverConfiguration.ExpansionSizeLimit && !flag)
			{
				mailRecipient = this.backupMailItem.AddRecipient(primarySmtpAddress, commitIfChipIsFull, out transportMailItem);
				addedToPrimary = false;
			}
			else
			{
				addedToPrimary = true;
				mailRecipient = this.mailItem.Recipients.Add(primarySmtpAddress);
			}
			mailRecipient.ORcpt = orcpt;
			mailRecipient.DsnRequested = dsnRequested;
			DirectoryItem.Set(entry, mailRecipient);
			Resolver.SetResolved(mailRecipient);
			if (!this.encounteredMemoryPressure)
			{
				if (!this.ShouldShrinkMemory())
				{
					transportMailItem.ADRecipientCache.AddCacheEntry(new SmtpProxyAddress(primarySmtpAddress, true), new Result<TransportMiniRecipient>(entry, null));
				}
				else
				{
					this.ClearADRecipientCache();
					this.encounteredMemoryPressure = true;
				}
			}
			if (!flag)
			{
				this.numRecipientsAddedForCurrentExpansion++;
			}
			return mailRecipient;
		}

		protected virtual bool ShouldShrinkMemory()
		{
			return Components.ResourceManager.ShouldShrinkDownMemoryCaches;
		}

		public void ClearAllPendingChipsStartingAtIndex(int index)
		{
			int num = this.backupMailItem.ClearAllPendingChipsStartingAtIndex(index, null);
			if (num > 0)
			{
				if (this.numRecipientsAddedForCurrentExpansion < num * ResolverConfiguration.ExpansionSizeLimit)
				{
					throw new InvalidOperationException("Trying to remove more recipients than was originally added");
				}
				this.numRecipientsAddedForCurrentExpansion -= num * ResolverConfiguration.ExpansionSizeLimit;
			}
		}

		public void ParkCurrentTopLevelRecipientAck(AckStatusAndResponse ackStatusAndResponse)
		{
			if (this.ackPending)
			{
				throw new InvalidOperationException("Ack already pending for the top level recipient");
			}
			this.ackPending = true;
			this.pendingAckStatusAndResponse = ackStatusAndResponse;
		}

		internal static bool IsEncapsulatedAddress(RoutingAddress address, string defaultDomainName)
		{
			return string.Equals(address.DomainPart, defaultDomainName, StringComparison.OrdinalIgnoreCase) && SmtpProxyAddress.IsEncapsulatedAddress((string)address);
		}

		internal static RoutingAddress GetPrimarySmtpAddress(ADRawEntry entry)
		{
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)entry[ADRecipientSchema.EmailAddresses];
			ProxyAddress proxyAddress = proxyAddressCollection.FindPrimary(ProxyAddressPrefix.Smtp);
			string address = (null == proxyAddress) ? SmtpAddress.Empty.ToString() : proxyAddress.AddressString;
			return new RoutingAddress(address);
		}

		internal static string GetLegacyExchangeDN(ADRawEntry entry)
		{
			return (string)entry[ADRecipientSchema.LegacyExchangeDN];
		}

		internal static bool IsSmtpAddress(ProxyAddress address)
		{
			return address.Prefix.Equals(ProxyAddressPrefix.Smtp);
		}

		internal static bool IsX500Address(ProxyAddress address)
		{
			return address.Prefix.Equals(ProxyAddressPrefix.X500);
		}

		internal static bool IsExAddress(ProxyAddress address)
		{
			return address.Prefix.Equals(ProxyAddressPrefix.LegacyDN);
		}

		internal static bool IsInvalidAddress(ProxyAddress address)
		{
			return address.Prefix.Equals(ProxyAddressPrefix.Invalid);
		}

		internal static void FailRecipient(MailRecipient recipient, SmtpResponse response)
		{
			recipient.Ack(AckStatus.Fail, response);
			if (Resolver.PerfCounters != null)
			{
				Resolver.PerfCounters.FailedRecipientsTotal.Increment();
			}
		}

		[Conditional("DEBUG")]
		internal void LogMailItem(string title, TransportMailItem mailItem)
		{
		}

		private static bool TryGetRecipientResolveAddress(MailRecipient recipient, out ProxyAddress resolveAddress)
		{
			resolveAddress = null;
			ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress>(0L, "Get ProxyAddress for {0}", recipient.Email);
			if (recipient.IsProcessed)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<AckStatus, SmtpResponse>(0L, "skipping acked recipient: AckStatus = {0}, SmtpResponse = {1}", recipient.AckStatus, recipient.SmtpResponse);
				return false;
			}
			if (Resolver.IsResolved(recipient))
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress>(0L, "skipping previously resolved recipient {0}", recipient.Email);
				return false;
			}
			if (Resolver.TryDeencapsulate(recipient.Email, out resolveAddress))
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<ProxyAddress>(0L, "inner address = {0}", resolveAddress);
				if (Resolver.IsSmtpAddress(resolveAddress))
				{
					ExTraceGlobals.ResolverTracer.TraceDebug(0L, "encapsulated SMTP address");
					Resolver.FailRecipient(recipient, AckReason.EncapsulatedSmtpAddress);
					return false;
				}
				if (Resolver.IsX500Address(resolveAddress))
				{
					ExTraceGlobals.ResolverTracer.TraceDebug(0L, "encapsulated X.500 address");
					Resolver.FailRecipient(recipient, AckReason.EncapsulatedX500Address);
					return false;
				}
				if (Resolver.IsInvalidAddress(resolveAddress))
				{
					ExTraceGlobals.ResolverTracer.TraceDebug(0L, "encapsulated INVALID address");
					Resolver.FailRecipient(recipient, AckReason.EncapsulatedInvalidAddress);
					return false;
				}
			}
			else
			{
				resolveAddress = new SmtpProxyAddress((string)recipient.Email, false);
			}
			return true;
		}

		private static TransportMailItem ForkMailItem(List<MailRecipient> recipientsToDefer, TransportMailItem mailItem, DeferReason deferReason)
		{
			TransportMailItem transportMailItem = mailItem.NewCloneWithoutRecipients();
			foreach (MailRecipient mailRecipient in recipientsToDefer)
			{
				mailRecipient.MoveTo(transportMailItem);
				if (deferReason == DeferReason.AmbiguousRecipient)
				{
					string text = mailRecipient.Email.ToString();
					Resolver.EventLogger.LogEvent(mailItem.OrganizationId, TransportEventLogConstants.Tuple_AmbiguousRecipient, text, text);
					string notificationReason = string.Format("More than one Active Directory object is configured with the recipient address {0}.", text);
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "A3C4E4B6-490E-40e2-89D6-7FBF03ADCFB5", null, notificationReason, ResultSeverityLevel.Warning, false);
				}
			}
			transportMailItem.CommitLazy();
			return transportMailItem;
		}

		private static bool IsExpandableEntry(ADRawEntry entry)
		{
			if (entry[ADRecipientSchema.ForwardingAddress] != null)
			{
				return true;
			}
			Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType = (Microsoft.Exchange.Data.Directory.Recipient.RecipientType)entry[ADRecipientSchema.RecipientType];
			return recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Group || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalSecurityGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.DynamicDistributionGroup;
		}

		private static void AddP2RecipientsToPrefetch(EmailRecipientCollection recipientCollection, List<ProxyAddress> lookupAddressList, HashSet<ProxyAddress> uniqueProxyAddresses, List<ProxyAddress> senderAndP2Recipients)
		{
			if (recipientCollection.Count > ADRecipientObjectSession.ReadMultipleMaxBatchSize * 5)
			{
				return;
			}
			foreach (EmailRecipient emailRecipient in recipientCollection)
			{
				if (SmtpAddress.IsValidSmtpAddress(emailRecipient.SmtpAddress))
				{
					ProxyAddress item = new SmtpProxyAddress(emailRecipient.SmtpAddress, false);
					if (uniqueProxyAddresses.Add(item))
					{
						lookupAddressList.Add(item);
						senderAndP2Recipients.Add(item);
					}
				}
			}
		}

		private TransportMiniRecipient GetADRawEntryFromCache(ProxyAddress proxyAddress)
		{
			if (proxyAddress == null)
			{
				return null;
			}
			Result<TransportMiniRecipient> result;
			if (this.mailItem.ADRecipientCache.TryGetValue(proxyAddress, out result))
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<ProxyAddress>(0L, "Find ADRawEntry for ProxyAddress {0} in the cache", proxyAddress);
				return result.Data;
			}
			ExTraceGlobals.ResolverTracer.TraceDebug<ProxyAddress>(0L, "Doesn't find ADRawEntry for ProxyAddress {0} in the cache", proxyAddress);
			return null;
		}

		private void LookupRecipientsAndSender()
		{
			List<ProxyAddress> list = new List<ProxyAddress>(this.mailItem.Recipients.Count + 2);
			List<MailRecipient> list2 = new List<MailRecipient>(this.mailItem.Recipients.Count);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2951097661U);
			foreach (MailRecipient mailRecipient in this.mailItem.Recipients.AllUnprocessed)
			{
				ProxyAddress item;
				if (Resolver.TryGetRecipientResolveAddress(mailRecipient, out item))
				{
					list.Add(item);
					list2.Add(mailRecipient);
				}
			}
			List<ProxyAddress> list3 = new List<ProxyAddress>();
			if (this.sender.P1Address != null)
			{
				list.Add(this.sender.P1Address);
				list3.Add(this.sender.P1Address);
			}
			if (this.sender.P2Address != null)
			{
				list.Add(this.sender.P2Address);
				list3.Add(this.sender.P2Address);
			}
			int count = list.Count;
			HashSet<ProxyAddress> uniqueProxyAddresses = new HashSet<ProxyAddress>(list);
			Resolver.AddP2RecipientsToPrefetch(this.mailItem.Message.To, list, uniqueProxyAddresses, list3);
			Resolver.AddP2RecipientsToPrefetch(this.mailItem.Message.Cc, list, uniqueProxyAddresses, list3);
			Resolver.AddP2RecipientsToPrefetch(this.mailItem.Message.Bcc, list, uniqueProxyAddresses, list3);
			IList<Result<TransportMiniRecipient>> list4 = this.mailItem.ADRecipientCache.FindAndCacheRecipients(list);
			List<MailRecipient> list5 = null;
			for (int i = 0; i < list4.Count; i++)
			{
				if (i < list2.Count)
				{
					bool flag;
					this.CopyADAttributesToRecipient(list4[i], list2[i], out flag);
					if (flag)
					{
						if (list5 == null)
						{
							list5 = new List<MailRecipient>();
						}
						list5.Add(list2[i]);
					}
				}
				else if (i < count)
				{
					this.UpdateSenderPerfCounters(list4[i], list[i]);
				}
			}
			if (list5 != null && list5.Count > 0)
			{
				if (Components.TransportAppConfig.Resolver.NDRForAmbiguousRecipients)
				{
					ExTraceGlobals.ResolverTracer.TraceError(0L, "NDRing for ambiguous recipients");
					using (List<MailRecipient>.Enumerator enumerator2 = list5.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							MailRecipient recipient = enumerator2.Current;
							Resolver.FailRecipient(recipient, AckReason.AmbiguousAddressPermanent);
						}
						goto IL_283;
					}
				}
				ExTraceGlobals.ResolverTracer.TraceDebug(0L, "Deferring for ambiguous recipients");
				this.BifurcateAndDeferAmbigousRecipients(list5, this.mailItem, this.taskContext, AckReason.AmbiguousAddressTransient);
			}
			IL_283:
			this.MarkSenderAndP2Recipients(this.mailItem.ADRecipientCache, list3);
		}

		private void MarkSenderAndP2Recipients(ADRecipientCache<TransportMiniRecipient> adRecipientCache, List<ProxyAddress> senderAndP2Recipients)
		{
			foreach (ProxyAddress proxyAddress in senderAndP2Recipients)
			{
				Result<TransportMiniRecipient> result;
				if (adRecipientCache.TryGetValue(proxyAddress, out result) && result.Data != null)
				{
					result.Data.SetSenderOrP2RecipientEntry();
				}
			}
		}

		private void CopyADAttributesToRecipient(Result<TransportMiniRecipient> result, MailRecipient recipient, out bool isAmbiguousRecipient)
		{
			string arg = recipient.Email.ToString();
			isAmbiguousRecipient = false;
			ExTraceGlobals.ResolverTracer.TraceDebug<string, ProviderError>(0L, "Lookup result for recipient {0} is {1}", arg, result.Error);
			if (result.Error == null)
			{
				TransportMiniRecipient data = result.Data;
				if (data != null)
				{
					string primarySmtpAddress = DirectoryItem.GetPrimarySmtpAddress(data);
					if (primarySmtpAddress == null)
					{
						ExTraceGlobals.ResolverTracer.TraceError(0L, "invalid or missing primary SMTP address");
						Resolver.FailRecipient(recipient, AckReason.BadOrMissingPrimarySmtpAddress);
						return;
					}
					DirectoryItem.Set(data, recipient);
					this.RewriteEmail(recipient, new SmtpProxyAddress(primarySmtpAddress, false), MessageTrackingSource.ROUTING);
					return;
				}
			}
			else if (result.Error is NonUniqueAddressError)
			{
				ExTraceGlobals.ResolverTracer.TraceError(0L, "ambiguous address");
				isAmbiguousRecipient = true;
				if (Resolver.PerfCounters != null)
				{
					Resolver.PerfCounters.AmbiguousRecipientsTotal.Increment();
					return;
				}
			}
			else if (result.Error is ObjectValidationError)
			{
				ExTraceGlobals.ResolverTracer.TraceError(0L, "Invalid data");
				Resolver.FailRecipient(recipient, AckReason.InvalidObjectOnSearch);
			}
		}

		private void UpdateSenderPerfCounters(Result<TransportMiniRecipient> result, ProxyAddress senderAddress)
		{
			if (Resolver.PerfCounters == null || result.Error == null)
			{
				return;
			}
			if (OneOffItem.IsLocalAddress(senderAddress, this.configuration.AcceptedDomains))
			{
				Resolver.PerfCounters.UnresolvedOrgSendersTotal.Increment();
			}
			if (result.Error is NonUniqueAddressError)
			{
				string text = senderAddress.ToString();
				ExTraceGlobals.ResolverTracer.TraceDebug<string>(0L, "Ambiguous sender {0}", text);
				Resolver.EventLogger.LogEvent(TransportEventLogConstants.Tuple_AmbiguousSender, text, new object[]
				{
					text
				});
				Resolver.PerfCounters.AmbiguousSendersTotal.Increment();
			}
		}

		private bool CheckRecipientLimit()
		{
			if (!this.Message.RecipientLimitVerified)
			{
				if (!DeliveryRestriction.IsPrivilegedSender(this.sender, this.isAuthenticated, this.configuration.PrivilegedSenders) && !this.sender.EffectiveRecipientLimit.IsUnlimited && this.recipientStack.Count > this.sender.EffectiveRecipientLimit.Value)
				{
					ExTraceGlobals.ResolverTracer.TraceDebug<int>(0L, "Exceeded sender recipient limit {0}", this.sender.EffectiveRecipientLimit.Value);
					this.mailItem.AddDsnParameters("MaxRecipientCount", this.sender.EffectiveRecipientLimit.Value);
					this.mailItem.AddDsnParameters("CurrentRecipientCount", this.recipientStack.Count);
					this.FailAllRecipients(AckReason.RecipientLimitExceeded);
					return false;
				}
				this.Message.RecipientLimitVerified = true;
			}
			return true;
		}

		private void FailAllRecipients(SmtpResponse response)
		{
			foreach (MailRecipient recipient in this.mailItem.Recipients.AllUnprocessed)
			{
				Resolver.FailRecipient(recipient, response);
			}
		}

		private bool CheckSenderSizeRestriction()
		{
			long num;
			long num2;
			RestrictionCheckResult restrictionCheckResult = DeliveryRestriction.CheckSenderSizeRestriction(this.sender, this.message.OriginalMessageSize, this.isAuthenticated, this.mailItem.IsJournalReport(), this.configuration.MaxSendSize, this.configuration.PrivilegedSenders, out num, out num2);
			ExTraceGlobals.ResolverTracer.TraceDebug(0L, "Sender Size Restriction Check returns {0}: sender {1} stream size {2} authenticated {3}", new object[]
			{
				(int)restrictionCheckResult,
				this.sender.PrimarySmtpAddress,
				this.message.OriginalMessageSize,
				this.isAuthenticated
			});
			if (ADRecipientRestriction.Failed(restrictionCheckResult))
			{
				if (restrictionCheckResult == (RestrictionCheckResult)2147483650U || restrictionCheckResult == (RestrictionCheckResult)2147483651U)
				{
					this.mailItem.AddDsnParameters("MaxMessageSizeInKB", num);
					this.mailItem.AddDsnParameters("CurrentMessageSizeInKB", num2);
				}
				this.FailAllRecipients(DeliveryRestriction.GetResponseForResult(restrictionCheckResult));
				return false;
			}
			return true;
		}

		private ResolverLogLevel GetEffectiveLogLevel()
		{
			ResolverLogLevel val = ResolverLogLevel.Disabled;
			int num;
			if (this.mailItem.ExtendedProperties.TryGetValue<int>("Microsoft.Exchange.Transport.ResolverLogLevel", out num))
			{
				val = (ResolverLogLevel)num;
			}
			return (ResolverLogLevel)Math.Max((int)ResolverConfiguration.ResolverLogLevel, (int)val);
		}

		private void CommitPendingChips()
		{
			this.backupMailItem.CommitPendingChips();
		}

		private void ClearADRecipientCache()
		{
			this.backupMailItem.ClearADRecipientCache();
			if (this.mailItem.ADRecipientCache != null)
			{
				this.mailItem.ADRecipientCache.Clear();
			}
		}

		private void CheckDLLimitsAndAckRecipientIfPending()
		{
			if (this.CheckDLLimitsRestrictions())
			{
				if (this.ackPending)
				{
					this.currentTopLevelRecipientInProcess.Recipient.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.Processed", true);
					ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress, AckStatus, SmtpResponse>((long)this.GetHashCode(), "Acking top level recipient {0}. AckStatus {1}, AckResponse {2}", this.currentTopLevelRecipientInProcess.Recipient.Email, this.pendingAckStatusAndResponse.AckStatus, this.pendingAckStatusAndResponse.SmtpResponse);
					this.currentTopLevelRecipientInProcess.Recipient.Ack(this.pendingAckStatusAndResponse.AckStatus, this.pendingAckStatusAndResponse.SmtpResponse);
				}
				this.resolverCache.MergeResultsFromCurrentExpansion();
			}
		}

		private void ResetStateForNextTopLevelRecipient(RecipientItem recipientItem)
		{
			this.currentTopLevelRecipientInProcess = recipientItem;
			this.numRecipientsAddedForCurrentExpansion = 0;
			this.topLevelGroupItem = null;
			this.ackPending = false;
			this.pendingAckStatusAndResponse = Resolver.NoopAckStatusAndResponse;
			this.originalMailItemIndex = this.mailItem.Recipients.Count;
		}

		private bool CheckDLLimitsRestrictions()
		{
			if (!ResolverConfiguration.LargeDGLimitEnforcementEnabled)
			{
				return true;
			}
			if (this.currentTopLevelRecipientInProcess == null)
			{
				return true;
			}
			if (ResolverConfiguration.LargeDGGroupCount > 0 && this.message.OriginalMessageSize > (long)ResolverConfiguration.LargeDGMaxMessageSize.ToBytes() && this.numRecipientsAddedForCurrentExpansion > ResolverConfiguration.LargeDGGroupCount)
			{
				ExTraceGlobals.ResolverTracer.TraceError((long)this.GetHashCode(), "Message size ({0}) exceeds the allowed size ({1})for large DG. Num recipients expanded {2}, Configured LargeDGGroupCount {3}. Failing DL expansion for {4}", new object[]
				{
					this.message.OriginalMessageSize,
					ResolverConfiguration.LargeDGMaxMessageSize,
					this.numRecipientsAddedForCurrentExpansion,
					ResolverConfiguration.LargeDGGroupCount,
					this.currentTopLevelRecipientInProcess.Recipient.Email
				});
				this.currentTopLevelRecipientInProcess.Recipient.AddDsnParameters("MaxRecipMessageSizeInKB", (long)ResolverConfiguration.LargeDGMaxMessageSize.ToKB());
				this.currentTopLevelRecipientInProcess.Recipient.AddDsnParameters("CurrentMessageSizeInKB", this.message.OriginalMessageSize >> 10);
				Resolver.FailRecipient(this.currentTopLevelRecipientInProcess.Recipient, AckReason.MessageTooLargeForDistributionList);
				this.DiscardRecipientsAddedForCurrentExpansion(AckReason.MessageTooLargeForDistributionList);
				return false;
			}
			RestrictedItem restrictedItem = this.currentTopLevelRecipientInProcess as RestrictedItem;
			if (restrictedItem != null && ResolverConfiguration.LargeDGGroupCountForUnRestrictedDG > 0 && this.numRecipientsAddedForCurrentExpansion > ResolverConfiguration.LargeDGGroupCountForUnRestrictedDG && !restrictedItem.IsDeliveryToGroupRestricted())
			{
				ExTraceGlobals.ResolverTracer.TraceError<RoutingAddress, int, int>((long)this.GetHashCode(), "Total number of recipients expanded for the distribution list {0} is ({1}) and that exceeds the configured limits for the recipient count for an UnRestrictedDL ({2}). Failing DL expansion.", this.currentTopLevelRecipientInProcess.Recipient.Email, this.numRecipientsAddedForCurrentExpansion, ResolverConfiguration.LargeDGGroupCountForUnRestrictedDG);
				Resolver.FailRecipient(this.currentTopLevelRecipientInProcess.Recipient, AckReason.DLExpansionBlockedNeedsSenderRestrictions);
				Resolver.EventLogger.LogEvent(this.mailItem.OrganizationId, TransportEventLogConstants.Tuple_NDRForUnrestrictedLargeDL, this.currentTopLevelRecipientInProcess.Recipient.ToString(), this.currentTopLevelRecipientInProcess.Recipient.ToString());
				string notificationReason = string.Format("Messages to the Distribution group {0} could not be delivered because of security policies.", this.currentTopLevelRecipientInProcess.Recipient.ToString());
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "NDRForUnrestrictedLargeDL", null, notificationReason, ResultSeverityLevel.Warning, false);
				this.DiscardRecipientsAddedForCurrentExpansion(AckReason.DLExpansionBlockedNeedsSenderRestrictions);
				return false;
			}
			return true;
		}

		private void DiscardRecipientsAddedForCurrentExpansion(SmtpResponse ackReason)
		{
			this.resolverCache.DiscardResultsFromCurrentExpansion();
			ExTraceGlobals.ResolverTracer.TraceDebug<int>((long)this.GetHashCode(), "Discarding recipients from the original mailItem. BeginIdx {0}", this.originalMailItemIndex);
			List<MailRecipient> list = new List<MailRecipient>();
			for (int i = this.originalMailItemIndex; i < this.mailItem.Recipients.Count; i++)
			{
				MailRecipient mailRecipient = this.mailItem.Recipients[i];
				mailRecipient.Ack(AckStatus.SuccessNoDsn, AckReason.RecipientDiscarded);
				list.Add(mailRecipient);
			}
			this.backupMailItem.DiscardChipsForCurrentExpansion(list);
			LatencyFormatter latencyFormatter = new LatencyFormatter(this.mailItem, Components.Configuration.LocalServer.TransportServer.Fqdn, true);
			MessageTrackingLog.TrackFailedRecipients(MessageTrackingSource.ROUTING, "resolver DL limit restrictions", this.mailItem, (string)this.currentTopLevelRecipientInProcess.Recipient.Email, list, ackReason, latencyFormatter);
		}

		public const string Resolved = "Microsoft.Exchange.Transport.Resolved";

		public const string Processed = "Microsoft.Exchange.Transport.Processed";

		public const string ResolverLogLevelOverride = "Microsoft.Exchange.Transport.ResolverLogLevel";

		private const string PerfInstanceName = "_total";

		public static IComparer<MailRecipient> RecipientHomeMDBComparer = new RecipientHomeMDBComparer();

		private static readonly AckStatusAndResponse NoopAckStatusAndResponse = new AckStatusAndResponse(AckStatus.Pending, SmtpResponse.Empty);

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ResolverTracer.Category, TransportEventLog.GetEventSource());

		private static ResolverPerfCountersInstance perfCounters;

		private ResolverConfiguration configuration;

		private TransportMailItem mailItem;

		private TaskContext taskContext;

		private ResolverMessage message;

		private Stack<RecipientItem> recipientStack;

		private Sender sender;

		private bool isAuthenticated;

		private bool encounteredMemoryPressure;

		private LimitedMailItem backupMailItem;

		private ResolverCache resolverCache;

		private RecipientItem currentTopLevelRecipientInProcess;

		private GroupItem topLevelGroupItem;

		private int originalMailItemIndex;

		private bool ackPending;

		private AckStatusAndResponse pendingAckStatusAndResponse = Resolver.NoopAckStatusAndResponse;

		private int numRecipientsAddedForCurrentExpansion;

		private Resolver.DeferBifurcatedDelegate deferHandler;

		public delegate void DeferBifurcatedDelegate(TransportMailItem mailItem, TaskContext taskContext, DeferReason reason, TimeSpan deferTime);
	}
}
