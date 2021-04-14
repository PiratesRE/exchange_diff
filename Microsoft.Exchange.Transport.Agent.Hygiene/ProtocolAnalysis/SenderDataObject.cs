using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProtocolAnalysis;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;
using Microsoft.Exchange.Transport.Agent.Hygiene;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Configuration;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	internal class SenderDataObject : SenderDBObject
	{
		public SenderDataObject(IPAddress senderIP) : base(senderIP, ExTraceGlobals.DatabaseTracer)
		{
		}

		public void ProcessSender(FactorySenderData factoryData, SenderReputationConfig settings, ProtocolAnalysisSrlSettings srlSettings, AcceptedDomainCollection acceptedDomains)
		{
			if (factoryData == null)
			{
				throw new ArgumentNullException("factoryData");
			}
			ProtocolAnalysisAgentFactory.PerformanceCounters.SenderProcessed();
			bool flag = false;
			bool reverseDnsQuery = false;
			bool flag2 = false;
			bool flag3 = true;
			string text = string.Empty;
			ProtocolAnalysisData protocolAnalysisData = null;
			if (base.Load())
			{
				ExTraceGlobals.DatabaseTracer.TraceDebug((long)this.GetHashCode(), "Load data for sender:{0}, OpenProxy:{1},{2},{3}, ReverseDNS:{4},{5},{6}, Update:{7},{8},{9}", new object[]
				{
					base.SenderAddress,
					base.OpenProxyDetectionTime,
					base.OpenProxyDetectionPending,
					base.OpenProxyStatus,
					base.ReverseDns,
					base.ReverseDnsQueryTime,
					base.ReverseDnsQueryPending,
					base.SenderReputationLevel,
					base.SenderReputationIsOpenProxy,
					base.SenderReputationExpirationTime
				});
				try
				{
					if (base.ProtocolAnalysisDataBlob != null && base.ProtocolAnalysisDataBlob.Length != 0)
					{
						BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(new AgentDeserializationBinder());
						protocolAnalysisData = (ProtocolAnalysisData)binaryFormatter.Deserialize(new MemoryStream(base.ProtocolAnalysisDataBlob));
						ExTraceGlobals.DatabaseTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "Successfully load and deserialize data for sender: {0}", base.SenderAddress);
						if (protocolAnalysisData.SenderDBData == null)
						{
							ExTraceGlobals.DatabaseTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "ProtocolAnalysisData SenderDBData deserialized to NULL for sender: {0}", base.SenderAddress);
							protocolAnalysisData = new ProtocolAnalysisData(DateTime.UtcNow, srlSettings.InitWinLen);
						}
					}
					else
					{
						protocolAnalysisData = new ProtocolAnalysisData(DateTime.UtcNow, srlSettings.InitWinLen);
						ExTraceGlobals.DatabaseTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "Sender {0} does not have protocol analysis data.", base.SenderAddress);
					}
					goto IL_207;
				}
				catch (SerializationException ex)
				{
					ExTraceGlobals.CalculateSrlTracer.TraceDebug<IPAddress, string>((long)this.GetHashCode(), "Failed to deserialize protocol analysis data for sender: {0}, error: {1}", base.SenderAddress, ex.Message);
					protocolAnalysisData = new ProtocolAnalysisData(DateTime.UtcNow, srlSettings.InitWinLen);
					goto IL_207;
				}
			}
			protocolAnalysisData = new ProtocolAnalysisData(DateTime.UtcNow, srlSettings.InitWinLen);
			ExTraceGlobals.DatabaseTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "Failed to load protocol analysis data for sender: {0}", base.SenderAddress);
			IL_207:
			protocolAnalysisData.SenderDBData.Merge(factoryData, string.IsNullOrEmpty(factoryData.ReverseDns) ? base.ReverseDns : factoryData.ReverseDns, base.SenderAddress, acceptedDomains);
			if (base.OpenProxyStatus == OPDetectionResult.IsOpenProxy && !base.OpenProxyDetectionPending)
			{
				if (DateTime.UtcNow.Subtract(base.OpenProxyDetectionTime).TotalHours < (double)settings.OpenProxyRescanInterval)
				{
					ExTraceGlobals.CalculateSrlTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "Sender {0} is found to be an open proxy.", base.SenderAddress);
					ProtocolAnalysisAgentFactory.PerformanceCounters.BlockSenderLocalOpenProxy();
					flag2 = true;
					text = AgentStrings.BlockSenderLocalOP;
					flag3 = false;
				}
				else
				{
					flag = true;
				}
			}
			if (DateTime.Compare(base.SenderReputationExpirationTime, DateTime.UtcNow) > 0)
			{
				if (base.SenderReputationLevel == -1)
				{
					ExTraceGlobals.CalculateSrlTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "Bypass local Srl calculation for sender {0}.", base.SenderAddress);
					ProtocolAnalysisAgentFactory.PerformanceCounters.BypassSrlCalculation();
					flag3 = false;
					flag = false;
				}
				else if (base.SenderReputationIsOpenProxy)
				{
					ExTraceGlobals.CalculateSrlTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "Sender {0} is identified as an open proxy by update service.", base.SenderAddress);
					ProtocolAnalysisAgentFactory.PerformanceCounters.BlockSenderRemoteOpenProxy();
					flag2 = true;
					text = AgentStrings.BlockSenderRemoteOP;
					flag3 = false;
					flag = false;
				}
				else if (base.SenderReputationLevel >= settings.SrlBlockThreshold)
				{
					ExTraceGlobals.CalculateSrlTracer.TraceDebug<IPAddress, int>((long)this.GetHashCode(), "Sender {0} has a high Srl value {1} from update service.", base.SenderAddress, base.SenderReputationLevel);
					ProtocolAnalysisAgentFactory.PerformanceCounters.BlockSenderRemoteSrl();
					flag2 = true;
					text = AgentStrings.BlockSenderRemoteSRL;
					flag3 = false;
					flag = false;
				}
			}
			if ((string.IsNullOrEmpty(factoryData.ReverseDns) || DateTime.UtcNow.Subtract(base.ReverseDnsQueryTime).TotalHours >= (double)settings.MinReverseDnsQueryPeriod) && !base.ReverseDnsQueryPending)
			{
				reverseDnsQuery = true;
			}
			if (string.IsNullOrEmpty(factoryData.ReverseDns) && base.ReverseDnsQueryTime.Equals(DateTime.MinValue))
			{
				ExTraceGlobals.CalculateSrlTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "First time sender:{0}, don't calculate srl.", base.SenderAddress);
				flag3 = false;
			}
			if (flag3)
			{
				int num = protocolAnalysisData.AlgorithmState(factoryData.NumMsgs, settings.SrlBlockThreshold, srlSettings, base.ReverseDns, base.SenderAddress);
				ProtocolAnalysisAgentFactory.PerformanceCounters.SenderSrl(num);
				ExTraceGlobals.CalculateSrlTracer.TraceDebug<IPAddress, int>((long)this.GetHashCode(), "Sender {0} has a local srl value {1}.", base.SenderAddress, num);
				if (num >= settings.SrlBlockThreshold)
				{
					ProtocolAnalysisAgentFactory.PerformanceCounters.BlockSenderLocalSrl();
					flag2 = true;
					text = AgentStrings.BlockSenderLocalSRL;
					flag = false;
					reverseDnsQuery = false;
				}
				else
				{
					protocolAnalysisData.Update(DateTime.UtcNow, settings.MinMessagesPerTimeSlice, settings.TimeSliceInterval);
					if (num >= settings.SrlBlockThreshold - 1 && !base.OpenProxyDetectionPending && !flag)
					{
						if (base.OpenProxyStatus == OPDetectionResult.IsOpenProxy)
						{
							throw new InvalidOperationException("The sender can't be an open proxy.");
						}
						if (DateTime.UtcNow.Subtract(base.OpenProxyDetectionTime).TotalHours >= (double)settings.OpenProxyRescanInterval)
						{
							flag = true;
						}
					}
				}
			}
			if (!settings.SenderBlockingEnabled && flag2)
			{
				ExTraceGlobals.CalculateSrlTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "Not block sender {0} because SenderBlockingEnabled is false.", base.SenderAddress);
				flag2 = false;
			}
			if (flag2)
			{
				ExTraceGlobals.CalculateSrlTracer.TraceDebug<IPAddress, string>((long)this.GetHashCode(), "Block sender {0}, reason: {1}.", base.SenderAddress, text);
				this.InsertWorkItem(base.SenderAddress, settings.SenderBlockingPeriod, "Comment: " + text);
				protocolAnalysisData.BlockSender(DateTime.UtcNow);
			}
			if (!settings.OpenProxyDetectionEnabled)
			{
				flag = false;
			}
			this.SaveProtocolAnalysisData(protocolAnalysisData, settings, flag, reverseDnsQuery);
		}

		internal void InsertWorkItem(IPAddress senderIP, WorkItemType workType, WorkItemPriority priority)
		{
			if (workType != WorkItemType.OpenProxyDetection && workType != WorkItemType.ReverseDnsQuery)
			{
				throw new ArgumentOutOfRangeException("workType", workType, "This flavor of InsertWorkItem is used only for open proxy and reverse dns lookup");
			}
			if (SenderDataObject.IsQueueFull() || ProtocolAnalysisAgentFactory.SrlCalculationDisabled)
			{
				return;
			}
			WorkQueue.EnqueueWorkItemData(new WorkItemData
			{
				SenderAddress = senderIP,
				WorkType = workType,
				Priority = priority,
				InsertTime = DateTime.UtcNow
			});
			ExTraceGlobals.DatabaseTracer.TraceDebug<IPAddress, WorkItemType>((long)this.GetHashCode(), "Insert sender {0} data into work queue for work type {1}", senderIP, workType);
		}

		internal void InsertWorkItem(IPAddress senderIP, int blockPeriod, string blockComment)
		{
			if (SenderDataObject.IsQueueFull() || ProtocolAnalysisAgentFactory.SrlCalculationDisabled)
			{
				return;
			}
			WorkQueue.EnqueueWorkItemData(new WorkItemData
			{
				SenderAddress = senderIP,
				BlockPeriod = blockPeriod,
				BlockComment = blockComment,
				WorkType = WorkItemType.BlockSender,
				Priority = WorkItemPriority.BlockSenderPriority,
				InsertTime = DateTime.UtcNow
			});
			ExTraceGlobals.DatabaseTracer.TraceDebug<IPAddress, WorkItemType>((long)this.GetHashCode(), "Insert sender {0} data into work queue for work type {1}", senderIP, WorkItemType.BlockSender);
		}

		private static bool IsQueueFull()
		{
			if (WorkQueue.Count < ConfigurationAccess.ConfigSettings.MaxWorkQueueSize)
			{
				return false;
			}
			ProtocolAnalysisAgent.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_AgentQueueFull, null, null);
			return true;
		}

		private bool SaveProtocolAnalysisData(ProtocolAnalysisData protocolAnalysisData, SenderReputationConfig settings, bool openProxyDetection, bool reverseDnsQuery)
		{
			bool flag = false;
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				if (reverseDnsQuery || openProxyDetection)
				{
					if (WorkQueue.Count > settings.MaxWorkQueueSize)
					{
						reverseDnsQuery = false;
						openProxyDetection = false;
						ExTraceGlobals.DatabaseTracer.TraceDebug((long)this.GetHashCode(), "Work queue table is full.");
					}
					else
					{
						WorkItemType workType = openProxyDetection ? WorkItemType.OpenProxyDetection : WorkItemType.ReverseDnsQuery;
						WorkItemPriority priority = openProxyDetection ? WorkItemPriority.OpenProxyPriority : WorkItemPriority.ReverseDnsQueryPriority;
						this.InsertWorkItem(base.SenderAddress, workType, priority);
					}
				}
				binaryFormatter.Serialize(memoryStream, protocolAnalysisData);
				flag = base.Update(memoryStream.ToArray(), openProxyDetection, reverseDnsQuery);
				ExTraceGlobals.DatabaseTracer.TraceDebug<IPAddress, bool>((long)this.GetHashCode(), "Write sender {0} data back to database, result {1}", base.SenderAddress, flag);
			}
			catch (SerializationException ex)
			{
				ExTraceGlobals.DatabaseTracer.TraceError<IPAddress, string>((long)this.GetHashCode(), "Failed to serialize sender {0} data, exception: {1}", base.SenderAddress, ex.Message);
			}
			finally
			{
				memoryStream.Dispose();
			}
			return flag;
		}

		private const int UpdateServiceBypassLocalSrl = -1;
	}
}
