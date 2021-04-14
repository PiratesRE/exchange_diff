using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Common;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.RemoteDelivery;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Exchange.Transport.Storage.Messaging;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal class ShadowRedundancyManager : IShadowRedundancyManager, IShadowRedundancyManagerFacade
	{
		public ShadowRedundancyManager(IShadowRedundancyConfigurationSource configurationSource, IShadowRedundancyPerformanceCounters perfCounters, ShadowRedundancyEventLogger shadowRedundancyEventLogger, IMessagingDatabase database)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "Constructor");
			if (configurationSource == null)
			{
				throw new ArgumentNullException("configurationSource");
			}
			if (perfCounters == null)
			{
				throw new ArgumentNullException("perfCounter");
			}
			if (shadowRedundancyEventLogger == null)
			{
				throw new ArgumentNullException("shadowRedundancyEventLogger");
			}
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			this.serverContextStringPool = new StringPool(StringComparer.OrdinalIgnoreCase);
			this.shadowRedundancyEventLogger = shadowRedundancyEventLogger;
			this.configurationSource = configurationSource;
			this.database = database;
			this.primaryServerInfoMap = new PrimaryServerInfoMap(shadowRedundancyEventLogger, this.database.ServerInfoTable, true);
			configurationSource.Load();
			configurationSource.SetShadowRedundancyConfigChangeNotification(new ShadowRedundancyConfigChange(this.NotifyConfigUpdated));
			ShadowRedundancyManager.shadowRedundancyPerformanceCounters = perfCounters;
			ShadowRedundancyManager.PrimaryServerInfoScanner primaryServerInfoScanner = new ShadowRedundancyManager.PrimaryServerInfoScanner(this.database);
			primaryServerInfoScanner.Load(this.primaryServerInfoMap);
			DateTime utcNow = DateTime.UtcNow;
			IEnumerable<PrimaryServerInfo> enumerable = null;
			lock (this.syncQueues)
			{
				enumerable = this.primaryServerInfoMap.RemoveExpiredServers(utcNow);
			}
			if (enumerable != null)
			{
				PrimaryServerInfo.CommitLazy(enumerable, this.database.ServerInfoTable);
			}
			if (this.primaryServerInfoMap.Count > 0)
			{
				PrimaryServerInfo active = this.primaryServerInfoMap.GetActive("$localhost$");
				if (active == null)
				{
					throw new InvalidOperationException("ShadowRedundancyManager: The current server database state Guid is not saved in database.");
				}
				if (!GuidHelper.TryParseGuid(active.DatabaseState, out this.databaseId))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "ShadowRedundancyManager: The current server database state '{0}' is not a valid Guid.", new object[]
					{
						active.DatabaseState
					}));
				}
				this.primaryServerInfoMap.Remove(active);
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string>(0L, "Database Id (as loaded from database) = '{0}'.", active.DatabaseState);
			}
			else
			{
				this.databaseId = Guid.NewGuid();
				PrimaryServerInfo primaryServerInfo = new PrimaryServerInfo(this.database.ServerInfoTable);
				primaryServerInfo.ServerFqdn = "$localhost$";
				primaryServerInfo.DatabaseState = this.databaseId.ToString();
				primaryServerInfo.StartTime = utcNow;
				primaryServerInfo.Version = this.Configuration.CompatibilityVersion;
				primaryServerInfo.Commit(TransactionCommitMode.MediumLatencyLazy);
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string>(0L, "DatabaseId (just created) = '{0}'.", primaryServerInfo.DatabaseState);
			}
			this.DatabaseIdForTransmit = Convert.ToBase64String(this.databaseId.ToByteArray());
			this.shadowSessionFactory = new ShadowSessionFactory(this);
			this.delayedAckCleanupTimer = new GuardedTimer(new TimerCallback(this.DelayedAckCleanupCallback), null, (long)this.configurationSource.DelayedAckCheckExpiryInterval.TotalMilliseconds, (long)this.configurationSource.DelayedAckCheckExpiryInterval.TotalMilliseconds);
		}

		public static IShadowRedundancyPerformanceCounters PerfCounters
		{
			get
			{
				return ShadowRedundancyManager.shadowRedundancyPerformanceCounters;
			}
		}

		public Guid DatabaseId
		{
			get
			{
				return this.databaseId;
			}
		}

		public string DatabaseIdForTransmit { get; private set; }

		public IShadowRedundancyConfigurationSource Configuration
		{
			get
			{
				return this.configurationSource;
			}
		}

		public object SyncQueues
		{
			get
			{
				return this.syncQueues;
			}
		}

		public IPrimaryServerInfoMap PrimaryServerInfos
		{
			get
			{
				return this.primaryServerInfoMap;
			}
		}

		public ShadowRedundancyEventLogger EventLogger
		{
			get
			{
				return this.shadowRedundancyEventLogger;
			}
		}

		private bool HeartbeatEnabled
		{
			get
			{
				return this.bootLoaderComplete && this.heartbeatEnabled && this.discardEventsToApplyAfterBootLoader == null;
			}
		}

		public static Guid GetShadowNextHopConnectorId(NextHopSolutionKey key)
		{
			return key.NextHopConnector;
		}

		public static bool IsShadowableDeliveryType(DeliveryType deliveryType)
		{
			return deliveryType == DeliveryType.DnsConnectorDelivery || deliveryType == DeliveryType.SmartHostConnectorDelivery || deliveryType == DeliveryType.SmtpRelayToDag || deliveryType == DeliveryType.SmtpRelayToMailboxDeliveryGroup || deliveryType == DeliveryType.SmtpRelayToConnectorSourceServers || deliveryType == DeliveryType.SmtpRelayToServers || deliveryType == DeliveryType.SmtpRelayToRemoteAdSite || deliveryType == DeliveryType.SmtpRelayWithinAdSite || deliveryType == DeliveryType.SmtpRelayWithinAdSiteToEdge || deliveryType == DeliveryType.Heartbeat;
		}

		public static void PrepareRecipientForShadowing(MailRecipient mailRecipient, string serverFqdn)
		{
			if (mailRecipient == null)
			{
				throw new ArgumentNullException("mailRecipient");
			}
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			NextHopSolutionKey nextHopSolutionKey = NextHopSolutionKey.CreateShadowNextHopSolutionKey(serverFqdn, Guid.Empty, null);
			mailRecipient.PrimaryServerFqdnGuid = new ShadowRedundancyManager.QualifiedPrimaryServerId(nextHopSolutionKey).ToString();
			mailRecipient.NextHop = nextHopSolutionKey;
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			if (initiallyPaused)
			{
				this.paused = true;
			}
			this.targetRunningState = targetRunningState;
			if (targetRunningState != ServiceState.Inactive)
			{
				this.heartbeatEnabled = true;
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "Heartbeats enabled");
				return;
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "Heartbeats not enabled as the service state is inactive");
		}

		public void Stop()
		{
			this.heartbeatEnabled = false;
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "Heartbeats disabled");
		}

		public void Pause()
		{
			this.paused = true;
		}

		public void Continue()
		{
			this.paused = false;
		}

		public TransportMailItem MoveUndeliveredRecipientsToNewClone(TransportMailItem mailItem)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (mailItem.NextHopSolutions.Count > 0)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Mail Item '{0}' with shadow message id '{1}' received with '{2}' next hop solutions.  This method is designed to be called from Boot Loader where there should be no NextHopSolutions", new object[]
				{
					mailItem.RecordId,
					mailItem.ShadowMessageId,
					mailItem.NextHopSolutions.Count
				}));
			}
			List<MailRecipient> list = (from r in mailItem.Recipients
			where string.IsNullOrEmpty(r.PrimaryServerFqdnGuid)
			select r).ToList<MailRecipient>();
			if (list.Count == 0)
			{
				return null;
			}
			TransportMailItem transportMailItem = mailItem.NewCloneWithoutRecipients();
			foreach (MailRecipient mailRecipient in list)
			{
				mailRecipient.MoveTo(transportMailItem);
			}
			transportMailItem.CommitLazy();
			return transportMailItem;
		}

		public void AddDiagnosticInfoTo(XElement componentElement, bool showBasic, bool showVerbose)
		{
			showBasic = (showBasic || showVerbose);
			componentElement.Add(new XElement("Configuration", new object[]
			{
				new XElement("delayedAckCheckExpiryInterval", this.configurationSource.DelayedAckCheckExpiryInterval),
				new XElement("delayedAckSkippingEnabled", this.configurationSource.DelayedAckSkippingEnabled),
				new XElement("delayedAckSkippingQueueLength", this.configurationSource.DelayedAckSkippingQueueLength),
				new XElement("discardEventExpireInterval", this.configurationSource.DiscardEventExpireInterval),
				new XElement("discardEventsCheckExpiryInterval", this.configurationSource.DiscardEventsCheckExpiryInterval),
				new XElement("enabled", this.configurationSource.Enabled),
				new XElement("compatibilityMode", this.configurationSource.CompatibilityVersion),
				new XElement("shadowMessagePreference", this.configurationSource.ShadowMessagePreference),
				new XElement("maxRemoteShadowAttempts", this.configurationSource.MaxRemoteShadowAttempts),
				new XElement("maxLocalShadowAttempts", this.configurationSource.MaxLocalShadowAttempts),
				new XElement("rejectMessageOnShadowFailure", this.configurationSource.RejectMessageOnShadowFailure),
				new XElement("heartbeatRetryCount", this.configurationSource.HeartbeatRetryCount),
				new XElement("heartbeatFrequency", this.configurationSource.HeartbeatFrequency),
				new XElement("primaryServerInfoCleanupInterval", this.configurationSource.PrimaryServerInfoCleanupInterval),
				new XElement("queueMaxIdleTimeInterval", this.configurationSource.QueueMaxIdleTimeInterval),
				new XElement("shadowMessageAutoDiscardInterval", this.configurationSource.ShadowMessageAutoDiscardInterval),
				new XElement("shadowQueueCheckExpiryInterval", this.configurationSource.ShadowQueueCheckExpiryInterval),
				new XElement("shadowServerInfoMaxIdleTimeInterval", this.configurationSource.ShadowServerInfoMaxIdleTimeInterval),
				new XElement("stringPoolCleanupInterval", this.configurationSource.StringPoolCleanupInterval)
			}));
			if (showBasic)
			{
				Dictionary<ShadowRedundancyManager.QualifiedPrimaryServerId, List<Guid>> dictionary = this.discardEventsToApplyAfterBootLoader;
				componentElement.Add(new object[]
				{
					new XElement("databaseStateId", this.databaseId),
					new XElement("bootLoaderComplete", this.bootLoaderComplete),
					new XElement("heartbeatEnabled", this.heartbeatEnabled),
					new XElement("shuttingDown", this.shuttingDown),
					new XElement("activeResubmits", this.activeResubmits),
					new XElement("lastPrimaryServerInfoMapCleanup", this.lastPrimaryServerInfoMapCleanup),
					new XElement("lastDelayedCleanupCompleteTime", this.lastDelayedCleanupCompleteTime),
					new XElement("primaryServerInfoMapCleanupGuard", this.primaryServerInfoMapCleanupGuard),
					new XElement("discardEventsToApplyAfterBootLoader", (dictionary == null) ? 0 : dictionary.Count),
					new XElement("queuedRecoveryResubmits", this.queuedRecoveryResubmits.Count)
				});
				lock (this.delayedAckEntries)
				{
					XElement xelement = new XElement("DelayedAckEntryCollection", new XElement("count", this.delayedAckEntries.Count));
					if (showVerbose)
					{
						using (Dictionary<Guid, ShadowRedundancyManager.DelayedAckEntry>.Enumerator enumerator = this.delayedAckEntries.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<Guid, ShadowRedundancyManager.DelayedAckEntry> keyValuePair = enumerator.Current;
								xelement.Add(new XElement("DelayedAck", new object[]
								{
									new XElement("shadowMessageId", keyValuePair.Key),
									new XElement("context", keyValuePair.Value.Context),
									new XElement("startTime", keyValuePair.Value.StartTime),
									new XElement("completionStatus", keyValuePair.Value.CompletionStatus),
									new XElement("state", keyValuePair.Value.State)
								}));
							}
							goto IL_57E;
						}
					}
					xelement.Add(new XElement("help", "Use 'verbose' argument to get the delayed ack entries."));
					IL_57E:
					componentElement.Add(xelement);
				}
				this.shadowServerInfoMapReaderWriterLock.EnterReadLock();
				try
				{
					XElement xelement2 = new XElement("ShadowServerCollection", new XElement("count", this.shadowServerInfoMap.Count));
					if (showVerbose || showBasic)
					{
						using (Dictionary<string, ShadowRedundancyManager.ShadowServerInfo>.Enumerator enumerator2 = this.shadowServerInfoMap.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								KeyValuePair<string, ShadowRedundancyManager.ShadowServerInfo> keyValuePair2 = enumerator2.Current;
								xelement2.Add(new XElement("ShadowServer", new object[]
								{
									new XElement("context", keyValuePair2.Key),
									keyValuePair2.Value.GetDiagnosticInfo(showVerbose)
								}));
							}
							goto IL_668;
						}
					}
					xelement2.Add(new XElement("help", "Use 'verbose' argument to get the delayed ack entries."));
					IL_668:
					componentElement.Add(xelement2);
				}
				finally
				{
					this.shadowServerInfoMapReaderWriterLock.ExitReadLock();
				}
				lock (this.syncAllMailItems)
				{
					XElement xelement3 = new XElement("ShadowMailItemsCollection", new XElement("count", this.allMailItemsByRecordId.Count));
					if (showVerbose)
					{
						using (Dictionary<long, TransportMailItem>.Enumerator enumerator3 = this.allMailItemsByRecordId.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								KeyValuePair<long, TransportMailItem> keyValuePair3 = enumerator3.Current;
								lock (keyValuePair3.Value)
								{
									xelement3.Add(new XElement("ShadowedTransportMailItem", new object[]
									{
										new XElement("internalId", keyValuePair3.Key),
										new XElement("internalMessageId", keyValuePair3.Value.InternetMessageId),
										new XElement("shadowMessageId", keyValuePair3.Value.ShadowMessageId),
										new XElement("nextHopSolutionsCount", (keyValuePair3.Value.NextHopSolutions == null) ? "null" : keyValuePair3.Value.NextHopSolutions.Count.ToString(CultureInfo.InvariantCulture)),
										new XElement("heloDomain", keyValuePair3.Value.HeloDomain),
										new XElement("dateReceived", keyValuePair3.Value.DateReceived),
										new XElement("status", keyValuePair3.Value.Status)
									}));
								}
							}
							goto IL_86A;
						}
					}
					xelement3.Add(new XElement("help", "Use 'verbose' argument to get the shadow mail item entries."));
					IL_86A:
					componentElement.Add(xelement3);
				}
				lock (this.completedResubmitEntries)
				{
					XElement xelement4 = new XElement("CompletedServerResubmitEvents", new XElement("count", this.completedResubmitEntries.Count));
					if (showVerbose)
					{
						using (Queue<ShadowRedundancyManager.CompletedResubmitEntry>.Enumerator enumerator4 = this.completedResubmitEntries.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								ShadowRedundancyManager.CompletedResubmitEntry completedResubmitEntry = enumerator4.Current;
								xelement4.Add(new XElement("ResubmitEvent", new object[]
								{
									new XElement("timestamp", completedResubmitEntry.Timestamp),
									new XElement("primaryServerFqdn", completedResubmitEntry.PrimaryServerFqdn),
									new XElement("reason", completedResubmitEntry.Reason),
									new XElement("messageCount", completedResubmitEntry.MessageCount)
								}));
							}
							goto IL_9B2;
						}
					}
					xelement4.Add(new XElement("help", "Use 'verbose' argument to get the completed server resubmit events."));
					IL_9B2:
					componentElement.Add(xelement4);
				}
				ShadowMessageQueue[] queueArray = this.GetQueueArray();
				XElement xelement5 = new XElement("ShadowQueues", new XElement("count", queueArray.Length));
				foreach (ShadowMessageQueue shadowMessageQueue in queueArray)
				{
					xelement5.Add(new XElement("ShadowQueue", new object[]
					{
						new XElement("Id", shadowMessageQueue.Id),
						new XElement("Key", shadowMessageQueue.Key.ToString()),
						new XElement("NextHopDomain", shadowMessageQueue.NextHopDomain),
						new XElement("IsEmpty", shadowMessageQueue.IsEmpty),
						new XElement("Suspended", shadowMessageQueue.Suspended),
						new XElement("HasHeartbeatFailure", shadowMessageQueue.HasHeartbeatFailure),
						new XElement("IsResubmissionSuppressed", shadowMessageQueue.IsResubmissionSuppressed),
						new XElement("Count", shadowMessageQueue.Count),
						new XElement("LastHeartbeatTime", shadowMessageQueue.LastHeartbeatTime),
						new XElement("LastExpiryCheck", shadowMessageQueue.LastExpiryCheck),
						new XElement("ValidDiscardIdCount", shadowMessageQueue.ValidDiscardIdCount),
						new XElement("IgnoredDiscardIdCount", shadowMessageQueue.IgnoredDiscardIdCount)
					}));
				}
				componentElement.Add(xelement5);
			}
		}

		public bool IsVersion(ShadowRedundancyCompatibilityVersion version)
		{
			return this.configurationSource.CompatibilityVersion == version;
		}

		public string GetShadowContextForInboundSession()
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "Getting shadow context for SmtpInSession");
			if (!this.IsVersion(ShadowRedundancyCompatibilityVersion.E15))
			{
				throw new InvalidOperationException("GetShadowContextForInboundSession called under non-E15 compatibility level");
			}
			return ShadowRedundancyManager.EncodeShadowContext(ShadowRedundancyManager.GetE15ShadowContext());
		}

		public string GetShadowContext(IEhloOptions ehloOptions, NextHopSolutionKey key)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<ShadowRedundancyCompatibilityVersion, bool, NextHopSolutionKey>(0L, "Getting shadow context for SmtpOutSession, compatibility version = {0}, XShadowRequest = {1}, key = {2}", this.configurationSource.CompatibilityVersion, ehloOptions.XShadowRequest, key);
			if (ehloOptions == null)
			{
				throw new ArgumentNullException("ehloOptions");
			}
			string shadowContext;
			if (this.IsVersion(ShadowRedundancyCompatibilityVersion.E14) || !ehloOptions.XShadowRequest)
			{
				Guid shadowNextHopConnectorId = ShadowRedundancyManager.GetShadowNextHopConnectorId(key);
				shadowContext = string.Format(CultureInfo.InvariantCulture, "{0}@{1}@{2}", new object[]
				{
					shadowNextHopConnectorId,
					SmtpReceiveServer.ServerName,
					key.NextHopTlsDomain
				});
			}
			else
			{
				shadowContext = ShadowRedundancyManager.GetE15ShadowContext();
			}
			return ShadowRedundancyManager.EncodeShadowContext(shadowContext);
		}

		public bool ShouldShadowMailItem(IReadOnlyMailItem mailItem)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			return !mailItem.IsHeartbeat && this.configurationSource.Enabled && !mailItem.IsPfReplica();
		}

		public IShadowSession GetShadowSession(ISmtpInSession inSession, bool isBdat)
		{
			if (inSession == null)
			{
				throw new ArgumentNullException("inSession");
			}
			if (!inSession.IsShadowedBySender && !inSession.IsPeerShadowSession)
			{
				return this.shadowSessionFactory.GetShadowSession(inSession, isBdat);
			}
			return new NullShadowSession();
		}

		public void SetSmtpInEhloOptions(EhloOptions ehloOptions, ReceiveConnector receiveConnector)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "Setting shadow redundancy EhloOptions");
			if (ehloOptions == null)
			{
				throw new ArgumentNullException("ehloOptions");
			}
			if (receiveConnector == null)
			{
				throw new ArgumentNullException("receiveConnector");
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<bool, ShadowRedundancyCompatibilityVersion, AuthMechanisms>(0L, "Enabled:{0} CompatibilityVersion:{1}, AuthMechanism:{2}", this.Configuration.Enabled, this.Configuration.CompatibilityVersion, receiveConnector.AuthMechanism);
			ehloOptions.XShadow = (this.Configuration.Enabled && this.Configuration.CompatibilityVersion == ShadowRedundancyCompatibilityVersion.E14 && (receiveConnector.AuthMechanism & (AuthMechanisms.ExchangeServer | AuthMechanisms.ExternalAuthoritative)) != AuthMechanisms.None);
			ehloOptions.XShadowRequest = (this.Configuration.Enabled && this.Configuration.CompatibilityVersion == ShadowRedundancyCompatibilityVersion.E15 && (receiveConnector.AuthMechanism & (AuthMechanisms.ExchangeServer | AuthMechanisms.ExternalAuthoritative)) != AuthMechanisms.None);
		}

		public void SetDelayedAckCompletedHandler(DelayedAckItemHandler delayedAckCompleted)
		{
			if (delayedAckCompleted == null)
			{
				throw new ArgumentNullException("delayedAckCompleted");
			}
			if (this.delayedAckCompleted != null)
			{
				throw new InvalidOperationException("delayedAckCompleted delegate already set.");
			}
			this.delayedAckCompleted = delayedAckCompleted;
		}

		public void ApplyDiscardEvents(string serverFqdn, NextHopSolutionKey solutionKey, ICollection<string> messageIds, out int invalid, out int notFound)
		{
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			if (messageIds == null)
			{
				throw new ArgumentNullException("messageIds");
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceError<int, string>(0L, "ApplyDiscardEvents(): Applying {0} events for primary server '{1}'", messageIds.Count, serverFqdn);
			invalid = 0;
			notFound = 0;
			if (!this.configurationSource.Enabled)
			{
				return;
			}
			Guid shadowNextHopConnectorId = ShadowRedundancyManager.GetShadowNextHopConnectorId(solutionKey);
			ShadowRedundancyManager.QualifiedPrimaryServerId serverId = new ShadowRedundancyManager.QualifiedPrimaryServerId(serverFqdn, shadowNextHopConnectorId, solutionKey.NextHopTlsDomain);
			if (messageIds.Count == 0)
			{
				return;
			}
			List<Guid> list = this.ConvertMessageIdsFromStringToGuid(messageIds);
			invalid = messageIds.Count - list.Count;
			if (invalid > 0)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceError<string, int>(0L, "ApplyDiscardEvents(): Primary server '{0}' provided '{1}' invalid discard ids.", serverFqdn, invalid);
			}
			this.ApplyDiscardEvents(serverId, list, out notFound);
		}

		public string[] QueryDiscardEvents(string queryingServerContext, int maxDiscardEvents)
		{
			if (string.IsNullOrEmpty(queryingServerContext))
			{
				throw new ArgumentNullException("queryingServerContext");
			}
			if (maxDiscardEvents < 1)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "maxDiscardEvents must be 1 or more.  Value Received ='{0}'.", new object[]
				{
					maxDiscardEvents
				}));
			}
			if (!this.configurationSource.Enabled)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string>(0L, "QueryDiscardEvents(serverContext='{0}') - shadowing disabled.", queryingServerContext);
				return new string[0];
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string, int>(0L, "QueryDiscardEvents(context='{0}', maxDiscardEvents='{1}')", queryingServerContext, maxDiscardEvents);
			ShadowRedundancyManager.ShadowServerInfo shadowServerInfo = this.GetShadowServerInfo(queryingServerContext, false);
			if (shadowServerInfo == null)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string>(0L, "QueryDiscardEvents(serverContext='{0}') - shadow server info not found.", queryingServerContext);
				return new string[0];
			}
			string[] array = shadowServerInfo.DequeueDiscardEvents(maxDiscardEvents);
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string, int, int>(0L, "QueryDiscardEvents(server='{0}', maxDiscardEvents='{1}') - Found {2} discard events.", queryingServerContext, maxDiscardEvents, array.Length);
			return array;
		}

		public void NotifyBootLoaderDone()
		{
			if (this.bootLoaderComplete)
			{
				throw new InvalidOperationException("NotifyBootLoaderDone() called more than once.");
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "NotifyBootLoaderDone: Boot loader done.");
			this.bootLoaderComplete = true;
			this.hashsetToDetectShadowGroupsOnStartup = null;
			lock (this.timerCreateDestroySync)
			{
				if (!this.shuttingDown)
				{
					this.discardsCleanupTimer = new GuardedTimer(new TimerCallback(this.DiscardsCleanupCallback), null, TimeSpan.Zero, this.configurationSource.DiscardEventsCheckExpiryInterval);
					this.stringPoolCleanupTimer = new GuardedTimer(new TimerCallback(this.StringPoolExpirationCallback), null, this.configurationSource.StringPoolCleanupInterval, this.configurationSource.StringPoolCleanupInterval);
				}
			}
			this.ProcessPendingDiscardEvents();
			this.ProcessPendingResubmits();
		}

		public void NotifyShuttingDown()
		{
			if (this.shuttingDown)
			{
				throw new InvalidOperationException("NotifyShuttingDown() called more than once.");
			}
			lock (this.timerCreateDestroySync)
			{
				this.shuttingDown = true;
				if (this.stringPoolCleanupTimer != null)
				{
					this.stringPoolCleanupTimer.Dispose(true);
				}
				if (this.discardsCleanupTimer != null)
				{
					this.discardsCleanupTimer.Dispose(true);
				}
				if (this.delayedAckCleanupTimer != null)
				{
					this.delayedAckCleanupTimer.Dispose(true);
				}
			}
			this.resubmitEvent.WaitOne();
		}

		public void NotifyMailItemBifurcated(string shadowServerContext, string shadowServerDiscardId)
		{
			ShadowRedundancyManager.QualifiedShadowServerDiscardId qualifiedDiscardId = new ShadowRedundancyManager.QualifiedShadowServerDiscardId(this.serverContextStringPool.Intern(shadowServerContext), shadowServerDiscardId);
			this.NotifyMailItemCreatedInShadowGroup(qualifiedDiscardId);
		}

		public void NotifyMailItemPreEnqueuing(IReadOnlyMailItem mailItem, TransportMessageQueue queue)
		{
			if (!this.ValidCandidateForDelayedAckSkipping(mailItem, queue))
			{
				return;
			}
			SubmitMessageQueue submitMessageQueue;
			RoutedMessageQueue routedMessageQueue;
			UnreachableMessageQueue unreachableMessageQueue;
			string queueId = this.GetQueueId(queue, out submitMessageQueue, out routedMessageQueue, out unreachableMessageQueue);
			if (queueId == null)
			{
				throw new ArgumentException(string.Format("Unexpected queue type '{0}'.", queue.GetType()));
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long, string>((long)this.GetHashCode(), "Mail item '{0}' queued in '{1}'.", mailItem.RecordId, queueId);
			string text = null;
			if (unreachableMessageQueue != null)
			{
				text = "Unreachable";
			}
			if (text == null && queue.Suspended)
			{
				text = "QueueIsSuspended";
			}
			if (text == null && routedMessageQueue != null && this.IsQueueInRetry(routedMessageQueue))
			{
				text = "QueueInRetry";
			}
			if (text == null && submitMessageQueue == null)
			{
				int activeCount = queue.ActiveCount;
				if (activeCount >= this.configurationSource.DelayedAckSkippingQueueLength)
				{
					text = string.Format(CultureInfo.InvariantCulture, "QueueLength={0}>={1}", new object[]
					{
						activeCount,
						this.configurationSource.DelayedAckSkippingQueueLength
					});
				}
			}
			if (text != null)
			{
				this.SkipDelayedAckForMessage(mailItem, text, queueId);
			}
		}

		public void NotifyMailItemDeferred(IReadOnlyMailItem mailItem, TransportMessageQueue queue, DateTime deferUntil)
		{
			if (!this.ValidCandidateForDelayedAckSkipping(mailItem, queue))
			{
				return;
			}
			SubmitMessageQueue submitMessageQueue;
			RoutedMessageQueue routedMessageQueue;
			UnreachableMessageQueue unreachableMessageQueue;
			string queueId = this.GetQueueId(queue, out submitMessageQueue, out routedMessageQueue, out unreachableMessageQueue);
			if (queueId == null)
			{
				throw new ArgumentException(string.Format("Unexpected queue type '{0}'.", queue.GetType()));
			}
			if (submitMessageQueue != null && deferUntil <= DateTime.UtcNow)
			{
				return;
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long, string>((long)this.GetHashCode(), "Mail item '{0}' deferred in '{1}'.", mailItem.RecordId, queueId);
			this.SkipDelayedAckForMessage(mailItem, "ItemDeferred=" + deferUntil.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo), queueId);
		}

		public void LinkSideEffectMailItem(IReadOnlyMailItem originalMailItem, TransportMailItem sideEffectMailItem)
		{
			if (originalMailItem == null)
			{
				throw new ArgumentNullException("originalMailItem");
			}
			if (sideEffectMailItem == null)
			{
				throw new ArgumentNullException("sideEffectMailItem");
			}
			if (!originalMailItem.IsShadowed())
			{
				throw new ArgumentException("Attempt to link to a non-shadowed message", "originalMailItem");
			}
			sideEffectMailItem.ShadowServerDiscardId = originalMailItem.ShadowServerDiscardId;
			sideEffectMailItem.ShadowServerContext = originalMailItem.ShadowServerContext;
			ShadowRedundancyManager.QualifiedShadowServerDiscardId qualifiedDiscardId = new ShadowRedundancyManager.QualifiedShadowServerDiscardId(originalMailItem.ShadowServerContext, originalMailItem.ShadowServerDiscardId);
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long, long>((long)this.GetHashCode(), "Linking side effect message {0} to shadow for original message {1}", sideEffectMailItem.RecordId, originalMailItem.RecordId);
			this.NotifyMailItemCreatedInShadowGroup(qualifiedDiscardId);
		}

		public void NotifyMailItemDelivered(TransportMailItem mailItem)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long>(0L, "NotifyMailItemDelivered() called for mail item with record id '{0}'.", mailItem.RecordId);
			this.NotifyMailItemInstanceHandled(mailItem);
		}

		public void NotifyMailItemPoison(TransportMailItem mailItem)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long>(0L, "NotifyMailItemPoison() called for mail item with record id '{0}'.", mailItem.RecordId);
			this.NotifyMailItemInstanceHandled(mailItem);
		}

		public void NotifyMailItemReleased(TransportMailItem mailItem)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long>(0L, "NotifyMailItemReleased() called for mail item with record id '{0}'.", mailItem.RecordId);
			this.NotifyMailItemInstanceHandled(mailItem);
		}

		public void NotifyPrimaryServerState(string serverFqdn, string state, ShadowRedundancyCompatibilityVersion version)
		{
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			if (string.IsNullOrEmpty(state))
			{
				state = "0de1e7ed-0de1-0de1-0de1-de1e7edele7e";
			}
			PrimaryServerInfo primaryServerInfo = this.primaryServerInfoMap.UpdateServerState(serverFqdn, state, version);
			if (!primaryServerInfo.IsActive)
			{
				bool flag = false;
				IEnumerable<ShadowMessageQueue> queues = this.GetQueues(serverFqdn);
				foreach (ShadowMessageQueue shadowMessageQueue in queues)
				{
					if (!shadowMessageQueue.IsEmpty)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					lock (this.queuedRecoveryResubmits)
					{
						ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string>(0L, "NotifyPrimaryServerState(): Queuing a resubmit for server '{0}'.", serverFqdn);
						this.queuedRecoveryResubmits.Add(serverFqdn);
						return;
					}
				}
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string>(0L, "NotifyPrimaryServerState(): There are no shadow messages for server '{0}', hence, no resubmit queued.", serverFqdn);
			}
		}

		public void NotifyServerViolatedSmtpContract(string serverFqdnOrContext)
		{
			if (string.IsNullOrEmpty(serverFqdnOrContext))
			{
				throw new ArgumentNullException("serverFqdnOrContext");
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceError<string>(0L, "NotifyServerViolatedSmtpContract() Server with Fqdn/Context '{0}' violdated Smtp protocol and hence state dumped.", serverFqdnOrContext);
			this.DiscardShadowResourcesForServer(serverFqdnOrContext);
		}

		public void EnqueueDelayedAckMessage(Guid shadowMessageId, object state, DateTime startTime, TimeSpan maxDelayDuration)
		{
			if (!this.IsVersion(ShadowRedundancyCompatibilityVersion.E14))
			{
				throw new InvalidOperationException("EnqueueDelayedAckMessage can only be called in E14 mode.");
			}
			if (shadowMessageId == Guid.Empty)
			{
				throw new ArgumentException("shadowMessageId must not be Guid.Empty", "shadowMessageId");
			}
			if (this.delayedAckCompleted == null)
			{
				throw new InvalidOperationException("EnqueueDelayedAckMessage() is called while delayedAckCompleted is null.");
			}
			ShadowRedundancyManager.DelayedAckEntry delayedAckEntry = new ShadowRedundancyManager.DelayedAckEntry(state, startTime, startTime.Add(maxDelayDuration));
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<ShadowRedundancyManager.DelayedAckEntry>(0L, "EnqueueDelayedAckMessage(): {0}", delayedAckEntry);
			lock (this.delayedAckEntries)
			{
				TransportHelpers.AttemptAddToDictionary<Guid, ShadowRedundancyManager.DelayedAckEntry>(this.delayedAckEntries, shadowMessageId, delayedAckEntry, null);
				if (!this.configurationSource.Enabled)
				{
					delayedAckEntry.MarkAsExpired("ShadowRedundancyDisabled");
				}
			}
		}

		public bool ShouldDelayAck()
		{
			return this.Configuration.Enabled && this.Configuration.CompatibilityVersion == ShadowRedundancyCompatibilityVersion.E14;
		}

		public bool ShouldSmtpOutSendXQDiscard(string serverFqdn)
		{
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			lock (this.syncQueues)
			{
				List<ShadowMessageQueue> list;
				if (this.serverFqdnToShadowQueueMap.TryGetValue(serverFqdn, out list))
				{
					foreach (ShadowMessageQueue shadowMessageQueue in list)
					{
						if (!shadowMessageQueue.IsEmpty)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool ShouldSmtpOutSendXShadow(Permission sessionPermissions, DeliveryType nextHopDeliveryType, IEhloOptions advertisedEhloOptions, SmtpSendConnectorConfig connector)
		{
			if ((sessionPermissions & Permission.SMTPSendXShadow) == Permission.None)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "ShouldSmtpOutSendXShadow returning false because of missing SMTPSendXShadow permission");
				return false;
			}
			if (connector != null && connector.FrontendProxyEnabled)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "ShouldSmtpOutSendXShadow returning false because send connector will proxy through FE");
				return false;
			}
			if (this.IsVersion(ShadowRedundancyCompatibilityVersion.E15) && nextHopDeliveryType == DeliveryType.Heartbeat && (advertisedEhloOptions.XShadow || advertisedEhloOptions.XShadowRequest))
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "ShouldSmtpOutSendXShadow returning true for heartbeat");
				return true;
			}
			return advertisedEhloOptions.XShadow && ShadowRedundancyManager.IsShadowableDeliveryType(nextHopDeliveryType);
		}

		public bool EnqueueShadowMailItem(TransportMailItem mailItem, NextHopSolution originalMessageSolution, string primaryServer, bool shadowedMailItem, AckStatus ackStatus)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (string.IsNullOrEmpty(primaryServer))
			{
				return false;
			}
			bool flag = ShadowRedundancyManager.ShouldShadowAckStatus(ackStatus);
			bool flag2 = this.configurationSource.Enabled && shadowedMailItem && originalMessageSolution.DeliveryStatus == DeliveryStatus.Complete && flag && this.ShouldShadowMailItem(mailItem);
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "EnqueueShadowMailItem(E14) params: shadowAckStatus={0} createShadow={1} primaryServer={2} shadowedMailItem={3} ackStatus={4} NextHopSolutionKey={5} DeliveryStatus={6}", new object[]
			{
				flag,
				flag2,
				primaryServer,
				shadowedMailItem,
				ackStatus,
				originalMessageSolution.NextHopSolutionKey,
				originalMessageSolution.DeliveryStatus
			});
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "EnqueueShadowMailItem(E14) mailitem: IsHeartbeat={0} IsPoison={1} IsActive={2} InternetMessageId={3} ShadowMessageId={4} ShadowServerContext={5} ShadowServerDiscardId={6}", new object[]
			{
				mailItem.IsHeartbeat,
				mailItem.IsPoison,
				mailItem.IsActive,
				mailItem.InternetMessageId,
				mailItem.ShadowMessageId,
				mailItem.ShadowServerContext,
				mailItem.ShadowServerDiscardId
			});
			if (flag2)
			{
				if (!ShadowRedundancyManager.IsShadowableDeliveryType(originalMessageSolution.NextHopSolutionKey.NextHopType.DeliveryType))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "This kind of next hop solution should not be shadowed.  Delivery Type : '{0}'.", new object[]
					{
						originalMessageSolution.NextHopSolutionKey.NextHopType.DeliveryType
					}));
				}
				ShadowMailItem shadowMailItem = this.CreateShadowMailItemAndReplaceSolution(mailItem, originalMessageSolution, primaryServer);
				this.EnqueueShadowMailItem(shadowMailItem);
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long, Guid, string>(0L, "Transport Mail Item '{0}' with ShadowMessageId '{1}' added to shadow queue '{2}'. (E14)", mailItem.RecordId, mailItem.ShadowMessageId, primaryServer);
			}
			return flag2;
		}

		public bool EnqueuePeerShadowMailItem(TransportMailItem mailItem, string primaryServer)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "Enqueuing shadow item without next hop");
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (string.IsNullOrEmpty(primaryServer))
			{
				throw new ArgumentNullException("primaryServer");
			}
			if (!this.IsVersion(ShadowRedundancyCompatibilityVersion.E15))
			{
				throw new InvalidOperationException("Attempt to enqueue a shadow message with compatibility == E14");
			}
			if (!this.configurationSource.Enabled)
			{
				mailItem.ShadowServerContext = null;
				mailItem.ReleaseFromActiveMaterializedLazy();
				return false;
			}
			ShadowMailItem shadowMailItem = this.CreateShadowMailItem(mailItem, primaryServer);
			this.EnqueueShadowMailItem(shadowMailItem);
			this.LogNewShadowMailItem(shadowMailItem);
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long, Guid, string>(0L, "Transport Mail Item '{0}' with ShadowMessageId '{1}' added to shadow queue '{2}'. (E15)", mailItem.RecordId, mailItem.ShadowMessageId, primaryServer);
			SystemProbeHelper.ShadowRedundancyTracer.TracePass<string>(mailItem, 0L, "Message added to the shadow queue {0}", primaryServer);
			mailItem.MinimizeMemory();
			return true;
		}

		public void EnqueueDiscardPendingMailItem(TransportMailItem mailItem)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "Enqueuing discard item");
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (string.IsNullOrEmpty(mailItem.ShadowServerContext) || string.IsNullOrEmpty(mailItem.ShadowServerDiscardId))
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "Ignoring invalid mail item");
				return;
			}
			this.EnqueueDiscardEventForShadowServer(mailItem, mailItem.GetExpiryTime(false));
		}

		public void UpdateQueues()
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<bool>(0L, "ShadowRedundancyManager updating queues heartbeatEnabled={0}", this.HeartbeatEnabled);
			ShadowMessageQueue[] queueArray = this.GetQueueArray();
			foreach (ShadowMessageQueue shadowMessageQueue in queueArray)
			{
				shadowMessageQueue.UpdateQueue(this.HeartbeatEnabled, this.paused);
				if (shadowMessageQueue.CanBeDeleted(this.configurationSource.QueueMaxIdleTimeInterval))
				{
					lock (this.syncQueues)
					{
						if (shadowMessageQueue.CanBeDeleted(this.configurationSource.QueueMaxIdleTimeInterval))
						{
							ShadowRedundancyManager.QualifiedPrimaryServerId key = new ShadowRedundancyManager.QualifiedPrimaryServerId(shadowMessageQueue.Key);
							List<ShadowMessageQueue> list;
							if (this.serverFqdnToShadowQueueMap.TryGetValue(key.Fqdn, out list) && list.Contains(shadowMessageQueue))
							{
								this.serverIdToShadowQueueMap.Remove(key);
								this.queueIdToShadowQueueMap.Remove(shadowMessageQueue.Id);
								if (list.Count > 1)
								{
									list.Remove(shadowMessageQueue);
								}
								else
								{
									this.serverFqdnToShadowQueueMap.Remove(key.Fqdn);
								}
								shadowMessageQueue.Delete();
							}
						}
					}
				}
			}
			if (this.bootLoaderComplete)
			{
				DateTime utcNow = DateTime.UtcNow;
				if (utcNow >= this.lastPrimaryServerInfoMapCleanup.Add(this.configurationSource.PrimaryServerInfoCleanupInterval))
				{
					try
					{
						if (Interlocked.Increment(ref this.primaryServerInfoMapCleanupGuard) == 1)
						{
							this.lastPrimaryServerInfoMapCleanup = utcNow;
							IEnumerable<PrimaryServerInfo> enumerable = null;
							lock (this.syncQueues)
							{
								enumerable = this.primaryServerInfoMap.RemoveExpiredServers(utcNow);
							}
							if (enumerable != null)
							{
								PrimaryServerInfo.CommitLazy(enumerable, this.database.ServerInfoTable);
							}
						}
					}
					finally
					{
						Interlocked.Decrement(ref this.primaryServerInfoMapCleanupGuard);
					}
				}
				this.ProcessPendingResubmits();
			}
		}

		public ShadowMessageQueue GetQueue(NextHopSolutionKey key)
		{
			if (key.NextHopType != NextHopType.ShadowRedundancy && key.NextHopType != NextHopType.Heartbeat)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "key has invalid delivery type '{0}'.", new object[]
				{
					key.NextHopType
				}));
			}
			ShadowRedundancyManager.QualifiedPrimaryServerId primaryServerId = new ShadowRedundancyManager.QualifiedPrimaryServerId(key);
			return this.GetQueue(primaryServerId);
		}

		public ShadowMessageQueue[] GetQueueArray()
		{
			ShadowMessageQueue[] result;
			lock (this.syncQueues)
			{
				ShadowMessageQueue[] array = new ShadowMessageQueue[this.serverIdToShadowQueueMap.Count];
				this.serverIdToShadowQueueMap.Values.CopyTo(array, 0);
				result = array;
			}
			return result;
		}

		public TransportMailItem GetMailItem(long mailItemId)
		{
			if (mailItemId <= 0L)
			{
				throw new ArgumentOutOfRangeException("The mailItemId is expected to be greater than 0, it is currently set to " + mailItemId);
			}
			TransportMailItem result;
			lock (this.syncAllMailItems)
			{
				this.allMailItemsByRecordId.TryGetValue(mailItemId, out result);
			}
			return result;
		}

		public void ProcessMailItemOnStartup(TransportMailItem mailItem)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (this.bootLoaderComplete)
			{
				throw new InvalidOperationException("ProcessMailItemOnStartup() must not be called except while boot loader is active.");
			}
			lock (mailItem)
			{
				if (string.Equals(mailItem.ShadowServerContext, "$localhost$", StringComparison.OrdinalIgnoreCase))
				{
					mailItem.ShadowServerContext = null;
					mailItem.ShadowServerDiscardId = null;
				}
				if (!this.configurationSource.Enabled || mailItem.IsHeartbeat)
				{
					ShadowRedundancyManager.ClearRecipientsPrimaryServerFqdnGuid(mailItem);
					mailItem.ReleaseFromShadowRedundancy();
				}
				else
				{
					if (mailItem.IsShadowed())
					{
						ShadowRedundancyManager.QualifiedShadowServerDiscardId qualifiedShadowServerDiscardId = new ShadowRedundancyManager.QualifiedShadowServerDiscardId(this.serverContextStringPool.Intern(mailItem.ShadowServerContext), mailItem.ShadowServerDiscardId);
						if (!this.hashsetToDetectShadowGroupsOnStartup.TryAdd(qualifiedShadowServerDiscardId))
						{
							this.NotifyMailItemCreatedInShadowGroup(qualifiedShadowServerDiscardId);
						}
					}
					Dictionary<NextHopSolutionKey, NextHopSolution> dictionary = null;
					foreach (MailRecipient mailRecipient in mailItem.Recipients)
					{
						string primaryServerFqdnGuid = mailRecipient.PrimaryServerFqdnGuid;
						if (!string.IsNullOrEmpty(primaryServerFqdnGuid))
						{
							ShadowRedundancyManager.QualifiedPrimaryServerId qualifiedPrimaryServerId;
							if (!ShadowRedundancyManager.QualifiedPrimaryServerId.TryParse(primaryServerFqdnGuid, out qualifiedPrimaryServerId))
							{
								ExTraceGlobals.ShadowRedundancyTracer.TraceError(0L, "Transport Mail Item '{0}' with ShadowMessageId '{1}' not shadowed for recipient '{2}' due to failure in parsing primaryServerFqdnGuid '{3}'", new object[]
								{
									mailItem.RecordId,
									mailItem.ShadowMessageId,
									mailRecipient.Email,
									primaryServerFqdnGuid
								});
								mailRecipient.PrimaryServerFqdnGuid = null;
							}
							else
							{
								NextHopSolutionKey key = NextHopSolutionKey.CreateShadowNextHopSolutionKey(qualifiedPrimaryServerId.Fqdn, qualifiedPrimaryServerId.NextHopGuid, mailRecipient.GetTlsDomain());
								NextHopSolution nextHopSolution;
								if ((dictionary == null && !mailItem.NextHopSolutions.TryGetValue(key, out nextHopSolution)) || !dictionary.TryGetValue(key, out nextHopSolution))
								{
									if (dictionary == null)
									{
										dictionary = new Dictionary<NextHopSolutionKey, NextHopSolution>(mailItem.NextHopSolutions);
									}
									nextHopSolution = new NextHopSolution(key);
									dictionary.Add(key, nextHopSolution);
								}
								nextHopSolution.AddRecipient(mailRecipient);
								mailRecipient.NextHop = nextHopSolution.NextHopSolutionKey;
							}
						}
					}
					if (dictionary != null)
					{
						mailItem.NextHopSolutions = dictionary;
					}
					bool flag2 = false;
					foreach (KeyValuePair<NextHopSolutionKey, NextHopSolution> keyValuePair in mailItem.NextHopSolutions)
					{
						if (keyValuePair.Key.NextHopType.DeliveryType == DeliveryType.ShadowRedundancy)
						{
							this.EnqueueShadowMailItem(this.CreateShadowMailItem(mailItem, keyValuePair.Value));
							flag2 = true;
						}
					}
					if (!flag2)
					{
						mailItem.ReleaseFromShadowRedundancy();
						ExTraceGlobals.ShadowRedundancyTracer.TraceError<long, Guid>(0L, "Transport Mail Item '{0}' with ShadowMessageId '{1}' not shadowed", mailItem.RecordId, mailItem.ShadowMessageId);
					}
					else
					{
						mailItem.CommitLazyAndDehydrateMessageIfPossible(Breadcrumb.DehydrateOnAddToShadowQueue);
					}
				}
			}
		}

		public void VisitMailItems(Func<TransportMailItem, bool> visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			lock (this.syncAllMailItems)
			{
				foreach (TransportMailItem arg in this.allMailItemsByRecordId.Values)
				{
					if (!visitor(arg))
					{
						break;
					}
				}
			}
		}

		public List<ShadowMessageQueue> FindByQueueIdentity(QueueIdentity queueIdentity)
		{
			if (queueIdentity == null)
			{
				throw new ArgumentNullException("queueIdentity");
			}
			if (!queueIdentity.IsLocal || queueIdentity.Type != QueueType.Shadow)
			{
				return new List<ShadowMessageQueue>();
			}
			if (queueIdentity.RowId > 0L)
			{
				List<ShadowMessageQueue> list = new List<ShadowMessageQueue>();
				ShadowMessageQueue shadowMessageQueue = this.FindById(queueIdentity.RowId);
				if (shadowMessageQueue != null)
				{
					list.Add(shadowMessageQueue);
				}
				return list;
			}
			return this.GetQueues(queueIdentity.NextHopDomain);
		}

		public void LoadQueue(RoutedQueueBase queueStorage)
		{
			if (queueStorage == null)
			{
				throw new ArgumentNullException("queueStorage");
			}
			NextHopSolutionKey key = new NextHopSolutionKey(queueStorage.NextHopType, queueStorage.NextHopDomain, queueStorage.NextHopConnector, queueStorage.NextHopTlsDomain);
			ShadowRedundancyManager.QualifiedPrimaryServerId qualifiedPrimaryServerId = new ShadowRedundancyManager.QualifiedPrimaryServerId(key);
			lock (this.syncQueues)
			{
				ShadowMessageQueue queue;
				if (!this.serverIdToShadowQueueMap.TryGetValue(qualifiedPrimaryServerId, out queue))
				{
					queue = new ShadowMessageQueue(queueStorage, key, new ShadowMessageQueue.ItemExpiredHandler(this.ItemExpiryHandlerCallback), this.configurationSource, new ShouldSuppressResubmission(this.ShouldSuppressResubmission), this.shadowRedundancyEventLogger, null, null);
				}
				this.AddQueue(qualifiedPrimaryServerId, queue);
			}
		}

		public void EvaluateHeartbeatAttempt(NextHopSolutionKey key, out bool sendHeartbeat, out bool abortHeartbeat)
		{
			if (key.NextHopType != NextHopType.Heartbeat)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unexpected delivery type {0}", new object[]
				{
					key.NextHopType.DeliveryType
				}));
			}
			sendHeartbeat = false;
			abortHeartbeat = false;
			ShadowMessageQueue queue = this.GetQueue(key);
			if (queue != null)
			{
				queue.EvaluateHeartbeatAttempt(out sendHeartbeat, out abortHeartbeat);
			}
			else
			{
				abortHeartbeat = true;
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<bool, bool, NextHopSolutionKey>(0L, "ShadowRedundancyManager.EvaluateHeartbeatAttempt returning send={0} abort={1} for queue={2}", sendHeartbeat, abortHeartbeat, key);
		}

		public void NotifyHeartbeatConfigChanged(NextHopSolutionKey key, out bool abortHeartbeat)
		{
			if (key.NextHopType != NextHopType.Heartbeat)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unexpected delivery type {0}", new object[]
				{
					key.NextHopType.DeliveryType
				}));
			}
			abortHeartbeat = false;
			ShadowMessageQueue queue = this.GetQueue(key);
			if (queue != null)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>(0L, "ShadowRedundancyManager.NotifyHeartbeatConfigChanged queue={0}", key);
				queue.NotifyHeartbeatConfigChanged(key, out abortHeartbeat);
				return;
			}
			abortHeartbeat = true;
		}

		public void NotifyHeartbeatRetry(NextHopSolutionKey key, out bool abortHeartbeat)
		{
			if (key.NextHopType != NextHopType.Heartbeat)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unexpected delivery type {0}", new object[]
				{
					key.NextHopType.DeliveryType
				}));
			}
			abortHeartbeat = false;
			ShadowMessageQueue queue = this.GetQueue(key);
			if (queue != null)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey>(0L, "ShadowRedundancyManager.NotifyHeartbeatRetry queue={0}", key);
				queue.UpdateHeartbeat(DateTime.UtcNow, key, false);
				return;
			}
			abortHeartbeat = true;
		}

		public void NotifyHeartbeatStatus(string serverFqdn, NextHopSolutionKey solutionKey, bool heartbeatSucceeded)
		{
			Guid shadowNextHopConnectorId = ShadowRedundancyManager.GetShadowNextHopConnectorId(solutionKey);
			ShadowRedundancyManager.QualifiedPrimaryServerId qualifiedPrimaryServerId = new ShadowRedundancyManager.QualifiedPrimaryServerId(serverFqdn, shadowNextHopConnectorId, solutionKey.NextHopTlsDomain);
			this.UpdateHeartbeat(qualifiedPrimaryServerId.Fqdn, heartbeatSucceeded);
		}

		public IEnumerable<PrimaryServerInfo> GetPrimaryServersForMessageResubmission()
		{
			foreach (PrimaryServerInfo primaryServerInfo in this.primaryServerInfoMap.GetAll())
			{
				bool isActive = primaryServerInfo.IsActive;
				yield return primaryServerInfo;
			}
			yield break;
		}

		void IShadowRedundancyManagerFacade.LinkSideEffectMailItemIfNeeded(ITransportMailItemFacade originalMailItem, ITransportMailItemFacade sideEffectMailItem)
		{
			TransportMailItem transportMailItem = (TransportMailItem)originalMailItem;
			if (transportMailItem.IsShadowed())
			{
				this.LinkSideEffectMailItem(transportMailItem, (TransportMailItem)sideEffectMailItem);
			}
		}

		protected virtual void SkipDelayedAckForMessage(IReadOnlyMailItem mailItem, string skipDelayedAckReason, string queueId)
		{
			lock (this.delayedAckEntries)
			{
				ShadowRedundancyManager.DelayedAckEntry delayedAckEntry;
				if (this.delayedAckEntries.TryGetValue(mailItem.ShadowMessageId, out delayedAckEntry))
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long, string, string>((long)this.GetHashCode(), "Delayed Ack for mail item '{0}' skipped due to '{1}' applying to queue '{2}'.", mailItem.RecordId, skipDelayedAckReason, queueId);
					delayedAckEntry.MarkAsSkipped(skipDelayedAckReason + ";NextHopDomain=" + queueId);
					this.FinalizeDelayedAck(mailItem.ShadowMessageId, delayedAckEntry, DateTime.UtcNow);
				}
			}
		}

		private static string GetE15ShadowContext()
		{
			return SmtpReceiveServer.ServerName;
		}

		private static string EncodeShadowContext(string shadowContext)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string>(0L, "Encoding shadow context: {0}", shadowContext);
			return Convert.ToBase64String(Encoding.Default.GetBytes(shadowContext));
		}

		private static bool ShouldShadowAckStatus(AckStatus ackStatus)
		{
			switch (ackStatus)
			{
			case AckStatus.Pending:
			case AckStatus.Retry:
			case AckStatus.Fail:
			case AckStatus.Resubmit:
			case AckStatus.Quarantine:
			case AckStatus.Skip:
				return false;
			case AckStatus.Success:
			case AckStatus.Expand:
			case AckStatus.Relay:
			case AckStatus.SuccessNoDsn:
				return true;
			default:
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Unknown AckStatus: {0}", new object[]
				{
					ackStatus.ToString()
				}));
			}
		}

		private static void ClearRecipientsPrimaryServerFqdnGuid(TransportMailItem mailItem)
		{
			foreach (MailRecipient mailRecipient in mailItem.Recipients)
			{
				if (!string.IsNullOrEmpty(mailRecipient.PrimaryServerFqdnGuid))
				{
					mailRecipient.PrimaryServerFqdnGuid = null;
				}
			}
		}

		private static void MarkMailItemAsDiscardPending(TransportMailItem mailItem)
		{
			if (string.IsNullOrEmpty(mailItem.ShadowServerContext))
			{
				throw new ArgumentNullException("mailItem.ShadowServerContext");
			}
			if (string.IsNullOrEmpty(mailItem.ShadowServerDiscardId))
			{
				throw new ArgumentNullException("mailItem.ShadowServerDiscardId");
			}
			mailItem.IsDiscardPending = true;
			mailItem.CommitLazy();
		}

		private static void MarkMailItemAsDiscardCompleted(TransportMailItem mailItem)
		{
			mailItem.ShadowServerContext = null;
			mailItem.ShadowServerDiscardId = null;
			mailItem.IsDiscardPending = false;
			mailItem.CommitLazy();
		}

		private bool IsQueueInRetry(RoutedMessageQueue queue)
		{
			return queue.RetryConnectionScheduled && queue.ActiveConnections == 0;
		}

		private void FinalizeDelayedAck(Guid shadowMessageId, ShadowRedundancyManager.DelayedAckEntry delayedAckEntry, DateTime now)
		{
			if (delayedAckEntry.CompletionStatus == null)
			{
				throw new InvalidOperationException("CompletionStatus must no be null at completion time.");
			}
			if (this.delayedAckCompleted(delayedAckEntry.State, delayedAckEntry.CompletionStatus.Value, now.Subtract(delayedAckEntry.StartTime), delayedAckEntry.Context))
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<Guid, DelayedAckCompletionStatus?>(0L, "FinalizeDelayedAck(): Completion callback notified successfully about delayed ack for message '{0}' with completion status '{1}'.", shadowMessageId, delayedAckEntry.CompletionStatus);
				if (!this.delayedAckEntries.Remove(shadowMessageId))
				{
					throw new InvalidOperationException("Expected delayedAckEntry is not found in list.");
				}
				if (delayedAckEntry.CompletionStatus.Value != DelayedAckCompletionStatus.Delivered)
				{
					ShadowRedundancyManager.PerfCounters.DelayedAckExpired(1L);
					return;
				}
			}
			else
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<Guid, DelayedAckCompletionStatus?>(0L, "FinalizeDelayedAck(): Completion callback returned false for message '{0}' with completion status '{1}'.  Will try again later.", shadowMessageId, delayedAckEntry.CompletionStatus);
			}
		}

		private bool ShouldSuppressResubmission(IEnumerable<INextHopServer> relatedBridgeheads)
		{
			if (this.targetRunningState != ServiceState.Active)
			{
				return true;
			}
			if (relatedBridgeheads == null)
			{
				return false;
			}
			foreach (INextHopServer nextHopServer in relatedBridgeheads)
			{
				string primaryFqdn = nextHopServer.IsIPAddress ? nextHopServer.Address.ToString() : nextHopServer.Fqdn;
				List<ShadowMessageQueue> queues = this.GetQueues(primaryFqdn);
				foreach (ShadowMessageQueue shadowMessageQueue in queues)
				{
					if (!shadowMessageQueue.IsEmpty && !shadowMessageQueue.Suspended && !shadowMessageQueue.HasHeartbeatFailure && !shadowMessageQueue.IsResubmissionSuppressed)
					{
						return false;
					}
				}
			}
			return true;
		}

		private ShadowMessageQueue FindById(long id)
		{
			ShadowMessageQueue result;
			lock (this.syncQueues)
			{
				ShadowMessageQueue shadowMessageQueue;
				result = (this.queueIdToShadowQueueMap.TryGetValue(id, out shadowMessageQueue) ? shadowMessageQueue : null);
			}
			return result;
		}

		private ShadowMessageQueue CreateQueue(ShadowRedundancyManager.QualifiedPrimaryServerId serverId, FindRelatedBridgeHeads findRelatedBridgeHeads, GetRoutedMessageQueueStatus getRoutedMessageQueueStatus)
		{
			ShadowMessageQueue shadowMessageQueue = ShadowMessageQueue.NewQueue(NextHopSolutionKey.CreateShadowNextHopSolutionKey(serverId.Fqdn, serverId.NextHopGuid), new ShadowMessageQueue.ItemExpiredHandler(this.ItemExpiryHandlerCallback), this.configurationSource, new ShouldSuppressResubmission(this.ShouldSuppressResubmission), this.shadowRedundancyEventLogger, findRelatedBridgeHeads, getRoutedMessageQueueStatus);
			this.AddQueue(serverId, shadowMessageQueue);
			return shadowMessageQueue;
		}

		private void ItemExpiryHandlerCallback(ShadowMessageQueue shadowMessageQueue, ShadowMailItem shadowMailItem)
		{
			bool flag = false;
			TransportMailItem transportMailItem = shadowMailItem.TransportMailItem;
			long recordId;
			Guid shadowMessageId;
			lock (transportMailItem)
			{
				recordId = transportMailItem.RecordId;
				shadowMessageId = transportMailItem.ShadowMessageId;
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "Shadow mail item expired for Transport Mail Item '{0}' with ShadowMessageId '{1}' from queue '{2}'.  DiscardReason '{3}'.", new object[]
				{
					recordId,
					shadowMessageId,
					shadowMessageQueue.NextHopDomain,
					shadowMailItem.DiscardReason
				});
				foreach (MailRecipient mailRecipient in shadowMailItem.NextHopSolution.Recipients)
				{
					mailRecipient.PrimaryServerFqdnGuid = null;
				}
				Dictionary<NextHopSolutionKey, NextHopSolution> dictionary = new Dictionary<NextHopSolutionKey, NextHopSolution>(transportMailItem.NextHopSolutions);
				dictionary.Remove(shadowMailItem.NextHopSolution.NextHopSolutionKey);
				transportMailItem.NextHopSolutions = dictionary;
				if (!transportMailItem.IsShadow())
				{
					this.MarkShadowMailAsDelivered(shadowMailItem, shadowMessageQueue.NextHopDomain);
					transportMailItem.ReleaseFromShadowRedundancy();
					flag = true;
				}
				else
				{
					transportMailItem.CommitLazy(shadowMailItem.NextHopSolution);
				}
			}
			if (flag)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<long, Guid>(0L, "Transport Mail Item '{0}' with ShadowMessageId '{1}' released.", recordId, shadowMessageId);
				lock (this.syncAllMailItems)
				{
					this.allMailItemsByRecordId.Remove(recordId);
				}
			}
			ShadowRedundancyManager.PerfCounters.DecrementCounter(ShadowRedundancyCounterId.AggregateShadowQueueLength);
			if (shadowMailItem.DiscardReason.Equals(DiscardReason.Expired))
			{
				ShadowRedundancyManager.PerfCounters.IncrementCounter(ShadowRedundancyCounterId.ShadowQueueAutoDiscardsTotal);
			}
		}

		private ShadowMailItem CreateShadowMailItemAndReplaceSolution(TransportMailItem mailItem, NextHopSolution originalMessageSolution, string serverFqdn)
		{
			Guid shadowNextHopConnectorId = ShadowRedundancyManager.GetShadowNextHopConnectorId(originalMessageSolution.NextHopSolutionKey);
			NextHopSolutionKey nextHopSolutionKey = NextHopSolutionKey.CreateShadowNextHopSolutionKey(serverFqdn, shadowNextHopConnectorId, originalMessageSolution.NextHopSolutionKey.NextHopTlsDomain);
			ShadowMailItem result;
			lock (mailItem)
			{
				foreach (MailRecipient mailRecipient in originalMessageSolution.Recipients)
				{
					if (!mailRecipient.IsProcessed)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Recipients in shadowed solutions are expected to be marked as processed.  Recipient '{0}'.", new object[]
						{
							mailRecipient
						}));
					}
					if (!string.IsNullOrEmpty(mailRecipient.PrimaryServerFqdnGuid))
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Recipients being shadowed must not already have PrimaryServerFqdnGuid set.  Recipient '{0}' has it set to '{1}'.", new object[]
						{
							mailRecipient,
							mailRecipient.PrimaryServerFqdnGuid
						}));
					}
					ShadowRedundancyManager.QualifiedPrimaryServerId qualifiedPrimaryServerId = new ShadowRedundancyManager.QualifiedPrimaryServerId(nextHopSolutionKey);
					mailRecipient.PrimaryServerFqdnGuid = qualifiedPrimaryServerId.ToString();
					mailRecipient.NextHop = nextHopSolutionKey;
				}
				Dictionary<NextHopSolutionKey, NextHopSolution> dictionary = new Dictionary<NextHopSolutionKey, NextHopSolution>(mailItem.NextHopSolutions);
				dictionary.Remove(originalMessageSolution.NextHopSolutionKey);
				mailItem.NextHopSolutions = dictionary;
				NextHopSolution shadowNextHopSolution = mailItem.UpdateNextHopSolutionTable(nextHopSolutionKey, originalMessageSolution.Recipients, true);
				result = this.CreateShadowMailItem(mailItem, shadowNextHopSolution);
			}
			return result;
		}

		private void EnqueueShadowMailItem(ShadowMailItem shadowMailItem)
		{
			bool flag = false;
			ShadowMessageQueue shadowMessageQueue = null;
			ShadowRedundancyManager.QualifiedPrimaryServerId qualifiedPrimaryServerId = new ShadowRedundancyManager.QualifiedPrimaryServerId(shadowMailItem.NextHopSolution.NextHopSolutionKey);
			try
			{
				this.AcquireMailItem(shadowMailItem.TransportMailItem);
				lock (this.syncQueues)
				{
					if (!this.serverIdToShadowQueueMap.TryGetValue(qualifiedPrimaryServerId, out shadowMessageQueue))
					{
						shadowMessageQueue = this.CreateQueue(qualifiedPrimaryServerId, null, null);
					}
					shadowMessageQueue.AddReference();
					flag = true;
				}
				shadowMessageQueue.Enqueue(shadowMailItem);
				lock (shadowMailItem.TransportMailItem)
				{
					if (DateTime.UtcNow < ((IQueueItem)shadowMailItem).Expiry)
					{
						foreach (MailRecipient mailRecipient in shadowMailItem.NextHopSolution.Recipients)
						{
							mailRecipient.PrimaryServerFqdnGuid = qualifiedPrimaryServerId.ToString();
						}
					}
					shadowMailItem.TransportMailItem.CommitLazy();
				}
				ShadowRedundancyManager.PerfCounters.IncrementCounter(ShadowRedundancyCounterId.AggregateShadowQueueLength);
			}
			finally
			{
				if (shadowMessageQueue != null && flag)
				{
					shadowMessageQueue.ReleaseReference();
				}
			}
		}

		private ShadowMailItem CreateShadowMailItem(TransportMailItem mailItem, NextHopSolution shadowNextHopSolution)
		{
			if (shadowNextHopSolution.NextHopSolutionKey.NextHopType.DeliveryType != DeliveryType.ShadowRedundancy)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "shadowNextHopSolution has invalid delivery type '{0}'.", new object[]
				{
					shadowNextHopSolution.NextHopSolutionKey.NextHopType.DeliveryType
				}));
			}
			DateTime dateTime = DateTime.UtcNow;
			DateTime expiryTime = mailItem.GetExpiryTime(false);
			if (expiryTime < dateTime)
			{
				dateTime = expiryTime;
			}
			return new ShadowMailItem(mailItem, shadowNextHopSolution, dateTime, this.configurationSource);
		}

		private ShadowMailItem CreateShadowMailItem(TransportMailItem mailItem, string serverFqdn)
		{
			NextHopSolutionKey key = NextHopSolutionKey.CreateShadowNextHopSolutionKey(serverFqdn, Guid.Empty, null);
			ShadowMailItem result;
			lock (mailItem)
			{
				NextHopSolution shadowNextHopSolution = mailItem.UpdateNextHopSolutionTable(key, mailItem.Recipients.All, true);
				result = this.CreateShadowMailItem(mailItem, shadowNextHopSolution);
			}
			return result;
		}

		private void AcquireMailItem(TransportMailItem mailItem)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			long recordId;
			lock (mailItem)
			{
				recordId = mailItem.RecordId;
			}
			lock (this.syncAllMailItems)
			{
				this.allMailItemsByRecordId[recordId] = mailItem;
			}
		}

		private void DiscardShadowResourcesForServer(string serverFqdnOrContext)
		{
			List<ShadowMessageQueue> queues = this.GetQueues(serverFqdnOrContext);
			foreach (ShadowMessageQueue shadowMessageQueue in queues)
			{
				shadowMessageQueue.DiscardAll();
			}
			ShadowRedundancyManager.ShadowServerInfo shadowServerInfo = this.GetShadowServerInfo(serverFqdnOrContext, false);
			if (shadowServerInfo != null)
			{
				shadowServerInfo.ClearDiscardEvents();
			}
		}

		private void EnqueueDiscardEventForShadowServer(TransportMailItem mailItem, DateTime mailItemExpiryTime)
		{
			if (!this.configurationSource.Enabled)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string, string>(0L, "Ignored discard event '{0}:{1}' due to shadow redundancy being disabled.", mailItem.ShadowServerContext, mailItem.ShadowServerDiscardId);
				ShadowRedundancyManager.MarkMailItemAsDiscardCompleted(mailItem);
				return;
			}
			ShadowRedundancyManager.ShadowServerInfo shadowServerInfo = this.GetShadowServerInfo(mailItem.ShadowServerContext, true);
			DateTime dateTime = DateTime.UtcNow.Add(this.configurationSource.DiscardEventExpireInterval);
			DateTime dateTime2 = (mailItemExpiryTime < dateTime) ? mailItemExpiryTime : dateTime;
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<DateTime, DateTime, DateTime>(0L, "Effective discard expiry time {0}, Mail item expiry time {1}, Discard interval based expirty time {2}", dateTime2, mailItemExpiryTime, dateTime);
			if (DateTime.UtcNow > mailItemExpiryTime)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string, string>(mailItem.MsgId, "Expired mail item pending discard, dropping the message. server context '{0}' and discard id '{1}'", mailItem.ShadowServerContext, mailItem.ShadowServerDiscardId);
				ShadowRedundancyManager.MarkMailItemAsDiscardCompleted(mailItem);
				return;
			}
			shadowServerInfo.EnqueueDiscardEvent(mailItem, dateTime2);
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<DateTime, string, string>(0L, "Enqueued discard event with expiry time '{0}', server context '{1}' and discard id '{2}'", dateTime2, mailItem.ShadowServerContext, mailItem.ShadowServerDiscardId);
		}

		private ShadowRedundancyManager.ShadowServerInfo GetShadowServerInfo(string serverContext, bool createIfNotFound)
		{
			this.shadowServerInfoMapReaderWriterLock.EnterUpgradeableReadLock();
			ShadowRedundancyManager.ShadowServerInfo shadowServerInfo;
			try
			{
				if (!this.shadowServerInfoMap.TryGetValue(serverContext, out shadowServerInfo) && createIfNotFound)
				{
					this.shadowServerInfoMapReaderWriterLock.EnterWriteLock();
					try
					{
						if (!this.shadowServerInfoMap.TryGetValue(serverContext, out shadowServerInfo))
						{
							serverContext = this.serverContextStringPool.Intern(serverContext);
							shadowServerInfo = new ShadowRedundancyManager.ShadowServerInfo(serverContext);
							this.shadowServerInfoMap.Add(serverContext, shadowServerInfo);
						}
					}
					finally
					{
						this.shadowServerInfoMapReaderWriterLock.ExitWriteLock();
					}
				}
			}
			finally
			{
				this.shadowServerInfoMapReaderWriterLock.ExitUpgradeableReadLock();
			}
			if (shadowServerInfo == null && createIfNotFound)
			{
				throw new InvalidOperationException("shadowServerInfo should never be null if createIfNotFound = true.");
			}
			return shadowServerInfo;
		}

		private void NotifyMailItemCreatedInShadowGroup(ShadowRedundancyManager.QualifiedShadowServerDiscardId qualifiedDiscardId)
		{
			lock (this.shadowGroupMap)
			{
				int num;
				if (!this.shadowGroupMap.TryGetValue(qualifiedDiscardId, out num))
				{
					num = 1;
				}
				else
				{
					num++;
				}
				this.shadowGroupMap[qualifiedDiscardId] = num;
			}
		}

		private void DelayedAckCleanupCallback(object state)
		{
			if (this.delayedAckEntries.Count == 0)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if ((utcNow - this.lastDelayedCleanupCompleteTime).Ticks < this.configurationSource.DelayedAckCheckExpiryInterval.Ticks / 2L)
			{
				return;
			}
			List<KeyValuePair<Guid, ShadowRedundancyManager.DelayedAckEntry>> list = null;
			lock (this.delayedAckEntries)
			{
				foreach (KeyValuePair<Guid, ShadowRedundancyManager.DelayedAckEntry> item in this.delayedAckEntries)
				{
					ShadowRedundancyManager.DelayedAckEntry value = item.Value;
					value.CheckForExpiry(utcNow);
					if (value.CompletionStatus != null)
					{
						if (list == null)
						{
							list = new List<KeyValuePair<Guid, ShadowRedundancyManager.DelayedAckEntry>>();
						}
						list.Add(item);
					}
				}
				if (list != null)
				{
					foreach (KeyValuePair<Guid, ShadowRedundancyManager.DelayedAckEntry> keyValuePair in list)
					{
						this.FinalizeDelayedAck(keyValuePair.Key, keyValuePair.Value, utcNow);
					}
				}
			}
			this.lastDelayedCleanupCompleteTime = DateTime.UtcNow;
			if (list != null)
			{
				list.Clear();
				list = null;
			}
		}

		private void DiscardsCleanupCallback(object state)
		{
			if (this.shuttingDown)
			{
				return;
			}
			DateTime.UtcNow.Subtract(this.configurationSource.DiscardEventExpireInterval);
			this.shadowServerInfoMapReaderWriterLock.EnterWriteLock();
			try
			{
				List<string> list = new List<string>();
				foreach (ShadowRedundancyManager.ShadowServerInfo shadowServerInfo in this.shadowServerInfoMap.Values)
				{
					shadowServerInfo.DeleteExpiredDiscardEvents();
					if (shadowServerInfo.CanBeDeleted(this.configurationSource.ShadowServerInfoMaxIdleTimeInterval))
					{
						list.Add(shadowServerInfo.ServerContext);
					}
				}
				foreach (string key in list)
				{
					this.shadowServerInfoMap.Remove(key);
				}
			}
			finally
			{
				this.shadowServerInfoMapReaderWriterLock.ExitWriteLock();
			}
		}

		private void StringPoolExpirationCallback(object state)
		{
			this.serverContextStringPool = new StringPool(StringComparer.OrdinalIgnoreCase);
		}

		private void ProcessPendingDiscardEvents()
		{
			lock (this.discardEventsToApplyAfterBootLoader)
			{
				foreach (KeyValuePair<ShadowRedundancyManager.QualifiedPrimaryServerId, List<Guid>> keyValuePair in this.discardEventsToApplyAfterBootLoader)
				{
					int num;
					this.ApplyDiscardEvents(keyValuePair.Key, keyValuePair.Value, out num);
					if (num > 0)
					{
						ExTraceGlobals.ShadowRedundancyTracer.TraceError<int, ShadowRedundancyManager.QualifiedPrimaryServerId, int>(0L, "Applied '{0}' discard events from primary server '{1}' with '{2}' not found.", keyValuePair.Value.Count, keyValuePair.Key, num);
					}
					else
					{
						ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<int, ShadowRedundancyManager.QualifiedPrimaryServerId>(0L, "Applied '{0}' discard events from primary server '{1}'.", keyValuePair.Value.Count, keyValuePair.Key);
					}
				}
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "All pending discard events were applied after boot loader complete.");
			}
			this.discardEventsToApplyAfterBootLoader = null;
		}

		private List<Guid> ConvertMessageIdsFromStringToGuid(ICollection<string> messageIds)
		{
			List<Guid> list = new List<Guid>(messageIds.Count);
			foreach (string text in messageIds)
			{
				Guid item;
				if (!GuidHelper.TryParseGuid(text, out item))
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceError<string>(0L, "ApplyDiscardEvents(): Discard id '{0}' is not a Guid.", text);
				}
				else
				{
					list.Add(item);
				}
			}
			return list;
		}

		private void ApplyDiscardEvents(ShadowRedundancyManager.QualifiedPrimaryServerId serverId, ICollection<Guid> messageIds, out int notFound)
		{
			notFound = 0;
			if (!this.configurationSource.Enabled)
			{
				return;
			}
			if (messageIds.Count == 0)
			{
				return;
			}
			if (!this.bootLoaderComplete)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "ApplyDiscardEvents suppressing because bootloader is not finished");
				lock (this.discardEventsToApplyAfterBootLoader)
				{
					List<Guid> list;
					if (!this.discardEventsToApplyAfterBootLoader.TryGetValue(serverId, out list))
					{
						this.discardEventsToApplyAfterBootLoader.Add(serverId, new List<Guid>(messageIds));
					}
					else
					{
						list.AddRange(messageIds);
					}
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string, int>(0L, "ApplyDiscardEvents(serverId='{0}', messages='{1}') - message(s) queued since boot loader is still running.", serverId.ToString(), messageIds.Count);
				}
				return;
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string, int>(0L, "ApplyDiscardEvents(serverId='{0}', messages='{1}')", serverId.ToString(), messageIds.Count);
			ShadowMessageQueue queue = this.GetQueue(serverId);
			if (queue == null)
			{
				notFound = messageIds.Count;
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<ShadowRedundancyManager.QualifiedPrimaryServerId, int>(0L, "ApplyDiscardEvents(): Shadow queues for '{0}' not found.  '{1}' discard event(s) not found.", serverId, notFound);
				return;
			}
			foreach (Guid guid in messageIds)
			{
				if (!queue.Discard(guid, DiscardReason.ExplicitlyDiscarded))
				{
					notFound++;
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<Guid, ShadowRedundancyManager.QualifiedPrimaryServerId>(0L, "ApplyDiscardEvents(): ShadowMailItem with shadow message id '{0}' not found in any queues for '{1}'.", guid, serverId);
				}
			}
		}

		private void MarkShadowMailAsDelivered(ShadowMailItem shadowMailItem, string serverFqdn)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<Guid, string>(0L, "MarkShadowMailAsDelivered(): Marking shadow message id '{0}' delivered from server '{1}'.", shadowMailItem.TransportMailItem.ShadowMessageId, serverFqdn);
			PrimaryServerInfo active = this.primaryServerInfoMap.GetActive(serverFqdn);
			if (active == null)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceWarning<string>(0L, "MarkShadowMailAsDelivered(): Primary server '{0}' not found in map.", serverFqdn);
				return;
			}
			Destination deliveredDestination = new Destination(Destination.DestinationType.Shadow, active.DatabaseState);
			foreach (MailRecipient mailRecipient in shadowMailItem.TransportMailItem.Recipients)
			{
				mailRecipient.DeliveredDestination = deliveredDestination;
				mailRecipient.DeliveryTime = new DateTime?(DateTime.UtcNow);
				Components.MessageResubmissionComponent.StoreRecipient(mailRecipient);
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<MailRecipient>(0L, "MarkShadowMailAsDelivered(): Moved recipient '{0}' to safety net.", mailRecipient);
			}
			SystemProbeHelper.ShadowRedundancyTracer.TracePass<string>(shadowMailItem.TransportMailItem, 0L, "Message delivered by {0} and moved to Safety Net shadow", serverFqdn);
		}

		private void NotifyMailItemInstanceHandled(TransportMailItem mailItem)
		{
			ShadowRedundancyManager.QualifiedShadowServerDiscardId qualifiedShadowServerDiscardId = new ShadowRedundancyManager.QualifiedShadowServerDiscardId(mailItem.ShadowServerContext, mailItem.ShadowServerDiscardId);
			bool flag = false;
			lock (this.shadowGroupMap)
			{
				int num;
				if (this.shadowGroupMap.TryGetValue(qualifiedShadowServerDiscardId, out num))
				{
					if (num <= 0)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Number of bifurcations for message '{0}' is '{1}'.  This number should always be positive.", new object[]
						{
							qualifiedShadowServerDiscardId.ToString(),
							num
						}));
					}
					if (num > 1)
					{
						num = (this.shadowGroupMap[qualifiedShadowServerDiscardId] = num - 1);
					}
					else
					{
						this.shadowGroupMap.Remove(qualifiedShadowServerDiscardId);
					}
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.NotifyMailItemHandled(mailItem, qualifiedShadowServerDiscardId, ((IQueueItem)mailItem).Expiry);
			}
		}

		private void NotifyMailItemHandled(TransportMailItem mailItem, ShadowRedundancyManager.QualifiedShadowServerDiscardId qualifiedDiscardId, DateTime mailItemExpiryTime)
		{
			if (qualifiedDiscardId.IsDelayedAck)
			{
				Guid guid;
				if (GuidHelper.TryParseGuid(qualifiedDiscardId.ServerDiscardId, out guid))
				{
					bool flag = false;
					ShadowRedundancyManager.DelayedAckEntry delayedAckEntry;
					lock (this.delayedAckEntries)
					{
						if (this.delayedAckEntries.TryGetValue(guid, out delayedAckEntry))
						{
							flag = true;
							this.delayedAckEntries.Remove(guid);
						}
					}
					if (flag)
					{
						ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string>(0L, "NotifyMailItemHandled(): Delayed ack message with ShadowMessageId '{0}' delivered successfully before timeout.  Notifying Smtp...", qualifiedDiscardId.ServerDiscardId);
						delayedAckEntry.MarkAsDelivered();
						if (this.delayedAckCompleted(delayedAckEntry.State, delayedAckEntry.CompletionStatus.Value, DateTime.UtcNow.Subtract(delayedAckEntry.StartTime), delayedAckEntry.Context))
						{
							return;
						}
						lock (this.delayedAckEntries)
						{
							TransportHelpers.AttemptAddToDictionary<Guid, ShadowRedundancyManager.DelayedAckEntry>(this.delayedAckEntries, guid, delayedAckEntry, null);
							return;
						}
					}
					ShadowRedundancyManager.PerfCounters.DelayedAckDeliveredAfterExpiry(1L);
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string>(0L, "NotifyMailItemHandled(): Delayed ack message with ShadowMessageId '{0}' has been delivered after delayed ack expired.", qualifiedDiscardId.ServerDiscardId);
					return;
				}
				ExTraceGlobals.ShadowRedundancyTracer.TraceError<string>(0L, "NotifyMailItemHandled(): ShadowMessageId '{0}' is not a valid Guid, delayed ack messages must have a Guid discard ids.  Generate Watson...", qualifiedDiscardId.ServerDiscardId);
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid ServerDiscardId ('{0}') for a delayed ack message.", new object[]
				{
					qualifiedDiscardId.ServerDiscardId
				}));
			}
			else
			{
				ShadowRedundancyManager.MarkMailItemAsDiscardPending(mailItem);
				this.EnqueueDiscardEventForShadowServer(mailItem, mailItemExpiryTime);
				SystemProbeHelper.ShadowRedundancyTracer.TracePass(mailItem, 0L, "Message delivered by primary hub server and moved to Safety Net.");
			}
		}

		private ShadowMessageQueue GetQueue(ShadowRedundancyManager.QualifiedPrimaryServerId primaryServerId)
		{
			ShadowMessageQueue shadowMessageQueue;
			lock (this.syncQueues)
			{
				this.serverIdToShadowQueueMap.TryGetValue(primaryServerId, out shadowMessageQueue);
			}
			if (shadowMessageQueue == null)
			{
				ExTraceGlobals.ShadowRedundancyTracer.TraceError<ShadowRedundancyManager.QualifiedPrimaryServerId>(0L, "Shadow Queue '{0}' not found", primaryServerId);
			}
			return shadowMessageQueue;
		}

		private List<ShadowMessageQueue> GetQueues(string primaryFqdn)
		{
			List<ShadowMessageQueue> result;
			lock (this.syncQueues)
			{
				List<ShadowMessageQueue> collection;
				if (!this.serverFqdnToShadowQueueMap.TryGetValue(primaryFqdn, out collection))
				{
					result = new List<ShadowMessageQueue>();
				}
				else
				{
					result = new List<ShadowMessageQueue>(collection);
				}
			}
			return result;
		}

		private void AddQueue(ShadowRedundancyManager.QualifiedPrimaryServerId serverId, ShadowMessageQueue queue)
		{
			if (serverId.Equals(ShadowRedundancyManager.QualifiedPrimaryServerId.Empty))
			{
				throw new ArgumentException("serverId is empty");
			}
			TransportHelpers.AttemptAddToDictionary<ShadowRedundancyManager.QualifiedPrimaryServerId, ShadowMessageQueue>(this.serverIdToShadowQueueMap, serverId, queue, null);
			TransportHelpers.AttemptAddToDictionary<long, ShadowMessageQueue>(this.queueIdToShadowQueueMap, queue.Id, queue, null);
			List<ShadowMessageQueue> list;
			if (!this.serverFqdnToShadowQueueMap.TryGetValue(serverId.Fqdn, out list))
			{
				list = new List<ShadowMessageQueue>();
				this.serverFqdnToShadowQueueMap.Add(serverId.Fqdn, list);
			}
			list.Add(queue);
		}

		private void ProcessPendingResubmits()
		{
			if (this.queuedRecoveryResubmits.Count > 0)
			{
				string[] array;
				lock (this.queuedRecoveryResubmits)
				{
					array = this.queuedRecoveryResubmits.ToArray();
					this.queuedRecoveryResubmits.Clear();
				}
				foreach (string serverFqdn in array)
				{
					this.ResubmitAsync(serverFqdn, ResubmitReason.ShadowStateChange);
				}
			}
		}

		private void ResubmitAsync(string serverFqdn, ResubmitReason resubmitReason)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string, ResubmitReason>(0L, "ResubmitAsync(): Scheduling a resubmit for server '{0}' with resubmit reason '{1}'.", serverFqdn, resubmitReason);
			lock (this.activeResubmitsSync)
			{
				this.activeResubmits++;
				if (this.activeResubmits == 1)
				{
					this.resubmitEvent.Reset();
				}
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ResubmitCallback), new ShadowRedundancyManager.ResubmitState(serverFqdn, resubmitReason));
		}

		private void ResubmitCallback(object state)
		{
			ShadowRedundancyManager.ResubmitState resubmitState = (ShadowRedundancyManager.ResubmitState)state;
			this.ResubmitInternal(resubmitState.ServerFqdn, resubmitState.ResubmitReason);
			lock (this.activeResubmitsSync)
			{
				this.activeResubmits--;
				if (this.activeResubmits == 0)
				{
					this.resubmitEvent.Set();
				}
				else if (this.activeResubmits < 0)
				{
					throw new InvalidOperationException("this.activeResubmits shouldn't be negative.");
				}
			}
		}

		private void ResubmitInternal(string serverFqdn, ResubmitReason resubmitReason)
		{
			ICollection<ShadowMessageQueue> queues = this.GetQueues(serverFqdn);
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<int, string, ResubmitReason>(0L, "ResubmitInternal(): Resubmit '{0}' queues for server '{1}' with resubmit reason '{2}'.", queues.Count, serverFqdn, resubmitReason);
			int num = 0;
			foreach (ShadowMessageQueue shadowMessageQueue in queues)
			{
				num += shadowMessageQueue.Resubmit(resubmitReason);
			}
			ShadowRedundancyManager.CompletedResubmitEntry item = new ShadowRedundancyManager.CompletedResubmitEntry(DateTime.UtcNow, serverFqdn, resubmitReason, num);
			lock (this.completedResubmitEntries)
			{
				this.completedResubmitEntries.Enqueue(item);
				if (this.completedResubmitEntries.Count > 1000)
				{
					this.completedResubmitEntries.Dequeue();
				}
			}
		}

		private void UpdateHeartbeat(string primaryServer, bool successful)
		{
			List<ShadowMessageQueue> queues = this.GetQueues(primaryServer);
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string, int, bool>(0L, "UpdateHeartbeat({0}): updating {1} queues with status {2}", primaryServer, queues.Count, successful);
			if (queues.Count > 0)
			{
				DateTime utcNow = DateTime.UtcNow;
				foreach (ShadowMessageQueue shadowMessageQueue in queues)
				{
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<NextHopSolutionKey, bool>(0L, "UpdateHeartbeat: updating queue {0} with status {1}", shadowMessageQueue.Key, successful);
					shadowMessageQueue.UpdateHeartbeat(utcNow, NextHopSolutionKey.Empty, successful);
				}
			}
		}

		private void NotifyConfigUpdated(IShadowRedundancyConfigurationSource oldConfiguration)
		{
			if (oldConfiguration == null)
			{
				throw new ArgumentNullException("oldConfiguration");
			}
			if (!this.configurationSource.Enabled)
			{
				lock (this.syncQueues)
				{
					foreach (List<ShadowMessageQueue> list in this.serverFqdnToShadowQueueMap.Values)
					{
						foreach (ShadowMessageQueue shadowMessageQueue in list)
						{
							shadowMessageQueue.DiscardAll();
						}
					}
				}
				try
				{
					this.shadowServerInfoMapReaderWriterLock.EnterWriteLock();
					this.shadowServerInfoMap.Clear();
					ShadowRedundancyManager.PerfCounters.DecrementCounterBy(ShadowRedundancyCounterId.RedundantMessageDiscardEvents, ShadowRedundancyManager.PerfCounters.RedundantMessageDiscardEvents);
				}
				finally
				{
					this.shadowServerInfoMapReaderWriterLock.ExitWriteLock();
				}
				lock (this.shadowGroupMap)
				{
					this.shadowGroupMap.Clear();
				}
			}
			if (this.configurationSource.HeartbeatRetryCount != oldConfiguration.HeartbeatRetryCount || this.configurationSource.HeartbeatFrequency != oldConfiguration.HeartbeatFrequency)
			{
				lock (this.syncQueues)
				{
					foreach (ShadowMessageQueue shadowMessageQueue2 in this.serverIdToShadowQueueMap.Values)
					{
						shadowMessageQueue2.NotifyConfigUpdated(oldConfiguration);
					}
				}
			}
		}

		private bool ValidCandidateForDelayedAckSkipping(IReadOnlyMailItem mailItem, TransportMessageQueue queue)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (queue == null)
			{
				throw new ArgumentNullException("queue");
			}
			return this.configurationSource.Enabled && this.configurationSource.DelayedAckSkippingEnabled && mailItem.IsDelayedAck();
		}

		private string GetQueueId(TransportMessageQueue queue, out SubmitMessageQueue submitMessageQueue, out RoutedMessageQueue routedMessageQueue, out UnreachableMessageQueue unreachableMessageQueue)
		{
			routedMessageQueue = null;
			unreachableMessageQueue = null;
			submitMessageQueue = (queue as SubmitMessageQueue);
			if (submitMessageQueue != null)
			{
				return "Submission";
			}
			routedMessageQueue = (queue as RoutedMessageQueue);
			if (routedMessageQueue != null)
			{
				return routedMessageQueue.Key.NextHopDomain;
			}
			unreachableMessageQueue = (queue as UnreachableMessageQueue);
			if (unreachableMessageQueue != null)
			{
				return "Unreachable";
			}
			return null;
		}

		private void LogNewShadowMailItem(ShadowMailItem shadowItem)
		{
			MessageTrackingLog.TrackHighAvailabilityReceive(MessageTrackingSource.SMTP, shadowItem.NextHopSolution.NextHopSolutionKey.NextHopDomain, shadowItem.TransportMailItem);
		}

		public const string CurrentServerToken = "$localhost$";

		public const string PrioritizationReason = "ShadowRedundancy";

		public const string DeletedState = "0de1e7ed-0de1-0de1-0de1-de1e7edele7e";

		private const int MaxCompletedResubmitEntries = 1000;

		private const string SubmissionQueue = "Submission";

		private const string UnreachableQueue = "Unreachable";

		internal static readonly ITracer ReceiveTracer = new CompositeTracer(ExTraceGlobals.SmtpReceiveTracer, ExTraceGlobals.ShadowRedundancyTracer);

		internal static readonly ITracer SendTracer = new CompositeTracer(ExTraceGlobals.SmtpSendTracer, ExTraceGlobals.ShadowRedundancyTracer);

		private static IShadowRedundancyPerformanceCounters shadowRedundancyPerformanceCounters;

		private readonly Dictionary<long, TransportMailItem> allMailItemsByRecordId = new Dictionary<long, TransportMailItem>();

		private readonly Dictionary<ShadowRedundancyManager.QualifiedPrimaryServerId, ShadowMessageQueue> serverIdToShadowQueueMap = new Dictionary<ShadowRedundancyManager.QualifiedPrimaryServerId, ShadowMessageQueue>();

		private readonly Dictionary<string, List<ShadowMessageQueue>> serverFqdnToShadowQueueMap = new Dictionary<string, List<ShadowMessageQueue>>(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<long, ShadowMessageQueue> queueIdToShadowQueueMap = new Dictionary<long, ShadowMessageQueue>();

		private readonly Dictionary<ShadowRedundancyManager.QualifiedShadowServerDiscardId, int> shadowGroupMap = new Dictionary<ShadowRedundancyManager.QualifiedShadowServerDiscardId, int>();

		private readonly Dictionary<string, ShadowRedundancyManager.ShadowServerInfo> shadowServerInfoMap = new Dictionary<string, ShadowRedundancyManager.ShadowServerInfo>(StringComparer.OrdinalIgnoreCase);

		private readonly IPrimaryServerInfoMap primaryServerInfoMap;

		private readonly Guid databaseId;

		private ShadowRedundancyEventLogger shadowRedundancyEventLogger;

		private DelayedAckItemHandler delayedAckCompleted;

		private Dictionary<Guid, ShadowRedundancyManager.DelayedAckEntry> delayedAckEntries = new Dictionary<Guid, ShadowRedundancyManager.DelayedAckEntry>();

		private DateTime lastPrimaryServerInfoMapCleanup = DateTime.UtcNow;

		private IShadowRedundancyConfigurationSource configurationSource;

		private object syncQueues = new object();

		private object syncAllMailItems = new object();

		private ReaderWriterLockSlim shadowServerInfoMapReaderWriterLock = new ReaderWriterLockSlim();

		private Dictionary<ShadowRedundancyManager.QualifiedPrimaryServerId, List<Guid>> discardEventsToApplyAfterBootLoader = new Dictionary<ShadowRedundancyManager.QualifiedPrimaryServerId, List<Guid>>();

		private List<string> queuedRecoveryResubmits = new List<string>();

		private int activeResubmits;

		private ManualResetEvent resubmitEvent = new ManualResetEvent(true);

		private object activeResubmitsSync = new object();

		private HashSet<ShadowRedundancyManager.QualifiedShadowServerDiscardId> hashsetToDetectShadowGroupsOnStartup = new HashSet<ShadowRedundancyManager.QualifiedShadowServerDiscardId>();

		private bool bootLoaderComplete;

		private StringPool serverContextStringPool;

		private GuardedTimer discardsCleanupTimer;

		private GuardedTimer delayedAckCleanupTimer;

		private DateTime lastDelayedCleanupCompleteTime = DateTime.MinValue;

		private int primaryServerInfoMapCleanupGuard;

		private GuardedTimer stringPoolCleanupTimer;

		private bool shuttingDown;

		private object timerCreateDestroySync = new object();

		private Queue<ShadowRedundancyManager.CompletedResubmitEntry> completedResubmitEntries = new Queue<ShadowRedundancyManager.CompletedResubmitEntry>();

		private IMessagingDatabase database;

		private ShadowSessionFactory shadowSessionFactory;

		private bool heartbeatEnabled;

		private ServiceState targetRunningState;

		private bool paused;

		internal struct QualifiedPrimaryServerId
		{
			public QualifiedPrimaryServerId(NextHopSolutionKey key)
			{
				this = new ShadowRedundancyManager.QualifiedPrimaryServerId(key.NextHopDomain, key.NextHopConnector, key.NextHopTlsDomain);
			}

			public QualifiedPrimaryServerId(string primaryFqdn, Guid nextHopGuid)
			{
				this = new ShadowRedundancyManager.QualifiedPrimaryServerId(primaryFqdn, nextHopGuid, string.Empty);
			}

			public QualifiedPrimaryServerId(string primaryFqdn, Guid nextHopGuid, string tlsDomain)
			{
				if (string.IsNullOrEmpty(primaryFqdn))
				{
					throw new ArgumentNullException("primaryFqdn");
				}
				if (tlsDomain == null)
				{
					throw new ArgumentNullException("tlsDomain");
				}
				this.fqdn = primaryFqdn;
				this.nextHopGuid = nextHopGuid;
				this.tlsDomain = tlsDomain;
			}

			public string Fqdn
			{
				get
				{
					return this.fqdn;
				}
			}

			public Guid NextHopGuid
			{
				get
				{
					return this.nextHopGuid;
				}
			}

			public string TlsDomain
			{
				get
				{
					if (this.tlsDomain != null)
					{
						return this.tlsDomain;
					}
					return string.Empty;
				}
			}

			public static bool TryParse(string stringToParse, out ShadowRedundancyManager.QualifiedPrimaryServerId primaryServerId)
			{
				if (string.IsNullOrEmpty(stringToParse))
				{
					throw new ArgumentNullException("stringToParse");
				}
				primaryServerId = ShadowRedundancyManager.QualifiedPrimaryServerId.Empty;
				string[] array = stringToParse.Split(new char[]
				{
					'/'
				});
				Guid empty = Guid.Empty;
				switch (array.Length)
				{
				case 1:
					primaryServerId = new ShadowRedundancyManager.QualifiedPrimaryServerId(stringToParse, Guid.Empty);
					break;
				case 2:
					if (array[0].Equals(string.Empty))
					{
						return false;
					}
					empty = Guid.Empty;
					if (!GuidHelper.TryParseGuid(array[1], out empty))
					{
						return false;
					}
					primaryServerId = new ShadowRedundancyManager.QualifiedPrimaryServerId(array[0], empty);
					break;
				case 3:
					if (array[0].Equals(string.Empty) || array[2].Equals(string.Empty))
					{
						primaryServerId = ShadowRedundancyManager.QualifiedPrimaryServerId.Empty;
						return false;
					}
					if (!GuidHelper.TryParseGuid(array[1], out empty))
					{
						return false;
					}
					primaryServerId = new ShadowRedundancyManager.QualifiedPrimaryServerId(array[0], empty, array[2]);
					break;
				}
				return true;
			}

			public override string ToString()
			{
				if (this.nextHopGuid == Guid.Empty)
				{
					return this.fqdn;
				}
				return string.Concat(new object[]
				{
					this.fqdn,
					'/',
					this.nextHopGuid.ToString(),
					string.IsNullOrEmpty(this.TlsDomain) ? string.Empty : ('/' + this.TlsDomain)
				});
			}

			public bool Equals(ShadowRedundancyManager.QualifiedPrimaryServerId other)
			{
				return this.nextHopGuid == other.nextHopGuid && this.fqdn.Equals(other.fqdn, StringComparison.OrdinalIgnoreCase) && this.TlsDomain.Equals(other.TlsDomain, StringComparison.OrdinalIgnoreCase);
			}

			public override bool Equals(object other)
			{
				return this.Equals((ShadowRedundancyManager.QualifiedPrimaryServerId)other);
			}

			public override int GetHashCode()
			{
				return StringComparer.OrdinalIgnoreCase.GetHashCode(this.fqdn) ^ this.nextHopGuid.GetHashCode() ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this.TlsDomain);
			}

			private const char PrimaryFqdnDelimiter = '/';

			public static readonly ShadowRedundancyManager.QualifiedPrimaryServerId Empty = default(ShadowRedundancyManager.QualifiedPrimaryServerId);

			private readonly string fqdn;

			private readonly Guid nextHopGuid;

			private readonly string tlsDomain;
		}

		internal struct QualifiedShadowServerDiscardId
		{
			public QualifiedShadowServerDiscardId(string serverContext, string serverDiscardId)
			{
				ShadowRedundancyManager.QualifiedShadowServerDiscardId.EnsureServerContextValid(serverContext);
				ShadowRedundancyManager.QualifiedShadowServerDiscardId.EnsureShadowDiscardIdValid(serverDiscardId);
				this.serverContext = serverContext;
				this.serverDiscardId = serverDiscardId;
			}

			public string ServerContext
			{
				get
				{
					return this.serverContext;
				}
			}

			public string ServerDiscardId
			{
				get
				{
					return this.serverDiscardId;
				}
			}

			public bool IsDelayedAck
			{
				get
				{
					return string.Equals(this.serverContext, "$localhost$", StringComparison.OrdinalIgnoreCase);
				}
			}

			public static void EnsureServerContextValid(string serverContext)
			{
				if (string.IsNullOrEmpty(serverContext))
				{
					throw new ArgumentNullException("serverContext");
				}
			}

			public static void EnsureShadowDiscardIdValid(string serverDiscardId)
			{
				if (string.IsNullOrEmpty(serverDiscardId))
				{
					throw new ArgumentNullException("serverDiscardId");
				}
			}

			public override string ToString()
			{
				return this.serverContext + "::" + this.serverDiscardId;
			}

			public bool Equals(ShadowRedundancyManager.QualifiedShadowServerDiscardId other)
			{
				return this.serverDiscardId.Equals(other.serverDiscardId, StringComparison.OrdinalIgnoreCase) && this.serverContext.Equals(other.serverContext, StringComparison.OrdinalIgnoreCase);
			}

			public override bool Equals(object other)
			{
				return this.Equals((ShadowRedundancyManager.QualifiedShadowServerDiscardId)other);
			}

			public override int GetHashCode()
			{
				return this.serverContext.GetHashCode() ^ this.serverDiscardId.GetHashCode();
			}

			private readonly string serverContext;

			private readonly string serverDiscardId;
		}

		private sealed class DelayedAckEntry
		{
			public DelayedAckEntry(object state, DateTime startTime, DateTime expiryTime)
			{
				if (state == null)
				{
					throw new ArgumentNullException("state");
				}
				this.state = state;
				this.startTime = startTime;
				this.expiryTime = expiryTime;
			}

			public object State
			{
				get
				{
					return this.state;
				}
			}

			public DateTime StartTime
			{
				get
				{
					return this.startTime;
				}
			}

			public DelayedAckCompletionStatus? CompletionStatus
			{
				get
				{
					return this.completionStatus;
				}
			}

			public string Context
			{
				get
				{
					return this.context;
				}
			}

			public void CheckForExpiry(DateTime expiryThreshold)
			{
				if (this.expiryTime <= expiryThreshold)
				{
					this.MarkAsExpired("Timeout");
				}
			}

			public void MarkAsDelivered()
			{
				if (this.completionStatus == null)
				{
					this.completionStatus = new DelayedAckCompletionStatus?(DelayedAckCompletionStatus.Delivered);
				}
			}

			public void MarkAsSkipped(string context)
			{
				if (this.completionStatus == null)
				{
					this.completionStatus = new DelayedAckCompletionStatus?(DelayedAckCompletionStatus.Skipped);
					this.context = context;
				}
			}

			public void MarkAsExpired(string context)
			{
				if (this.completionStatus == null)
				{
					this.completionStatus = new DelayedAckCompletionStatus?(DelayedAckCompletionStatus.Expired);
					this.context = context;
				}
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "[DelayedAckEntry: state='{0}' startTime='{1}' expiryTime='{2}' completionStatus='{3}' delay='{4}' context='{5}']", new object[]
				{
					this.state,
					this.startTime,
					this.expiryTime,
					(this.completionStatus != null) ? this.completionStatus.Value.ToString() : "<null>",
					DateTime.UtcNow - this.startTime,
					this.context ?? "<null>"
				});
			}

			private readonly DateTime startTime;

			private readonly DateTime expiryTime;

			private object state;

			private DelayedAckCompletionStatus? completionStatus;

			private string context;
		}

		private sealed class ResubmitState
		{
			public ResubmitState(string serverFqdn, ResubmitReason resubmitReason)
			{
				if (string.IsNullOrEmpty(serverFqdn))
				{
					throw new ArgumentNullException("serverFqdn");
				}
				ShadowMessageQueue.EnsureValidResubmitReason(resubmitReason);
				this.serverFqdn = serverFqdn;
				this.resubmitReason = resubmitReason;
			}

			public string ServerFqdn
			{
				get
				{
					return this.serverFqdn;
				}
			}

			public ResubmitReason ResubmitReason
			{
				get
				{
					return this.resubmitReason;
				}
			}

			private readonly string serverFqdn;

			private readonly ResubmitReason resubmitReason;
		}

		private sealed class ShadowServerInfo
		{
			public ShadowServerInfo(string serverContext)
			{
				ShadowRedundancyManager.QualifiedShadowServerDiscardId.EnsureServerContextValid(serverContext);
				this.serverContext = serverContext;
				this.lastActivity = DateTime.UtcNow;
			}

			public string ServerContext
			{
				get
				{
					return this.serverContext;
				}
			}

			public bool CanBeDeleted(TimeSpan idleTime)
			{
				bool result;
				lock (this)
				{
					result = ((this.queue == null || this.queue.Count == 0) && DateTime.UtcNow >= this.lastActivity.Add(idleTime));
				}
				return result;
			}

			public string[] DequeueDiscardEvents(int max)
			{
				if (this.queue == null)
				{
					return new string[0];
				}
				int num;
				string[] array;
				lock (this)
				{
					num = Math.Min(this.queue.Count, max);
					ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<int>(0L, "Preparing to return {0} discard ids", num);
					array = new string[num];
					for (int i = 0; i < num; i++)
					{
						ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow discardEntryForShadow = this.queue.Dequeue();
						array[i] = discardEntryForShadow.MessageId;
						ShadowRedundancyManager.MarkMailItemAsDiscardCompleted(discardEntryForShadow.MailItem);
						ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<string>(0L, "Dequeued discard id {0}", array[i] ?? "NULL");
					}
					this.lastActivity = DateTime.UtcNow;
				}
				ShadowRedundancyManager.PerfCounters.DecrementCounterBy(ShadowRedundancyCounterId.RedundantMessageDiscardEvents, (long)num);
				return array;
			}

			public XElement GetDiagnosticInfo(bool showVerbose)
			{
				XElement xelement = new XElement("ShadowServerInfo", new XElement("lastActivity", this.lastActivity));
				lock (this)
				{
					int num;
					if (this.queue == null || this.queue.Count == 0)
					{
						num = 0;
					}
					else
					{
						num = this.queue.Count;
					}
					xelement.Add(new XElement("discardEventsCount", num));
					if (showVerbose && this.queue != null)
					{
						using (Queue<ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow>.Enumerator enumerator = this.queue.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow discardEntryForShadow = enumerator.Current;
								xelement.Add(new XElement("discardEventMessageId", discardEntryForShadow.MessageId));
							}
							goto IL_E8;
						}
					}
					xelement.Add(new XElement("help", "Use 'verbose' argument to show discard events."));
					IL_E8:;
				}
				return xelement;
			}

			public void EnqueueDiscardEvent(TransportMailItem mailItem, DateTime expiryTime)
			{
				lock (this)
				{
					if (this.queue == null)
					{
						this.queue = new Queue<ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow>();
					}
					this.queue.Enqueue(new ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow(mailItem, expiryTime));
					this.lastActivity = DateTime.UtcNow;
				}
				ShadowRedundancyManager.PerfCounters.IncrementCounter(ShadowRedundancyCounterId.RedundantMessageDiscardEvents);
			}

			public void ClearDiscardEvents()
			{
				int num = 0;
				lock (this)
				{
					num = this.queue.Count;
					foreach (ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow discardEntryForShadow in this.queue)
					{
						ShadowRedundancyManager.MarkMailItemAsDiscardCompleted(discardEntryForShadow.MailItem);
					}
					this.queue = null;
					this.lastActivity = DateTime.UtcNow;
				}
				ShadowRedundancyManager.PerfCounters.DecrementCounterBy(ShadowRedundancyCounterId.RedundantMessageDiscardEvents, (long)num);
			}

			public void DeleteExpiredDiscardEvents()
			{
				if (this.queue == null)
				{
					return;
				}
				int num = 0;
				lock (this)
				{
					foreach (ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow discardEntryForShadow in this.queue)
					{
						if (discardEntryForShadow.IsExpired())
						{
							num++;
						}
					}
					if (num == 0)
					{
						return;
					}
					Queue<ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow> queue;
					if (this.queue.Count - num > 0)
					{
						queue = new Queue<ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow>(this.queue.Count - num);
					}
					else
					{
						queue = new Queue<ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow>();
					}
					foreach (ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow item in this.queue)
					{
						if (item.IsExpired())
						{
							ShadowRedundancyManager.MarkMailItemAsDiscardCompleted(item.MailItem);
						}
						else
						{
							queue.Enqueue(item);
						}
					}
					this.queue = queue;
				}
				if (num > 0)
				{
					ShadowRedundancyManager.PerfCounters.IncrementCounterBy(ShadowRedundancyCounterId.RedundantMessageDiscardEventsExpired, (long)num);
					ShadowRedundancyManager.PerfCounters.DecrementCounterBy(ShadowRedundancyCounterId.RedundantMessageDiscardEvents, (long)num);
				}
			}

			public override int GetHashCode()
			{
				return this.serverContext.GetHashCode();
			}

			private readonly string serverContext;

			private DateTime lastActivity;

			private Queue<ShadowRedundancyManager.ShadowServerInfo.DiscardEntryForShadow> queue;

			private struct DiscardEntryForShadow
			{
				public DiscardEntryForShadow(TransportMailItem mailItem, DateTime expiryTime)
				{
					ShadowRedundancyManager.QualifiedShadowServerDiscardId.EnsureShadowDiscardIdValid(mailItem.ShadowServerDiscardId);
					this.mailItem = mailItem;
					this.expiryTime = expiryTime;
					this.discardId = mailItem.ShadowServerDiscardId;
				}

				public TransportMailItem MailItem
				{
					get
					{
						return this.mailItem;
					}
				}

				public string MessageId
				{
					get
					{
						return this.discardId;
					}
				}

				public bool IsExpired()
				{
					return this.expiryTime <= DateTime.UtcNow;
				}

				private readonly TransportMailItem mailItem;

				private readonly DateTime expiryTime;

				private readonly string discardId;
			}
		}

		private sealed class PrimaryServerInfoScanner : ChunkingScanner
		{
			public PrimaryServerInfoScanner(IMessagingDatabase database)
			{
				this.database = database;
			}

			public void Load(IPrimaryServerInfoMap primaryServerInfoMap)
			{
				if (primaryServerInfoMap == null)
				{
					throw new ArgumentNullException("primaryServerInfoMap");
				}
				if (primaryServerInfoMap.Count > 0)
				{
					throw new ArgumentException("primaryServerInfoMap must be empty.");
				}
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "PrimaryServerInfoScanner.Load(): Started loading primary server info from database.");
				this.primaryServerInfoMap = primaryServerInfoMap;
				using (DataTableCursor cursor = this.database.ServerInfoTable.GetCursor())
				{
					using (Transaction transaction = cursor.BeginTransaction())
					{
						this.Scan(transaction, cursor, true);
					}
				}
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<int>(0L, "PrimaryServerInfoScanner.Load(): Loaded '{0}' primary server info records from database (including current server state).", this.primaryServerInfoMap.Count);
				this.primaryServerInfoMap = null;
			}

			protected override ChunkingScanner.ScanControl HandleRecord(DataTableCursor cursor)
			{
				PrimaryServerInfo primaryServerInfo = PrimaryServerInfo.Load(cursor, this.database.ServerInfoTable);
				this.primaryServerInfoMap.Add(primaryServerInfo);
				return ChunkingScanner.ScanControl.Continue;
			}

			private IPrimaryServerInfoMap primaryServerInfoMap;

			private IMessagingDatabase database;
		}

		private sealed class CompletedResubmitEntry
		{
			public CompletedResubmitEntry(DateTime timestamp, string primaryServerFqdn, ResubmitReason reason, int messageCount)
			{
				this.timestamp = timestamp;
				this.primaryServerFqdn = primaryServerFqdn;
				this.reason = reason;
				this.messageCount = messageCount;
			}

			public DateTime Timestamp
			{
				get
				{
					return this.timestamp;
				}
			}

			public string PrimaryServerFqdn
			{
				get
				{
					return this.primaryServerFqdn;
				}
			}

			public ResubmitReason Reason
			{
				get
				{
					return this.reason;
				}
			}

			public int MessageCount
			{
				get
				{
					return this.messageCount;
				}
			}

			private readonly DateTime timestamp;

			private readonly string primaryServerFqdn;

			private readonly ResubmitReason reason;

			private readonly int messageCount;
		}
	}
}
