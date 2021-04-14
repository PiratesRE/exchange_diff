using System;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.ProtocolAnalysisBg;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class StsWorkItem
	{
		public bool Poll()
		{
			WorkItemData workItemData = WorkQueue.DequeueWorkItemData();
			if (workItemData == null)
			{
				return false;
			}
			this.item = new WorkItemData();
			this.item.Priority = workItemData.Priority;
			this.item.SenderAddress = workItemData.SenderAddress;
			this.item.InsertTime = workItemData.InsertTime;
			this.item.BlockPeriod = workItemData.BlockPeriod;
			this.item.BlockComment = workItemData.BlockComment;
			this.item.WorkType = workItemData.WorkType;
			return true;
		}

		public IPAddress SenderAddress
		{
			get
			{
				return this.item.SenderAddress;
			}
		}

		public WorkItemType WorkType
		{
			get
			{
				return this.item.WorkType;
			}
		}

		public int BlockPeriod
		{
			get
			{
				return this.item.BlockPeriod;
			}
		}

		public string BlockComment
		{
			get
			{
				return this.item.BlockComment;
			}
		}

		public void UpdateOpenProxyResult(OPDetectionResult result, ProxyType positiveProxyType, string message, IPAddress sender)
		{
			Interlocked.Decrement(ref ProtocolAnalysisBgAgent.NumDetections);
			if (ProtocolAnalysisBgAgent.ShutDown)
			{
				return;
			}
			switch (result)
			{
			case OPDetectionResult.Unknown:
				ProtocolAnalysisBgAgent.PerformanceCounters.UnknownOpenProxy();
				break;
			case OPDetectionResult.IsOpenProxy:
				ProtocolAnalysisBgAgent.PerformanceCounters.PositiveOpenProxy(positiveProxyType);
				break;
			case OPDetectionResult.NotOpenProxy:
				ProtocolAnalysisBgAgent.PerformanceCounters.NegativeOpenProxy();
				break;
			}
			ProtocolAnalysisBgAgent.PerformanceCounters.TotalOpenProxy();
			Database.UpdateOpenProxy(sender.ToString(), result, message, ExTraceGlobals.OnOpenProxyDetectionTracer);
		}

		public void StartOpenProxyTest(IPEndPoint hostEndpoint)
		{
			if (hostEndpoint == null)
			{
				return;
			}
			Interlocked.Increment(ref ProtocolAnalysisBgAgent.NumDetections);
			if (!StsUtil.IsValidSenderIP(hostEndpoint.Address))
			{
				ExTraceGlobals.OnOpenProxyDetectionTracer.TraceError((long)this.GetHashCode(), "Invalid local IP Address");
				this.UpdateOpenProxyResult(OPDetectionResult.Unknown, ProxyType.None, string.Empty, this.SenderAddress);
				return;
			}
			this.endOPDetectionCallback = new StsWorkItem.EndOPDetectionCallback(this.UpdateOpenProxyResult);
			this.proxyTest = new ProxyTest(this.endOPDetectionCallback, ProtocolAnalysisBgAgent.GetProxyEnumerator());
			OPDetectionResult opdetectionResult = this.proxyTest.BeginOPDetection(null, ProxyType.None, new NetworkCredential(), hostEndpoint, this.SenderAddress);
			if (opdetectionResult != OPDetectionResult.Pending)
			{
				this.UpdateOpenProxyResult(opdetectionResult, ProxyType.None, string.Empty, this.SenderAddress);
			}
		}

		public void StartOpenProxyTest(IPEndPoint hostEndpoint, IPAddress proxyIP, int proxyPort, ProxyType proxyType, NetworkCredential authInfo)
		{
			if (hostEndpoint == null)
			{
				return;
			}
			Interlocked.Increment(ref ProtocolAnalysisBgAgent.NumDetections);
			if (!StsUtil.IsValidSenderIP(hostEndpoint.Address) || !StsUtil.IsValidSenderIP(proxyIP))
			{
				ExTraceGlobals.OnOpenProxyDetectionTracer.TraceError((long)this.GetHashCode(), "Invalid local or proxy IP Address");
				this.UpdateOpenProxyResult(OPDetectionResult.Unknown, ProxyType.None, string.Empty, this.SenderAddress);
				return;
			}
			this.endOPDetectionCallback = new StsWorkItem.EndOPDetectionCallback(this.UpdateOpenProxyResult);
			this.proxyTest = new ProxyTest(this.endOPDetectionCallback, ProtocolAnalysisBgAgent.GetProxyEnumerator());
			ProxyEndPoint[] path = new ProxyEndPoint[]
			{
				new ProxyEndPoint(proxyIP, proxyPort, ProxyType.None, new NetworkCredential())
			};
			OPDetectionResult opdetectionResult = this.proxyTest.BeginOPDetection(path, proxyType, authInfo, hostEndpoint, this.SenderAddress);
			if (opdetectionResult != OPDetectionResult.Pending)
			{
				this.UpdateOpenProxyResult(opdetectionResult, ProxyType.None, string.Empty, this.SenderAddress);
			}
		}

		public void StartReverseDnsQuery()
		{
			if (ProtocolAnalysisAgentFactory.SrlCalculationDisabled)
			{
				ExTraceGlobals.OnDnsQueryTracer.TraceError((long)this.GetHashCode(), "rDNS cannot be done: DNS server list is empty");
				return;
			}
			DnsStatus arg = DnsStatus.InfoNoRecords;
			if (this.WorkType != WorkItemType.ReverseDnsQuery)
			{
				throw new InvalidOperationException("The work type doesn't match.");
			}
			this.endPtrDnsResolutionCallback = new AsyncCallback(this.EndPtrDnsResolution);
			ExTraceGlobals.OnDnsQueryTracer.TraceDebug<IPAddress, DnsStatus>((long)this.GetHashCode(), "Call PtrAsyncDns.BeginDnsResolution, target:{0}, result:{1}", this.SenderAddress, arg);
			Interlocked.Increment(ref ProtocolAnalysisBgAgent.NumQueries);
			TransportFacades.Dns.BeginResolveToNames(this.SenderAddress, this.endPtrDnsResolutionCallback, this.SenderAddress);
		}

		private void EndPtrDnsResolution(IAsyncResult ar)
		{
			string[] array;
			DnsStatus dnsStatus = Dns.EndResolveToNames(ar, out array);
			Interlocked.Decrement(ref ProtocolAnalysisBgAgent.NumQueries);
			if (ProtocolAnalysisBgAgent.ShutDown)
			{
				return;
			}
			string reverseDns = string.Empty;
			if (dnsStatus == DnsStatus.Success)
			{
				reverseDns = array[0];
				ProtocolAnalysisBgAgent.PerformanceCounters.ReverseDnsSucc();
			}
			else
			{
				ProtocolAnalysisBgAgent.PerformanceCounters.ReverseDnsFail();
			}
			Database.UpdateReverseDns(((IPAddress)ar.AsyncState).ToString(), reverseDns, ExTraceGlobals.OnDnsQueryTracer);
		}

		public void BlockSender(SmtpServer smtpServer)
		{
			if (this.WorkType != WorkItemType.BlockSender)
			{
				throw new InvalidOperationException("The work type doesn't match.");
			}
			if (smtpServer == null)
			{
				ExTraceGlobals.OnSenderBlockingTracer.TraceError((long)this.GetHashCode(), "Cant block sender as SmtpServer object is not available.");
				throw new ArgumentNullException("smtpServer", "SmtpServer is null for BlockSender.");
			}
			ProtocolAnalysisBgAgent.PerformanceCounters.BlockSender();
			PermissionCheckResults permissionCheckResults = smtpServer.IPPermission.Check(this.SenderAddress);
			if (permissionCheckResults == PermissionCheckResults.None)
			{
				ExTraceGlobals.OnSenderBlockingTracer.TraceDebug<IPAddress, int, string>((long)this.GetHashCode(), "Block sender {0}, interval: {1}, reason: {2}", this.SenderAddress, this.BlockPeriod, this.BlockComment);
				smtpServer.IPPermission.AddRestriction(this.SenderAddress, new TimeSpan(this.BlockPeriod, 0, 0), this.BlockComment);
				return;
			}
			ExTraceGlobals.OnSenderBlockingTracer.TraceDebug<IPAddress, PermissionCheckResults>((long)this.GetHashCode(), "Can't block sender {0} since it is already on {1} list", this.SenderAddress, permissionCheckResults);
		}

		private WorkItemData item;

		private ProxyTest proxyTest;

		private AsyncCallback endPtrDnsResolutionCallback;

		private StsWorkItem.EndOPDetectionCallback endOPDetectionCallback;

		public delegate void EndOPDetectionCallback(OPDetectionResult result, ProxyType type, string message, IPAddress sender);
	}
}
