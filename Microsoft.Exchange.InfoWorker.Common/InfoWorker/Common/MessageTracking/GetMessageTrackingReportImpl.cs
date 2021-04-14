using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Tracking;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.Transport.Logging.Search;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class GetMessageTrackingReportImpl
	{
		public TrackingErrorCollection Errors
		{
			get
			{
				return this.directoryContext.Errors;
			}
		}

		internal GetMessageTrackingReportImpl(DirectoryContext directoryContext, SearchScope scope, MessageTrackingReportId messageTrackingReportId, LogCache logCache, ReportConstraints constraints)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug<MessageTrackingReportId>(this.GetHashCode(), "Getting report for: {0}", messageTrackingReportId);
			this.directoryContext = directoryContext;
			this.defaultDomain = ServerCache.Instance.GetDefaultDomain(this.directoryContext.OrganizationId);
			string fqdn = ServerCache.Instance.GetLocalServer().Fqdn;
			this.trackingDiscovery = new TrackingDiscovery(directoryContext);
			this.scope = scope;
			this.messageTrackingReportId = messageTrackingReportId;
			this.userCulture = Thread.CurrentThread.CurrentCulture;
			this.trackingContext = new TrackingContext(logCache ?? new LogCache(DateTime.MinValue, DateTime.MaxValue, this.directoryContext.TrackingBudget), this.directoryContext, this.messageTrackingReportId);
			this.trackingContext.ReportTemplate = constraints.ReportTemplate;
			if (constraints.ReportTemplate == ReportTemplate.RecipientPath)
			{
				this.trackingContext.SelectedRecipient = constraints.RecipientPathFilter[0].ToString();
			}
			this.findAdditionalRecords = new WSAdditionalRecords<FindParameters, GetMessageTrackingReportImpl.FindCachedItem>(new QueryMethod<FindParameters, GetMessageTrackingReportImpl.FindCachedItem>(this.FindMessageReceiveBasic));
			this.getAdditionalRecords = new WSAdditionalRecords<WSGetParameters, WSGetResult>(new QueryMethod<WSGetParameters, WSGetResult>(this.WSGetTrackingReportBasic));
			this.constraints = constraints;
		}

		internal MessageTrackingReport Execute()
		{
			InfoWorkerMessageTrackingPerformanceCounters.GetMessageTrackingReportExecuted.Increment();
			uint budgetUsed = this.directoryContext.TrackingBudget.BudgetUsed;
			RecipientEventData recipientEventData = null;
			TimeSpan elapsed = this.directoryContext.TrackingBudget.Elapsed;
			try
			{
				TrackingAuthority trackingAuthority = null;
				RecipientEventData eventsForSenderDomain = this.GetEventsForSenderDomain(out trackingAuthority);
				if (eventsForSenderDomain == null)
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "No events returned for sender domain", new object[0]);
					return null;
				}
				if (this.constraints.ReportTemplate == ReportTemplate.Summary)
				{
					ReferralEvaluator referralEvaluator = new ReferralEvaluator(this.directoryContext, new ReferralEvaluator.TryProcessReferralMethod(this.TryProcessReferral), new ReferralEvaluator.GetAuthorityAndRemapReferralMethod(this.GetAuthorityAndRemapReferral), string.Empty, this.scope);
					IEnumerable<List<RecipientTrackingEvent>> initialEvents = GetMessageTrackingReportImpl.ConvertSummaryEventsToPaths(eventsForSenderDomain.Events);
					referralEvaluator.Evaluate(initialEvents, trackingAuthority.TrackingAuthorityKind);
					List<RecipientTrackingEvent> leaves = referralEvaluator.GetLeaves();
					if (leaves != null && leaves.Count != 0)
					{
						recipientEventData = new RecipientEventData(leaves);
					}
				}
				else if (this.constraints.ReportTemplate == ReportTemplate.RecipientPath)
				{
					string text = this.constraints.RecipientPathFilter[0].ToString();
					ReferralEvaluator referralEvaluator2 = new ReferralEvaluator(this.directoryContext, new ReferralEvaluator.TryProcessReferralMethod(this.TryProcessReferral), new ReferralEvaluator.GetAuthorityAndRemapReferralMethod(this.GetAuthorityAndRemapReferral), text, this.scope);
					IEnumerable<List<RecipientTrackingEvent>> initialEvents2 = GetMessageTrackingReportImpl.ConvertRecipientPathModeEventsToPaths(eventsForSenderDomain);
					referralEvaluator2.Evaluate(initialEvents2, trackingAuthority.TrackingAuthorityKind);
					recipientEventData = referralEvaluator2.GetEventDataForRecipient(text);
				}
				if (recipientEventData == null)
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "No events after following referrals", new object[0]);
					return null;
				}
			}
			finally
			{
				long incrementValue = (long)(this.directoryContext.TrackingBudget.Elapsed - elapsed).TotalMilliseconds;
				InfoWorkerMessageTrackingPerformanceCounters.GetMessageTrackingReportProcessingTime.IncrementBy(incrementValue);
				this.directoryContext.TrackingBudget.RestoreBudgetTo(budgetUsed);
			}
			return new MessageTrackingReport(this.messageTrackingReportId, this.submittedDateTime, this.subject, this.fromAddress, this.fromDisplayName, this.submissionRecipientAddresses, this.submissionRecipientDisplayNames, recipientEventData);
		}

		private RecipientEventData GetEventsForSenderDomain(out TrackingAuthority authority)
		{
			bool flag = false;
			authority = null;
			try
			{
				authority = this.trackingDiscovery.FindLocationByDomainAndServer(this.messageTrackingReportId.Domain, this.messageTrackingReportId.Server, SmtpAddress.Empty, false, out flag);
			}
			catch (TrackingTransientException)
			{
			}
			catch (TrackingFatalException)
			{
			}
			if (authority == null)
			{
				return null;
			}
			if (!authority.IsAllowedScope(this.scope))
			{
				TraceWrapper.SearchLibraryTracer.TraceError<string, string>(this.GetHashCode(), "ReportID's domain and server authority {0} is out of the current scope {1}", Names<TrackingAuthorityKind>.Map[(int)authority.TrackingAuthorityKind], Names<SearchScope>.Map[(int)this.scope]);
				return null;
			}
			if (authority.TrackingAuthorityKind == TrackingAuthorityKind.CurrentSite)
			{
				return this.RpcGetTrackingReport(this.messageTrackingReportId);
			}
			if (authority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteSiteInCurrentOrg || authority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteTrustedOrg || authority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteForest)
			{
				return this.GetEventsForRemoteSender(authority);
			}
			return null;
		}

		private RecipientEventData GetEventsForRemoteSender(TrackingAuthority authority)
		{
			MessageTrackingReportType messageTrackingReportType = null;
			RecipientEventData recipientEventData = this.WSGetTrackingReport(this.messageTrackingReportId, (WebServiceTrackingAuthority)authority, out messageTrackingReportType);
			if (recipientEventData == null)
			{
				return null;
			}
			this.subject = messageTrackingReportType.Subject;
			EmailAddressType sender = messageTrackingReportType.Sender;
			if (sender != null)
			{
				this.fromAddress = new SmtpAddress?(SmtpAddress.Parse(messageTrackingReportType.Sender.EmailAddress));
				this.fromDisplayName = messageTrackingReportType.Sender.Name;
			}
			this.submittedDateTime = messageTrackingReportType.SubmitTime;
			EmailAddressType[] originalRecipients = messageTrackingReportType.OriginalRecipients;
			if (originalRecipients != null)
			{
				this.submissionRecipientAddresses = new SmtpAddress[originalRecipients.Length];
				this.submissionRecipientDisplayNames = new string[originalRecipients.Length];
				for (int i = 0; i < originalRecipients.Length; i++)
				{
					this.submissionRecipientAddresses[i] = SmtpAddress.Parse(originalRecipients[i].EmailAddress);
					this.submissionRecipientDisplayNames[i] = originalRecipients[i].Name;
				}
			}
			return recipientEventData;
		}

		private void GetRecipients(MessageTrackingLogEntry rootEvent)
		{
			if (this.constraints.ReportTemplate == ReportTemplate.RecipientPath)
			{
				return;
			}
			if (rootEvent.EventId == MessageTrackingEvent.SUBMIT && rootEvent.Source == MessageTrackingSource.STOREDRIVER)
			{
				if (rootEvent.RecipientAddresses == null || rootEvent.RecipientAddresses.Length == 0)
				{
					TraceWrapper.SearchLibraryTracer.TraceError<int, string, string>(this.GetHashCode(), "SUBMIT had no recipients, eventId={0}, sender={1}, server={2}", (int)rootEvent.EventId, rootEvent.SenderAddress, rootEvent.Server);
					TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "No recipients for STOREDRIVER SUBMIT event from server {0}", new object[]
					{
						rootEvent.Server
					});
				}
				this.submissionRecipientAddresses = new SmtpAddress[rootEvent.RecipientAddresses.Length];
				this.submissionRecipientDisplayNames = rootEvent.RecipientAddresses;
				for (int i = 0; i < rootEvent.RecipientAddresses.Length; i++)
				{
					this.submissionRecipientAddresses[i] = SmtpAddress.Parse(rootEvent.RecipientAddresses[i]);
				}
				return;
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug<string, string>(this.GetHashCode(), "The root event was not STOREDRIVER SUBMIT, it was: {0} {1}", Names<MessageTrackingSource>.Map[(int)rootEvent.Source], Names<MessageTrackingEvent>.Map[(int)rootEvent.EventId]);
		}

		private List<RecipientTrackingEvent> ConvertToRecipientTrackingEvent(List<MessageTrackingLogEntry> logEntries)
		{
			List<RecipientTrackingEvent> list = new List<RecipientTrackingEvent>(logEntries.Count);
			foreach (MessageTrackingLogEntry messageTrackingLogEntry in logEntries)
			{
				if (!messageTrackingLogEntry.IsEntryCompatible)
				{
					string exception = string.Format("Log entry on {0} for message-id {1} is not compatible with this server, skipping the rest of the entries", messageTrackingLogEntry.Server, messageTrackingLogEntry.MessageId);
					this.Errors.Add(ErrorCode.LogVersionIncompatible, messageTrackingLogEntry.Server, string.Empty, exception);
					break;
				}
				RecipientTrackingEvent recipientTrackingEvent = this.CreateRecipientTrackingEvent(messageTrackingLogEntry);
				if (recipientTrackingEvent != null)
				{
					list.Add(recipientTrackingEvent);
				}
			}
			return list;
		}

		private RecipientTrackingEvent CreateRecipientTrackingEvent(MessageTrackingLogEntry logEntry)
		{
			GetMessageTrackingReportImpl.GetEventInfoMethod getEventInfoMethod = null;
			if (!this.directoryContext.IsTenantInScope(logEntry.TenantId))
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string, OrganizationId, string>(this.GetHashCode(), "row skipped for msg-id: {0}.  Getting recipient event for tenant {1}, but row is for {2}", logEntry.MessageId, this.directoryContext.OrganizationId, logEntry.TenantId);
				return null;
			}
			GetMessageTrackingReportImpl.EventInfo eventInfo;
			if (GetMessageTrackingReportImpl.rawEventToEventInfoGetter.TryGetValue(logEntry.EventId, out getEventInfoMethod))
			{
				eventInfo = getEventInfoMethod(logEntry, this);
			}
			else
			{
				GetMessageTrackingReportImpl.rawEventToEventInfo.TryGetValue(logEntry.EventId, out eventInfo);
			}
			if (eventInfo == null)
			{
				if (this.constraints.ReportTemplate != ReportTemplate.Summary)
				{
					return null;
				}
				eventInfo = GetMessageTrackingReportImpl.generalPendingEventInfo;
			}
			EventType eventType = eventInfo.EventType;
			DeliveryStatus deliveryStatus = eventInfo.DeliveryStatus;
			EventDescription eventDescription = eventInfo.EventDescription;
			if (logEntry.RecipientAddresses == null || logEntry.RecipientAddresses.Length == 0)
			{
				return null;
			}
			SmtpAddress recipientAddress = SmtpAddress.Parse(logEntry.RecipientAddress);
			string[] eventData = null;
			GetMessageTrackingReportImpl.GetEventDataMethod getEventDataMethod = null;
			if (GetMessageTrackingReportImpl.eventDataGetters.TryGetValue(eventDescription, out getEventDataMethod))
			{
				eventData = getEventDataMethod(logEntry, this);
			}
			string text = string.Empty;
			if (eventDescription == EventDescription.Submitted)
			{
				text = ((!string.IsNullOrEmpty(logEntry.ClientHostName)) ? logEntry.ClientHostName : logEntry.ClientIP);
			}
			return new RecipientTrackingEvent(this.defaultDomain, recipientAddress, logEntry.RecipientAddress, deliveryStatus, eventType, eventDescription, eventData, (!string.IsNullOrEmpty(text)) ? text : logEntry.Server, logEntry.Time, logEntry.InternalMessageId, null, logEntry.HiddenRecipient, logEntry.BccRecipient, logEntry.RootAddress, (string)logEntry.ArbitrationMailboxAddress, logEntry.InitMessageId);
		}

		private void AppendRichTrackingDataIfNeeded(RecipientEventData recipientEventData, List<MessageTrackingLogEntry> rawEvents)
		{
			if (recipientEventData == null)
			{
				return;
			}
			List<RecipientTrackingEvent> events = recipientEventData.Events;
			if (events != null)
			{
				MessageTrackingLogEntry messageTrackingLogEntry = rawEvents[rawEvents.Count - 1];
				RecipientTrackingEvent recipEvent = events[events.Count - 1];
				if (messageTrackingLogEntry.EventId == MessageTrackingEvent.HAREDIRECT)
				{
					if (events.Count <= 1)
					{
						return;
					}
					messageTrackingLogEntry = rawEvents[rawEvents.Count - 2];
					recipEvent = events[events.Count - 2];
				}
				TrackedUser trackedUser = TrackedUser.Create(messageTrackingLogEntry.RecipientAddress, this.directoryContext.TenantGalSession);
				if (trackedUser == null)
				{
					TraceWrapper.SearchLibraryTracer.TraceError<string>(this.GetHashCode(), "Recipient: {0} lookup error, queue/inbox/read-status data may be incorrect (we will do our best)", messageTrackingLogEntry.RecipientAddress);
					trackedUser = TrackedUser.CreateUnresolved(messageTrackingLogEntry.RecipientAddress);
				}
				this.AppendQueueViewerDiagnosticIfNeeded(trackedUser, messageTrackingLogEntry, events);
				this.AppendInboxRuleEventIfNeeded(trackedUser, messageTrackingLogEntry, events);
				this.AppendInboxRuleForwardToDelegateEventIfNeeded(trackedUser, messageTrackingLogEntry, events);
				this.AppendRecipientReadStatusIfNeeded(trackedUser, recipEvent, events);
			}
		}

		private void AppendQueueViewerDiagnosticIfNeeded(TrackedUser recipient, MessageTrackingLogEntry lastRawEvent, List<RecipientTrackingEvent> events)
		{
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.MessageTracking.QueueViewerDiagnostics.Enabled)
			{
				return;
			}
			if (!this.constraints.ReturnQueueEvents || this.constraints.ReportTemplate != ReportTemplate.RecipientPath)
			{
				return;
			}
			if (lastRawEvent.EventId == MessageTrackingEvent.DELIVER || lastRawEvent.EventId == MessageTrackingEvent.POISONMESSAGE || lastRawEvent.EventId == MessageTrackingEvent.FAIL || lastRawEvent.EventId == MessageTrackingEvent.SEND || lastRawEvent.EventId == MessageTrackingEvent.INITMESSAGECREATED)
			{
				return;
			}
			if (lastRawEvent.InternalMessageId == 0L)
			{
				return;
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug<string, int>(this.GetHashCode(), "Getting queue status for {0}. Last event was: {1}", lastRawEvent.RecipientAddress, (int)lastRawEvent.EventId);
			string text;
			DateTime? dateTime;
			string text2;
			bool flag;
			using (QueueClient queueClient = new QueueClient(lastRawEvent.Server, this.directoryContext))
			{
				RecipientStatus? recipientStatus;
				DateTime? dateTime2;
				if (!queueClient.GetRecipientStatus(lastRawEvent.InternalMessageId, lastRawEvent.MessageId, lastRawEvent.RecipientAddress, out recipientStatus, out text, out dateTime, out dateTime2, out text2, out flag))
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "No status available", new object[0]);
					return;
				}
			}
			EventDescription eventDescription;
			string[] eventData;
			if (flag)
			{
				if (text == null)
				{
					TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "No last error for unreachable queue", new object[0]);
					return;
				}
				eventDescription = EventDescription.QueueRetryNoRetryTime;
				eventData = new string[]
				{
					text2,
					text
				};
			}
			else
			{
				if (text == null || dateTime == null)
				{
					TraceWrapper.SearchLibraryTracer.TraceError<string>(this.GetHashCode(), "Missing last-error, last-retry or next-retry for queue: {0}", text2);
					return;
				}
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Got queue diagnostic: {0}", text);
				eventData = new string[]
				{
					lastRawEvent.Server,
					GetMessageTrackingReportImpl.FormatDateTime(lastRawEvent.Time.ToLocalTime(), this.userCulture),
					GetMessageTrackingReportImpl.FormatDateTime(dateTime.Value.ToLocalTime(), this.userCulture),
					ExTimeZone.CurrentTimeZone.LocalizableDisplayName.ToString(this.userCulture),
					text
				};
				eventDescription = EventDescription.QueueRetry;
			}
			RecipientTrackingEvent item = new RecipientTrackingEvent(this.defaultDomain, recipient.SmtpAddress, recipient.DisplayName, DeliveryStatus.Pending, EventType.Pending, eventDescription, eventData, lastRawEvent.Server, lastRawEvent.Time, lastRawEvent.InternalMessageId, null, lastRawEvent.HiddenRecipient, lastRawEvent.BccRecipient, lastRawEvent.RootAddress, null, null);
			events.Add(item);
		}

		private void AppendInboxRuleEventIfNeeded(TrackedUser recipient, MessageTrackingLogEntry lastEntry, List<RecipientTrackingEvent> events)
		{
			if (this.constraints.ReportTemplate != ReportTemplate.RecipientPath || lastEntry.EventId != MessageTrackingEvent.DELIVER || string.IsNullOrEmpty(lastEntry.Folder))
			{
				return;
			}
			RecipientTrackingEvent item = new RecipientTrackingEvent(this.defaultDomain, recipient.SmtpAddress, recipient.DisplayName, DeliveryStatus.Delivered, EventType.Deliver, EventDescription.MovedToFolderByInboxRule, new string[]
			{
				lastEntry.Folder
			}, lastEntry.Server, lastEntry.Time, lastEntry.InternalMessageId, null, lastEntry.HiddenRecipient, lastEntry.BccRecipient, lastEntry.RootAddress, null, null);
			events.Add(item);
		}

		private void AppendInboxRuleForwardToDelegateEventIfNeeded(TrackedUser recipient, MessageTrackingLogEntry lastEntry, List<RecipientTrackingEvent> events)
		{
			if (this.constraints.ReportTemplate != ReportTemplate.RecipientPath || lastEntry.EventId != MessageTrackingEvent.PROCESS || !string.Equals("Mailbox Rules Agent.DelegateAccess", lastEntry.RecipientStatus, StringComparison.Ordinal))
			{
				return;
			}
			RecipientTrackingEvent item = new RecipientTrackingEvent(this.defaultDomain, recipient.SmtpAddress, recipient.DisplayName, DeliveryStatus.Delivered, EventType.Deliver, EventDescription.ForwardedToDelegateAndDeleted, null, lastEntry.Server, lastEntry.Time, lastEntry.InternalMessageId, null, lastEntry.HiddenRecipient, lastEntry.BccRecipient, lastEntry.RootAddress, null, null);
			events.Add(item);
		}

		private void AppendRecipientReadStatusIfNeeded(TrackedUser trackedUser, RecipientTrackingEvent recipEvent, List<RecipientTrackingEvent> events)
		{
			if (this.constraints.ReportTemplate != ReportTemplate.RecipientPath || recipEvent.EventDescription != EventDescription.Delivered)
			{
				return;
			}
			if (!ServerCache.Instance.ReadStatusReportingEnabled(this.directoryContext) || !trackedUser.ReadStatusTrackingEnabled || trackedUser.ADUser == null || !trackedUser.IsMailbox)
			{
				return;
			}
			ExchangePrincipal mailboxOwner = null;
			try
			{
				mailboxOwner = ExchangePrincipal.FromADUser(this.directoryContext.TenantConfigSession.SessionSettings, trackedUser.ADUser, RemotingOptions.AllowCrossSite);
			}
			catch (ObjectNotFoundException arg)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress, ObjectNotFoundException>(this.GetHashCode(), "Cannot get ExchangePrincipal for recipient: {0}, will not get read status. Exception: {1}", recipEvent.RecipientAddress, arg);
				return;
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress, string>(this.GetHashCode(), "Checking READ status for recipient: {0}, Message-Id: {1}", recipEvent.RecipientAddress, this.messageTrackingReportId.MessageId);
			try
			{
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, "Client=ELC;Action=MessageTracking"))
				{
					IStorePropertyBag[] array = AllItemsFolderHelper.FindItemsFromInternetId(mailboxSession, this.messageTrackingReportId.MessageId, GetMessageTrackingReportImpl.readStatusProperties);
					if (array == null || array.Length == 0)
					{
						TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "No matching message found for recipient", new object[0]);
					}
					else
					{
						TraceWrapper.SearchLibraryTracer.TraceDebug<int>(this.GetHashCode(), "Found {0} matching messages", array.Length);
						foreach (IStorePropertyBag storePropertyBag in array)
						{
							if (storePropertyBag.TryGetProperty(MessageItemSchema.TransportMessageHeaders) is PropertyError)
							{
								TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Skipping a sent item copy.", new object[0]);
							}
							else
							{
								object obj = storePropertyBag.TryGetProperty(MessageItemSchema.Flags);
								int num = (int)obj;
								TraceWrapper.SearchLibraryTracer.TraceDebug<int>(this.GetHashCode(), "PR_MESSAGE_FLAGS = {0:x}", num);
								if ((num & 1024) == 1024)
								{
									RecipientTrackingEvent item = new RecipientTrackingEvent(recipEvent.Domain, recipEvent.RecipientAddress, recipEvent.RecipientDisplayName, recipEvent.Status, recipEvent.EventType, EventDescription.Read, null, recipEvent.Server, DateTime.MinValue, 0L, null, recipEvent.HiddenRecipient, new bool?(recipEvent.BccRecipient), recipEvent.RootAddress, null, null);
									events.Add(item);
									return;
								}
							}
						}
						RecipientTrackingEvent item2 = new RecipientTrackingEvent(recipEvent.Domain, recipEvent.RecipientAddress, recipEvent.RecipientDisplayName, recipEvent.Status, recipEvent.EventType, EventDescription.NotRead, null, recipEvent.Server, DateTime.MinValue, 0L, null, recipEvent.HiddenRecipient, new bool?(recipEvent.BccRecipient), recipEvent.RootAddress, null, null);
						events.Add(item2);
					}
				}
			}
			catch (StoragePermanentException ex)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<StoragePermanentException>(this.GetHashCode(), "Store permanent exception: {0}", ex);
				this.Errors.Add(ErrorCode.ReadStatusError, string.Empty, string.Empty, ex.ToString());
			}
			catch (StorageTransientException ex2)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<StorageTransientException>(this.GetHashCode(), "Store transient exception: {0}", ex2);
				this.Errors.Add(ErrorCode.ReadStatusError, string.Empty, string.Empty, ex2.ToString());
			}
		}

		private bool TryProcessReferral(RecipientTrackingEvent referralEvent, TrackingAuthority authority, out IEnumerable<List<RecipientTrackingEvent>> paths)
		{
			EventDescription eventDescription = referralEvent.EventDescription;
			paths = null;
			TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress, string, string>(this.GetHashCode(), "Processing referral for recipient: {0}, Event: {1}, authority kind {2}", referralEvent.RecipientAddress, Names<EventDescription>.Map[(int)eventDescription], Names<TrackingAuthorityKind>.Map[(int)authority.TrackingAuthorityKind]);
			MessageTrackingReportId receiveTrackingId = this.FindMessageReceive(referralEvent, authority);
			if (receiveTrackingId == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Did not find message entry point in remote tracking authority", new object[0]);
				return false;
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug<MessageTrackingReportId>(this.GetHashCode(), "Found entrypoint in remote-tracking authority, ID: {0}", receiveTrackingId);
			RecipientEventData recipientEventData;
			if (receiveTrackingId == MessageTrackingReportId.LegacyExchange)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress>(this.GetHashCode(), "Converting Pending event into TransferredToLegacyExchangeServer event for recipient: {0}.", referralEvent.RecipientAddress);
				referralEvent.ConvertRecipientTrackingEvent(DeliveryStatus.Transferred, EventType.Transferred, EventDescription.TransferredToLegacyExchangeServer);
				recipientEventData = new RecipientEventData(new List<RecipientTrackingEvent>(0));
			}
			else
			{
				if (authority.TrackingAuthorityKind == TrackingAuthorityKind.Undefined)
				{
					bool serverNotFound = false;
					TrackingAuthority newAuthority = null;
					TrackingBaseException ex = this.TryExecuteTask(delegate
					{
						newAuthority = this.trackingDiscovery.FindLocationByDomainAndServer(receiveTrackingId.Domain, receiveTrackingId.Server, SmtpAddress.Empty, false, out serverNotFound);
					});
					if (ex != null)
					{
						TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Error in FindLocationByDomainAndServer during referral, aborting", new object[0]);
						return false;
					}
					if (serverNotFound)
					{
						string formatString = string.Format("Next hop server not found: {0}\\{1}", receiveTrackingId.Domain, receiveTrackingId.Server);
						TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), formatString, new object[0]);
						return false;
					}
					authority = newAuthority;
					TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Tracking authority modified: Undefined -> {0}", Names<TrackingAuthorityKind>.Map[(int)authority.TrackingAuthorityKind]);
				}
				recipientEventData = this.GetTrackingReport(receiveTrackingId, authority);
				if (recipientEventData == null)
				{
					TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Could not generate a tracking-report in remote-authority", new object[0]);
					return false;
				}
				TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Got tracking report", new object[0]);
			}
			if (this.constraints.ReportTemplate == ReportTemplate.Summary)
			{
				paths = GetMessageTrackingReportImpl.ConvertSummaryEventsToPaths(recipientEventData.Events);
			}
			else
			{
				paths = GetMessageTrackingReportImpl.ConvertRecipientPathModeEventsToPaths(recipientEventData);
			}
			return true;
		}

		private TrackingAuthority GetAuthorityAndRemapReferral(ref RecipientTrackingEvent referralEvent)
		{
			EventDescription eventDescription = referralEvent.EventDescription;
			if ((eventDescription == EventDescription.SmtpSendCrossSite && referralEvent.EventData.Length != 2) || (eventDescription == EventDescription.SmtpSendCrossForest && referralEvent.EventData.Length != 3) || (eventDescription == EventDescription.TransferredToForeignOrg && referralEvent.EventData.Length != 1) || (eventDescription == EventDescription.SubmittedCrossSite && referralEvent.EventData.Length != 2))
			{
				TraceWrapper.SearchLibraryTracer.TraceError<string, int, string>(this.GetHashCode(), "Incorrect event-data count for {0}, there were {1} elements. Data is from server: {2}", Names<EventDescription>.Map[(int)eventDescription], referralEvent.EventData.Length, referralEvent.Server);
				TrackingFatalException.RaiseED(ErrorCode.UnexpectedErrorPermanent, "WS-Response Validation Error: Incorrect EventData count {0} in response for {1}", new object[]
				{
					referralEvent.EventData.Length,
					Names<EventDescription>.Map[(int)eventDescription]
				});
			}
			string text = null;
			string text2 = null;
			bool allowChildDomains = false;
			if (eventDescription == EventDescription.SmtpSendCrossSite)
			{
				if (referralEvent.EventData.Length != 2)
				{
					throw new InvalidOperationException();
				}
				text2 = referralEvent.Domain;
				text = referralEvent.EventData[1];
				TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Following cross-site SMTP send", new object[0]);
			}
			else if (eventDescription == EventDescription.SmtpSendCrossForest)
			{
				if (referralEvent.EventData.Length != 3)
				{
					throw new InvalidOperationException();
				}
				text2 = referralEvent.EventData[2];
				text = referralEvent.EventData[1];
				allowChildDomains = true;
				TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Following cross-forest SMTP send", new object[0]);
			}
			else if (eventDescription == EventDescription.TransferredToForeignOrg)
			{
				if (referralEvent.EventData.Length != 1)
				{
					throw new InvalidOperationException();
				}
				text2 = referralEvent.EventData[0];
				text = null;
				TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Following cross-org send", new object[0]);
			}
			else if (eventDescription == EventDescription.TransferredToPartnerOrg)
			{
				text = null;
				text2 = referralEvent.RecipientAddress.Domain;
			}
			else
			{
				if (eventDescription == EventDescription.SmtpSend)
				{
					return RemoteOrgTrackingAuthority.Instance;
				}
				if (eventDescription == EventDescription.PendingModeration)
				{
					if (string.IsNullOrEmpty(referralEvent.ExtendedProperties.ArbitrationMailboxAddress) || string.IsNullOrEmpty(referralEvent.ExtendedProperties.InitMessageId))
					{
						TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Arbitration address not available for PendingModeration event", new object[0]);
						return null;
					}
					return UndefinedTrackingAuthority.Instance;
				}
				else
				{
					if (eventDescription != EventDescription.SubmittedCrossSite)
					{
						return null;
					}
					text2 = referralEvent.Domain;
					text = referralEvent.EventData[1];
					TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Following cross-site SD SUBMIT", new object[0]);
				}
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug<string, string>(this.GetHashCode(), "NextHopServer={0}, NextHopDomain={1}", text, text2);
			TrackingAuthority trackingAuthority = null;
			bool flag = false;
			TrackingBaseException ex = null;
			try
			{
				trackingAuthority = this.trackingDiscovery.FindLocationByDomainAndServer(text2, text, referralEvent.RecipientAddress, allowChildDomains, out flag);
			}
			catch (TrackingTransientException ex2)
			{
				ex = ex2;
			}
			catch (TrackingFatalException ex3)
			{
				ex = ex3;
			}
			if ((eventDescription == EventDescription.SmtpSendCrossForest || eventDescription == EventDescription.TransferredToPartnerOrg) && ((trackingAuthority == null && flag) || trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.Undefined))
			{
				trackingAuthority = UndefinedTrackingAuthority.Instance;
			}
			else if (ex != null)
			{
				if (!ex.IsAlreadyLogged)
				{
					this.Errors.Errors.Add(ex.TrackingError);
				}
				return null;
			}
			this.RemapEventForAuthority(ref referralEvent, trackingAuthority, text2, text);
			if (trackingAuthority == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<SmtpAddress>(this.GetHashCode(), "Could not get a tracking authority for {0}", referralEvent.RecipientAddress);
				return null;
			}
			if (trackingAuthority.TrackingAuthorityKind != TrackingAuthorityKind.RemoteSiteInCurrentOrg && trackingAuthority.TrackingAuthorityKind != TrackingAuthorityKind.RemoteTrustedOrg && trackingAuthority.TrackingAuthorityKind != TrackingAuthorityKind.RemoteForest && trackingAuthority.TrackingAuthorityKind != TrackingAuthorityKind.CurrentSite && trackingAuthority.TrackingAuthorityKind != TrackingAuthorityKind.Undefined)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress, string>(this.GetHashCode(), "Cannot follow referral for {0}, authority kind is {1}", referralEvent.RecipientAddress, Names<TrackingAuthorityKind>.Map[(int)trackingAuthority.TrackingAuthorityKind]);
				return null;
			}
			return trackingAuthority;
		}

		private void RemapEventForAuthority(ref RecipientTrackingEvent referralEvent, TrackingAuthority trackingAuthority, string nextHopDomain, string nextHopServer)
		{
			if (referralEvent.EventDescription != EventDescription.TransferredToForeignOrg && referralEvent.EventDescription != EventDescription.TransferredToPartnerOrg && referralEvent.EventDescription != EventDescription.SmtpSendCrossForest && referralEvent.EventDescription != EventDescription.SmtpSendCrossSite && referralEvent.EventDescription != EventDescription.SmtpSend)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Referral event {0} does not need to be remapped", Names<EventDescription>.Map[(int)referralEvent.EventDescription]);
				return;
			}
			if (string.IsNullOrEmpty(nextHopServer))
			{
				nextHopServer = nextHopDomain;
			}
			if (trackingAuthority == null)
			{
				referralEvent.ConvertRecipientTrackingEvent(referralEvent.Status, referralEvent.EventType, EventDescription.TransferredToForeignOrg);
				referralEvent.EventData = new string[]
				{
					nextHopDomain
				};
				return;
			}
			if (trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.Undefined)
			{
				referralEvent.ConvertRecipientTrackingEvent(referralEvent.Status, referralEvent.EventType, EventDescription.SmtpSend);
				referralEvent.EventData = new string[]
				{
					referralEvent.Server,
					nextHopServer
				};
				return;
			}
			if (trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteForest)
			{
				referralEvent.ConvertRecipientTrackingEvent(referralEvent.Status, referralEvent.EventType, EventDescription.SmtpSendCrossForest);
				referralEvent.EventData = new string[]
				{
					referralEvent.Server,
					nextHopServer,
					nextHopDomain
				};
				return;
			}
			if (trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteTrustedOrg)
			{
				referralEvent.ConvertRecipientTrackingEvent(referralEvent.Status, referralEvent.EventType, EventDescription.TransferredToPartnerOrg);
				referralEvent.EventData = new string[0];
				return;
			}
			if (trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteSiteInCurrentOrg)
			{
				referralEvent.ConvertRecipientTrackingEvent(referralEvent.Status, referralEvent.EventType, EventDescription.SmtpSendCrossSite);
				referralEvent.EventData = new string[]
				{
					referralEvent.Server,
					nextHopServer
				};
				return;
			}
			if (trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.CurrentSite)
			{
				referralEvent.ConvertRecipientTrackingEvent(referralEvent.Status, referralEvent.EventType, EventDescription.SmtpSend);
				referralEvent.EventData = new string[]
				{
					referralEvent.Server,
					nextHopServer
				};
			}
		}

		private RecipientEventData GetTrackingReport(MessageTrackingReportId reportId, TrackingAuthority authority)
		{
			if (authority.TrackingAuthorityKind == TrackingAuthorityKind.CurrentSite)
			{
				return this.RpcGetTrackingReport(reportId);
			}
			if (authority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteSiteInCurrentOrg || authority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteTrustedOrg || authority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteForest)
			{
				MessageTrackingReportType messageTrackingReportType;
				return this.WSGetTrackingReport(reportId, (WebServiceTrackingAuthority)authority, out messageTrackingReportType);
			}
			return null;
		}

		private RecipientEventData RpcGetTrackingReport(MessageTrackingReportId reportId)
		{
			TrackingContext trackingContext = new TrackingContext(this.trackingContext.Cache, this.directoryContext, reportId);
			trackingContext.ReportTemplate = this.constraints.ReportTemplate;
			if (this.constraints.ReportTemplate == ReportTemplate.RecipientPath)
			{
				trackingContext.SelectedRecipient = this.constraints.RecipientPathFilter[0].ToString();
			}
			LogDataAnalyzer logDataAnalyzer = new LogDataAnalyzer(trackingContext);
			MessageTrackingLogEntry messageTrackingLogEntry;
			List<MessageTrackingLogEntry> list = logDataAnalyzer.AnalyzeLogData(reportId.MessageId, reportId.InternalMessageId, out messageTrackingLogEntry);
			List<RecipientTrackingEvent> list2 = null;
			if (list.Count > 0)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<int>(this.GetHashCode(), "Found {0} entries", list.Count);
				list2 = this.ConvertToRecipientTrackingEvent(list);
			}
			if (this.constraints.ReportTemplate == ReportTemplate.Summary && messageTrackingLogEntry != null)
			{
				this.subject = messageTrackingLogEntry.Subject;
				this.fromAddress = new SmtpAddress?(new SmtpAddress(messageTrackingLogEntry.SenderAddress));
				this.fromDisplayName = messageTrackingLogEntry.SenderAddress;
				this.submittedDateTime = messageTrackingLogEntry.Time;
				this.GetRecipients(messageTrackingLogEntry);
				return new RecipientEventData(list2);
			}
			List<List<RecipientTrackingEvent>> list3 = null;
			if (list2 != null && list2.Count > 0)
			{
				RecipientTrackingEvent recipientTrackingEvent = list2[list2.Count - 1];
				if ((recipientTrackingEvent.EventDescription == EventDescription.TransferredToForeignOrg || recipientTrackingEvent.EventDescription == EventDescription.SmtpSendCrossSite || recipientTrackingEvent.EventDescription == EventDescription.SmtpSendCrossForest || recipientTrackingEvent.EventDescription == EventDescription.TransferredToPartnerOrg) && list[list.Count - 1].EventId != MessageTrackingEvent.HAREDIRECT)
				{
					list3 = new List<List<RecipientTrackingEvent>>(1);
					list3.Add(list2);
					return new RecipientEventData(null, list3);
				}
				RecipientEventData recipientEventData = new RecipientEventData(list2);
				this.AppendRichTrackingDataIfNeeded(recipientEventData, list);
				return recipientEventData;
			}
			else
			{
				List<List<MessageTrackingLogEntry>> handedOffRecipPaths = logDataAnalyzer.GetHandedOffRecipPaths();
				if (handedOffRecipPaths != null)
				{
					list3 = new List<List<RecipientTrackingEvent>>(handedOffRecipPaths.Count);
					foreach (List<MessageTrackingLogEntry> logEntries in handedOffRecipPaths)
					{
						List<RecipientTrackingEvent> item = this.ConvertToRecipientTrackingEvent(logEntries);
						list3.Add(item);
					}
					return new RecipientEventData(null, list3);
				}
				return null;
			}
		}

		private RecipientEventData WSGetTrackingReport(MessageTrackingReportId reportId, WebServiceTrackingAuthority wsAuthority, out MessageTrackingReportType report)
		{
			report = null;
			WSGetParameters key = new WSGetParameters(reportId, wsAuthority);
			WSGetResult wsgetResult = this.getAdditionalRecords.FindAndCache(key, false);
			if (wsgetResult == null)
			{
				return null;
			}
			report = wsgetResult.Report;
			return wsgetResult.RecipientEventData;
		}

		private WSGetResult WSGetTrackingReportBasic(WSGetParameters getParams, WSGetResult currentCache, out KeyValuePair<WSGetParameters, WSGetResult>[] additionalRecords)
		{
			MessageTrackingReportId messageTrackingReportId = getParams.MessageTrackingReportId;
			WebServiceTrackingAuthority wsauthority = getParams.WSAuthority;
			additionalRecords = GetMessageTrackingReportImpl.EmptyGetRecords;
			IWebServiceBinding ewsBinding = wsauthority.GetEwsBinding(this.directoryContext);
			Exception ex = null;
			InternalGetMessageTrackingReportResponse internalGetMessageTrackingReportResponse = null;
			TraceWrapper.SearchLibraryTracer.TraceDebug<MessageTrackingReportId>(this.GetHashCode(), "WSGetTrackingReport: {0}", messageTrackingReportId);
			try
			{
				internalGetMessageTrackingReportResponse = ewsBinding.GetMessageTrackingReport(messageTrackingReportId.ToString(), this.constraints.ReportTemplate, this.constraints.RecipientPathFilter, wsauthority.AssociatedScope, this.constraints.ReturnQueueEvents, this.directoryContext.TrackingBudget);
			}
			catch (TrackingTransientException ex2)
			{
				ex = ex2;
			}
			catch (TrackingFatalException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<Exception>(this.GetHashCode(), "Error getting report-id: Exception: {0}", ex);
				return null;
			}
			if (internalGetMessageTrackingReportResponse == null || internalGetMessageTrackingReportResponse.Response == null || internalGetMessageTrackingReportResponse.Response.MessageTrackingReport == null || internalGetMessageTrackingReportResponse.RecipientTrackingEvents == null || internalGetMessageTrackingReportResponse.RecipientTrackingEvents.Count == 0)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<MessageTrackingReportId>(this.GetHashCode(), "Empty result finding report, report-id: {0}", this.messageTrackingReportId);
				return null;
			}
			MessageTrackingReportType messageTrackingReport = internalGetMessageTrackingReportResponse.Response.MessageTrackingReport;
			Dictionary<string, RecipientEventData> dictionary = RecipientEventData.DeserializeMultiple(internalGetMessageTrackingReportResponse.RecipientTrackingEvents);
			this.TraceRecipientData(dictionary);
			WSGetResult result = null;
			RecipientEventData recipientEventData;
			if (dictionary.TryGetValue(messageTrackingReportId.ToString(), out recipientEventData))
			{
				TraceWrapper.SearchLibraryTracer.TraceError<int, int>(this.GetHashCode(), "SP1 returned mainRecipientEventData, eventCount={0}, handoffPathsCount={1}", (recipientEventData.Events == null) ? 0 : recipientEventData.Events.Count, (recipientEventData.HandoffPaths == null) ? 0 : recipientEventData.HandoffPaths.Count);
				dictionary.Remove(messageTrackingReportId.ToString());
			}
			else if (dictionary.TryGetValue(string.Empty, out recipientEventData))
			{
				TraceWrapper.SearchLibraryTracer.TraceError<int, int>(this.GetHashCode(), "RTM returned mainRecipientEventData, eventCount={0}, handoffPathsCount={1}", (recipientEventData.Events == null) ? 0 : recipientEventData.Events.Count, (recipientEventData.HandoffPaths == null) ? 0 : recipientEventData.HandoffPaths.Count);
				dictionary.Remove(string.Empty);
			}
			else
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "Only additional results were present in return data", new object[0]);
				recipientEventData = null;
			}
			if (recipientEventData != null)
			{
				result = new WSGetResult
				{
					RecipientEventData = recipientEventData,
					Report = messageTrackingReport
				};
			}
			if (dictionary.Count > 0)
			{
				int num = 0;
				additionalRecords = new KeyValuePair<WSGetParameters, WSGetResult>[dictionary.Count];
				foreach (KeyValuePair<string, RecipientEventData> keyValuePair in dictionary)
				{
					MessageTrackingReportId reportId;
					if (!MessageTrackingReportId.TryParse(keyValuePair.Key, out reportId))
					{
						TraceWrapper.SearchLibraryTracer.TraceError<string>(this.GetHashCode(), "Cannot parse report ID from additional records, skipping: {0}", keyValuePair.Key);
					}
					else
					{
						additionalRecords[num++] = new KeyValuePair<WSGetParameters, WSGetResult>(new WSGetParameters(reportId, wsauthority), new WSGetResult
						{
							RecipientEventData = keyValuePair.Value,
							Report = messageTrackingReport
						});
					}
				}
			}
			return result;
		}

		private List<MessageTrackingSearchResult> LocalFindMessageReceiveBasic(FindParameters findParams, out bool wholeForestSearched)
		{
			RecipientTrackingEvent referralEvent = findParams.ReferralEvent;
			TrackingAuthority authority = findParams.Authority;
			bool flag = referralEvent.EventDescription == EventDescription.PendingModeration;
			wholeForestSearched = false;
			if (authority.TrackingAuthorityKind != TrackingAuthorityKind.CurrentSite && authority.TrackingAuthorityKind != TrackingAuthorityKind.Undefined)
			{
				throw new ArgumentException("Authority must be CurrentSite or Undefined, in local organization");
			}
			TrackedUser trackedUser = TrackedUser.Create(referralEvent.RecipientAddress.ToString(), this.directoryContext.TenantGalSession);
			if (trackedUser == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<SmtpAddress>(this.GetHashCode(), "ADUser object corrupted: {0}", referralEvent.RecipientAddress);
				this.Errors.Add(ErrorCode.InvalidADData, string.Empty, string.Format("Recipient mailbox not found for {0}", referralEvent.RecipientAddress), string.Empty);
				return null;
			}
			TrackedUser[] recipients = new TrackedUser[]
			{
				trackedUser
			};
			TrackedUser mailbox = null;
			if (flag)
			{
				mailbox = TrackedUser.Create(referralEvent.ExtendedProperties.ArbitrationMailboxAddress, this.directoryContext.TenantGalSession);
			}
			SearchScope searchScope = (SearchScope)Math.Min((int)authority.AssociatedScope, (int)this.scope);
			SearchMessageTrackingReportImpl searchMessageTrackingReportImpl = new SearchMessageTrackingReportImpl(this.directoryContext, searchScope, mailbox, null, referralEvent.ServerHint, recipients, this.trackingContext.Cache, null, flag ? referralEvent.ExtendedProperties.InitMessageId : this.messageTrackingReportId.MessageId, Unlimited<uint>.UnlimitedValue, false, false, true, flag);
			List<MessageTrackingSearchResult> list = searchMessageTrackingReportImpl.Execute();
			wholeForestSearched = searchMessageTrackingReportImpl.WholeForestSearchExecuted;
			if (list == null)
			{
				return null;
			}
			return list;
		}

		private MessageTrackingReportId FindMessageReceive(RecipientTrackingEvent sendEvent, TrackingAuthority authority)
		{
			bool flag = sendEvent.EventDescription == EventDescription.PendingModeration;
			FindParameters key = new FindParameters(flag ? sendEvent.ExtendedProperties.InitMessageId : this.messageTrackingReportId.MessageId, sendEvent, authority);
			TraceWrapper.SearchLibraryTracer.TraceDebug<TrackingAuthority>(this.GetHashCode(), "Getting results from authority: {0}", authority);
			GetMessageTrackingReportImpl.FindCachedItem findCachedItem = this.findAdditionalRecords.FindAndCache(key, false);
			if (findCachedItem == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<MessageTrackingReportId>(this.GetHashCode(), "Null result finding message, report-id: {0}", this.messageTrackingReportId);
				return null;
			}
			MessageTrackingReportId messageTrackingReportId = this.FindReportIdWithMatchingRecipientInSearchResults(findCachedItem.Results, sendEvent.RecipientAddress);
			if (!flag && messageTrackingReportId == null && !findCachedItem.EntireForestSearched)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress, TrackingAuthority>(this.GetHashCode(), "No cached results for recipient {0}. Going to {1} again and bypass the WS cache to try and get the result.", sendEvent.RecipientAddress, authority);
				GetMessageTrackingReportImpl.FindCachedItem findCachedItem2 = this.findAdditionalRecords.FindAndCache(key, true);
				messageTrackingReportId = this.FindReportIdWithMatchingRecipientInSearchResults(findCachedItem2.Results, sendEvent.RecipientAddress);
			}
			return messageTrackingReportId;
		}

		private MessageTrackingReportId FindReportIdWithMatchingRecipientInSearchResults(IEnumerable<MessageTrackingSearchResult> searchResults, SmtpAddress recipientEmailAddress)
		{
			foreach (MessageTrackingSearchResult messageTrackingSearchResult in searchResults)
			{
				if (messageTrackingSearchResult == null || messageTrackingSearchResult.RecipientAddresses == null)
				{
					TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Skipping result that is null or has no recipients.", new object[0]);
				}
				else
				{
					int num = Array.BinarySearch<SmtpAddress>(messageTrackingSearchResult.RecipientAddresses, recipientEmailAddress);
					if (num >= 0)
					{
						return messageTrackingSearchResult.MessageTrackingReportId;
					}
				}
			}
			return null;
		}

		private GetMessageTrackingReportImpl.FindCachedItem FindMessageReceiveBasic(FindParameters findParams, GetMessageTrackingReportImpl.FindCachedItem currentCachedItem, out KeyValuePair<FindParameters, GetMessageTrackingReportImpl.FindCachedItem>[] additionalRecords)
		{
			additionalRecords = GetMessageTrackingReportImpl.EmptyFindRecords;
			if (currentCachedItem != null && (currentCachedItem.EntireForestSearched || currentCachedItem.AddressesSearched.Contains(findParams.ReferralEvent.RecipientAddress)))
			{
				return currentCachedItem;
			}
			IList<MessageTrackingSearchResult> list = null;
			bool flag = false;
			TrackingAuthorityKind trackingAuthorityKind = findParams.Authority.TrackingAuthorityKind;
			RecipientTrackingEvent referralEvent = findParams.ReferralEvent;
			if (trackingAuthorityKind == TrackingAuthorityKind.CurrentSite || trackingAuthorityKind == TrackingAuthorityKind.Undefined)
			{
				list = this.LocalFindMessageReceiveBasic(findParams, out flag);
			}
			else if (trackingAuthorityKind == TrackingAuthorityKind.RemoteSiteInCurrentOrg || trackingAuthorityKind == TrackingAuthorityKind.RemoteTrustedOrg || trackingAuthorityKind == TrackingAuthorityKind.RemoteForest)
			{
				list = this.WSFindMessageReceiveBasic(findParams, out flag);
			}
			if (list == null || list.Count == 0)
			{
				return currentCachedItem;
			}
			foreach (MessageTrackingSearchResult messageTrackingSearchResult in list)
			{
				Array.Sort<SmtpAddress>(messageTrackingSearchResult.RecipientAddresses);
			}
			if (currentCachedItem == null)
			{
				currentCachedItem = new GetMessageTrackingReportImpl.FindCachedItem();
			}
			if (flag)
			{
				currentCachedItem.EntireForestSearched = true;
				currentCachedItem.Results.Clear();
			}
			currentCachedItem.Results.InsertRange(0, list);
			currentCachedItem.AddressesSearched.Add(referralEvent.RecipientAddress);
			return currentCachedItem;
		}

		private List<MessageTrackingSearchResult> WSFindMessageReceiveBasic(FindParameters findParams, out bool wholeForestSearched)
		{
			RecipientTrackingEvent referralEvent = findParams.ReferralEvent;
			WebServiceTrackingAuthority webServiceTrackingAuthority = (WebServiceTrackingAuthority)findParams.Authority;
			wholeForestSearched = false;
			IWebServiceBinding ewsBinding = webServiceTrackingAuthority.GetEwsBinding(this.directoryContext);
			new FindMessageTrackingReportRequestType();
			SmtpAddress? federatedDeliveryMailbox = null;
			if (referralEvent.EventDescription == EventDescription.TransferredToPartnerOrg && referralEvent.EventData != null && referralEvent.EventData.Length > 0)
			{
				federatedDeliveryMailbox = new SmtpAddress?(new SmtpAddress(referralEvent.EventData[0]));
				if (!federatedDeliveryMailbox.Value.IsValidAddress)
				{
					TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Federated delivery email address invalid", new object[0]);
					TrackingFatalException.AddAndRaiseED(this.Errors, ErrorCode.UnexpectedErrorPermanent, "Referral event had invalid Federated Delivery email {0}", new object[]
					{
						federatedDeliveryMailbox.Value
					});
				}
			}
			Exception ex = null;
			FindMessageTrackingReportResponseMessageType findMessageTrackingReportResponseMessageType = null;
			try
			{
				findMessageTrackingReportResponseMessageType = ewsBinding.FindMessageTrackingReport(webServiceTrackingAuthority.Domain, null, new SmtpAddress?(referralEvent.RecipientAddress), referralEvent.ServerHint, federatedDeliveryMailbox, webServiceTrackingAuthority.AssociatedScope, this.messageTrackingReportId.MessageId, string.Empty, false, false, false, DateTime.MinValue, DateTime.MaxValue, this.directoryContext.TrackingBudget);
			}
			catch (TrackingTransientException ex2)
			{
				ex = ex2;
			}
			catch (TrackingFatalException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				if (GetMessageTrackingReportImpl.IsPageNotFoundAvailabilityException(ex))
				{
					return new List<MessageTrackingSearchResult>(1)
					{
						GetMessageTrackingReportImpl.CreateSearchResultWithLegacyExchangeReportId(referralEvent)
					};
				}
				TraceWrapper.SearchLibraryTracer.TraceError<MessageTrackingReportId, Exception>(this.GetHashCode(), "Error finding message, report-id: {0}, Exception: {1}", this.messageTrackingReportId, ex);
				return new List<MessageTrackingSearchResult>(0);
			}
			else
			{
				if (findMessageTrackingReportResponseMessageType == null)
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug<MessageTrackingReportId>(this.GetHashCode(), "Got null response when getting report-id:{0}", this.messageTrackingReportId);
					return new List<MessageTrackingSearchResult>(0);
				}
				wholeForestSearched = (string.IsNullOrEmpty(findMessageTrackingReportResponseMessageType.ExecutedSearchScope) || Names<SearchScope>.Map[1].Equals(findMessageTrackingReportResponseMessageType.ExecutedSearchScope, StringComparison.Ordinal));
				if (findMessageTrackingReportResponseMessageType.MessageTrackingSearchResults == null)
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug<string, MessageTrackingReportId>(this.GetHashCode(), "No results found from EWS call to {0}, report-id: {1}", webServiceTrackingAuthority.Domain, this.messageTrackingReportId);
					return new List<MessageTrackingSearchResult>(0);
				}
				List<MessageTrackingSearchResult> searchResults = new List<MessageTrackingSearchResult>(findMessageTrackingReportResponseMessageType.MessageTrackingSearchResults.Length);
				FindMessageTrackingSearchResultType[] messageTrackingSearchResults = findMessageTrackingReportResponseMessageType.MessageTrackingSearchResults;
				for (int i = 0; i < messageTrackingSearchResults.Length; i++)
				{
					FindMessageTrackingSearchResultType wsResult = messageTrackingSearchResults[i];
					TrackingBaseException ex4 = this.TryExecuteTask(delegate
					{
						searchResults.Add(MessageTrackingSearchResult.Create(wsResult, ewsBinding.TargetInfoForDisplay));
					});
					if (ex4 != null)
					{
						TraceWrapper.SearchLibraryTracer.TraceError<string>(this.GetHashCode(), "Unable to convert ws result returned with id {0}", wsResult.MessageTrackingReportId);
					}
				}
				return searchResults;
			}
		}

		private void TraceRecipientData(Dictionary<string, RecipientEventData> recipEventData)
		{
			if (this.directoryContext.DiagnosticsContext.VerboseDiagnostics || ExTraceGlobals.SearchLibraryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				foreach (KeyValuePair<string, RecipientEventData> keyValuePair in recipEventData)
				{
					if (keyValuePair.Value.Events != null)
					{
						foreach (RecipientTrackingEvent recipientTrackingEvent in keyValuePair.Value.Events)
						{
							TraceWrapper.SearchLibraryTracer.TraceDebug<string, SmtpAddress>(0, "Event: {0}, {1}", keyValuePair.Key, recipientTrackingEvent.RecipientAddress);
						}
					}
					int num = 0;
					if (keyValuePair.Value.HandoffPaths != null)
					{
						foreach (List<RecipientTrackingEvent> list in keyValuePair.Value.HandoffPaths)
						{
							foreach (RecipientTrackingEvent recipientTrackingEvent2 in list)
							{
								TraceWrapper.SearchLibraryTracer.TraceDebug<string, int, SmtpAddress>(0, "HandoffPath: {0}, {1}, {2}", keyValuePair.Key, num, recipientTrackingEvent2.RecipientAddress);
							}
							num++;
						}
					}
				}
			}
		}

		private static bool IsPageNotFoundAvailabilityException(Exception e)
		{
			TrackingTransientException ex = e as TrackingTransientException;
			if (ex == null || ex.InnerException == null)
			{
				return false;
			}
			AvailabilityException ex2 = ex.InnerException as AvailabilityException;
			if (ex2 == null || ex2.InnerException == null)
			{
				return false;
			}
			WebException ex3 = ex2.InnerException as WebException;
			return ex3 != null && WebServiceBinding.IsPageNotFoundWebException(ex3);
		}

		private static MessageTrackingSearchResult CreateSearchResultWithLegacyExchangeReportId(RecipientTrackingEvent sendEvent)
		{
			return new MessageTrackingSearchResult(MessageTrackingReportId.LegacyExchange, SmtpAddress.Empty, string.Empty, new SmtpAddress[]
			{
				sendEvent.RecipientAddress
			}, string.Empty, sendEvent.Date, string.Empty, string.Empty);
		}

		private TrackingBaseException TryExecuteTask(Action action)
		{
			TrackingBaseException ex = null;
			try
			{
				action();
			}
			catch (TrackingFatalException ex2)
			{
				ex = ex2;
				if (!ex2.IsAlreadyLogged)
				{
					this.Errors.Errors.Add(ex2.TrackingError);
				}
			}
			catch (TrackingTransientException ex3)
			{
				ex = ex3;
				if (!ex3.IsAlreadyLogged)
				{
					this.Errors.Errors.Add(ex3.TrackingError);
				}
			}
			if (ex != null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<TrackingBaseException>(0, "TrackingException occurred: {0}", ex);
				return ex;
			}
			return null;
		}

		private static GetMessageTrackingReportImpl.EventInfo GetSendEventInfo(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			if (logEntry.Source == MessageTrackingSource.SMTP)
			{
				ServerInfo serverInfo = ServerCache.Instance.FindMailboxOrHubServer(logEntry.NextHopFqdnOrName, 32UL);
				if (serverInfo.Status == ServerStatus.NotFound)
				{
					string domain = new SmtpAddress(logEntry.RecipientAddress).Domain;
					if (!string.IsNullOrEmpty(domain) && implObject.trackingDiscovery.IsCrossForestDomain(domain))
					{
						return new GetMessageTrackingReportImpl.EventInfo(EventType.SmtpSend, DeliveryStatus.Pending, EventDescription.SmtpSendCrossForest);
					}
					if (!string.IsNullOrEmpty(domain) && ServerCache.Instance.IsRemoteTrustedOrg(implObject.directoryContext.OrganizationId, domain))
					{
						return new GetMessageTrackingReportImpl.EventInfo(EventType.SmtpSend, DeliveryStatus.Pending, EventDescription.TransferredToPartnerOrg);
					}
					return new GetMessageTrackingReportImpl.EventInfo(EventType.SmtpSend, DeliveryStatus.Transferred, EventDescription.TransferredToForeignOrg);
				}
				else
				{
					if (serverInfo.Status == ServerStatus.LegacyExchangeServer)
					{
						return new GetMessageTrackingReportImpl.EventInfo(EventType.SmtpSend, DeliveryStatus.Transferred, EventDescription.TransferredToLegacyExchangeServer);
					}
					EventDescription eventDescription = logEntry.IsNextHopCrossSite(implObject.directoryContext) ? EventDescription.SmtpSendCrossSite : EventDescription.SmtpSend;
					return new GetMessageTrackingReportImpl.EventInfo(EventType.SmtpSend, DeliveryStatus.Pending, eventDescription);
				}
			}
			else
			{
				if (logEntry.Source == MessageTrackingSource.GATEWAY)
				{
					return new GetMessageTrackingReportImpl.EventInfo(EventType.SmtpSend, DeliveryStatus.Transferred, EventDescription.TransferredToForeignOrg);
				}
				if (logEntry.Source == MessageTrackingSource.AGENT && Parse.IsSMSRecipient(logEntry.RecipientAddress))
				{
					return new GetMessageTrackingReportImpl.EventInfo(EventType.Transferred, DeliveryStatus.Transferred, EventDescription.TransferredToForeignOrg);
				}
				return null;
			}
		}

		private static GetMessageTrackingReportImpl.EventInfo GetReceiveEventInfo(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			if (logEntry.Source == MessageTrackingSource.SMTP)
			{
				return new GetMessageTrackingReportImpl.EventInfo(EventType.SmtpReceive, DeliveryStatus.Pending, EventDescription.SmtpReceive);
			}
			if (logEntry.Source == MessageTrackingSource.STOREDRIVER)
			{
				return new GetMessageTrackingReportImpl.EventInfo(EventType.Submit, DeliveryStatus.Pending, EventDescription.Submitted);
			}
			if (logEntry.Source == MessageTrackingSource.AGENT)
			{
				return new GetMessageTrackingReportImpl.EventInfo(EventType.Redirect, DeliveryStatus.Pending, EventDescription.RulesCc);
			}
			return null;
		}

		private static GetMessageTrackingReportImpl.EventInfo GetFailEventInfo(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			if (logEntry.Source == MessageTrackingSource.AGENT && string.Equals(logEntry.SourceContext, "Transport Rule Agent", StringComparison.OrdinalIgnoreCase))
			{
				return new GetMessageTrackingReportImpl.EventInfo(EventType.Fail, DeliveryStatus.Unsuccessful, EventDescription.FailedTransportRules);
			}
			return new GetMessageTrackingReportImpl.EventInfo(EventType.Fail, DeliveryStatus.Unsuccessful, EventDescription.FailedGeneral);
		}

		private static GetMessageTrackingReportImpl.EventInfo GetExpandedEventInfo(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			if (string.IsNullOrEmpty(logEntry.FederatedDeliveryAddress))
			{
				return new GetMessageTrackingReportImpl.EventInfo(EventType.Expand, DeliveryStatus.Pending, EventDescription.Expanded);
			}
			string domain = new SmtpAddress(logEntry.FederatedDeliveryAddress).Domain;
			bool flag = !string.IsNullOrEmpty(domain) && ServerCache.Instance.IsRemoteTrustedOrg(implObject.directoryContext.OrganizationId, domain);
			return new GetMessageTrackingReportImpl.EventInfo(EventType.SmtpSend, flag ? DeliveryStatus.Pending : DeliveryStatus.Transferred, flag ? EventDescription.TransferredToPartnerOrg : EventDescription.TransferredToForeignOrg);
		}

		private static GetMessageTrackingReportImpl.EventInfo GetProcessedEventInfo(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			if (string.IsNullOrEmpty(logEntry.RecipientStatus))
			{
				return null;
			}
			if (logEntry.RecipientStatus.IndexOf("Approval Processing Agent", StringComparison.OrdinalIgnoreCase) >= 0 || logEntry.RecipientStatus.IndexOf("Mailbox Rules Agent", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return new GetMessageTrackingReportImpl.EventInfo(EventType.Deliver, DeliveryStatus.Delivered, EventDescription.Delivered);
			}
			return null;
		}

		private static GetMessageTrackingReportImpl.EventInfo GetSubmitEventInfo(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			return new GetMessageTrackingReportImpl.EventInfo(EventType.Submit, DeliveryStatus.Pending, logEntry.IsNextHopCrossSite(implObject.directoryContext) ? EventDescription.SubmittedCrossSite : EventDescription.Submitted);
		}

		private static GetMessageTrackingReportImpl.EventInfo GetHARedirectEventInfo(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			return new GetMessageTrackingReportImpl.EventInfo(EventType.SmtpSend, DeliveryStatus.Pending, logEntry.IsNextHopCrossSite(implObject.directoryContext) ? EventDescription.SmtpSendCrossSite : EventDescription.SmtpSend);
		}

		private static string[] GetResolvedEventData(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			if (string.IsNullOrEmpty(logEntry.RelatedRecipientAddress) || string.IsNullOrEmpty(logEntry.RecipientAddress))
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string, string>(implObject.GetHashCode(), "Either original [{0}] or final [{1}] addresses are not available, this is non-fatal, we will just display a simpler message", logEntry.RelatedRecipientAddress, logEntry.RecipientAddress);
				return null;
			}
			return new string[]
			{
				logEntry.RelatedRecipientAddress,
				logEntry.RecipientAddress
			};
		}

		private static string[] GetExpandEventData(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			SmtpAddress arg = new SmtpAddress(logEntry.RelatedRecipientAddress);
			TrackedUser trackedUser = TrackedUser.Create(arg.ToString(), implObject.directoryContext.TenantGalSession);
			if (trackedUser == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<SmtpAddress>(implObject.GetHashCode(), "DL {0} was resolved but not able to be validated, this is non-fatal, display name of DL will be unavailable", arg);
				return new string[]
				{
					logEntry.RelatedRecipientAddress,
					logEntry.RelatedRecipientAddress
				};
			}
			return new string[]
			{
				trackedUser.SmtpAddress.ToString(),
				trackedUser.DisplayName
			};
		}

		private static string[] GetSmtpSendEventData(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			string text;
			if (!string.IsNullOrEmpty(logEntry.NextHopFqdnOrName))
			{
				text = logEntry.NextHopFqdnOrName;
			}
			else
			{
				text = logEntry.ServerIP;
			}
			return new string[]
			{
				logEntry.Server,
				text
			};
		}

		private static string[] GetSmtpSendCrossForestEventData(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			string text;
			if (!string.IsNullOrEmpty(logEntry.NextHopFqdnOrName))
			{
				text = logEntry.NextHopFqdnOrName;
			}
			else
			{
				text = logEntry.ServerIP;
			}
			return new string[]
			{
				logEntry.Server,
				text,
				SmtpAddress.Parse(logEntry.RecipientAddress).Domain
			};
		}

		private static string[] GetTransferredToForeignOrgEventData(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			return new string[]
			{
				SmtpAddress.Parse(logEntry.RecipientAddress).Domain
			};
		}

		private static string[] GetTransferredToPartnerOrgEventData(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			if (logEntry.FederatedDeliveryAddress == null)
			{
				return null;
			}
			return new string[]
			{
				logEntry.FederatedDeliveryAddress
			};
		}

		private static string[] TransferredToLegacyExchangeServer(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			string text;
			if (!string.IsNullOrEmpty(logEntry.NextHopFqdnOrName))
			{
				text = logEntry.NextHopFqdnOrName;
			}
			else
			{
				text = logEntry.ServerIP;
			}
			return new string[]
			{
				logEntry.Server,
				text
			};
		}

		private static string[] GetSmtpReceiveEventData(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			string text;
			if (!string.IsNullOrEmpty(logEntry.ClientHostName))
			{
				text = logEntry.ClientHostName;
			}
			else
			{
				text = logEntry.ClientIP;
			}
			return new string[]
			{
				logEntry.Server,
				text
			};
		}

		private static string[] GetFailEventData(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			if (string.IsNullOrEmpty(logEntry.RecipientStatus))
			{
				return null;
			}
			if (logEntry.Source == MessageTrackingSource.SMTP && string.Equals(logEntry.SourceContext, "Content Filter Agent", StringComparison.OrdinalIgnoreCase))
			{
				return new string[]
				{
					logEntry.RecipientStatus,
					"Content Filter Agent"
				};
			}
			return new string[]
			{
				logEntry.RecipientStatus
			};
		}

		private static string[] GetSubmitCrossSiteEventData(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			string text;
			if (!string.IsNullOrEmpty(logEntry.NextHopFqdnOrName))
			{
				text = logEntry.NextHopFqdnOrName;
			}
			else
			{
				text = logEntry.ServerIP;
			}
			return new string[]
			{
				logEntry.Server,
				text
			};
		}

		private static string[] GetSubmitEventData(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject)
		{
			return new string[]
			{
				logEntry.Server
			};
		}

		private static string FormatDateTime(DateTime dateTime, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = CultureInfo.CurrentUICulture;
			}
			if (dateTime < culture.Calendar.MinSupportedDateTime)
			{
				dateTime = culture.Calendar.MinSupportedDateTime;
			}
			else if (dateTime > culture.Calendar.MaxSupportedDateTime)
			{
				dateTime = culture.Calendar.MaxSupportedDateTime;
			}
			return dateTime.ToString(culture);
		}

		private static Dictionary<MessageTrackingEvent, GetMessageTrackingReportImpl.EventInfo> CreateRawEventToEventInfoMap()
		{
			Dictionary<MessageTrackingEvent, GetMessageTrackingReportImpl.EventInfo> dictionary = new Dictionary<MessageTrackingEvent, GetMessageTrackingReportImpl.EventInfo>();
			dictionary[MessageTrackingEvent.BADMAIL] = new GetMessageTrackingReportImpl.EventInfo(EventType.Fail, DeliveryStatus.Unsuccessful, EventDescription.FailedGeneral);
			dictionary[MessageTrackingEvent.DELIVER] = new GetMessageTrackingReportImpl.EventInfo(EventType.Deliver, DeliveryStatus.Delivered, EventDescription.Delivered);
			dictionary[MessageTrackingEvent.DSN] = new GetMessageTrackingReportImpl.EventInfo(EventType.Fail, DeliveryStatus.Unsuccessful, EventDescription.FailedGeneral);
			dictionary[MessageTrackingEvent.FAIL] = new GetMessageTrackingReportImpl.EventInfo(EventType.Fail, DeliveryStatus.Unsuccessful, EventDescription.FailedGeneral);
			dictionary[MessageTrackingEvent.INITMESSAGECREATED] = new GetMessageTrackingReportImpl.EventInfo(EventType.InitMessageCreated, DeliveryStatus.Pending, EventDescription.PendingModeration);
			dictionary[MessageTrackingEvent.MODERATORAPPROVE] = new GetMessageTrackingReportImpl.EventInfo(EventType.ModeratorApprove, DeliveryStatus.Pending, EventDescription.ApprovedModeration);
			dictionary[MessageTrackingEvent.MODERATORREJECT] = new GetMessageTrackingReportImpl.EventInfo(EventType.ModeratorRejected, DeliveryStatus.Unsuccessful, EventDescription.FailedModeration);
			dictionary[MessageTrackingEvent.POISONMESSAGE] = new GetMessageTrackingReportImpl.EventInfo(EventType.Fail, DeliveryStatus.Unsuccessful, EventDescription.FailedGeneral);
			dictionary[MessageTrackingEvent.REDIRECT] = new GetMessageTrackingReportImpl.EventInfo(EventType.Redirect, DeliveryStatus.Pending, EventDescription.Forwarded);
			dictionary[MessageTrackingEvent.RESOLVE] = new GetMessageTrackingReportImpl.EventInfo(EventType.Resolve, DeliveryStatus.Pending, EventDescription.Resolved);
			dictionary[MessageTrackingEvent.RESUBMIT] = new GetMessageTrackingReportImpl.EventInfo(EventType.Submit, DeliveryStatus.Pending, EventDescription.Submitted);
			dictionary[MessageTrackingEvent.DEFER] = new GetMessageTrackingReportImpl.EventInfo(EventType.Defer, DeliveryStatus.Pending, EventDescription.MessageDefer);
			dictionary[MessageTrackingEvent.DUPLICATEDELIVER] = new GetMessageTrackingReportImpl.EventInfo(EventType.Deliver, DeliveryStatus.Delivered, EventDescription.Delivered);
			dictionary[MessageTrackingEvent.MODERATIONEXPIRE] = new GetMessageTrackingReportImpl.EventInfo(EventType.Fail, DeliveryStatus.Unsuccessful, EventDescription.ExpiredWithNoModerationDecision);
			return dictionary;
		}

		private static Dictionary<MessageTrackingEvent, GetMessageTrackingReportImpl.GetEventInfoMethod> CreateRawEventToEventInfoGetterMap()
		{
			Dictionary<MessageTrackingEvent, GetMessageTrackingReportImpl.GetEventInfoMethod> dictionary = new Dictionary<MessageTrackingEvent, GetMessageTrackingReportImpl.GetEventInfoMethod>();
			dictionary[MessageTrackingEvent.SEND] = new GetMessageTrackingReportImpl.GetEventInfoMethod(GetMessageTrackingReportImpl.GetSendEventInfo);
			dictionary[MessageTrackingEvent.RECEIVE] = new GetMessageTrackingReportImpl.GetEventInfoMethod(GetMessageTrackingReportImpl.GetReceiveEventInfo);
			dictionary[MessageTrackingEvent.FAIL] = new GetMessageTrackingReportImpl.GetEventInfoMethod(GetMessageTrackingReportImpl.GetFailEventInfo);
			dictionary[MessageTrackingEvent.EXPAND] = new GetMessageTrackingReportImpl.GetEventInfoMethod(GetMessageTrackingReportImpl.GetExpandedEventInfo);
			dictionary[MessageTrackingEvent.PROCESS] = new GetMessageTrackingReportImpl.GetEventInfoMethod(GetMessageTrackingReportImpl.GetProcessedEventInfo);
			dictionary[MessageTrackingEvent.SUBMIT] = new GetMessageTrackingReportImpl.GetEventInfoMethod(GetMessageTrackingReportImpl.GetSubmitEventInfo);
			dictionary[MessageTrackingEvent.HAREDIRECT] = new GetMessageTrackingReportImpl.GetEventInfoMethod(GetMessageTrackingReportImpl.GetHARedirectEventInfo);
			return dictionary;
		}

		private static Dictionary<EventDescription, GetMessageTrackingReportImpl.GetEventDataMethod> CreateEventDataGetters()
		{
			Dictionary<EventDescription, GetMessageTrackingReportImpl.GetEventDataMethod> dictionary = new Dictionary<EventDescription, GetMessageTrackingReportImpl.GetEventDataMethod>();
			dictionary[EventDescription.Resolved] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.GetResolvedEventData);
			dictionary[EventDescription.Expanded] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.GetExpandEventData);
			dictionary[EventDescription.SmtpReceive] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.GetSmtpReceiveEventData);
			dictionary[EventDescription.TransferredToForeignOrg] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.GetTransferredToForeignOrgEventData);
			dictionary[EventDescription.TransferredToPartnerOrg] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.GetTransferredToPartnerOrgEventData);
			dictionary[EventDescription.TransferredToLegacyExchangeServer] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.TransferredToLegacyExchangeServer);
			dictionary[EventDescription.SmtpSend] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.GetSmtpSendEventData);
			dictionary[EventDescription.SmtpSendCrossSite] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.GetSmtpSendEventData);
			dictionary[EventDescription.SmtpSendCrossForest] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.GetSmtpSendCrossForestEventData);
			dictionary[EventDescription.FailedGeneral] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.GetFailEventData);
			dictionary[EventDescription.Submitted] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.GetSubmitEventData);
			dictionary[EventDescription.SubmittedCrossSite] = new GetMessageTrackingReportImpl.GetEventDataMethod(GetMessageTrackingReportImpl.GetSubmitCrossSiteEventData);
			return dictionary;
		}

		private static IEnumerable<List<RecipientTrackingEvent>> ConvertRecipientPathModeEventsToPaths(RecipientEventData recipEventData)
		{
			if (recipEventData.Events != null && recipEventData.Events.Count > 0)
			{
				yield return recipEventData.Events;
			}
			else if (recipEventData.HandoffPaths != null)
			{
				foreach (List<RecipientTrackingEvent> path in recipEventData.HandoffPaths)
				{
					yield return path;
				}
			}
			yield break;
		}

		private static IEnumerable<List<RecipientTrackingEvent>> ConvertSummaryEventsToPaths(List<RecipientTrackingEvent> recipEvents)
		{
			if (recipEvents != null)
			{
				foreach (RecipientTrackingEvent recipEvent in recipEvents)
				{
					yield return new List<RecipientTrackingEvent>(0)
					{
						recipEvent
					};
				}
			}
			yield break;
		}

		private static readonly KeyValuePair<FindParameters, GetMessageTrackingReportImpl.FindCachedItem>[] EmptyFindRecords = new KeyValuePair<FindParameters, GetMessageTrackingReportImpl.FindCachedItem>[0];

		private static readonly KeyValuePair<WSGetParameters, WSGetResult>[] EmptyGetRecords = new KeyValuePair<WSGetParameters, WSGetResult>[0];

		private static GetMessageTrackingReportImpl.EventInfo generalPendingEventInfo = new GetMessageTrackingReportImpl.EventInfo(EventType.Pending, DeliveryStatus.Pending, EventDescription.Pending);

		private static Dictionary<MessageTrackingEvent, GetMessageTrackingReportImpl.EventInfo> rawEventToEventInfo = GetMessageTrackingReportImpl.CreateRawEventToEventInfoMap();

		private static Dictionary<MessageTrackingEvent, GetMessageTrackingReportImpl.GetEventInfoMethod> rawEventToEventInfoGetter = GetMessageTrackingReportImpl.CreateRawEventToEventInfoGetterMap();

		private static BitArray initialEventRows = MessageTrackingLogRow.GetColumnFilter(new MessageTrackingField[]
		{
			MessageTrackingField.SenderAddress,
			MessageTrackingField.RecipientAddress,
			MessageTrackingField.Timestamp,
			MessageTrackingField.MessageSubject
		});

		private static Dictionary<EventDescription, GetMessageTrackingReportImpl.GetEventDataMethod> eventDataGetters = GetMessageTrackingReportImpl.CreateEventDataGetters();

		private static PropertyDefinition[] readStatusProperties = new PropertyDefinition[]
		{
			MessageItemSchema.Flags,
			MessageItemSchema.TransportMessageHeaders
		};

		private SearchScope scope;

		private TrackingDiscovery trackingDiscovery;

		private ReportConstraints constraints;

		private TrackingContext trackingContext;

		private MessageTrackingReportId messageTrackingReportId;

		private DirectoryContext directoryContext;

		private string defaultDomain;

		private CultureInfo userCulture;

		private string subject;

		private DateTime submittedDateTime;

		private SmtpAddress? fromAddress;

		private string fromDisplayName;

		private SmtpAddress[] submissionRecipientAddresses = new SmtpAddress[0];

		private string[] submissionRecipientDisplayNames = new string[0];

		private WSAdditionalRecords<FindParameters, GetMessageTrackingReportImpl.FindCachedItem> findAdditionalRecords;

		private WSAdditionalRecords<WSGetParameters, WSGetResult> getAdditionalRecords;

		private class EventInfo
		{
			public EventInfo(EventType eventType, DeliveryStatus deliveryStatus, EventDescription eventDescription)
			{
				this.eventType = eventType;
				this.deliveryStatus = deliveryStatus;
				this.eventDescription = eventDescription;
			}

			public EventType EventType
			{
				get
				{
					return this.eventType;
				}
			}

			public DeliveryStatus DeliveryStatus
			{
				get
				{
					return this.deliveryStatus;
				}
			}

			public EventDescription EventDescription
			{
				get
				{
					return this.eventDescription;
				}
			}

			private EventType eventType;

			private DeliveryStatus deliveryStatus;

			private EventDescription eventDescription;
		}

		internal class RecipientReferral
		{
			public RecipientReferral(RecipientTrackingEvent recipientTrackingEvent, TrackingAuthority authority)
			{
				this.recipientTrackingEvent = recipientTrackingEvent;
				this.authority = authority;
			}

			public RecipientTrackingEvent RecipientTrackingEvent
			{
				get
				{
					return this.recipientTrackingEvent;
				}
			}

			public TrackingAuthority Authority
			{
				get
				{
					return this.authority;
				}
			}

			private RecipientTrackingEvent recipientTrackingEvent;

			private TrackingAuthority authority;
		}

		private delegate string[] GetEventDataMethod(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject);

		private delegate GetMessageTrackingReportImpl.EventInfo GetEventInfoMethod(MessageTrackingLogEntry logEntry, GetMessageTrackingReportImpl implObject);

		internal class FindCachedItem
		{
			internal List<MessageTrackingSearchResult> Results
			{
				get
				{
					return this.results;
				}
			}

			internal bool EntireForestSearched { get; set; }

			internal HashSet<SmtpAddress> AddressesSearched
			{
				get
				{
					return this.addressesSearched;
				}
			}

			internal FindCachedItem()
			{
			}

			private HashSet<SmtpAddress> addressesSearched = new HashSet<SmtpAddress>();

			private List<MessageTrackingSearchResult> results = new List<MessageTrackingSearchResult>();
		}
	}
}
