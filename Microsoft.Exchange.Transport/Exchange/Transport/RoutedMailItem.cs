using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport
{
	internal class RoutedMailItem : ILockableItem, IQueueItem, IReadOnlyMailItem, ISystemProbeTraceable
	{
		public RoutedMailItem(TransportMailItem mailItem, NextHopSolutionKey key)
		{
			this.mailItem = mailItem;
			if (!mailItem.NextHopSolutions.TryGetValue(key, out this.solution))
			{
				throw new ArgumentException("Invalid solution key used to create routed mail item", "key");
			}
		}

		public int MailItemRecipientCount
		{
			get
			{
				return this.mailItem.Recipients.Count;
			}
		}

		public DeliveryType DeliveryType
		{
			get
			{
				return this.solution.NextHopSolutionKey.NextHopType.DeliveryType;
			}
		}

		public DateTime RoutingTimeStamp
		{
			get
			{
				return this.mailItem.RoutingTimeStamp;
			}
		}

		public UnreachableReason UnreachableReasons
		{
			get
			{
				UnreachableSolution unreachableSolution = this.solution as UnreachableSolution;
				if (unreachableSolution == null)
				{
					return UnreachableReason.None;
				}
				return unreachableSolution.Reasons;
			}
		}

		public ADRecipientCache<TransportMiniRecipient> ADRecipientCache
		{
			get
			{
				return this.mailItem.ADRecipientCache;
			}
		}

		public string Auth
		{
			get
			{
				return this.mailItem.Auth;
			}
		}

		public MultilevelAuthMechanism AuthMethod
		{
			get
			{
				return this.mailItem.AuthMethod;
			}
		}

		public BodyType BodyType
		{
			get
			{
				return this.mailItem.BodyType;
			}
		}

		public DateTime DateReceived
		{
			get
			{
				return this.mailItem.DateReceived;
			}
		}

		public DeferReason DeferReason
		{
			get
			{
				return this.deferReason;
			}
			set
			{
				this.deferReason = value;
			}
		}

		public bool IsInactive
		{
			get
			{
				return this.solution.IsInactive;
			}
		}

		public MailDirectionality Directionality
		{
			get
			{
				return this.mailItem.Directionality;
			}
		}

		public DsnFormat DsnFormat
		{
			get
			{
				if (this.suppressBodyInDsn)
				{
					return DsnFormat.Headers;
				}
				return this.mailItem.DsnFormat;
			}
		}

		public DsnParameters DsnParameters
		{
			get
			{
				return this.dsnParameters;
			}
		}

		public string EnvId
		{
			get
			{
				return this.mailItem.EnvId;
			}
		}

		public IReadOnlyExtendedPropertyCollection ExtendedProperties
		{
			get
			{
				if (this.deliveryQueueMailboxSubComponent != LatencyComponent.None)
				{
					if (this.extendedPropertyDictionaryCopy == null)
					{
						this.extendedPropertyDictionaryCopy = new ExtendedPropertyDictionary();
						this.extendedPropertyDictionaryCopy.CloneFrom((ExtendedPropertyDictionary)this.mailItem.ExtendedProperties);
					}
					this.extendedPropertyDictionaryCopy.SetValue<string>("Microsoft.Exchange.Transport.DeliveryQueueMailboxSubComponent", this.deliveryQueueMailboxSubComponent.ToString());
					this.extendedPropertyDictionaryCopy.SetValue<long>("Microsoft.Exchange.Transport.DeliveryQueueMailboxSubComponentLatency", (long)this.deliveryQueueMailboxSubComponentLatency.TotalSeconds);
					return this.extendedPropertyDictionaryCopy;
				}
				return this.mailItem.ExtendedProperties;
			}
		}

		public TimeSpan ExtensionToExpiryDuration
		{
			get
			{
				return this.mailItem.ExtensionToExpiryDuration;
			}
		}

		public RoutingAddress From
		{
			get
			{
				return this.mailItem.From;
			}
		}

		public string HeloDomain
		{
			get
			{
				return this.mailItem.HeloDomain;
			}
		}

		public string InternetMessageId
		{
			get
			{
				return this.mailItem.InternetMessageId;
			}
		}

		public Guid NetworkMessageId
		{
			get
			{
				return this.mailItem.NetworkMessageId;
			}
		}

		public Guid SystemProbeId
		{
			get
			{
				return this.mailItem.SystemProbeId;
			}
		}

		public bool IsProbe
		{
			get
			{
				return this.mailItem.IsProbe;
			}
		}

		public string ProbeName
		{
			get
			{
				return this.mailItem.ProbeName;
			}
		}

		public bool PersistProbeTrace
		{
			get
			{
				return this.mailItem.PersistProbeTrace;
			}
		}

		public bool IsHeartbeat
		{
			get
			{
				return this.mailItem.IsHeartbeat;
			}
		}

		public bool IsActive
		{
			get
			{
				return this.mailItem.IsActive;
			}
		}

		public LatencyTracker LatencyTracker
		{
			get
			{
				if (this.latencyTracker == null)
				{
					this.latencyTracker = LatencyTracker.Clone(this.mailItem.LatencyTracker);
				}
				return this.latencyTracker;
			}
		}

		public byte[] LegacyXexch50Blob
		{
			get
			{
				return this.mailItem.LegacyXexch50Blob;
			}
		}

		public EmailMessage Message
		{
			get
			{
				return this.mailItem.Message;
			}
		}

		public string Oorg
		{
			get
			{
				return this.mailItem.Oorg;
			}
		}

		public string ExoAccountForest
		{
			get
			{
				return this.mailItem.ExoAccountForest;
			}
		}

		public string ExoTenantContainer
		{
			get
			{
				return this.mailItem.ExoTenantContainer;
			}
		}

		public Guid ExternalOrganizationId
		{
			get
			{
				return this.mailItem.ExternalOrganizationId;
			}
		}

		public string PrioritizationReason
		{
			get
			{
				return this.mailItem.PrioritizationReason;
			}
		}

		public MimeDocument MimeDocument
		{
			get
			{
				return this.mailItem.MimeDocument;
			}
		}

		public string MimeFrom
		{
			get
			{
				return this.mailItem.MimeFrom;
			}
		}

		public RoutingAddress MimeSender
		{
			get
			{
				return this.mailItem.MimeSender;
			}
		}

		public long MimeSize
		{
			get
			{
				return this.mailItem.MimeSize;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.mailItem.OrganizationId;
			}
		}

		public RoutingAddress OriginalFrom
		{
			get
			{
				return this.mailItem.OriginalFrom;
			}
		}

		public int PoisonCount
		{
			get
			{
				return this.mailItem.PoisonCount;
			}
		}

		public int PoisonForRemoteCount
		{
			get
			{
				return this.poisonForRemoteCount;
			}
		}

		public bool IsPoison
		{
			get
			{
				return this.mailItem.IsPoison;
			}
		}

		public string ReceiveConnectorName
		{
			get
			{
				return this.mailItem.ReceiveConnectorName;
			}
		}

		public IReadOnlyMailRecipientCollection Recipients
		{
			get
			{
				return this.solution;
			}
		}

		public bool DelayDeferLogged
		{
			get
			{
				return this.delayDeferLogged;
			}
			private set
			{
				this.delayDeferLogged = value;
			}
		}

		public bool RetryDeferLogged
		{
			get
			{
				return this.retryDeferLogged;
			}
			private set
			{
				this.retryDeferLogged = value;
			}
		}

		public long RecordId
		{
			get
			{
				return this.mailItem.RecordId;
			}
		}

		public bool RetryDeliveryIfRejected
		{
			get
			{
				return this.mailItem.RetryDeliveryIfRejected;
			}
		}

		public MimePart RootPart
		{
			get
			{
				return this.mailItem.RootPart;
			}
		}

		public int Scl
		{
			get
			{
				return this.mailItem.Scl;
			}
		}

		public Guid ShadowMessageId
		{
			get
			{
				return this.mailItem.ShadowMessageId;
			}
		}

		public string ShadowServerContext
		{
			get
			{
				return this.mailItem.ShadowServerContext;
			}
		}

		public string ShadowServerDiscardId
		{
			get
			{
				return this.mailItem.ShadowServerDiscardId;
			}
		}

		public IPAddress SourceIPAddress
		{
			get
			{
				return this.mailItem.SourceIPAddress;
			}
		}

		public string Subject
		{
			get
			{
				return this.mailItem.Subject;
			}
		}

		public bool SuppressBodyInDsn
		{
			get
			{
				return this.suppressBodyInDsn;
			}
			set
			{
				this.suppressBodyInDsn = value;
			}
		}

		public PerTenantTransportSettings TransportSettings
		{
			get
			{
				return this.mailItem.TransportSettings;
			}
		}

		public DateTime DeferUntil
		{
			get
			{
				return this.solution.DeferUntil;
			}
			set
			{
				this.solution.DeferUntil = value;
			}
		}

		DateTime IQueueItem.Expiry
		{
			get
			{
				AdminActionStatus adminActionStatus = this.solution.AdminActionStatus;
				if (adminActionStatus == AdminActionStatus.PendingDeleteWithNDR || adminActionStatus == AdminActionStatus.PendingDeleteWithOutNDR)
				{
					return DateTime.MinValue;
				}
				return this.mailItem.Expiry;
			}
		}

		public DeliveryPriority Priority
		{
			get
			{
				return this.mailItem.Priority;
			}
			set
			{
				throw new NotImplementedException("Priority is NOT settable on RMI.");
			}
		}

		public DateTime OriginalEnqueuedTime
		{
			get
			{
				return this.originalEnqueuedTime;
			}
		}

		public DateTime EnqueuedTime
		{
			get
			{
				return this.enqueuedTime;
			}
			set
			{
				this.enqueuedTime = value;
				if (this.originalEnqueuedTime == DateTime.MinValue)
				{
					this.originalEnqueuedTime = value;
				}
			}
		}

		public bool Deferred
		{
			get
			{
				return this.solution.Deferred;
			}
			set
			{
				this.solution.Deferred = value;
			}
		}

		public RiskLevel RiskLevel
		{
			get
			{
				return this.solution.NextHopSolutionKey.RiskLevel;
			}
		}

		public bool IsDeletedByAdmin
		{
			get
			{
				return !this.mailItem.IsActive || this.solution.AdminActionStatus == AdminActionStatus.PendingDeleteWithNDR || this.solution.AdminActionStatus == AdminActionStatus.PendingDeleteWithOutNDR;
			}
		}

		public bool IsSuspendedByAdmin
		{
			get
			{
				return this.solution.AdminActionStatus == AdminActionStatus.Suspended || this.solution.AdminActionStatus == AdminActionStatus.SuspendedInSubmissionQueue;
			}
		}

		public AccessToken AccessToken
		{
			get
			{
				return this.solution.AccessToken;
			}
			set
			{
				this.solution.AccessToken = value;
			}
		}

		public ThrottlingContext ThrottlingContext
		{
			get
			{
				return this.throttlingContext;
			}
			set
			{
				this.throttlingContext = value;
			}
		}

		public WaitCondition CurrentCondition
		{
			get
			{
				return this.solution.CurrentCondition;
			}
			set
			{
				this.solution.CurrentCondition = value;
			}
		}

		public QueuedRecipientsByAgeToken QueuedRecipientsByAgeToken
		{
			get
			{
				return this.queuedRecipientsByAgeToken;
			}
			set
			{
				this.queuedRecipientsByAgeToken = value;
			}
		}

		public DateTimeOffset LockExpirationTime
		{
			get
			{
				return this.solution.LockExpirationTime;
			}
			set
			{
				this.solution.LockExpirationTime = value;
			}
		}

		public string LockReason
		{
			get
			{
				return this.solution.LockReason;
			}
			set
			{
				this.solution.LockReason = value;
				if (!string.IsNullOrEmpty(value))
				{
					if (this.lockReasonHistory == null)
					{
						this.lockReasonHistory = new List<string>();
					}
					this.lockReasonHistory.Add(value);
				}
			}
		}

		public IEnumerable<string> LockReasonHistory
		{
			get
			{
				if (this.lockReasonHistory == null)
				{
					return this.mailItem.LockReasonHistory;
				}
				if (this.mailItem.LockReasonHistory != null)
				{
					return this.mailItem.LockReasonHistory.Concat(this.lockReasonHistory);
				}
				return this.lockReasonHistory;
			}
		}

		public LazyBytes FastIndexBlob
		{
			get
			{
				return this.mailItem.FastIndexBlob;
			}
		}

		public LastError LastQueueLevelError
		{
			get
			{
				return this.lastQueueLevelError;
			}
			set
			{
				this.lastQueueLevelError = value;
			}
		}

		public DateTime LatencyStartTime
		{
			get
			{
				return this.mailItem.LatencyStartTime;
			}
		}

		internal bool RetriedDueToDeliverySourceLimit
		{
			get
			{
				return this.retriedDueToDeliverySourceLimit;
			}
			set
			{
				this.retriedDueToDeliverySourceLimit = value;
			}
		}

		private DateTime DelayNotificationTime
		{
			get
			{
				TimeSpan t;
				if (Components.TransportAppConfig.RemoteDelivery.PriorityQueuingEnabled)
				{
					t = Components.TransportAppConfig.RemoteDelivery.DelayNotificationTimeout(((IQueueItem)this.mailItem).Priority);
				}
				else
				{
					t = Components.Configuration.LocalServer.TransportServer.DelayNotificationTimeout;
				}
				return this.mailItem.DateReceived + t;
			}
		}

		public void CacheTransportSettings()
		{
			this.mailItem.CacheTransportSettings();
		}

		public bool IsJournalReport()
		{
			return this.mailItem.IsJournalReport();
		}

		public bool IsPfReplica()
		{
			return this.mailItem.IsPfReplica();
		}

		public bool IsShadowed()
		{
			return this.mailItem.IsShadowed();
		}

		public bool IsDelayedAck()
		{
			return this.mailItem.IsDelayedAck();
		}

		public TransportMailItem NewCloneWithoutRecipients(bool shareRecipientCache)
		{
			TransportMailItem result;
			lock (this.mailItem)
			{
				result = this.mailItem.NewCloneWithoutRecipients(shareRecipientCache, this.latencyTracker ?? this.mailItem.LatencyTracker, this.solution);
			}
			return result;
		}

		public Stream OpenMimeReadStream()
		{
			return this.mailItem.OpenMimeReadStream();
		}

		public bool IsWorkNeeded(DateTime now)
		{
			if (!Monitor.TryEnter(this.mailItem))
			{
				return false;
			}
			try
			{
				if (this.IsDeletedByAdmin || this.DelayNotificationTime >= now)
				{
					return false;
				}
				if (now - this.mailItem.DateReceived >= TimeSpan.FromMinutes(30.0) && this.mailItem.PendingDatabaseUpdates)
				{
					return true;
				}
				foreach (MailRecipient mailRecipient in this.solution.Recipients)
				{
					if (mailRecipient.IsDelayDsnNeeded)
					{
						return true;
					}
				}
			}
			finally
			{
				Monitor.Exit(this.mailItem);
			}
			return false;
		}

		MessageContext IQueueItem.GetMessageContext(MessageProcessingSource source)
		{
			return new MessageContext(this.RecordId, this.InternetMessageId, source);
		}

		void IQueueItem.Update()
		{
			if (Monitor.TryEnter(this.mailItem))
			{
				try
				{
					this.InternalProcessDeferral();
				}
				finally
				{
					Monitor.Exit(this.mailItem);
				}
			}
		}

		public void Ack(AckStatus ackStatus, SmtpResponse smtpResponse, Queue<AckStatusAndResponse> recipientResponses, IEnumerable<MailRecipient> recipientsToTrack, MessageTrackingSource messageTrackingSource, string messageTrackingSourceContext, AckDetails ackDetails, bool reportEndToEndLatencies, DeferReason resubmitDeferReason, TimeSpan? resubmitDeferInterval, TimeSpan? retryInterval, string remoteMta, bool shadowed, string primaryServer, bool resetTimeForOrar, out bool shouldEnqueue)
		{
			shouldEnqueue = false;
			lock (this.mailItem)
			{
				this.UpdateE2ELatencyBucketsOnAck(recipientResponses);
				bool flag2 = RoutedMailItem.AllRecipientLimitRetries(recipientResponses);
				Destination deliveredDestination = null;
				if (this.solution.NextHopSolutionKey.NextHopType.DeliveryType == DeliveryType.SmtpDeliveryToMailbox)
				{
					deliveredDestination = new Destination(Destination.DestinationType.Mdb, this.solution.NextHopSolutionKey.NextHopConnector);
				}
				if (ackDetails != null)
				{
					ackDetails.LastRetryTime = new DateTime?(this.EnqueuedTime);
				}
				List<MailRecipient> list = new List<MailRecipient>();
				List<MailRecipient> list2 = new List<MailRecipient>();
				bool flag3;
				bool flag4;
				this.mailItem.Ack(ackStatus, smtpResponse, ackDetails, this.solution.Recipients, this.solution.AdminActionStatus, recipientResponses, deliveredDestination, list2, out flag3, out flag4);
				shouldEnqueue = (flag3 || flag4);
				TimeSpan timeSpan = TimeSpan.Zero;
				string str = null;
				bool flag5 = flag4 && !flag3;
				if (flag5)
				{
					this.numConsecutiveRetries++;
					if (retryInterval != null)
					{
						timeSpan = retryInterval.Value;
						if (string.IsNullOrEmpty(messageTrackingSourceContext))
						{
							str = string.Format("Deferring message with provided retry time of: {0}", timeSpan);
						}
					}
					else if (Components.TransportAppConfig.RemoteDelivery.MessageRetryIntervalProgressiveBackoffEnabled && (this.Priority == DeliveryPriority.High || this.Priority == DeliveryPriority.Normal))
					{
						timeSpan = Components.TransportAppConfig.RemoteDelivery.GetMessageRetryInterval(this.numConsecutiveRetries, TransportDeliveryTypes.internalDeliveryTypes.Contains(this.DeliveryType));
						str = string.Format("Progressive backoff retry time of: {0}", timeSpan);
					}
					else
					{
						timeSpan = Components.Configuration.LocalServer.TransportServer.MessageRetryInterval;
						str = string.Format("Using server config retry time of {0}", timeSpan);
					}
					this.DeferUntil = DateTime.UtcNow + timeSpan;
					this.UpdateDeliveryQueueMailboxLatency(timeSpan, smtpResponse, flag2);
					if (!flag2)
					{
						goto IL_259;
					}
					this.deferReason = DeferReason.RecipientThreadLimitExceeded;
					if (this.numConsecutiveRetries < Components.TransportAppConfig.RemoteDelivery.DeprioritizeOnRecipientThreadLimitExceededCount || this.Priority != DeliveryPriority.Normal)
					{
						goto IL_259;
					}
					shouldEnqueue = false;
					flag5 = false;
					using (List<MailRecipient>.Enumerator enumerator = this.solution.Recipients.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MailRecipient mailRecipient = enumerator.Current;
							if (mailRecipient.AckStatus == AckStatus.Retry)
							{
								mailRecipient.Ack(AckStatus.Resubmit, AckReason.RecipientThreadLimitExceeded);
								list.Add(mailRecipient);
							}
						}
						goto IL_259;
					}
				}
				this.numConsecutiveRetries = 0;
				IL_259:
				if (shouldEnqueue)
				{
					this.solution.DeliveryStatus = DeliveryStatus.Enqueued;
				}
				else if (list2.Count > 0 || list.Count > 0)
				{
					this.solution.DeliveryStatus = DeliveryStatus.PendingResubmit;
				}
				else
				{
					this.solution.DeliveryStatus = DeliveryStatus.Complete;
				}
				if (this.solution.NextHopSolutionKey.NextHopType != NextHopType.Heartbeat)
				{
					Components.OrarGenerator.GenerateOrarMessage(this, resetTimeForOrar);
					Components.DsnGenerator.GenerateDSNs(this, remoteMta, DsnGenerator.CallerComponent.Delivery);
					string messageTrackingSourceContext2 = messageTrackingSourceContext + str;
					this.Track(ackStatus, smtpResponse, recipientsToTrack, messageTrackingSource, messageTrackingSourceContext2, ackDetails, reportEndToEndLatencies, flag5);
				}
				if (this.solution.DeliveryStatus == DeliveryStatus.Complete)
				{
					Components.ShadowRedundancyComponent.ShadowRedundancyManager.EnqueueShadowMailItem(this.mailItem, this.solution, primaryServer, shadowed, ackStatus);
				}
				this.Commit();
				if (list2.Count > 0)
				{
					this.Resubmit(list2, resubmitDeferReason, resubmitDeferInterval, false, null);
				}
				if (list.Count > 0)
				{
					this.Resubmit(list, DeferReason.RecipientThreadLimitExceeded, new TimeSpan?(timeSpan), false, delegate(TransportMailItem mailItemToSubmit)
					{
						mailItemToSubmit.Priority = DeliveryPriority.Low;
						mailItemToSubmit.PrioritizationReason = "RecipientThreadLimitExceeded";
					});
				}
			}
		}

		public void Ack(AckStatus ackStatus, SmtpResponse smtpResponse, MessageTrackingSource messageTrackingSource, string messageTrackingSourceContext, AckDetails ackDetails, bool resetTimeForOrar, out bool deletedByAdmin, out RiskLevel riskLevel, out DeliveryPriority priority)
		{
			if (ackStatus == AckStatus.Success)
			{
				throw new ArgumentOutOfRangeException("ackStatus", "This overload does not support AckStatus.Success");
			}
			lock (this.mailItem)
			{
				if (this.IsDeletedByAdmin)
				{
					deletedByAdmin = true;
					riskLevel = RiskLevel.Normal;
					priority = DeliveryPriority.Normal;
				}
				else
				{
					deletedByAdmin = false;
					riskLevel = this.RiskLevel;
					priority = this.Priority;
					bool flag2;
					this.Ack(ackStatus, smtpResponse, null, this.Recipients, messageTrackingSource, messageTrackingSourceContext, ackDetails, ackStatus == AckStatus.Fail, DeferReason.None, null, null, null, false, null, resetTimeForOrar, out flag2);
				}
			}
		}

		public void AbortHeartbeat()
		{
			lock (this.mailItem)
			{
				this.mailItem.Recipients[0].Ack(AckStatus.SuccessNoDsn, SmtpResponse.Empty);
				this.mailItem.Ack(AckStatus.Fail, SmtpResponse.Empty, this.solution.Recipients, null);
				this.Commit();
			}
		}

		public bool Defer()
		{
			bool result;
			lock (this.mailItem)
			{
				if (this.InternalProcessDeferral())
				{
					this.Deferred = true;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public void Lock(string lockReason)
		{
			lock (this.mailItem)
			{
				this.LockReason = lockReason;
				this.solution.DeliveryStatus = DeliveryStatus.Enqueued;
				foreach (MailRecipient mailRecipient in this.solution.Recipients)
				{
					if (mailRecipient.Status == Status.Ready)
					{
						mailRecipient.Status = Status.Locked;
					}
				}
			}
		}

		public void Dehydrate(Breadcrumb breadcrumb)
		{
			lock (this.mailItem)
			{
				this.mailItem.CommitLazyAndDehydrateMessageIfPossible(breadcrumb);
			}
		}

		public bool Activate()
		{
			lock (this.mailItem)
			{
				if (this.IsDeletedByAdmin)
				{
					return false;
				}
				foreach (MailRecipient mailRecipient in this.solution.Recipients)
				{
					if (mailRecipient.Status == Status.Retry)
					{
						mailRecipient.Status = Status.Ready;
					}
				}
				this.solution.DeliveryStatus = DeliveryStatus.Enqueued;
				if (this.mailItem.DeferReason != DeferReason.None)
				{
					LatencyTracker.EndTrackLatency(TransportMailItem.GetDeferLatencyComponent(this.mailItem.DeferReason), this.mailItem.LatencyTracker);
				}
				this.deferReason = DeferReason.None;
				this.Deferred = false;
			}
			return true;
		}

		public RoutedMailItem.PrepareForDeliveryResult PrepareForDelivery()
		{
			RoutedMailItem.PrepareForDeliveryResult result;
			lock (this.mailItem)
			{
				if (this.IsDeletedByAdmin)
				{
					result = RoutedMailItem.PrepareForDeliveryResult.IgnoreDeleted;
				}
				else if (this.solution.AdminActionStatus == AdminActionStatus.Suspended)
				{
					this.PrepareForSuspension();
					result = RoutedMailItem.PrepareForDeliveryResult.Requeue;
				}
				else if (this.AccessToken != null && !this.AccessToken.Validate(this.AccessToken.Condition))
				{
					this.AccessToken = null;
					result = RoutedMailItem.PrepareForDeliveryResult.Requeue;
				}
				else
				{
					this.solution.DeliveryStatus = DeliveryStatus.InDelivery;
					result = RoutedMailItem.PrepareForDeliveryResult.Deliver;
				}
			}
			return result;
		}

		public bool PrepareForSuspension()
		{
			lock (this.mailItem)
			{
				if (this.solution.AdminActionStatus == AdminActionStatus.Suspended)
				{
					this.solution.DeliveryStatus = DeliveryStatus.DequeuedAndDeferred;
					((IQueueItem)this).DeferUntil = DateTime.MaxValue;
					return true;
				}
			}
			return false;
		}

		public void InitializeDeliveryLatencyTracking()
		{
			LatencyTracker.EndAndBeginTrackLatency(LatencyTracker.GetDeliveryQueueLatencyComponent(this.DeliveryType), LatencyComponent.Delivery, this.latencyTracker);
		}

		public void FinalizeDeliveryLatencyTracking(LatencyComponent deliveryComponent)
		{
			LatencyTracker.EndTrackLatency(LatencyComponent.Delivery, deliveryComponent, this.latencyTracker);
		}

		public void TrackSuccessfulConnectLatency(LatencyComponent connectComponent)
		{
			LatencyTracker.EndTrackLatency(LatencyComponent.Delivery, connectComponent, this.latencyTracker);
			LatencyTracker.BeginTrackLatency(LatencyComponent.Delivery, this.latencyTracker);
		}

		public void AddDsnParameters(string key, object value)
		{
			if (this.dsnParameters == null)
			{
				this.dsnParameters = new DsnParameters();
			}
			this.dsnParameters[key] = value;
		}

		public void IncrementPoisonForRemoteCount()
		{
			this.poisonForRemoteCount++;
		}

		public bool ShouldDequeueForResubmit()
		{
			lock (this.mailItem)
			{
				if (this.IsDeletedByAdmin)
				{
					return true;
				}
				if (this.solution.AdminActionStatus == AdminActionStatus.Suspended)
				{
					return false;
				}
				this.solution.DeliveryStatus = DeliveryStatus.PendingResubmit;
			}
			return true;
		}

		public bool Resubmit(DeferReason resubmitDeferReason, TimeSpan? resubmitDeferInterval, bool routeForHighAvailability, Action<TransportMailItem> updateBeforeResubmit = null)
		{
			return this.Resubmit(this.solution.Recipients.ToArray(), resubmitDeferReason, resubmitDeferInterval, routeForHighAvailability, updateBeforeResubmit);
		}

		private bool Resubmit(IList<MailRecipient> recipientsToResubmit, DeferReason resubmitDeferReason, TimeSpan? resubmitDeferInterval, bool routeForHighAvailability, Action<TransportMailItem> updateBeforeResubmit)
		{
			if (recipientsToResubmit == null || recipientsToResubmit.Count == 0)
			{
				throw new ArgumentException("recipientsToResubmit");
			}
			foreach (MailRecipient mailRecipient in recipientsToResubmit)
			{
				if (this.solution.Recipients.IndexOf(mailRecipient) == -1)
				{
					throw new ArgumentException(string.Format("recipient <{0}> is not in the solution recipient list", mailRecipient.Email.ToString()));
				}
			}
			TransportMailItem transportMailItem = null;
			lock (this.mailItem)
			{
				if (!this.CanBeResubmitted())
				{
					return false;
				}
				if (recipientsToResubmit.Count != this.solution.Recipients.Count || this.NeedForkBeforeResubmit())
				{
					transportMailItem = this.CreateForkedMailItem(recipientsToResubmit);
					MessageTrackingLog.TrackTransfer(MessageTrackingSource.QUEUE, transportMailItem, this.RecordId, "Forking for resubmit");
				}
				else
				{
					Dictionary<NextHopSolutionKey, NextHopSolution> nextHopSolutions = new Dictionary<NextHopSolutionKey, NextHopSolution>();
					this.mailItem.NextHopSolutions = nextHopSolutions;
					Components.RemoteDeliveryComponent.ReleaseMailItem(this.mailItem);
					transportMailItem = this.mailItem;
				}
				transportMailItem.RouteForHighAvailability = routeForHighAvailability;
				SystemProbeHelper.SmtpSendTracer.TracePass(this.mailItem, 0L, "message resubmitting in Delivery");
				MessageTrackingLog.TrackResubmit(MessageTrackingSource.QUEUE, transportMailItem, this.mailItem, (resubmitDeferReason != DeferReason.None) ? resubmitDeferReason.ToString() : "resubmitting in Delivery");
				this.PrepareRecipientsForResubmit(recipientsToResubmit);
				Resolver.ClearResolverAndTransportSettings(transportMailItem);
				this.solution.DeliveryStatus = DeliveryStatus.Complete;
				if (updateBeforeResubmit != null)
				{
					updateBeforeResubmit(transportMailItem);
				}
			}
			this.TransferLatencyAndLockInformation(transportMailItem);
			if (resubmitDeferInterval != null && resubmitDeferInterval.Value > TimeSpan.Zero)
			{
				transportMailItem.DeferReason = resubmitDeferReason;
				transportMailItem.DeferUntil = DateTime.UtcNow.Add(resubmitDeferInterval.Value);
				LatencyTracker.BeginTrackLatency(TransportMailItem.GetDeferLatencyComponent(resubmitDeferReason), transportMailItem.LatencyTracker);
			}
			Components.CategorizerComponent.EnqueueSubmittedMessage(transportMailItem);
			return true;
		}

		private bool CanBeResubmitted()
		{
			lock (this.mailItem)
			{
				if (this.IsDeletedByAdmin)
				{
					return false;
				}
				if (this.IsHeartbeat)
				{
					this.AbortHeartbeat();
					return false;
				}
			}
			return true;
		}

		public void Poison()
		{
			TransportMailItem transportMailItem = null;
			lock (this.mailItem)
			{
				foreach (MailRecipient mailRecipient in this.Recipients)
				{
					mailRecipient.UpdateForPoisonMessage();
				}
				if (this.NeedForkBeforePoisoning())
				{
					transportMailItem = this.CreateForkedMailItem(this.solution.Recipients.ToArray());
					MessageTrackingLog.TrackTransfer(MessageTrackingSource.QUEUE, transportMailItem, this.RecordId, "Forking for poison");
				}
				else
				{
					transportMailItem = this.mailItem;
					Dictionary<NextHopSolutionKey, NextHopSolution> nextHopSolutions = new Dictionary<NextHopSolutionKey, NextHopSolution>();
					this.mailItem.NextHopSolutions = nextHopSolutions;
					Components.RemoteDeliveryComponent.ReleaseMailItem(this.mailItem);
				}
				this.solution.DeliveryStatus = DeliveryStatus.Complete;
			}
			this.TransferLatencyAndLockInformation(transportMailItem);
			transportMailItem.PoisonCount = int.MaxValue;
			if (Components.MessageDepotComponent.Enabled)
			{
				MessageDepotMailItem item = new MessageDepotMailItem(transportMailItem);
				Components.MessageDepotComponent.MessageDepot.Add(item);
			}
			else
			{
				Components.QueueManager.PoisonMessageQueue.Enqueue(transportMailItem);
			}
			MessageTrackingLog.TrackPoisonMessage(MessageTrackingSource.QUEUE, transportMailItem);
		}

		public Stream OpenMimeReadStream(bool downConvert)
		{
			return this.mailItem.OpenMimeReadStream(downConvert);
		}

		private static void MarkForDelayDsn(MailRecipient recipient)
		{
			if (recipient.IsDelayDsnNeeded)
			{
				recipient.DsnNeeded = DsnFlags.Delay;
				recipient.SmtpResponse = SmtpResponse.MessageDelayed;
			}
		}

		internal void UpdateE2ELatencyBucketsOnEnqueue()
		{
			this.UpdateE2ELatencyBuckets(null);
		}

		private void UpdateE2ELatencyBucketsOnAck(Queue<AckStatusAndResponse> recipientResponses)
		{
			this.UpdateE2ELatencyBuckets(recipientResponses);
		}

		private void UpdateE2ELatencyBuckets(Queue<AckStatusAndResponse> recipientResponses)
		{
			if (this.externalDeliveryEnqueueTime != DateTime.MinValue || this.Recipients.Count == 0)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = utcNow - this.LatencyStartTime;
			if (timeSpan < TimeSpan.Zero)
			{
				return;
			}
			if (recipientResponses == null)
			{
				if (TransportDeliveryTypes.externalDeliveryTypes.Contains(this.DeliveryType))
				{
					Components.RemoteDeliveryComponent.GetEndToEndLatencyBuckets().RecordExternalSendLatency(this.Priority, timeSpan, this.Recipients.Count);
					this.externalDeliveryEnqueueTime = utcNow;
					return;
				}
			}
			else if (this.DeliveryType == DeliveryType.SmtpDeliveryToMailbox)
			{
				int num = (from r in recipientResponses
				where r.AckStatus == AckStatus.Success || r.AckStatus == AckStatus.Fail
				select r).Count<AckStatusAndResponse>();
				if (num > 0)
				{
					Components.RemoteDeliveryComponent.GetEndToEndLatencyBuckets().RecordMailboxDeliveryLatency(this.Priority, timeSpan, num);
				}
			}
		}

		private static bool AllRecipientLimitRetries(Queue<AckStatusAndResponse> recipientResponses)
		{
			bool result = false;
			if (recipientResponses != null)
			{
				if (recipientResponses.Any((AckStatusAndResponse r) => r.AckStatus == AckStatus.Retry))
				{
					result = (from r in recipientResponses
					where r.AckStatus == AckStatus.Retry
					select r).All((AckStatusAndResponse r) => r.SmtpResponse.StatusText != null && r.SmtpResponse.StatusText.Length > 0 && r.SmtpResponse.StatusText[0].Equals(AckReason.RecipientThreadLimitExceeded.StatusText[0]));
				}
			}
			return result;
		}

		private void UpdateDeliveryQueueMailboxLatency(TimeSpan latency, SmtpResponse smtpResponse, bool allRecipientLimitRetries)
		{
			LatencyComponent latencyComponent = LatencyComponent.None;
			if (allRecipientLimitRetries)
			{
				latencyComponent = LatencyComponent.DeliveryQueueMailboxRecipientThreadLimitExceeded;
			}
			else
			{
				string text = smtpResponse.ToString();
				foreach (KeyValuePair<string, LatencyComponent> keyValuePair in RoutedMailItem.smtpResponseToLatencyComponentMap)
				{
					if (text.StartsWith(keyValuePair.Key, StringComparison.InvariantCultureIgnoreCase))
					{
						latencyComponent = keyValuePair.Value;
						break;
					}
				}
			}
			if (latencyComponent != LatencyComponent.None)
			{
				this.deliveryQueueMailboxSubComponent = latencyComponent;
				this.deliveryQueueMailboxSubComponentLatency += latency;
			}
		}

		private void Track(AckStatus ackStatus, SmtpResponse smtpResponse, IEnumerable<MailRecipient> recipientsToTrack, MessageTrackingSource messageTrackingSource, string messageTrackingSourceContext, AckDetails ackDetails, bool reportEndToEndLatencies, bool willRetry)
		{
			LatencyFormatter latencyFormatter = null;
			if (!willRetry)
			{
				latencyFormatter = new LatencyFormatter(this, Components.Configuration.LocalServer.TransportServer.Fqdn, reportEndToEndLatencies, this.externalDeliveryEnqueueTime, this.LatencyStartTime);
			}
			MessageTrackingLog.TrackRelayedAndFailed(messageTrackingSource, messageTrackingSourceContext, this, recipientsToTrack, ackDetails, smtpResponse, latencyFormatter);
			if (willRetry)
			{
				IEnumerable<MailRecipient> enumerable = (from r in recipientsToTrack
				where r.AckStatus == AckStatus.Retry
				select r).ToArray<MailRecipient>();
				SystemProbeHelper.SmtpSendTracer.TracePass<SmtpResponse>(this, 0L, "going into retry with smtp response {0}", smtpResponse);
				MessageTrackingLog.TrackDefer(messageTrackingSource, messageTrackingSourceContext, this, enumerable, ackDetails);
				if (messageTrackingSource == MessageTrackingSource.SMTP)
				{
					foreach (MailRecipient mailRecipient in enumerable)
					{
						mailRecipient.SetSmtpDeferLogged();
					}
				}
				this.RetryDeferLogged = true;
			}
		}

		public void TrackDeferRetryOrSuspended(SmtpResponse smtpResponse, AckDetails ackDetails)
		{
			this.RetryDeferLogged = true;
			this.InternalTrackDefer(smtpResponse, null, ackDetails, null);
		}

		public void TrackDeferDelay(SmtpResponse smtpResponse, AckDetails ackDetails, string sourceContext)
		{
			this.DelayDeferLogged = true;
			this.InternalTrackDefer(smtpResponse, sourceContext, ackDetails, null);
		}

		public bool TrackDeferIfRecipientsInRetry(SmtpResponse smtpResponse, AckDetails ackDetails)
		{
			MailRecipient mailRecipient = null;
			foreach (MailRecipient mailRecipient2 in this.Recipients)
			{
				if (mailRecipient2.Status == Status.Retry)
				{
					mailRecipient = mailRecipient2;
					break;
				}
			}
			if (mailRecipient == null)
			{
				return false;
			}
			List<MailRecipient> list = new List<MailRecipient>();
			foreach (MailRecipient mailRecipient3 in this.Recipients)
			{
				if (!mailRecipient3.IsProcessed)
				{
					list.Add(mailRecipient3);
				}
			}
			this.InternalTrackDefer(mailRecipient.SmtpResponse.Equals(SmtpResponse.Empty) ? smtpResponse : mailRecipient.SmtpResponse, null, ackDetails, list);
			this.RetryDeferLogged = true;
			return true;
		}

		public WaitCondition GetCondition()
		{
			if (this.CurrentCondition == null)
			{
				if (Components.TransportAppConfig.ThrottlingConfig.DeliveryTenantThrottlingEnabled)
				{
					Guid tenantId = TransportMailItemWrapper.GetTenantId(this);
					this.CurrentCondition = new TenantBasedCondition(tenantId);
				}
				else if (Components.TransportAppConfig.ThrottlingConfig.DeliverySenderThrottlingEnabled)
				{
					this.CurrentCondition = new SenderBasedCondition(TransportMailItem.GetSourceID(this.mailItem));
				}
			}
			return this.CurrentCondition;
		}

		public long GetCurrentMimeSize()
		{
			return this.mailItem.GetCurrrentMimeSize();
		}

		private void InternalTrackDefer(SmtpResponse smtpResponse, string sourceContext, AckDetails ackDetails, List<MailRecipient> retryRecipients)
		{
			if (retryRecipients != null)
			{
				using (List<MailRecipient>.Enumerator enumerator = retryRecipients.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MailRecipient mailRecipient = enumerator.Current;
						mailRecipient.SmtpResponse = smtpResponse;
					}
					goto IL_6E;
				}
			}
			foreach (MailRecipient mailRecipient2 in this.Recipients)
			{
				if (!mailRecipient2.IsProcessed)
				{
					mailRecipient2.SmtpResponse = smtpResponse;
				}
			}
			IL_6E:
			SystemProbeHelper.SmtpSendTracer.TracePass<SmtpResponse>(this, 0L, "going into retry with smtp response {0}", smtpResponse);
			MessageTrackingLog.TrackDefer(MessageTrackingSource.QUEUE, sourceContext, this, retryRecipients, ackDetails);
		}

		private void Commit()
		{
			if (this.mailItem.Status == Status.Complete)
			{
				this.mailItem.ReleaseFromRemoteDelivery();
				Components.RemoteDeliveryComponent.ReleaseMailItem(this.mailItem);
				return;
			}
			this.solution.CheckAdminAction();
			this.mailItem.CommitLazy(this.solution);
		}

		private bool InternalProcessDeferral()
		{
			if (this.IsDeletedByAdmin)
			{
				return false;
			}
			this.GenerateDelayDsnIfNeeded();
			this.mailItem.CommitLazyAndDehydrateMessageIfPossible(Breadcrumb.DehydrateOnRoutedMailItemDeferral);
			return true;
		}

		private void GenerateDelayDsnIfNeeded()
		{
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.Transport.DelayDsn.Enabled)
			{
				return;
			}
			if (this.DelayNotificationTime < DateTime.UtcNow && this.solution.DeliveryStatus == DeliveryStatus.Enqueued)
			{
				this.solution.Recipients.ForEach(new Action<MailRecipient>(RoutedMailItem.MarkForDelayDsn));
				LastError lastError = new LastError((this.lastQueueLevelError != null) ? this.lastQueueLevelError.LastAttemptFqdn : string.Empty, (this.lastQueueLevelError != null) ? this.lastQueueLevelError.LastAttemptEndpoint : null, null, SmtpResponse.MessageDelayed, this.lastQueueLevelError);
				Components.DsnGenerator.GenerateDSNs(this.mailItem, this.solution.Recipients, lastError);
			}
		}

		private void PrepareRecipientsForResubmit(IEnumerable<MailRecipient> recipients)
		{
			foreach (MailRecipient mailRecipient in recipients)
			{
				Resolver.ClearResolverProperties(mailRecipient);
				mailRecipient.SmtpResponse = SmtpResponse.Empty;
				mailRecipient.NextHop = NextHopSolutionKey.Empty;
				mailRecipient.UnreachableReason = UnreachableReason.None;
				mailRecipient.Type = MailRecipientType.Unknown;
				mailRecipient.FinalDestination = string.Empty;
				mailRecipient.RetryCount = 0;
				mailRecipient.ResetRoutingOverride();
				mailRecipient.ResetTlsDomain();
				if (mailRecipient.Status == Status.Retry)
				{
					mailRecipient.Status = Status.Ready;
				}
			}
		}

		private bool NeedForkBeforeResubmit()
		{
			Dictionary<NextHopSolutionKey, NextHopSolution> nextHopSolutions = this.mailItem.NextHopSolutions;
			if (nextHopSolutions.Count > 1)
			{
				foreach (NextHopSolution nextHopSolution in nextHopSolutions.Values)
				{
					if (nextHopSolution != this.solution && nextHopSolution.DeliveryStatus != DeliveryStatus.Complete)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private bool NeedForkBeforePoisoning()
		{
			return this.NeedForkBeforeResubmit();
		}

		private TransportMailItem CreateForkedMailItem(IList<MailRecipient> recipientsToFork)
		{
			LatencyTracker latencyTrackerToClone = (this.latencyTracker == null) ? this.mailItem.LatencyTracker : null;
			TransportMailItem transportMailItem = this.mailItem.NewCloneWithoutRecipients(false, latencyTrackerToClone, this.solution);
			foreach (MailRecipient mailRecipient in recipientsToFork)
			{
				mailRecipient.MoveTo(transportMailItem);
				this.solution.Recipients.Remove(mailRecipient);
			}
			transportMailItem.CommitLazy();
			return transportMailItem;
		}

		private void TransferLatencyAndLockInformation(TransportMailItem mailItem)
		{
			if (this.latencyTracker != null)
			{
				mailItem.LatencyTracker = LatencyTracker.Clone(this.latencyTracker);
			}
			if (this.LockReasonHistory != null)
			{
				mailItem.LockReasonHistory = this.LockReasonHistory;
			}
		}

		internal bool UpdateProperties(ExtensibleMessageInfo properties, bool resubmit)
		{
			if (resubmit && properties.OutboundIPPool > 0 && properties.OutboundIPPool != this.solution.GetOutboundIPPool())
			{
				this.Resubmit(this.solution.Recipients.ToArray(), DeferReason.None, null, false, delegate(TransportMailItem mailItemToSubmit)
				{
					foreach (MailRecipient mailRecipient in mailItemToSubmit.Recipients)
					{
						mailRecipient.OutboundIPPool = properties.OutboundIPPool;
					}
					this.solution.AdminActionStatus = AdminActionStatus.None;
				});
				return true;
			}
			return false;
		}

		private static readonly Dictionary<string, LatencyComponent> smtpResponseToLatencyComponentMap = new Dictionary<string, LatencyComponent>
		{
			{
				AckReason.DeliverAgentTransientFailure.ToString(),
				LatencyComponent.DeliveryQueueMailboxDeliverAgentTransientFailure
			},
			{
				AckReason.DynamicMailboxDatabaseThrottlingLimitExceeded.ToString(),
				LatencyComponent.DeliveryQueueMailboxDynamicMailboxDatabaseThrottlingLimitExceeded
			},
			{
				SmtpResponse.InsufficientResource.ToString(),
				LatencyComponent.DeliveryQueueMailboxInsufficientResources
			},
			{
				AckReason.MDBOffline.ToString(),
				LatencyComponent.DeliveryQueueMailboxMailboxDatabaseOffline
			},
			{
				AckReason.MailboxServerOffline.ToString(),
				LatencyComponent.DeliveryQueueMailboxMailboxServerOffline
			},
			{
				"432 4.2.0 STOREDRV.Deliver.Exception:StorageTransientException.MapiExceptionLockViolation",
				LatencyComponent.DeliveryQueueMailboxMapiExceptionLockViolation
			},
			{
				"432 4.2.0 STOREDRV.Deliver.Exception:StorageTransientException.MapiExceptionTimeout",
				LatencyComponent.DeliveryQueueMailboxMapiExceptionTimeout
			},
			{
				AckReason.MaxConcurrentMessageSizeLimitExceeded.ToString(),
				LatencyComponent.DeliveryQueueMailboxMaxConcurrentMessageSizeLimitExceeded
			}
		};

		private TransportMailItem mailItem;

		private NextHopSolution solution;

		private LatencyTracker latencyTracker;

		private DsnParameters dsnParameters;

		private DateTime enqueuedTime;

		private DateTime originalEnqueuedTime;

		private DeferReason deferReason;

		private bool delayDeferLogged;

		private bool retryDeferLogged;

		private bool retriedDueToDeliverySourceLimit;

		private bool suppressBodyInDsn;

		private List<string> lockReasonHistory;

		private ThrottlingContext throttlingContext;

		private int poisonForRemoteCount;

		private LastError lastQueueLevelError;

		private QueuedRecipientsByAgeToken queuedRecipientsByAgeToken;

		private DateTime externalDeliveryEnqueueTime = DateTime.MinValue;

		private int numConsecutiveRetries;

		private LatencyComponent deliveryQueueMailboxSubComponent;

		private TimeSpan deliveryQueueMailboxSubComponentLatency = TimeSpan.Zero;

		private ExtendedPropertyDictionary extendedPropertyDictionaryCopy;

		public enum PrepareForDeliveryResult
		{
			Deliver,
			IgnoreDeleted,
			Requeue
		}
	}
}
