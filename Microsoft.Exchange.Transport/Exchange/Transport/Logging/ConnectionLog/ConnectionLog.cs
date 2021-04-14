using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport.Logging.ConnectionLog
{
	internal class ConnectionLog
	{
		private static string[] PriorityNamesForActiveQueueCounts
		{
			get
			{
				if (!ConnectionLog.priorityNamesForQueueCountsInitialized)
				{
					ConnectionLog.InitializePriorityNamesForQueueCounts();
				}
				return ConnectionLog.priorityNamesForActiveQueueCounts;
			}
		}

		private static string[] PriorityNamesForRetryQueueCounts
		{
			get
			{
				if (!ConnectionLog.priorityNamesForQueueCountsInitialized)
				{
					ConnectionLog.InitializePriorityNamesForQueueCounts();
				}
				return ConnectionLog.priorityNamesForRetryQueueCounts;
			}
		}

		internal static LogSchema Schema
		{
			get
			{
				if (ConnectionLog.schema == null)
				{
					LogSchema value = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Transport Connectivity Log", ConnectionLog.Row.Headers);
					Interlocked.CompareExchange<LogSchema>(ref ConnectionLog.schema, value, null);
				}
				return ConnectionLog.schema;
			}
		}

		public static void Start()
		{
			ConnectionLog.CreateLog();
			ConnectionLog.Configure(Components.Configuration.LocalServer);
			ConnectionLog.RegisterConfigurationChangeHandlers();
		}

		public static void Stop()
		{
			ConnectionLog.UnregisterConfigurationChangeHandlers();
			if (ConnectionLog.log != null)
			{
				ConnectionLog.log.Close();
			}
		}

		public static void FlushBuffer()
		{
			if (ConnectionLog.log != null)
			{
				ConnectionLog.log.Flush();
			}
		}

		public static void SmtpConnectionStart(ulong id, NextHopSolutionKey nextHop, int totalCount, int[] activeCountsPerPriority, int[] retryCountsPerPriority, string logMessage = null)
		{
			ConnectionLog.SmtpConnectionStart(id, nextHop, string.Format(CultureInfo.InvariantCulture, "{0} {1};QueueLength={2}. {3}", new object[]
			{
				nextHop.NextHopType,
				nextHop.NextHopConnector,
				ConnectionLog.GetFormattedQueueCountsForConnectionLog(totalCount, activeCountsPerPriority, retryCountsPerPriority),
				logMessage ?? string.Empty
			}));
		}

		public static void SmtpConnectionFailover(ulong newSessionid, ulong previousSessionId, string host, SessionSetupFailureReason reason)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			ConnectionLog.Row row = new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(newSessionid),
				Destination = (host ?? string.Empty),
				Direction = "*",
				Source = "SMTP",
				Description = string.Format("Session Failover; previous session id = {0}; reason = {1}", ConnectionLog.SessionIdToString(previousSessionId), reason)
			};
			row.Log("SmtpConnectionFailover");
		}

		public static void SmtpConnectionStart(ulong id, NextHopSolutionKey nextHop, string description)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(id),
				Destination = nextHop.NextHopDomain,
				Direction = "+",
				Source = "SMTP",
				Description = description
			}.Log("SmtpConnectionStart");
		}

		public static void SmtpConnectionStartCacheHit(ulong cachedSessionID, ulong oldSessionID, NextHopSolutionKey nextHop, int totalCount, int[] activeCountsPerPriority, int[] retryCountsPerPriority, string logMessage)
		{
			string description = string.Format("{0} {1} CachedSession Replacing {2};{3}. {4}", new object[]
			{
				nextHop.NextHopType.ToString(),
				nextHop.NextHopConnector.ToString(),
				ConnectionLog.SessionIdToString(oldSessionID),
				ConnectionLog.GetFormattedQueueCountsForConnectionLog(totalCount, activeCountsPerPriority, retryCountsPerPriority),
				logMessage
			});
			ConnectionLog.SmtpConnectionStart(cachedSessionID, nextHop, description);
		}

		public static void SmtpHostResolved(ulong id, string host, TargetHost[] hosts, bool hostIsOutboundProxy)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			ConnectionLog.Row row = new ConnectionLog.Row();
			row.Session = ConnectionLog.SessionIdToString(id);
			row.Destination = host;
			row.Direction = ">";
			row.Source = "SMTP";
			int num = 256;
			StringBuilder stringBuilder = new StringBuilder(num);
			if (hostIsOutboundProxy)
			{
				stringBuilder.Append("Outbound proxy via ");
			}
			for (int i = 0; i < hosts.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(hosts[i].Name);
				stringBuilder.Append("[");
				int num2 = 0;
				foreach (IPAddress ipaddress in hosts[i].IPAddresses)
				{
					if (num2++ != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(ipaddress.ToString());
				}
				stringBuilder.Append("]");
				if (stringBuilder.Length > num)
				{
					stringBuilder.Length = num - 3;
					stringBuilder.Append("...");
					break;
				}
			}
			row.Description = stringBuilder.ToString();
			row.Log("SmtpHostResolved");
		}

		public static void SmtpHostResolutionFailed(ulong id, string host, IPAddress reportingServer, string reason, string diagnosticInfo)
		{
			ConnectionLog.SmtpHostResolutionFailed(id, host, reportingServer, reason, diagnosticInfo, false);
		}

		public static void SmtpHostResolutionFailedForOutboundProxyFrontend(ulong id, string host, IPAddress reportingServer, string reason, string diagnosticInfo)
		{
			ConnectionLog.SmtpHostResolutionFailed(id, host, reportingServer, reason, diagnosticInfo, true);
		}

		public static void SmtpConnected(ulong id, string host, IPAddress address)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(id),
				Destination = host,
				Direction = ">",
				Source = "SMTP",
				Description = "Established connection to " + address.ToString()
			}.Log("SmtpConnected");
		}

		public static void SmtpConnectionFailed(ulong id, string host, IPAddress address, ushort port, SocketException ex)
		{
			ConnectionLog.SmtpConnectionFailed(id, host, address, port, ex, null);
		}

		public static void SmtpConnectionFailed(ulong id, string host, IPAddress address, string targetHostName, ushort port, bool ipAddressMarkedUnhealthy, ExDateTime ipAddressNextRetryTime, int currentIpAddressConnectionCount, int currentIpAddressFailureCount, bool targetHostMarkedUnhealthy, ExDateTime targetHostRetryTime, int currentTargetHostConnectionCount, int currentTargetHostFailureCount, SocketException ex)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			string str = ConnectionLog.FormatTargetFailureStatus("TargetHost", targetHostName, port, targetHostMarkedUnhealthy, currentTargetHostConnectionCount, currentTargetHostFailureCount, targetHostRetryTime);
			string str2 = ConnectionLog.FormatTargetFailureStatus("TargetIPAddress", address.ToString(), port, ipAddressMarkedUnhealthy, currentIpAddressConnectionCount, currentIpAddressFailureCount, ipAddressNextRetryTime);
			ConnectionLog.SmtpConnectionFailed(id, host, address, port, ex, str + str2);
		}

		public static void SmtpConnectionStop(ulong id, string host, string description, ulong count, ulong bytes, ulong discardIds)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			ConnectionLog.Row row = new ConnectionLog.Row();
			row.Session = ConnectionLog.SessionIdToString(id);
			row.Destination = host;
			row.Direction = "-";
			row.Source = "SMTP";
			string text = string.Format(CultureInfo.InvariantCulture, "Messages: {0} Bytes: {1}", new object[]
			{
				count.ToString(NumberFormatInfo.InvariantInfo),
				bytes.ToString(NumberFormatInfo.InvariantInfo)
			});
			if (discardIds != 0UL)
			{
				text += string.Format(CultureInfo.InvariantCulture, " DiscardIDs: {0}", new object[]
				{
					discardIds.ToString(NumberFormatInfo.InvariantInfo)
				});
			}
			if (description != null)
			{
				text += string.Format(CultureInfo.InvariantCulture, " ({0})", new object[]
				{
					description
				});
			}
			row.Description = text;
			row.Log("SmtpConnectionStop");
		}

		public static void SmtpConnectionStopDueToCacheHit(ulong sessionID, ulong cachedSessionID, string host)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(sessionID),
				Destination = host,
				Direction = "-",
				Source = "SMTP",
				Description = string.Format("Using Cached Session {0}", ConnectionLog.SessionIdToString(cachedSessionID))
			}.Log("SmtpConnectionStop");
		}

		public static void SmtpConnectionAborted(ulong id, string host, IPAddress address)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(id),
				Destination = host,
				Direction = "-",
				Source = "SMTP",
				Description = "Aborted connection to " + address.ToString()
			}.Log("SmtpConnectionAborted");
		}

		public static void MapiDeliveryConnectionStart(ulong id, string mdb, string description)
		{
			ConnectionLog.MapiDeliveryConnectionLog(id, mdb, "+", description, "MapiDeliveryConnectionStart");
		}

		public static void MapiDeliveryConnectionStartingDelivery(ulong id, string mdb)
		{
			ConnectionLog.MapiDeliveryConnectionLog(id, mdb, ">", "Starting delivery", "MapiDeliveryConnectionStartingDelivery");
		}

		public static void MapiDeliveryConnectionServerThreadLimitReached(string mdb, string serverFqdn, int limit)
		{
			ConnectionLog.MapiDeliveryConnectionLog(0UL, mdb, "*", string.Concat(new object[]
			{
				"Throttled delivery due to server limit for ",
				serverFqdn,
				" with threshold ",
				limit
			}), "MapiDeliveryConnectionServerThreadLimitReached");
		}

		public static void MapiDeliveryConnectionServerConnect(ulong id, string mdb, string serverFqdn, string sessionType)
		{
			ConnectionLog.MapiDeliveryConnectionLog(id, mdb, ">", "Connecting to server " + serverFqdn + " session type " + sessionType, "MapiDeliveryConnectionServerConnect");
		}

		public static void MapiDeliveryConnectionMdbThreadLimitReached(string mdb, int limit)
		{
			ConnectionLog.MapiDeliveryConnectionLog(0UL, mdb, "*", "Throttled delivery due to MDB limit " + limit, "MapiDeliveryConnectionMdbThreadLimitReached");
		}

		public static void MapiDeliveryConnectionRecipientThreadLimitReached(ulong sessionId, RoutingAddress routingAddress, string mdb, int limit)
		{
			ConnectionLog.MapiDeliveryConnectionLog(sessionId, mdb, ">", string.Format("Throttled delivery for recipient {0} due to concurrency limit {1}", routingAddress.ToString(), limit), "MapiDeliveryConnectionRecipientThreadLimitReached");
		}

		public static void MapiDeliveryConnectionServerNotFound(string mdb, string message)
		{
			ConnectionLog.MapiDeliveryConnectionLog(0UL, mdb, "*", "Failed to locate server: " + message, "MapiDeliveryConnectionServerNotFound");
		}

		public static void MapiDeliveryConnectionServerConnectFailed(ulong id, string mdb, string serverFqdn)
		{
			ConnectionLog.MapiDeliveryConnectionLog(id, mdb, ">", "Failed to connect to server " + serverFqdn, "MapiDeliveryConnectionServerConnectFailed");
		}

		public static void MapiDeliveryConnectionRetired(ulong id, string mdb)
		{
			ConnectionLog.MapiDeliveryConnectionLog(id, mdb, "-", "Retired", "MapiDeliveryConnectionRetired");
		}

		public static void MapiDeliveryConnectionStop(ulong id, string mdb, ulong count, ulong bytes, ulong recipientCount)
		{
			string description = string.Concat(new string[]
			{
				"Messages: ",
				count.ToString(NumberFormatInfo.InvariantInfo),
				" Bytes: ",
				bytes.ToString(NumberFormatInfo.InvariantInfo),
				" Recipients: ",
				recipientCount.ToString(NumberFormatInfo.InvariantInfo)
			});
			ConnectionLog.MapiDeliveryConnectionLog(id, mdb, "-", description, "MapiDeliveryConnectionStop");
		}

		public static void MapiDeliveryConnectionLost(ulong id, string mdb, string description, ulong count, ulong bytes, ulong recipientCount)
		{
			description = string.Concat(new string[]
			{
				"Messages: ",
				count.ToString(NumberFormatInfo.InvariantInfo),
				" Bytes: ",
				bytes.ToString(NumberFormatInfo.InvariantInfo),
				" Recipients: ",
				recipientCount.ToString(NumberFormatInfo.InvariantInfo),
				" ",
				description
			});
			ConnectionLog.MapiDeliveryConnectionLog(id, mdb, "-", description, "MapiDeliveryConnectionLost");
		}

		public static void MapiDeliveryConnectionDeliveryQueueStats(Dictionary<QueueStatus, ConnectionLog.AggregateQueueStats> queueStats)
		{
			string text = "Queues: ";
			foreach (KeyValuePair<QueueStatus, ConnectionLog.AggregateQueueStats> keyValuePair in queueStats)
			{
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					keyValuePair.Key,
					":ActiveMsgs=",
					keyValuePair.Value.ActiveMessageCount,
					"/RetryMsgs=",
					keyValuePair.Value.RetryMessageCount,
					"/Queues=",
					keyValuePair.Value.QueueCount,
					";"
				});
			}
			ConnectionLog.MapiDeliveryConnectionLog(0UL, string.Empty, "*", text, "MapiDeliveryConnectionDeliveryQueueStats");
		}

		public static void MapiSubmissionConnectionStart(ulong id, string mdb, string description)
		{
			ConnectionLog.MapiSubmissionConnectionLog(id, mdb, "+", description, "MapiSubmissionConnectionStart");
		}

		public static void MapiSubmissionAborted(ulong id, string mdb, string description)
		{
			ConnectionLog.MapiSubmissionConnectionLog(id, mdb, ">", "Aborted; " + description, "MapiSubmissionConnectionAborted");
		}

		public static void MapiSubmissionFailed(ulong id, string mdb, string description)
		{
			ConnectionLog.MapiSubmissionConnectionLog(id, mdb, ">", "Failed; " + description, "MapiSubmissionConnectionFailed");
		}

		public static void MapiSubmissionConnectionStop(ulong id, string mdb, long totalSubmissions, long shadowSubmissions, long bytes, long recipientCount, int failures, bool reachedLimit, bool idle)
		{
			string description = string.Format(CultureInfo.InvariantCulture, "RegularSubmissions: {0} ShadowSubmissions: {1} Bytes: {2} Recipients: {3} Failures: {4} ReachedLimit: {5} Idle: {6}", new object[]
			{
				totalSubmissions,
				shadowSubmissions,
				bytes,
				recipientCount,
				failures,
				reachedLimit,
				idle
			});
			ConnectionLog.MapiSubmissionConnectionLog(id, mdb, "-", description, "MapiSubmissionConnectionStop");
		}

		public static void AggregationConnectionStart(string sourceComponent, string sessionId, string destination, Guid subscription)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = sessionId,
				Destination = destination,
				Direction = "+",
				Source = sourceComponent,
				Description = subscription.ToString()
			}.Log("AggregationConnectionStart");
		}

		public static void AggregationConnectionStop(string sourceComponent, string sessionId, string destination, ulong countMessages)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = sessionId,
				Destination = destination,
				Direction = "-",
				Source = sourceComponent,
				Description = "Messages: " + countMessages.ToString(NumberFormatInfo.InvariantInfo)
			}.Log("AggregationConnectionStop");
		}

		public static void DeliveryAgentStart(ulong id, string agentName, NextHopSolutionKey nextHop)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(id),
				Destination = nextHop.NextHopDomain,
				Direction = "+",
				Source = "DeliveryAgent",
				Description = string.Format("DeliveryAgent {0} invoked for connector {1}", agentName, nextHop.NextHopConnector)
			}.Log("DeliveryAgentStart");
		}

		public static void DeliveryAgentConnected(ulong id, string remoteHost, SmtpResponse smtpResponse)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(id),
				Destination = remoteHost,
				Direction = ">",
				Source = "DeliveryAgent",
				Description = string.Format("Connection to {0} has succeeded with status {1}", remoteHost, smtpResponse)
			}.Log("DeliveryAgentConnected");
		}

		public static void DeliveryAgentConnectionFailed(ulong id, string host)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(id),
				Destination = host,
				Direction = ">",
				Source = "DeliveryAgent",
				Description = "Failed to create a connection"
			}.Log("DeliveryAgentConnectionFailed");
		}

		public static void DeliveryAgentPermanentFailure(ulong id, string host, SmtpResponse smtpResponse)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(id),
				Destination = host,
				Direction = ">",
				Source = "DeliveryAgent",
				Description = string.Format("Connection with remote host encountered a permanent failure with status {0}", smtpResponse)
			}.Log("DeliveryAgentPermanentFailure");
		}

		public static void DeliveryAgentQueueRetry(ulong id, string host, SmtpResponse smtpResponse)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(id),
				Destination = host,
				Direction = ">",
				Source = "DeliveryAgent",
				Description = string.Format("Connection with remote host encountered a transient failure with status {0}", smtpResponse)
			}.Log("DeliveryAgentQueueRetry");
		}

		public static void DeliveryAgentDisconnected(ulong id, string host, SmtpResponse smtpResponse)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(id),
				Destination = host,
				Direction = ">",
				Source = "DeliveryAgent",
				Description = string.Format("Connection ended with status {0}", smtpResponse)
			}.Log("DeliveryAgentDisconnected");
		}

		public static void DeliveryAgentStop(ulong id, string host, int messages, long bytes)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(id),
				Destination = host,
				Direction = "-",
				Source = "DeliveryAgent",
				Description = string.Format("Messages:{0} Bytes:{1}", messages.ToString(NumberFormatInfo.InvariantInfo), bytes.ToString(NumberFormatInfo.InvariantInfo))
			}.Log("DeliveryAgentStop");
		}

		public static void TransportStart(int maxConcurrentSubmissions, int maxConcurrentDeliveries, string maxSmtpOutConnections)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Direction = "*",
				Source = "Transport",
				Description = string.Concat(new object[]
				{
					"service started; MaxConcurrentSubmissions=",
					maxConcurrentSubmissions,
					"; MaxConcurrentDeliveries=",
					maxConcurrentDeliveries,
					"; MaxSmtpOutConnections=",
					maxSmtpOutConnections
				})
			}.Log("TransportServiceStart");
		}

		public static void TransportStop()
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Direction = "*",
				Source = "Transport",
				Description = "service stopped"
			}.Log("TransportServiceStop");
		}

		internal static void StartTest(string directory)
		{
			ConnectionLog.CreateLog();
			ConnectionLog.enabled = true;
			ConnectionLog.log.Configure(directory, TimeSpan.FromDays(1.0), 0L, 0L, 0, TimeSpan.MaxValue, TimeSpan.FromSeconds(1.0));
		}

		internal static string SessionIdToString(ulong sessionId)
		{
			return sessionId.ToString("X16", NumberFormatInfo.InvariantInfo);
		}

		private static void InitializePriorityNamesForQueueCounts()
		{
			ConnectionLog.priorityNamesForActiveQueueCounts = new string[QueueManager.PriorityToInstanceIndexMap.Count];
			ConnectionLog.priorityNamesForRetryQueueCounts = new string[QueueManager.PriorityToInstanceIndexMap.Count];
			foreach (DeliveryPriority key in QueueManager.PriorityToInstanceIndexMap.Keys)
			{
				string text = string.Empty;
				string text2 = string.Empty;
				switch (key)
				{
				case DeliveryPriority.High:
					text = "AH";
					text2 = "RH";
					break;
				case DeliveryPriority.Normal:
					text = "AN";
					text2 = "RN";
					break;
				case DeliveryPriority.Low:
					text = "AL";
					text2 = "RL";
					break;
				case DeliveryPriority.None:
					text = "ANo";
					text2 = "RNo";
					break;
				}
				ConnectionLog.priorityNamesForActiveQueueCounts[QueueManager.PriorityToInstanceIndexMap[key]] = text;
				ConnectionLog.priorityNamesForRetryQueueCounts[QueueManager.PriorityToInstanceIndexMap[key]] = text2;
			}
			ConnectionLog.priorityNamesForQueueCountsInitialized = true;
		}

		private static string GetFormattedQueueCountsForConnectionLog(int totalCount, int[] activeCountsPerPriority, int[] retryCountsPerPriority)
		{
			if (activeCountsPerPriority == null || retryCountsPerPriority == null)
			{
				return "<no priority counts>";
			}
			if (ConnectionLog.PriorityNamesForActiveQueueCounts.Length != activeCountsPerPriority.Length || ConnectionLog.PriorityNamesForRetryQueueCounts.Length != retryCountsPerPriority.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("length of PriorityNamesForActiveQueueCounts:{0} don't match that of activeCountsPerPriority:{1} or length of PriorityNamesForRetryQueueCounts:{2} don't match that of retryCountsPerPriority:{3}", new object[]
				{
					ConnectionLog.PriorityNamesForActiveQueueCounts.Length,
					activeCountsPerPriority.Length,
					ConnectionLog.PriorityNamesForRetryQueueCounts.Length,
					retryCountsPerPriority.Length
				}));
			}
			StringBuilder stringBuilder = new StringBuilder(string.Format("TQ={0};", totalCount));
			stringBuilder.Append(ConnectionLog.GetFormattedIndividualQueueCounts(activeCountsPerPriority, ConnectionLog.PriorityNamesForActiveQueueCounts));
			stringBuilder.Append(ConnectionLog.GetFormattedIndividualQueueCounts(retryCountsPerPriority, ConnectionLog.PriorityNamesForRetryQueueCounts));
			return stringBuilder.ToString();
		}

		private static string GetFormattedIndividualQueueCounts(int[] countsPerPriority, string[] namesPerPriority)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			bool flag = false;
			foreach (string value in namesPerPriority)
			{
				if (countsPerPriority[num] > 0)
				{
					if (flag)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(value);
					stringBuilder.Append("=");
					stringBuilder.Append(countsPerPriority[num]);
					flag = true;
				}
				num++;
			}
			if (flag)
			{
				stringBuilder.Append(";");
			}
			return stringBuilder.ToString();
		}

		private static void CreateLog()
		{
			ConnectionLog.log = new AsyncLog("CONNECTLOG", new LogHeaderFormatter(ConnectionLog.Schema), "ConnectivityLog");
		}

		private static void RegisterConfigurationChangeHandlers()
		{
			Components.Configuration.LocalServerChanged += ConnectionLog.Configure;
		}

		private static void UnregisterConfigurationChangeHandlers()
		{
			Components.Configuration.LocalServerChanged -= ConnectionLog.Configure;
		}

		private static void Configure(TransportServerConfiguration server)
		{
			Server transportServer = server.TransportServer;
			ConnectionLog.enabled = transportServer.ConnectivityLogEnabled;
			if (ConnectionLog.enabled)
			{
				if (transportServer.ConnectivityLogPath == null || string.IsNullOrEmpty(transportServer.ConnectivityLogPath.PathName))
				{
					ConnectionLog.enabled = false;
					return;
				}
				ConnectionLog.log.Configure(transportServer.ConnectivityLogPath.PathName, transportServer.ConnectivityLogMaxAge, (long)(transportServer.ConnectivityLogMaxDirectorySize.IsUnlimited ? 0UL : transportServer.ConnectivityLogMaxDirectorySize.Value.ToBytes()), (long)(transportServer.ConnectivityLogMaxFileSize.IsUnlimited ? 0UL : transportServer.ConnectivityLogMaxFileSize.Value.ToBytes()), Components.TransportAppConfig.Logging.ConnectivityLogBufferSize, Components.TransportAppConfig.Logging.ConnectivityLogFlushInterval, Components.TransportAppConfig.Logging.ConnectivityLogAsyncInterval);
			}
		}

		private static string FormatTargetFailureStatus(string targetType, string targetName, ushort targetPort, bool targetMarkedUnhealthy, int currentTargetConnectionCount, int currentTargetFailureCount, ExDateTime nextRetryTime)
		{
			string result;
			if (targetMarkedUnhealthy)
			{
				result = string.Format(CultureInfo.InvariantCulture, "[{0}:{1}:{2}|MarkedUnhealthy|FailureCount:{3}|NextRetryTime:{4}]", new object[]
				{
					targetType,
					targetName,
					targetPort,
					currentTargetFailureCount,
					nextRetryTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo)
				});
			}
			else if (currentTargetConnectionCount != -2147483648)
			{
				result = string.Format(CultureInfo.InvariantCulture, "[{0}:{1}:{2}|NotMarkedUnhealthy|ActiveConnectionCount:{3}]", new object[]
				{
					targetType,
					targetName,
					targetPort,
					currentTargetConnectionCount
				});
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		private static void SmtpConnectionFailed(ulong id, string host, IPAddress address, ushort port, SocketException ex, string additionalDescr)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ConnectionLog.SessionIdToString(id),
				Destination = host,
				Direction = ">",
				Source = "SMTP",
				Description = string.Format(CultureInfo.InvariantCulture, "Failed connection to {0}:{1} ({2}:{3}){4}", new object[]
				{
					address,
					port,
					ex.SocketErrorCode.ToString(),
					ex.ErrorCode.ToString("X8", NumberFormatInfo.InvariantInfo),
					additionalDescr ?? string.Empty
				})
			}.Log("SmtpConnectionFailed");
		}

		private static void MapiDeliveryConnectionLog(ulong id, string mdb, string direction, string description, string component)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ((id == 0UL) ? string.Empty : ConnectionLog.SessionIdToString(id)),
				Destination = mdb,
				Direction = direction,
				Source = "MapiDelivery",
				Description = description
			}.Log(component);
		}

		private static void MapiSubmissionConnectionLog(ulong id, string mdb, string direction, string description, string component)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			new ConnectionLog.Row
			{
				Session = ((id == 0UL) ? string.Empty : ConnectionLog.SessionIdToString(id)),
				Destination = mdb,
				Direction = direction,
				Source = "MapiSubmission",
				Description = description
			}.Log(component);
		}

		private static void SmtpHostResolutionFailed(ulong id, string host, IPAddress reportingServer, string reason, string diagnosticInfo, bool nextHopIsOutboundProxyFrontend)
		{
			if (!ConnectionLog.enabled)
			{
				return;
			}
			ConnectionLog.Row row = new ConnectionLog.Row();
			row.Session = ConnectionLog.SessionIdToString(id);
			row.Destination = host;
			row.Direction = ">";
			row.Source = "SMTP";
			if (nextHopIsOutboundProxyFrontend)
			{
				row.Description = string.Format("{0} reported for the outbound proxy frontend server fqdn by {1}. {2}", reason, reportingServer, diagnosticInfo);
			}
			else
			{
				row.Description = string.Format("{0} reported by {1}. {2}", reason, reportingServer, diagnosticInfo);
			}
			row.Log("SmtpHostResolutionFailed");
		}

		private const string LogComponentName = "ConnectivityLog";

		private static LogSchema schema;

		private static AsyncLog log;

		private static bool enabled;

		private static string[] priorityNamesForActiveQueueCounts;

		private static string[] priorityNamesForRetryQueueCounts;

		private static bool priorityNamesForQueueCountsInitialized;

		internal class AggregateQueueStats
		{
			internal int ActiveMessageCount { get; set; }

			internal int RetryMessageCount { get; set; }

			internal int QueueCount { get; set; }

			internal AggregateQueueStats(int activeMessageCount, int retryMessageCount)
			{
				this.ActiveMessageCount = activeMessageCount;
				this.RetryMessageCount = retryMessageCount;
				this.QueueCount = 1;
			}
		}

		internal struct Direction
		{
			public const string StartConnection = "+";

			public const string ExistingConnection = ">";

			public const string Information = "*";

			public const string StopConnection = "-";
		}

		private struct Source
		{
			public const string Smtp = "SMTP";

			public const string MapiDelivery = "MapiDelivery";

			public const string MapiSubmission = "MapiSubmission";

			public const string DeliveryAgent = "DeliveryAgent";

			public const string Transport = "Transport";
		}

		private sealed class Row : LogRowFormatter
		{
			public Row() : base(ConnectionLog.Schema)
			{
			}

			public static string[] Headers
			{
				get
				{
					if (ConnectionLog.Row.headers == null)
					{
						string[] array = new string[Enum.GetValues(typeof(ConnectionLog.Row.Field)).Length];
						array[0] = "date-time";
						array[1] = "session";
						array[2] = "source";
						array[3] = "Destination";
						array[4] = "direction";
						array[5] = "description";
						Interlocked.CompareExchange<string[]>(ref ConnectionLog.Row.headers, array, null);
					}
					return ConnectionLog.Row.headers;
				}
			}

			public string Source
			{
				set
				{
					base[2] = value;
				}
			}

			public string Session
			{
				set
				{
					base[1] = value;
				}
			}

			public string Destination
			{
				set
				{
					base[3] = value;
				}
			}

			public string Direction
			{
				set
				{
					base[4] = value;
				}
			}

			public string Description
			{
				set
				{
					base[5] = value;
				}
			}

			public void Log(string component)
			{
				try
				{
					ConnectionLog.log.Append(this, 0);
				}
				catch (ObjectDisposedException)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Connection log " + component + " append failed with ObjectDisposedException");
				}
			}

			private static string[] headers;

			public enum Field
			{
				Time,
				Session,
				Source,
				Destination,
				Direction,
				Description
			}
		}
	}
}
