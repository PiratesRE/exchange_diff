using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DiagnosticsAggregation;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net.DiagnosticsAggregation;
using Microsoft.Exchange.Transport.QueueViewer;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport.DiagnosticsAggregationService
{
	internal static class DiagnosticsAggregationHelper
	{
		public static bool TryGetParsedQueueInfo(LocalLongFullPath logPath, out QueueAggregationInfo queueAggregationInfo, out Exception exception)
		{
			queueAggregationInfo = null;
			exception = null;
			if (logPath == null || string.IsNullOrEmpty(logPath.PathName))
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError(0L, "LogPath is not configured");
				exception = new ApplicationException("LogPath is not configured");
				return false;
			}
			try
			{
				string snapshotFileFullPath = DiagnosticsAggregationHelper.GetSnapshotFileFullPath(logPath.PathName);
				SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(QueueAggregationInfo));
				using (FileStream fileStream = File.OpenRead(snapshotFileFullPath))
				{
					queueAggregationInfo = (safeXmlSerializer.Deserialize(fileStream) as QueueAggregationInfo);
					return true;
				}
			}
			catch (ArgumentException ex)
			{
				exception = ex;
			}
			catch (NotSupportedException ex2)
			{
				exception = ex2;
			}
			catch (IOException ex3)
			{
				exception = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				exception = ex4;
			}
			catch (InvalidOperationException ex5)
			{
				exception = ex5;
			}
			catch (XmlException ex6)
			{
				exception = ex6;
			}
			if (exception != null)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<Exception>(0L, "Deserializing queue information failed. Details: {0}", exception);
			}
			return false;
		}

		public static void LogQueueInfo(LocalLongFullPath logPath)
		{
			if (logPath == null || string.IsNullOrEmpty(logPath.PathName))
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError(0L, "Queue log path is not configured");
				return;
			}
			string snapshotFileFullPath = DiagnosticsAggregationHelper.GetSnapshotFileFullPath(logPath.PathName);
			if (!DiagnosticsAggregationHelper.TryCreateDirectory(snapshotFileFullPath))
			{
				return;
			}
			ExTraceGlobals.DiagnosticsAggregationTracer.TraceDebug<string>(0L, "Collecting queue info. {0}", snapshotFileFullPath);
			QueueAggregationInfo allInterestingQueues = DiagnosticsAggregationHelper.GetAllInterestingQueues();
			allInterestingQueues.Time = DateTime.UtcNow;
			int num = 0;
			while (num < 2 && !DiagnosticsAggregationHelper.TrySerializeQueueInfoToFile(snapshotFileFullPath, allInterestingQueues))
			{
				num++;
			}
			QueueLog.Log(allInterestingQueues);
		}

		public static HashSet<ADObjectId> GetGroupForServer(Server localServer, ITopologyConfigurationSession session)
		{
			HashSet<ADObjectId> hashSet;
			if (localServer.DatabaseAvailabilityGroup != null)
			{
				DatabaseAvailabilityGroup databaseAvailabilityGroup = session.Read<DatabaseAvailabilityGroup>(localServer.DatabaseAvailabilityGroup);
				hashSet = ((databaseAvailabilityGroup != null && databaseAvailabilityGroup.Servers != null) ? new HashSet<ADObjectId>(databaseAvailabilityGroup.Servers) : new HashSet<ADObjectId>());
			}
			else
			{
				if (localServer.ServerSite != null)
				{
					ADPagedReader<Server> adpagedReader = session.FindPaged<Server>(null, QueryScope.SubTree, new AndFilter(new QueryFilter[]
					{
						new BitMaskAndFilter(ServerSchema.CurrentServerRole, 32UL),
						new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, localServer.ServerSite),
						new NotFilter(new ExistsFilter(ServerSchema.DatabaseAvailabilityGroup)),
						DiagnosticsAggregationHelper.IsE14OrHigherQueryFilter
					}), null, 0);
					hashSet = new HashSet<ADObjectId>();
					using (IEnumerator<Server> enumerator = adpagedReader.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Server server = enumerator.Current;
							if (server.MajorVersion == localServer.MajorVersion)
							{
								hashSet.Add(server.Id);
							}
						}
						return hashSet;
					}
				}
				hashSet = new HashSet<ADObjectId>();
			}
			return hashSet;
		}

		private static bool TrySerializeQueueInfoToFile(string fullPath, QueueAggregationInfo queueObject)
		{
			bool result;
			try
			{
				using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
				{
					SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(QueueAggregationInfo));
					safeXmlSerializer.Serialize(fileStream, queueObject);
					result = true;
				}
			}
			catch (ArgumentException arg)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<ArgumentException>(0L, "Error occured while serializing queue info. Details: {0}", arg);
				result = false;
			}
			catch (InvalidOperationException arg2)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<InvalidOperationException>(0L, "Error occured while serializing queue info. Details: {0}", arg2);
				result = false;
			}
			catch (SecurityException arg3)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<SecurityException>(0L, "Error occured while serializing queue info. Details: {0}", arg3);
				result = false;
			}
			catch (IOException arg4)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<IOException>(0L, "Error occured while serializing queue info. Details: {0}", arg4);
				result = false;
			}
			catch (UnauthorizedAccessException arg5)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<UnauthorizedAccessException>(0L, "Error occured while serializing queue info. Details: {0}", arg5);
				result = false;
			}
			catch (XmlException arg6)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<XmlException>(0L, "Error occured while serializing queue info. Details: Unable to deserialize the data element. XmlException={0}", arg6);
				result = false;
			}
			return result;
		}

		private static QueueAggregationInfo GetAllInterestingQueues()
		{
			int num = 0;
			QueueAggregationInfo queueAggregationInfo = new QueueAggregationInfo();
			SubmitMessageQueue submitMessageQueue = Components.CategorizerComponent.SubmitMessageQueue;
			RemoteMessageQueue unreachableMessageQueue = RemoteDeliveryComponent.UnreachableMessageQueue;
			PoisonMessageQueue poisonMessageQueue = Components.QueueManager.PoisonMessageQueue;
			num += submitMessageQueue.TotalCount;
			num += unreachableMessageQueue.TotalCount;
			num += poisonMessageQueue.Count;
			if (submitMessageQueue.IsInterestingQueueToLog())
			{
				queueAggregationInfo.QueueInfo.Add(DiagnosticsAggregationHelper.GetLocalQueueInfo(QueueInfoFactory.NewSubmissionQueueInfo(), submitMessageQueue.Key));
			}
			if (unreachableMessageQueue.IsInterestingQueueToLog())
			{
				queueAggregationInfo.QueueInfo.Add(DiagnosticsAggregationHelper.GetLocalQueueInfo(QueueInfoFactory.NewUnreachableQueueInfo(), unreachableMessageQueue.Key));
			}
			if (poisonMessageQueue.Count > 0)
			{
				queueAggregationInfo.QueueInfo.Add(DiagnosticsAggregationHelper.GetLocalQueueInfo(QueueInfoFactory.NewPoisonQueueInfo(), NextHopSolutionKey.Empty));
			}
			foreach (RoutedMessageQueue routedMessageQueue in Components.RemoteDeliveryComponent.GetQueueArray())
			{
				num += routedMessageQueue.TotalCount;
				if (routedMessageQueue.IsInterestingQueueToLog())
				{
					queueAggregationInfo.QueueInfo.Add(DiagnosticsAggregationHelper.GetLocalQueueInfo(QueueInfoFactory.NewDeliveryQueueInfo(routedMessageQueue), routedMessageQueue.Key));
				}
			}
			queueAggregationInfo.TotalMessageCount = num;
			queueAggregationInfo.PoisonMessageCount = poisonMessageQueue.Count;
			return queueAggregationInfo;
		}

		private static bool TryCreateDirectory(string fullPath)
		{
			string directoryName = Path.GetDirectoryName(fullPath);
			Exception ex = null;
			try
			{
				Log.CreateLogDirectory(directoryName);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			catch (InvalidOperationException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<string, Exception>(0L, "Creation of directory {0} failed. Details: {1}", directoryName, ex);
			}
			return ex == null;
		}

		private static LocalQueueInfo GetLocalQueueInfo(ExtensibleQueueInfo queueInfo, NextHopSolutionKey nextHopSolutionKey)
		{
			LocalQueueInfo localQueueInfo = new LocalQueueInfo();
			localQueueInfo.MessageCount = queueInfo.MessageCount;
			localQueueInfo.DeferredMessageCount = queueInfo.DeferredMessageCount;
			localQueueInfo.DeliveryType = queueInfo.DeliveryType.ToString();
			localQueueInfo.LockedMessageCount = queueInfo.LockedMessageCount;
			localQueueInfo.NextHopDomain = queueInfo.NextHopDomain;
			localQueueInfo.NextHopKey = nextHopSolutionKey.ToShortString();
			localQueueInfo.QueueIdentity = queueInfo.QueueIdentity.ToString();
			localQueueInfo.ServerIdentity = queueInfo.QueueIdentity.Server;
			localQueueInfo.Status = queueInfo.Status.ToString();
			localQueueInfo.RiskLevel = queueInfo.RiskLevel.ToString();
			localQueueInfo.OutboundIPPool = queueInfo.OutboundIPPool.ToString();
			localQueueInfo.Velocity = queueInfo.Velocity;
			localQueueInfo.NextHopCategory = queueInfo.NextHopCategory.ToString();
			localQueueInfo.IncomingRate = queueInfo.IncomingRate;
			localQueueInfo.OutgoingRate = queueInfo.OutgoingRate;
			localQueueInfo.TlsDomain = queueInfo.TlsDomain;
			localQueueInfo.NextHopConnector = queueInfo.NextHopConnector;
			string lastError;
			if (LastError.TryParseSmtpResponseString(queueInfo.LastError, out lastError))
			{
				localQueueInfo.LastError = lastError;
			}
			else
			{
				localQueueInfo.LastError = queueInfo.LastError;
			}
			return localQueueInfo;
		}

		private static string GetSnapshotFileFullPath(string logPath)
		{
			return Path.Combine(logPath, "QueueSnapShot.xml");
		}

		public static readonly string DiagnosticsAggregationEndpointFormat = "net.tcp://{0}:{1}/";

		public static readonly QueryFilter IsE15OrHigherQueryFilter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, Server.E15MinVersion);

		public static readonly QueryFilter IsE14OrHigherQueryFilter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, Server.E14MinVersion);
	}
}
