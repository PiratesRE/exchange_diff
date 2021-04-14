using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProtocolAnalysisBg;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Agent.Hygiene;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class ProtocolAnalysisBgAgent
	{
		public ProtocolAnalysisBgAgent(SmtpServer server)
		{
			this.server = server;
			this.hostIPAddress = new IPAddress[0];
			this.hostAddressIndex = 0;
			this.proxyIPAddress = new IPAddress[0];
			this.proxyAddressIndex = 0;
			this.pendingProxyIPQuery = false;
			ProtocolAnalysisBgAgent.LoadConfiguration();
			ConfigurationAccess.HandleConfigChangeEvent += this.OnConfigChanged;
		}

		private static string RemoveWildCards(string domain)
		{
			if (string.IsNullOrEmpty(domain) || (domain.Length == 1 && domain[0] == '*'))
			{
				return null;
			}
			if (domain.StartsWith("*.", StringComparison.OrdinalIgnoreCase))
			{
				return domain.Substring("*.".Length);
			}
			return domain;
		}

		private static void LoadConfiguration()
		{
			ProtocolAnalysisBgAgent.Settings = ConfigurationAccess.ConfigSettings;
		}

		public void Shutdown()
		{
			this.OnShutdownHandler();
		}

		public void Startup()
		{
			this.OnStartupHandler();
		}

		private void OnConfigChanged(object o, ConfigChangedEventArgs e)
		{
			if (e != null && e.Fields != null)
			{
				return;
			}
			ProtocolAnalysisBgAgent.Settings = ConfigurationAccess.ConfigSettings;
		}

		private void OnStartupHandler()
		{
			ExTraceGlobals.AgentTracer.TraceDebug((long)this.GetHashCode(), "OnStartupHandler");
			ProtocolAnalysisBgAgent.proxyDetectors = new ProxyType[7];
			ProtocolAnalysisBgAgent.proxyDetectors[0] = ProxyType.Wingate;
			ProtocolAnalysisBgAgent.proxyDetectors[1] = ProxyType.Socks4;
			ProtocolAnalysisBgAgent.proxyDetectors[2] = ProxyType.Socks5;
			ProtocolAnalysisBgAgent.proxyDetectors[3] = ProxyType.HttpConnect;
			ProtocolAnalysisBgAgent.proxyDetectors[4] = ProxyType.HttpPost;
			ProtocolAnalysisBgAgent.proxyDetectors[5] = ProxyType.Cisco;
			ProtocolAnalysisBgAgent.proxyDetectors[6] = ProxyType.Telnet;
			this.hostDnsResolutionTimer = new Timer(new TimerCallback(this.RetrieveServerIPs), null, TimeSpan.Zero, new TimeSpan(1, 0, 0));
			this.proxyDnsResolutionTimer = new Timer(new TimerCallback(this.RetrieveProxyIPs), null, TimeSpan.Zero, new TimeSpan(1, 0, 0));
			this.tablePurgeTimer = new Timer(new TimerCallback(this.PurgeTableTimerHandler), null, new TimeSpan(0, 0, 10), new TimeSpan(1, 0, 0));
			this.poller = new Thread(new ThreadStart(this.PollerThreadProc));
			this.poller.Start();
		}

		private void OnShutdownHandler()
		{
			ExTraceGlobals.AgentTracer.TraceDebug((long)this.GetHashCode(), "OnShutdownHandler");
			try
			{
				ProtocolAnalysisBgAgent.ShutDown = true;
				if (this.proxyDnsCompletionEvent != null)
				{
					this.proxyDnsCompletionEvent.Set();
				}
				if (this.hostDnsCompletionEvent != null)
				{
					this.hostDnsCompletionEvent.Set();
				}
				if (this.poller != null)
				{
					this.poller.Join();
				}
				this.WaitForAllDetections(5U);
				ConfigurationAccess.Unsubscribe();
				Database.Detach();
			}
			finally
			{
				if (this.hostDnsResolutionTimer != null)
				{
					this.hostDnsResolutionTimer.Dispose();
				}
				this.hostDnsResolutionTimer = null;
				if (this.proxyDnsResolutionTimer != null)
				{
					this.proxyDnsResolutionTimer.Dispose();
				}
				this.proxyDnsResolutionTimer = null;
				if (this.tablePurgeTimer != null)
				{
					this.tablePurgeTimer.Dispose();
				}
				this.tablePurgeTimer = null;
			}
		}

		private void RetrieveServerIPs(object state)
		{
			if (ProtocolAnalysisAgentFactory.SrlCalculationDisabled)
			{
				return;
			}
			try
			{
				List<string> list = new List<string>();
				foreach (Microsoft.Exchange.Data.Transport.AcceptedDomain acceptedDomain in this.server.AcceptedDomains)
				{
					if (acceptedDomain.IsInCorporation)
					{
						string text = ProtocolAnalysisBgAgent.RemoveWildCards(acceptedDomain.NameSpecification);
						if (!string.IsNullOrEmpty(text))
						{
							list.Add(text);
						}
					}
				}
				PaBgSmtpMxDns paBgSmtpMxDns = new PaBgSmtpMxDns(ExTraceGlobals.OnDnsQueryTracer, new PaBgSmtpMxDns.EndMxDnsResolutionCallback(this.EndHostIPResolution));
				paBgSmtpMxDns.BeginSmtpMxQueries(list);
			}
			catch (ArgumentException ex)
			{
				ProtocolAnalysisAgentFactory.LogSrlCalculationDisabled();
				ExTraceGlobals.OnDnsQueryTracer.TraceError<string>((long)this.GetHashCode(), "Failed to retrieve sender IPs. Error: {0}", ex.Message);
			}
		}

		private void RetrieveProxyIPs(object state)
		{
			if (this.pendingProxyIPQuery || ProtocolAnalysisBgAgent.Settings.ProxyServerType == ProxyType.None)
			{
				return;
			}
			IPAddress ipaddress;
			if (IPAddress.TryParse(ProtocolAnalysisBgAgent.Settings.ProxyServerName, out ipaddress))
			{
				lock (this.syncObject)
				{
					this.proxyIPAddress = new IPAddress[1];
					this.proxyIPAddress[0] = ipaddress;
					this.proxyAddressIndex = 0;
					if (this.proxyDnsResolutionTimer != null)
					{
						this.proxyDnsResolutionTimer.Change(-1, -1);
					}
				}
				return;
			}
			try
			{
				if (this.proxyDnsResolutionTimer != null)
				{
					this.proxyDnsResolutionTimer.Change(new TimeSpan(0, 5, 0), new TimeSpan(0, 5, 0));
				}
				this.pendingProxyIPQuery = true;
				ExTraceGlobals.OnDnsQueryTracer.TraceDebug<string>((long)this.GetHashCode(), "Call Dns.BeginResolveToAddresses:{0}", ProtocolAnalysisBgAgent.Settings.ProxyServerName);
				TransportFacades.Dns.BeginResolveToAddresses(ProtocolAnalysisBgAgent.Settings.ProxyServerName, AddressFamily.InterNetwork, new AsyncCallback(this.EndProxyIPResolution), null);
				this.proxyDnsCompletionEvent.Reset();
			}
			catch
			{
				this.pendingProxyIPQuery = false;
				throw;
			}
		}

		private void EndHostIPResolution(DnsStatus status, TargetHost[] targetHosts)
		{
			if (this.hostDnsCompletionEvent != null)
			{
				this.hostDnsCompletionEvent.Set();
			}
			if (targetHosts == null || status != DnsStatus.Success)
			{
				return;
			}
			int num = 0;
			lock (this.syncObject)
			{
				for (int i = 0; i < targetHosts.Length; i++)
				{
					num += targetHosts[i].IPAddresses.Count;
				}
				this.hostIPAddress = new IPAddress[num];
				num = 0;
				for (int i = 0; i < targetHosts.Length; i++)
				{
					for (int j = 0; j < targetHosts[i].IPAddresses.Count; j++)
					{
						this.hostIPAddress[num++] = targetHosts[i].IPAddresses[j];
					}
				}
				this.hostAddressIndex = 0;
			}
			ExTraceGlobals.OnDnsQueryTracer.TraceDebug<int>((long)this.GetHashCode(), "Query sender IPs returned {0} IP addresses", num);
		}

		private void EndProxyIPResolution(IAsyncResult ar)
		{
			IPAddress[] array;
			DnsStatus dnsStatus = Dns.EndResolveToAddresses(ar, out array);
			this.pendingProxyIPQuery = false;
			if (dnsStatus == DnsStatus.Success)
			{
				lock (this.syncObject)
				{
					this.proxyIPAddress = array;
					this.proxyAddressIndex = 0;
					this.proxyDnsResolutionTimer.Change(new TimeSpan(1, 0, 0), new TimeSpan(1, 0, 0));
				}
				ExTraceGlobals.OnDnsQueryTracer.TraceDebug<int>((long)this.GetHashCode(), "Query proxy IPs returned {0} IP addresses", this.proxyIPAddress.Length);
			}
			else
			{
				ExTraceGlobals.OnDnsQueryTracer.TraceDebug((long)this.GetHashCode(), "Query proxy IPs failed.");
			}
			if (this.proxyDnsCompletionEvent != null)
			{
				this.proxyDnsCompletionEvent.Set();
			}
		}

		public static IEnumerator GetProxyEnumerator()
		{
			return ProtocolAnalysisBgAgent.proxyDetectors.GetEnumerator();
		}

		public static ushort[] GetProxyPortList(ProxyType proxyType)
		{
			MultiValuedProperty<int> multiValuedProperty;
			switch (proxyType)
			{
			case ProxyType.Socks4:
				multiValuedProperty = ProtocolAnalysisBgAgent.Settings.Socks4Ports;
				break;
			case ProxyType.Socks5:
				multiValuedProperty = ProtocolAnalysisBgAgent.Settings.Socks5Ports;
				break;
			case ProxyType.HttpConnect:
				multiValuedProperty = ProtocolAnalysisBgAgent.Settings.HttpConnectPorts;
				break;
			case ProxyType.HttpPost:
				multiValuedProperty = ProtocolAnalysisBgAgent.Settings.HttpPostPorts;
				break;
			case ProxyType.Telnet:
				multiValuedProperty = ProtocolAnalysisBgAgent.Settings.TelnetPorts;
				break;
			case ProxyType.Cisco:
				multiValuedProperty = ProtocolAnalysisBgAgent.Settings.CiscoPorts;
				break;
			case ProxyType.Wingate:
				multiValuedProperty = ProtocolAnalysisBgAgent.Settings.WingatePorts;
				break;
			default:
				throw new LocalizedException(AgentStrings.InvalidProxyType);
			}
			int[] array = (int[])Array.CreateInstance(typeof(int), multiValuedProperty.Count);
			ushort[] array2 = (ushort[])Array.CreateInstance(typeof(ushort), multiValuedProperty.Count);
			int num = 0;
			multiValuedProperty.CopyTo(array, 0);
			foreach (int num2 in array)
			{
				array2[num++] = (ushort)num2;
			}
			return array2;
		}

		public void PollerThreadProc()
		{
			this.hostDnsCompletionEvent.WaitOne();
			this.hostDnsCompletionEvent.Reset();
			this.proxyDnsCompletionEvent.WaitOne();
			this.proxyDnsCompletionEvent.Reset();
			ExTraceGlobals.AgentTracer.TraceDebug((long)this.GetHashCode(), "Agent's worker thread started");
			StsWorkItem stsWorkItem = new StsWorkItem();
			while (!ProtocolAnalysisBgAgent.ShutDown)
			{
				if ((long)ProtocolAnalysisBgAgent.NumDetections + ProtocolAnalysisBgAgent.NumQueries < (long)ProtocolAnalysisBgAgent.Settings.MaxPendingOperations && stsWorkItem.Poll())
				{
					ExTraceGlobals.AgentTracer.TraceDebug<IPAddress, WorkItemType>((long)this.GetHashCode(), "Load work item sender: {0}, type: {1}", stsWorkItem.SenderAddress, stsWorkItem.WorkType);
					switch (stsWorkItem.WorkType)
					{
					case WorkItemType.OpenProxyDetection:
						if (ProtocolAnalysisBgAgent.Settings.ProxyServerType != ProxyType.None)
						{
							stsWorkItem.StartOpenProxyTest(this.GetRandomHostEndpoint(), this.GetRandomProxyIPAddress(), (int)((ushort)ProtocolAnalysisBgAgent.Settings.ProxyServerPort), ProtocolAnalysisBgAgent.Settings.ProxyServerType, new NetworkCredential());
						}
						else
						{
							stsWorkItem.StartOpenProxyTest(this.GetRandomHostEndpoint());
						}
						break;
					case WorkItemType.ReverseDnsQuery:
						stsWorkItem.StartReverseDnsQuery();
						break;
					case WorkItemType.BlockSender:
						stsWorkItem.BlockSender(this.server);
						break;
					default:
						ExTraceGlobals.AgentTracer.TraceError<WorkItemType>((long)this.GetHashCode(), "Invalid work type: {0}", stsWorkItem.WorkType);
						break;
					}
				}
				else
				{
					Thread.Sleep(2000);
				}
			}
			ExTraceGlobals.AgentTracer.TraceDebug((long)this.GetHashCode(), "Agent's worker thread started");
		}

		private void PurgeTableTimerHandler(object state)
		{
			Database.PurgeTable(new TimeSpan(ProtocolAnalysisBgAgent.Settings.TablePurgeInterval, 0, 0), new TimeSpan(ProtocolAnalysisBgAgent.Settings.TablePurgeInterval, 0, 0), ExTraceGlobals.DatabaseTracer);
			ExTraceGlobals.DatabaseTracer.TraceDebug((long)this.GetHashCode(), "Purge database records for PA and OP");
		}

		private void WaitForAllDetections(uint maxWaitSeconds)
		{
			while (ProtocolAnalysisBgAgent.NumDetections > 0 || ProtocolAnalysisBgAgent.NumQueries > 0L || this.pendingProxyIPQuery)
			{
				if (maxWaitSeconds == 0U)
				{
					return;
				}
				maxWaitSeconds -= 1U;
				Thread.Sleep(1000);
			}
		}

		private IPEndPoint GetRandomHostEndpoint()
		{
			IPAddress address = IPAddress.Any;
			lock (this.syncObject)
			{
				if (this.hostIPAddress.Length > 0)
				{
					if (this.hostAddressIndex >= this.hostIPAddress.Length)
					{
						this.hostAddressIndex = 0;
					}
					address = this.hostIPAddress[this.hostAddressIndex++];
				}
			}
			if (this.hostIPAddress.Length <= 0)
			{
				return null;
			}
			return new IPEndPoint(address, 25);
		}

		private IPAddress GetRandomProxyIPAddress()
		{
			IPAddress result = IPAddress.Any;
			lock (this.syncObject)
			{
				if (this.proxyIPAddress.Length > 0)
				{
					if (this.proxyAddressIndex >= this.proxyIPAddress.Length)
					{
						this.proxyAddressIndex = 0;
					}
					result = this.proxyIPAddress[this.proxyAddressIndex++];
				}
			}
			return result;
		}

		internal const int DetectionTimeoutInSeconds = 10;

		internal const string SmtpGreeting = "220 ";

		private const string WildCardPrefix = "*.";

		private const char WildCardChar = '*';

		private const int SmtpPort = 25;

		private const int DnsResolutionIntervalInHours = 1;

		private const int PurgeTableIntervalInHours = 1;

		private const int DnsRetryIntervalInMinutes = 5;

		private const int PollIntervalInSeconds = 2;

		internal static ExEventLog EventLogger = new ExEventLog(ExTraceGlobals.FactoryTracer.Category, "MSExchange Antispam");

		internal static SenderReputationConfig Settings;

		internal static bool ShutDown;

		private static ProxyType[] proxyDetectors;

		private readonly object syncObject = new object();

		private IPAddress[] hostIPAddress;

		private int hostAddressIndex;

		private Timer hostDnsResolutionTimer;

		private ManualResetEvent hostDnsCompletionEvent = new ManualResetEvent(true);

		private IPAddress[] proxyIPAddress;

		private int proxyAddressIndex;

		private Timer proxyDnsResolutionTimer;

		private bool pendingProxyIPQuery;

		private ManualResetEvent proxyDnsCompletionEvent = new ManualResetEvent(true);

		private Timer tablePurgeTimer;

		private Thread poller;

		private SmtpServer server;

		public static int NumDetections;

		public static long NumQueries;

		public static long NumSockets;

		internal sealed class PerformanceCounters
		{
			public static void PositiveOpenProxy(ProxyType type)
			{
				switch (type)
				{
				case ProxyType.Socks4:
					ProtocolAnalysisBgPerfCounters.Socks4Proxy.Increment();
					return;
				case ProxyType.Socks5:
					ProtocolAnalysisBgPerfCounters.Socks5Proxy.Increment();
					return;
				case ProxyType.HttpConnect:
					ProtocolAnalysisBgPerfCounters.HttpConnectProxy.Increment();
					return;
				case ProxyType.HttpPost:
					ProtocolAnalysisBgPerfCounters.HttpPostProxy.Increment();
					return;
				case ProxyType.Telnet:
					ProtocolAnalysisBgPerfCounters.TelnetProxy.Increment();
					return;
				case ProxyType.Cisco:
					ProtocolAnalysisBgPerfCounters.CiscoProxy.Increment();
					return;
				case ProxyType.Wingate:
					ProtocolAnalysisBgPerfCounters.WingateProxy.Increment();
					return;
				default:
					return;
				}
			}

			public static void NegativeOpenProxy()
			{
				ProtocolAnalysisBgPerfCounters.NegativeOpenProxy.Increment();
			}

			public static void UnknownOpenProxy()
			{
				ProtocolAnalysisBgPerfCounters.UnknownOpenProxy.Increment();
			}

			public static void TotalOpenProxy()
			{
				ProtocolAnalysisBgPerfCounters.TotalOpenProxy.Increment();
			}

			public static void ReverseDnsSucc()
			{
				ProtocolAnalysisBgPerfCounters.ReverseDnsSucc.Increment();
			}

			public static void ReverseDnsFail()
			{
				ProtocolAnalysisBgPerfCounters.ReverseDnsFail.Increment();
			}

			public static void BlockSender()
			{
				ProtocolAnalysisBgPerfCounters.BlockSender.Increment();
			}

			public static void RemoveCounters()
			{
				ProtocolAnalysisBgPerfCounters.Socks4Proxy.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.Socks5Proxy.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.HttpConnectProxy.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.HttpPostProxy.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.TelnetProxy.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.CiscoProxy.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.WingateProxy.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.NegativeOpenProxy.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.UnknownOpenProxy.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.TotalOpenProxy.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.ReverseDnsSucc.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.ReverseDnsFail.RawValue = 0L;
				ProtocolAnalysisBgPerfCounters.BlockSender.RawValue = 0L;
			}
		}
	}
}
