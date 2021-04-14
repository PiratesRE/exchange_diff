using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage.ResourceHealth;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.MailboxTransport.Delivery;
using Microsoft.Exchange.Transport.LoggingCommon;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal sealed class DeliveryThrottling : DisposeTrackableBase, IDeliveryThrottling, IDisposable
	{
		private DeliveryThrottling()
		{
			this.getMDBThreadLimitAndHealth = new GetMDBThreadLimitAndHealth(DeliveryThrottling.GetDatabaseThreadLimitAndHealth);
			this.deliveryThrottlingLog = new DeliveryThrottlingLog();
			this.deliveryThrottlingLog.Configure(DeliveryConfiguration.Instance.Throttling);
			this.deliveryThrottlingLogWorker = new DeliveryThrottlingLogWorker(this.deliveryThrottlingLog);
		}

		internal DeliveryThrottling(GetMDBThreadLimitAndHealth getMDBThreadLimitAndHealth, IThrottlingConfig throttlingConfig, IDeliveryThrottlingLogWorker throttlingLogWorker)
		{
			this.getMDBThreadLimitAndHealth = getMDBThreadLimitAndHealth;
			this.deliveryThrottlingLog = new DeliveryThrottlingLog();
			this.deliveryThrottlingLog.Configure(throttlingConfig ?? DeliveryConfiguration.Instance.Throttling);
			this.deliveryThrottlingLogWorker = (throttlingLogWorker ?? new DeliveryThrottlingLogWorker(this.deliveryThrottlingLog));
		}

		public static IDeliveryThrottling Instance
		{
			get
			{
				if (DeliveryThrottling.instance == null)
				{
					lock (DeliveryThrottling.syncRoot)
					{
						if (DeliveryThrottling.instance == null)
						{
							DeliveryThrottling.instance = new DeliveryThrottling();
						}
					}
				}
				return DeliveryThrottling.instance;
			}
			set
			{
				lock (DeliveryThrottling.syncRoot)
				{
					DeliveryThrottling.instance = value;
				}
			}
		}

		public IMailboxDatabaseCollectionManager MailboxDatabaseCollectionManager
		{
			get
			{
				return this.mailboxDatabaseCollectionManager;
			}
		}

		public IDeliveryThrottlingLog DeliveryThrottlingLog
		{
			get
			{
				return this.deliveryThrottlingLog;
			}
		}

		public IDeliveryThrottlingLogWorker DeliveryThrottlingLogWorker
		{
			get
			{
				return this.deliveryThrottlingLogWorker;
			}
		}

		public XElement DeliveryServerDiagnostics
		{
			get
			{
				return DeliveryThrottling.deliveryServerThreadMap.GetDiagnosticInfo(new XElement("DeliveryServerThreadMap"));
			}
		}

		public XElement DeliveryDatabaseDiagnostics
		{
			get
			{
				if (DeliveryConfiguration.Instance.Throttling.DynamicMailboxDatabaseThrottlingEnabled)
				{
					return this.mailboxDatabaseCollectionManager.GetDiagnosticInfo(new XElement("DeliveryDatabases"));
				}
				return DeliveryThrottling.deliveryDatabaseThreadMap.GetDiagnosticInfo(new XElement("DeliveryDatabaseThreadMap"));
			}
		}

		public XElement DeliveryRecipientDiagnostics
		{
			get
			{
				return DeliveryThrottling.deliveryRecipientThreadMap.GetDiagnosticInfo(new XElement("DeliveryRecipientThreadMap"));
			}
		}

		public GetMDBThreadLimitAndHealth GetMDBThreadLimitAndHealth
		{
			get
			{
				return this.getMDBThreadLimitAndHealth;
			}
		}

		public void ResetSession(long smtpSessionId)
		{
			ThrottleSession throttleSession = DeliveryThrottling.sessionMap.TryGetSession(smtpSessionId);
			if (throttleSession != null)
			{
				if (throttleSession.Recipients != null)
				{
					IList<RoutingAddress> list = throttleSession.Recipients.Keys.ToList<RoutingAddress>();
					foreach (RoutingAddress routingAddress in list)
					{
						int num = throttleSession.Recipients[routingAddress];
						for (int i = 0; i < num; i++)
						{
							this.DecrementRecipient(smtpSessionId, routingAddress);
						}
					}
				}
				lock (DeliveryThrottling.syncMessageSize)
				{
					DeliveryThrottling.DecrementSessionMessageSize(throttleSession);
				}
				if (throttleSession.Mdb != null)
				{
					DeliveryThrottling.deliveryDatabaseThreadMap.Decrement(throttleSession.Mdb.ToString());
					throttleSession.Mdb = null;
				}
			}
		}

		public void ClearSession(long smtpSessionId)
		{
			ThrottleSession throttleSession = DeliveryThrottling.sessionMap.TryGetSession(smtpSessionId);
			if (throttleSession != null)
			{
				this.ResetSession(smtpSessionId);
				DeliveryThrottling.sessionMap.RemoveSession(smtpSessionId);
				DeliveryThrottling.deliveryServerThreadMap.Decrement("localhost");
			}
		}

		public void DecrementRecipient(long smtpSessionId, RoutingAddress recipient)
		{
			DeliveryThrottling.sessionMap.RemoveRecipient(smtpSessionId, recipient);
			DeliveryThrottling.deliveryRecipientThreadMap.Decrement(recipient);
		}

		public void DecrementCurrentMessageSize(long smtpSessionId)
		{
			ThrottleSession throttleSession = DeliveryThrottling.sessionMap.TryGetSession(smtpSessionId);
			if (throttleSession != null)
			{
				lock (DeliveryThrottling.syncMessageSize)
				{
					DeliveryThrottling.DecrementSessionMessageSize(throttleSession);
				}
			}
		}

		public bool CheckAndTrackThrottleServer(long smtpSessionId)
		{
			if (!DeliveryThrottling.deliveryServerThreadMap.TryCheckAndIncrement("localhost", (ulong)smtpSessionId, string.Empty))
			{
				DeliveryThrottling.Diag.TraceDebug<int, long>(0L, "Connection is skipped as it exceeds delivery server thread limit of {0}. Smtp session id: {1}", DeliveryThrottling.deliveryServerThreadMap.ThreadLimit, smtpSessionId);
				this.DeliveryThrottlingLogWorker.TrackMDBServerThrottle(true, (double)DeliveryThrottling.deliveryServerThreadMap.ThreadLimit);
				return false;
			}
			this.DeliveryThrottlingLogWorker.TrackMDBServerThrottle(false, (double)DeliveryThrottling.deliveryServerThreadMap.ThreadLimit);
			DeliveryThrottling.sessionMap.AddSession(smtpSessionId);
			return true;
		}

		public void UpdateMdbThreadCounters()
		{
			if (DeliveryConfiguration.Instance.Throttling.DynamicMailboxDatabaseThrottlingEnabled)
			{
				this.mailboxDatabaseCollectionManager.UpdateMdbThreadCounters();
				return;
			}
			DeliveryThrottling.deliveryDatabaseThreadMap.UpdateMdbThreadCounters();
		}

		public bool CheckAndTrackThrottleMDB(Guid databaseGuid, long smtpSessionId, out List<KeyValuePair<string, double>> mdbHealthMonitorValues)
		{
			mdbHealthMonitorValues = null;
			string text = databaseGuid.ToString();
			int num;
			if (!DeliveryThrottling.deliveryDatabaseThreadMap.TryCheckAndIncrement(text, this.getMDBThreadLimitAndHealth(databaseGuid, out num, out mdbHealthMonitorValues), (ulong)smtpSessionId))
			{
				DeliveryThrottling.Diag.TraceDebug<string, int>(0L, "Connection to database \"{0}\" is skipped as it exceeds delivery database thread limit of {1}", text, DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnections);
				this.DeliveryThrottlingLogWorker.TrackMDBThrottle(true, text, (double)DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnections, mdbHealthMonitorValues, ThrottlingResource.Threads);
				return false;
			}
			this.DeliveryThrottlingLogWorker.TrackMDBThrottle(false, text, (double)DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnections, mdbHealthMonitorValues, ThrottlingResource.Threads);
			DeliveryThrottling.sessionMap.SetMdb(smtpSessionId, databaseGuid);
			return true;
		}

		public bool CheckAndTrackDynamicThrottleMDBPendingConnections(Guid databaseGuid, IMailboxDatabaseConnectionManager mdbConnectionManager, long smtpSessionId, IPAddress sessionRemoteEndPointAddress, out List<KeyValuePair<string, double>> mdbHealthMonitorValues)
		{
			mdbHealthMonitorValues = null;
			ArgumentValidator.ThrowIfNull("databaseGuid", databaseGuid);
			ArgumentValidator.ThrowIfNull("mdbConnectionManager", mdbConnectionManager);
			ArgumentValidator.ThrowIfNull("sessionRemoteEndPointAddress", sessionRemoteEndPointAddress);
			bool result;
			try
			{
				int num = -1;
				int num2 = -1;
				if (mdbConnectionManager.GetMdbHealthAndAddConnection(smtpSessionId, sessionRemoteEndPointAddress, out num, out mdbHealthMonitorValues, out num2))
				{
					DeliveryThrottling.Diag.TraceDebug<Guid, long, IPAddress>(0, (long)this.GetHashCode(), "Dynamic Throttling: Connection added. MDB {0} SeesionId {1} IP {2}", databaseGuid, smtpSessionId, sessionRemoteEndPointAddress);
					this.DeliveryThrottlingLogWorker.TrackMDBThrottle(false, databaseGuid.ToString(), (double)DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnections, mdbHealthMonitorValues, ThrottlingResource.Threads_MaxPerHub);
					mdbConnectionManager.UpdateLastActivityTime(smtpSessionId);
					result = true;
				}
				else
				{
					DeliveryThrottling.Diag.TraceDebug<long>(0, (long)this.GetHashCode(), "Dynamic Throttling: Connection from the remote IP address is already in the pending queue. SessionId {0}", smtpSessionId);
					this.DeliveryThrottlingLogWorker.TrackMDBThrottle(true, databaseGuid.ToString(), (double)DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnections, mdbHealthMonitorValues, ThrottlingResource.Threads_MaxPerHub);
					result = false;
				}
			}
			catch (InvalidOperationException ex)
			{
				DeliveryThrottling.Diag.TraceDebug<long, string>(0, (long)this.GetHashCode(), "Dynamic Throttling: Error attempting to add connection. SessionId {0} Error: {1}", smtpSessionId, ex.ToString());
				result = false;
			}
			return result;
		}

		public bool CheckAndTrackDynamicThrottleMDBTimeout(Guid databaseGuid, IMailboxDatabaseConnectionInfo mdbConnectionInfo, IMailboxDatabaseConnectionManager mdbConnectionManager, long smtpSessionId, IPAddress sessionRemoteEndPointAddress, TimeSpan connectionWaitTime, List<KeyValuePair<string, double>> mdbHealthMonitorValues)
		{
			ArgumentValidator.ThrowIfNull("databaseGuid", databaseGuid);
			ArgumentValidator.ThrowIfNull("mdbConnectionManager", mdbConnectionManager);
			ArgumentValidator.ThrowIfNull("sessionRemoteEndPointAddress", sessionRemoteEndPointAddress);
			if (!mdbConnectionManager.TryAcquire(smtpSessionId, sessionRemoteEndPointAddress, connectionWaitTime, out mdbConnectionInfo))
			{
				DeliveryThrottling.Diag.TraceDebug<Guid, long, IPAddress>(0, (long)this.GetHashCode(), "Dynamic Throttling: Connection was unable to be acquired. Dynamic Throttling Limit Exceeded. MDB {0} SessionId {1} IP {2}", databaseGuid, smtpSessionId, sessionRemoteEndPointAddress);
				this.DeliveryThrottlingLogWorker.TrackMDBThrottle(true, databaseGuid.ToString(), (double)DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnections, mdbHealthMonitorValues, ThrottlingResource.Threads_PendingConnectionTimedOut);
				return false;
			}
			DeliveryThrottling.Diag.TraceDebug<Guid, long, IPAddress>(0, (long)this.GetHashCode(), "Dynamic Throttling: Connection was acquired. MDB {0} SessionId {1} IP {2}", databaseGuid, smtpSessionId, sessionRemoteEndPointAddress);
			this.DeliveryThrottlingLogWorker.TrackMDBThrottle(false, databaseGuid.ToString(), (double)DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnections, mdbHealthMonitorValues, ThrottlingResource.Threads_PendingConnectionTimedOut);
			return true;
		}

		public bool CheckAndTrackThrottleRecipient(RoutingAddress recipient, long smtpSessionId, string mdbName, Guid tenantId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("recipient.ToString()", recipient.ToString());
			ArgumentValidator.ThrowIfNullOrEmpty("mdbName", mdbName);
			ArgumentValidator.ThrowIfNull("tenantId", tenantId);
			if (!DeliveryThrottling.deliveryRecipientThreadMap.TryCheckAndIncrement(recipient, (ulong)smtpSessionId, string.Empty))
			{
				DeliveryThrottling.Diag.TraceDebug<string, int>(0L, "Delivery for recipient \"{0}\" is skipped as it exceeds delivery recipient thread limit of {1}", recipient.ToString(), DeliveryConfiguration.Instance.Throttling.RecipientThreadLimit);
				this.DeliveryThrottlingLogWorker.TrackRecipientThrottle(true, Utils.RedactRoutingAddressIfNecessary(recipient, Utils.IsRedactionNecessary()), tenantId, mdbName, (double)DeliveryConfiguration.Instance.Throttling.RecipientThreadLimit);
				return false;
			}
			this.DeliveryThrottlingLogWorker.TrackRecipientThrottle(false, Utils.RedactRoutingAddressIfNecessary(recipient, Utils.IsRedactionNecessary()), tenantId, mdbName, (double)DeliveryConfiguration.Instance.Throttling.RecipientThreadLimit);
			DeliveryThrottling.sessionMap.AddRecipient(smtpSessionId, recipient);
			return true;
		}

		public void SetSessionMessageSize(long messageSize, long smtpSessionId)
		{
			ThrottleSession throttleSession = DeliveryThrottling.sessionMap.TryGetSession(smtpSessionId);
			if (throttleSession != null)
			{
				DeliveryThrottling.DecrementSessionMessageSize(throttleSession);
				if (messageSize < 0L)
				{
					messageSize = 0L;
				}
				throttleSession.MessageSize = messageSize;
			}
		}

		public bool CheckAndTrackThrottleConcurrentMessageSizeLimit(long smtpSessionId, int numOfRecipients)
		{
			ThrottleSession throttleSession = DeliveryThrottling.sessionMap.TryGetSession(smtpSessionId);
			bool flag = false;
			if (throttleSession != null && throttleSession.MessageSize >= 0L)
			{
				ulong num = Convert.ToUInt64(throttleSession.MessageSize);
				lock (DeliveryThrottling.syncMessageSize)
				{
					if (DeliveryThrottling.totalConcurrentMessageSize < DeliveryThrottling.maxConcurrentMessageSizeLimit)
					{
						DeliveryThrottling.totalConcurrentMessageSize += num;
						flag = true;
					}
					else
					{
						throttleSession.MessageSize = 0L;
					}
				}
			}
			if (!flag)
			{
				DeliveryThrottling.Diag.TraceDebug<long, ulong>(0L, "Delivery for smtp session \"{0}\" is skipped as it exceeds max concurrent message size limit of {1}", smtpSessionId, DeliveryThrottling.maxConcurrentMessageSizeLimit);
				this.DeliveryThrottlingLogWorker.TrackConcurrentMessageSizeThrottle(true, DeliveryThrottling.maxConcurrentMessageSizeLimit, numOfRecipients);
			}
			else
			{
				this.DeliveryThrottlingLogWorker.TrackConcurrentMessageSizeThrottle(false, DeliveryThrottling.maxConcurrentMessageSizeLimit, numOfRecipients);
			}
			return flag;
		}

		private static void DecrementSessionMessageSize(ThrottleSession session)
		{
			ulong num = Convert.ToUInt64(session.MessageSize);
			if (DeliveryThrottling.totalConcurrentMessageSize >= num)
			{
				DeliveryThrottling.totalConcurrentMessageSize -= num;
			}
			else
			{
				DeliveryThrottling.totalConcurrentMessageSize = 0UL;
			}
			session.MessageSize = 0L;
		}

		public bool TryGetDatabaseHealth(Guid databaseGuid, out int health)
		{
			List<KeyValuePair<string, double>> list = null;
			return this.TryGetDatabaseHealth(databaseGuid, out health, out list);
		}

		public bool TryGetDatabaseHealth(Guid databaseGuid, out int health, out List<KeyValuePair<string, double>> monitorHealthValues)
		{
			health = -1;
			monitorHealthValues = null;
			if (databaseGuid != Guid.Empty)
			{
				ResourceLoadDelayInfo.Initialize();
				ResourceKey[] array = new ResourceKey[]
				{
					new MdbResourceHealthMonitorKey(databaseGuid),
					new MdbReplicationResourceHealthMonitorKey(databaseGuid)
				};
				ResourceLoad[] resourceLoadList = null;
				ResourceKey resourceKey;
				ResourceLoad resourceLoad;
				ResourceLoadDelayInfo.GetWorstResourceAndAllHealthValues(WorkloadType.Transport, array, out resourceLoadList, out resourceKey, out resourceLoad);
				if (resourceKey != null)
				{
					switch (resourceLoad.State)
					{
					case ResourceLoadState.Underloaded:
					case ResourceLoadState.Full:
						health = 100;
						break;
					case ResourceLoadState.Overloaded:
						health = (int)(100.0 / resourceLoad.LoadRatio);
						break;
					case ResourceLoadState.Critical:
						health = 0;
						break;
					}
					monitorHealthValues = this.GetMDBHealthMonitors(array, resourceLoadList);
					return true;
				}
			}
			else
			{
				DeliveryThrottling.Diag.TraceDebug<string>(0L, "Database Guid is empty for {0}.", StoreDriverDelivery.MailboxServerFqdn);
			}
			return false;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DeliveryThrottling>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.mailboxDatabaseCollectionManager != null)
				{
					this.mailboxDatabaseCollectionManager.Dispose();
					this.mailboxDatabaseCollectionManager = null;
				}
				if (this.deliveryThrottlingLog != null)
				{
					this.deliveryThrottlingLog.Close();
					this.deliveryThrottlingLog = null;
				}
			}
		}

		private static int GetDatabaseThreadLimitAndHealth(Guid databaseGuid, out int databaseHealthMeasure, out List<KeyValuePair<string, double>> monitorHealthValues)
		{
			if (!DeliveryConfiguration.Instance.Throttling.MailboxDeliveryThrottlingEnabled)
			{
				databaseHealthMeasure = -1;
				monitorHealthValues = null;
				return DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnections;
			}
			DeliveryThrottling.Instance.TryGetDatabaseHealth(databaseGuid, out databaseHealthMeasure, out monitorHealthValues);
			return DeliveryThrottling.GetDatabaseThreadLimit(databaseHealthMeasure, databaseGuid);
		}

		internal static int GetDatabaseThreadLimit(int databaseHealthMeasure, Guid databaseGuid)
		{
			int maxMailboxDeliveryPerMdbConnections = DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnections;
			Microsoft.Exchange.Diagnostics.Components.Data.Directory.ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3058052413U, ref maxMailboxDeliveryPerMdbConnections);
			if (databaseHealthMeasure == 100)
			{
				return maxMailboxDeliveryPerMdbConnections;
			}
			DeliveryThrottling.Diag.TraceDebug<Guid, int>(0L, "Health of database {0} is {1}", databaseGuid, databaseHealthMeasure);
			DatabaseHealthBreadcrumb databaseHealthBreadcrumb = new DatabaseHealthBreadcrumb();
			databaseHealthBreadcrumb.DatabaseHealth = databaseHealthMeasure;
			databaseHealthBreadcrumb.DatabaseGuid = databaseGuid;
			StoreDriverDeliveryDiagnostics.HealthHistory.Drop(databaseHealthBreadcrumb);
			if (databaseHealthMeasure == -1)
			{
				return maxMailboxDeliveryPerMdbConnections;
			}
			int num;
			if (databaseHealthMeasure > DeliveryConfiguration.Instance.Throttling.MdbHealthMediumToHighThreshold)
			{
				num = DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnectionsHighHealthPercent;
			}
			else if (databaseHealthMeasure > DeliveryConfiguration.Instance.Throttling.MdbHealthLowToMediumThreshold)
			{
				num = DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnectionsMediumHealthPercent;
			}
			else if (databaseHealthMeasure > 0)
			{
				num = DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnectionsLowHealthPercent;
			}
			else
			{
				num = DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnectionsLowestHealthPercent;
			}
			return (int)Math.Ceiling((double)(maxMailboxDeliveryPerMdbConnections * num) / 100.0);
		}

		private List<KeyValuePair<string, double>> GetMDBHealthMonitors(ResourceKey[] resourceKeyList, ResourceLoad[] resourceLoadList)
		{
			List<KeyValuePair<string, double>> list = null;
			if (resourceLoadList != null && resourceLoadList.Length > 0)
			{
				list = new List<KeyValuePair<string, double>>();
				for (int i = 0; i < resourceLoadList.Length; i++)
				{
					if (resourceLoadList[i] != ResourceLoad.Unknown)
					{
						list.Add(new KeyValuePair<string, double>(resourceKeyList[i].GetType().Name, resourceLoadList[i].LoadRatio));
					}
				}
			}
			return list;
		}

		private const string MailboxServerFqdn = "localhost";

		private static readonly Trace Diag = Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery.ExTraceGlobals.StoreDriverDeliveryTracer;

		private static DeliveryServerThreadMap deliveryServerThreadMap = new DeliveryServerThreadMap(DeliveryThrottling.Diag);

		private static DeliveryDatabaseThreadMap deliveryDatabaseThreadMap = new DeliveryDatabaseThreadMap(DeliveryThrottling.Diag);

		private static DeliveryRecipientThreadMap deliveryRecipientThreadMap = new DeliveryRecipientThreadMap(DeliveryThrottling.Diag);

		private static ThrottleSessionMap sessionMap = new ThrottleSessionMap();

		private static volatile IDeliveryThrottling instance;

		private static object syncRoot = new object();

		private static object syncMessageSize = new object();

		private static ulong totalConcurrentMessageSize = 0UL;

		private static ulong maxConcurrentMessageSizeLimit = DeliveryConfiguration.Instance.Throttling.MaxConcurrentMessageSizeLimit;

		private IMailboxDatabaseCollectionManager mailboxDatabaseCollectionManager = MailboxDatabaseCollectionManagerFactory.Create();

		private GetMDBThreadLimitAndHealth getMDBThreadLimitAndHealth;

		private DeliveryThrottlingLog deliveryThrottlingLog;

		private readonly IDeliveryThrottlingLogWorker deliveryThrottlingLogWorker;
	}
}
