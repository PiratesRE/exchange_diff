using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Agent.Hygiene;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class PaBgSmtpMxDns
	{
		public PaBgSmtpMxDns(Trace tracer, PaBgSmtpMxDns.EndMxDnsResolutionCallback endMxDnsResolutionCallback)
		{
			if (endMxDnsResolutionCallback == null || tracer == null)
			{
				throw new LocalizedException(AgentStrings.InvalidArgument);
			}
			this.tracer = tracer;
			this.endMxDnsResolutionCallback = endMxDnsResolutionCallback;
		}

		public void BeginSmtpMxQueries(IList<string> domains)
		{
			this.queryDomains = domains;
			this.queryResults = new QueryResult[this.queryDomains.Count];
			this.hostCount = 0;
			this.tracer.TraceDebug((long)this.GetHashCode(), "BeginSmtpMxQueries");
			this.IssueSmtpMxQueries();
		}

		private void IssueSmtpMxQueries()
		{
			if (ProtocolAnalysisBgAgent.ShutDown)
			{
				return;
			}
			for (int i = 0; i < this.queryDomains.Count; i++)
			{
				this.IssueDnsQuery(this.tracer, this.queryDomains[i], i);
			}
		}

		private void EndDnsResolution(IAsyncResult ar)
		{
			TargetHost[] array;
			DnsStatus dnsStatus = Dns.EndResolveToMailServers(ar, out array);
			int num = Convert.ToInt32(ar.AsyncState, CultureInfo.InvariantCulture);
			lock (this.syncObject)
			{
				this.queriesCompleted++;
				if (dnsStatus == DnsStatus.Success)
				{
					this.dnsStatus = DnsStatus.Success;
				}
				this.queryResults[num].Status = dnsStatus;
				this.queryResults[num].TargetHosts = array;
				if (dnsStatus == DnsStatus.Success && array != null && array.Length > 0)
				{
					this.hostCount += array.Length;
				}
				if (this.queriesCompleted >= this.queryDomains.Count)
				{
					if (!ProtocolAnalysisBgAgent.ShutDown)
					{
						if (this.hostCount > 0)
						{
							ArrayList arrayList = new ArrayList();
							for (int i = 0; i < this.queryResults.Length; i++)
							{
								if (this.queryResults[i].Status == DnsStatus.Success && this.queryResults[i].TargetHosts != null)
								{
									for (int j = 0; j < this.queryResults[i].TargetHosts.Length; j++)
									{
										if (this.queryResults[i].TargetHosts[j].IPAddresses != null && this.queryResults[i].TargetHosts[j].IPAddresses.Count != 0)
										{
											arrayList.Add(this.queryResults[i].TargetHosts[j]);
										}
									}
								}
							}
							if (arrayList.Count == 0)
							{
								this.endMxDnsResolutionCallback(this.dnsStatus, null);
							}
							else
							{
								TargetHost[] array2 = new TargetHost[arrayList.Count];
								arrayList.CopyTo(array2);
								this.tracer.TraceDebug<DnsStatus, int, IPAddress>((long)this.GetHashCode(), "EndDnsResolution> status: {0}, hosts found: {1}, first IP: {2}", this.dnsStatus, this.hostCount, array2[0].IPAddresses[0]);
								this.endMxDnsResolutionCallback(this.dnsStatus, array2);
							}
						}
						else
						{
							this.endMxDnsResolutionCallback(this.dnsStatus, null);
							this.tracer.TraceDebug((long)this.GetHashCode(), "EndDnsResolution> Failed to resolve Dns name");
						}
					}
				}
			}
		}

		private void IssueDnsQuery(Trace tracer, string questionName, int index)
		{
			tracer.TraceDebug<string>((long)this.GetHashCode(), "Call SmtpMxAsyncDns.BeginDnsResolution, target: {0}", questionName);
			if (ProtocolAnalysisAgentFactory.SrlCalculationDisabled)
			{
				return;
			}
			TransportFacades.Dns.BeginResolveToMailServers(questionName, new AsyncCallback(this.EndDnsResolution), index);
		}

		private QueryResult[] queryResults;

		private IList<string> queryDomains;

		private DnsStatus dnsStatus = DnsStatus.InfoNoRecords;

		private int queriesCompleted;

		private int hostCount;

		private Trace tracer;

		private object syncObject = new object();

		private PaBgSmtpMxDns.EndMxDnsResolutionCallback endMxDnsResolutionCallback;

		public delegate void EndMxDnsResolutionCallback(DnsStatus status, TargetHost[] hosts);
	}
}
