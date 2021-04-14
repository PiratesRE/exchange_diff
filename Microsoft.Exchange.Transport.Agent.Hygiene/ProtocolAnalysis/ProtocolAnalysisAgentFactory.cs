using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProtocolAnalysis;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Configuration;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Update;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	public sealed class ProtocolAnalysisAgentFactory : SmtpReceiveAgentFactory
	{
		public ProtocolAnalysisAgentFactory()
		{
			this.factorySenders = new HybridDictionary(100);
			this.sendersToFlush = new ArrayList();
			ProtocolAnalysisAgent.AgentFactory = this;
			this.LoadConfiguration();
			Database.Attach();
			this.LoadSrlConfiguration();
			ConfigurationAccess.HandleConfigChangeEvent += this.OnConfigChanged;
			this.factQueue = new ProtocolAnalysisAgentFactory.FactoryPendingQueue();
			this.shutDown = false;
			this.queueThread = new Thread(new ThreadStart(this.QueueThreadProc));
			this.queueThread.Start();
			this.bgAgentFactory = new ProtocolAnalysisBgAgentFactory();
			this.updateAgentFactory = new StsUpdateAgentFactory();
		}

		internal static bool SrlCalculationDisabled
		{
			get
			{
				return ProtocolAnalysisAgentFactory.IsDnsServerListEmpty();
			}
		}

		internal ProtocolAnalysisBgAgentFactory BgAgentFactory
		{
			get
			{
				return this.bgAgentFactory;
			}
		}

		public static bool IsDnsServerListEmpty()
		{
			return 0 == TransportFacades.Dns.ServerList.Count;
		}

		public override void Close()
		{
			this.shutDown = true;
			this.queueThread.Join(5000);
			ConfigurationAccess.Unsubscribe();
			Database.Detach();
			ProtocolAnalysisAgentFactory.PerformanceCounters.RemoveCounters();
			this.bgAgentFactory.Close();
			this.updateAgentFactory.Close();
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server", "ProtocolAnalysisAgentFactory.CreateAgent: server parameter must not be null");
			}
			this.smtpFactoryServer = server;
			ProtocolAnalysisAgent result = new ProtocolAnalysisAgent(this, server, this.settings);
			this.bgAgentFactory.CreateAgent(server);
			this.updateAgentFactory.CreateAgent(server);
			return result;
		}

		public void OnDisconnect(IDictionary agentSenders)
		{
			this.factQueue.Enqueue(agentSenders);
		}

		internal static void LogSrlCalculationDisabled()
		{
			ProtocolAnalysisAgent.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_DnsNotConfigured, null, null);
		}

		private void OnConfigChanged(object o, ConfigChangedEventArgs e)
		{
			if (e != null && e.Fields != null)
			{
				this.srlSettings.Fields = e.Fields;
				return;
			}
			this.LoadConfiguration();
		}

		private void LoadConfiguration()
		{
			this.settings = ConfigurationAccess.ConfigSettings;
			this.maxIdleTime = this.settings.MaxIdleTime;
		}

		private void LoadSrlConfiguration()
		{
			this.srlSettings = new ProtocolAnalysisSrlSettings();
			PropertyBag propertyBag = Database.ScanSrlConfiguration();
			if (propertyBag == null || propertyBag.Count < 74)
			{
				this.srlSettings.InitializeDefaults();
				return;
			}
			this.srlSettings.Fields = propertyBag;
		}

		private void QueueThreadProc()
		{
			ExTraceGlobals.FactoryTracer.TraceDebug(0L, "ProtocolAnalysisAgentFactory thread starts");
			while (!this.shutDown)
			{
				IDictionary dictionary = this.factQueue.Dequeue();
				if (dictionary == null)
				{
					Thread.Sleep(1000);
				}
				else
				{
					this.ProcessSenderData(dictionary);
				}
			}
			this.FlushIdleSenders();
			if (this.factorySenders.Count != 0)
			{
				throw new InvalidOperationException("There are still senders left");
			}
			ExTraceGlobals.FactoryTracer.TraceDebug(0L, "ProtocolAnalysisAgentFactory thread shuts down");
		}

		private void ProcessSenderData(IDictionary agentSenders)
		{
			IDictionaryEnumerator enumerator = agentSenders.GetEnumerator();
			while (!this.shutDown && enumerator.MoveNext())
			{
				bool flag = false;
				FactorySenderData factorySenderData = null;
				AgentSenderData agentSenderData = (AgentSenderData)enumerator.Value;
				IPAddress ipaddress = (IPAddress)enumerator.Key;
				if (agentSenderData == null)
				{
					throw new InvalidOperationException("Failed to retrieve value from SenderCollection.");
				}
				if (!StsUtil.IsValidSenderIP(ipaddress))
				{
					ExTraceGlobals.FactoryTracer.TraceDebug(0L, "senderIP is not valid.  Exiting ProcessSenderData()");
					return;
				}
				int num = agentSenderData.NumMsgs;
				if (this.factorySenders.Contains(ipaddress))
				{
					factorySenderData = (FactorySenderData)this.factorySenders[ipaddress];
					if (factorySenderData == null)
					{
						throw new InvalidOperationException("Can't find senderCollection inside agent factory.");
					}
					num += factorySenderData.NumMsgs;
				}
				if (num >= this.settings.MinMessagesPerDatabaseTransaction)
				{
					flag = true;
					this.factorySenders.Remove(ipaddress);
				}
				else
				{
					if (factorySenderData == null)
					{
						factorySenderData = new FactorySenderData(DateTime.UtcNow);
					}
					factorySenderData.Merge(agentSenderData);
					this.factorySenders[ipaddress] = factorySenderData;
					if (!this.factorySenders.Contains(ipaddress))
					{
						throw new InvalidOperationException("Failed to add sender back to factory collection.");
					}
				}
				if (!this.shutDown && flag)
				{
					this.FlushSender(ipaddress, factorySenderData, agentSenderData);
				}
			}
			if (this.shutDown)
			{
				return;
			}
			this.FlushIdleSenders();
		}

		private void FlushIdleSenders()
		{
			IDictionaryEnumerator enumerator = this.factorySenders.GetEnumerator();
			while (enumerator.MoveNext())
			{
				FactorySenderData factorySenderData = (FactorySenderData)enumerator.Value;
				if (this.shutDown || DateTime.UtcNow.Subtract(factorySenderData.LastUpdateTime).TotalMinutes >= (double)this.maxIdleTime)
				{
					this.sendersToFlush.Add((IPAddress)enumerator.Key);
				}
			}
			for (int i = 0; i < this.sendersToFlush.Count; i++)
			{
				IPAddress ipaddress = (IPAddress)this.sendersToFlush[i];
				FactorySenderData factoryData = (FactorySenderData)this.factorySenders[ipaddress];
				this.factorySenders.Remove(ipaddress);
				this.FlushSender(ipaddress, factoryData, null);
			}
			this.sendersToFlush.Clear();
		}

		private void FlushSender(IPAddress senderIP, FactorySenderData factoryData, AgentSenderData agentData)
		{
			if (!StsUtil.IsValidSenderIP(senderIP))
			{
				throw new ArgumentOutOfRangeException("senderIP", senderIP, "The sender address must be valid");
			}
			ExTraceGlobals.CalculateSrlTracer.TraceDebug<IPAddress>(0L, "Ready to flush sender data, IP: {0}", senderIP);
			if (factoryData == null)
			{
				factoryData = new FactorySenderData(DateTime.UtcNow);
			}
			if (agentData != null)
			{
				factoryData.Merge(agentData);
			}
			SenderDataObject senderDataObject = new SenderDataObject(senderIP);
			if (this.shutDown)
			{
				return;
			}
			senderDataObject.ProcessSender(factoryData, this.settings, this.srlSettings, this.smtpFactoryServer.AcceptedDomains);
		}

		private const int MinSrlValues = 74;

		internal static bool FirstConnect = true;

		private HybridDictionary factorySenders;

		private ArrayList sendersToFlush;

		private ProtocolAnalysisAgentFactory.FactoryPendingQueue factQueue;

		private Thread queueThread;

		private bool shutDown;

		private SmtpServer smtpFactoryServer;

		private int maxIdleTime;

		private ProtocolAnalysisBgAgentFactory bgAgentFactory;

		private StsUpdateAgentFactory updateAgentFactory;

		private SenderReputationConfig settings;

		private ProtocolAnalysisSrlSettings srlSettings;

		internal sealed class PerformanceCounters
		{
			public static void SenderSrl(int srl)
			{
				if (srl < 0 || srl > 9)
				{
					throw new ArgumentOutOfRangeException("srl", srl, "SRL must be within 0-9");
				}
				if (ProtocolAnalysisAgentFactory.PerformanceCounters.srlCounters != null)
				{
					ProtocolAnalysisAgentFactory.PerformanceCounters.srlCounters[srl].Increment();
				}
			}

			public static void BlockSenderLocalSrl()
			{
				ProtocolAnalysisPerfCounters.BlockSenderLocalSrl.Increment();
			}

			public static void BlockSenderRemoteSrl()
			{
				ProtocolAnalysisPerfCounters.BlockSenderRemoteSrl.Increment();
			}

			public static void BlockSenderLocalOpenProxy()
			{
				ProtocolAnalysisPerfCounters.BlockSenderLocalOpenProxy.Increment();
			}

			public static void BlockSenderRemoteOpenProxy()
			{
				ProtocolAnalysisPerfCounters.BlockSenderRemoteOpenProxy.Increment();
			}

			public static void BypassSrlCalculation()
			{
				ProtocolAnalysisPerfCounters.BypassSrlCalculation.Increment();
			}

			public static void SenderProcessed()
			{
				ProtocolAnalysisPerfCounters.SenderProcessed.Increment();
			}

			public static void RemoveCounters()
			{
				ProtocolAnalysisPerfCounters.BlockSenderLocalSrl.RawValue = 0L;
				ProtocolAnalysisPerfCounters.BlockSenderRemoteSrl.RawValue = 0L;
				ProtocolAnalysisPerfCounters.BlockSenderLocalOpenProxy.RawValue = 0L;
				ProtocolAnalysisPerfCounters.BlockSenderRemoteOpenProxy.RawValue = 0L;
				ProtocolAnalysisPerfCounters.BypassSrlCalculation.RawValue = 0L;
				ProtocolAnalysisPerfCounters.SenderProcessed.RawValue = 0L;
				foreach (ExPerformanceCounter exPerformanceCounter in ProtocolAnalysisAgentFactory.PerformanceCounters.srlCounters)
				{
					exPerformanceCounter.RawValue = 0L;
				}
			}

			private static ExPerformanceCounter[] srlCounters = new ExPerformanceCounter[]
			{
				ProtocolAnalysisPerfCounters.SenderSRL0,
				ProtocolAnalysisPerfCounters.SenderSRL1,
				ProtocolAnalysisPerfCounters.SenderSRL2,
				ProtocolAnalysisPerfCounters.SenderSRL3,
				ProtocolAnalysisPerfCounters.SenderSRL4,
				ProtocolAnalysisPerfCounters.SenderSRL5,
				ProtocolAnalysisPerfCounters.SenderSRL6,
				ProtocolAnalysisPerfCounters.SenderSRL7,
				ProtocolAnalysisPerfCounters.SenderSRL8,
				ProtocolAnalysisPerfCounters.SenderSRL9
			};
		}

		internal sealed class FactoryPendingQueue
		{
			public IDictionary Dequeue()
			{
				if (this.factQueue.Count > 0)
				{
					return (IDictionary)this.factQueue.Dequeue();
				}
				return null;
			}

			public void Enqueue(IDictionary sender)
			{
				this.factQueue.Enqueue(sender);
			}

			private Queue factQueue = Queue.Synchronized(new Queue());
		}
	}
}
