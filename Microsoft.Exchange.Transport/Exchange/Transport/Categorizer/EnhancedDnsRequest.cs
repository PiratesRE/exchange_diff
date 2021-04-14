using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class EnhancedDnsRequest
	{
		public EnhancedDnsRequest(int requestId, IEnumerable<INextHopServer> serversToResolve, EnhancedDnsRequest.QueryType queryType, DeliveryType deliveryType, SmtpSendConnectorConfig connector, RiskLevel riskLevel, int outboundIPPool, bool nextHopIsOutboundProxy, EnhancedDnsRequestContext requestContext)
		{
			RoutingUtils.ThrowIfNull(serversToResolve, "serversToResolve");
			this.RequestId = requestId;
			this.queryType = queryType;
			this.requestContext = requestContext;
			this.queries = new List<EnhancedDnsRequest.DnsQuery>(64);
			bool flag = true;
			foreach (INextHopServer nextHopServer in serversToResolve)
			{
				ushort targetPort = EnhancedDnsRequest.GetTargetPort(deliveryType, connector, riskLevel, outboundIPPool, nextHopServer.IsFrontendAndHubColocatedServer, nextHopIsOutboundProxy);
				bool flag2 = nextHopServer.IsIPAddress ? Components.UnhealthyTargetFilterComponent.UnhealthyTargetIPAddressFilter.IsUnhealthy(new IPAddressPortPair(nextHopServer.Address, targetPort)) : Components.UnhealthyTargetFilterComponent.UnhealthyTargetFqdnFilter.IsUnhealthy(new FqdnPortPair(nextHopServer.Fqdn, targetPort));
				if (flag && !flag2)
				{
					this.queries.Clear();
					this.numResponses = 0;
					flag = false;
				}
				if (flag || !flag2)
				{
					this.queries.Add(new EnhancedDnsRequest.DnsQuery(nextHopServer, requestId, targetPort, ref this.numResponses));
				}
			}
			if (this.queries.Count == 0)
			{
				throw new ArgumentException("serversToResolve must not be an empty list", "serversToResolve");
			}
		}

		public static IAsyncResult CompleteWithStatus(int requestId, DnsStatus status, EnhancedDnsRequestContext requestContext, AsyncCallback requestCallback, object stateObject)
		{
			ExTraceGlobals.RoutingTracer.TraceDebug<int, DnsStatus>(0L, "Request ID {0}: completing with status '{1}'", requestId, status);
			LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(requestId, stateObject, requestCallback);
			lazyAsyncResult.ErrorCode = (int)status;
			lazyAsyncResult.InvokeCallback(new EnhancedDnsStatusResult(status, IPAddress.None, requestContext, string.Empty));
			return lazyAsyncResult;
		}

		public IAsyncResult Resolve(EnhancedDns dns, DnsServerList list, DnsQueryOptions options, AsyncCallback requestCallback, object stateObject)
		{
			this.asyncResult = new LazyAsyncResult(this, stateObject, requestCallback);
			this.asyncResult.ErrorCode = 0;
			if (this.queries.Count == this.numResponses)
			{
				ExTraceGlobals.RoutingTracer.TraceDebug<int>((long)this.GetHashCode(), "Request ID={0} contains only explicit IP addresses; completing", this.RequestId);
				this.Complete();
			}
			else if (list == null || (list.Count == 0 && (list.Cache == null || list.Cache.Count == 0)))
			{
				for (int i = 0; i < this.queries.Count; i++)
				{
					if (DnsStatus.Pending == this.queries[i].Status)
					{
						this.queries[i].Status = DnsStatus.ErrorRetry;
					}
				}
				this.Complete();
			}
			else
			{
				for (int j = 0; j < this.queries.Count; j++)
				{
					if (DnsStatus.Pending == this.queries[j].Status)
					{
						if (this.queryType == EnhancedDnsRequest.QueryType.MXQuery)
						{
							ExTraceGlobals.RoutingTracer.TraceDebug<int, string>((long)this.GetHashCode(), "Request ID={0}: submitting MX query for '{1}'", this.RequestId, this.queries[j].Domain);
							dns.BeginResolveToMailServers(this.queries[j].Domain, list, options, new AsyncCallback(this.HandleDnsResult), j);
						}
						else
						{
							ExTraceGlobals.RoutingTracer.TraceDebug<int, string>((long)this.GetHashCode(), "Request ID={0}: submitting A query for '{1}'", this.RequestId, this.queries[j].Domain);
							dns.BeginResolveToAddresses(this.queries[j].Domain, list, options, new AsyncCallback(this.HandleDnsResult), j);
						}
					}
				}
			}
			return this.asyncResult;
		}

		private static bool IsFatalError(DnsStatus status)
		{
			switch (status)
			{
			case DnsStatus.InfoDomainNonexistent:
			case DnsStatus.InfoMxLoopback:
			case DnsStatus.ErrorInvalidData:
				return true;
			default:
				return false;
			}
		}

		private static ushort GetTargetPort(DeliveryType deliveryType, SmtpSendConnectorConfig connector, RiskLevel riskLevel, int outboundIPPool, bool isFrontendAndHubColocatedServer, bool nextHopIsOutboundProxy)
		{
			ushort result;
			if (deliveryType == DeliveryType.SmtpDeliveryToMailbox)
			{
				result = 475;
			}
			else if (isFrontendAndHubColocatedServer && !nextHopIsOutboundProxy)
			{
				result = 2525;
			}
			else if (outboundIPPool > 0)
			{
				result = (ushort)outboundIPPool;
			}
			else if (riskLevel == RiskLevel.Low)
			{
				result = (ushort)Components.TransportAppConfig.SmtpOutboundProxyConfiguration.LowRiskPoolPort;
			}
			else if (riskLevel == RiskLevel.Bulk)
			{
				result = (ushort)Components.TransportAppConfig.SmtpOutboundProxyConfiguration.BulkRiskPoolPort;
			}
			else if (riskLevel == RiskLevel.High)
			{
				result = (ushort)Components.TransportAppConfig.SmtpOutboundProxyConfiguration.HighRiskPoolPort;
			}
			else if (connector.Port != 0)
			{
				result = (ushort)connector.Port;
			}
			else
			{
				result = 25;
			}
			return result;
		}

		private void Complete()
		{
			DnsStatus dnsStatus = DnsStatus.Success;
			DnsStatus dnsStatus2 = DnsStatus.Success;
			IPAddress ipaddress = IPAddress.None;
			IPAddress ipaddress2 = IPAddress.None;
			int num = 0;
			using (IEnumerator<EnhancedDnsRequest.DnsQuery> enumerator = this.queries.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					EnhancedDnsRequest.DnsQuery query = enumerator.Current;
					if (query.Domain == null)
					{
						if (query.Result == null)
						{
							throw new InvalidOperationException("NULL explicit IP address in ExtendedDnsRequest.Complete()");
						}
						num++;
					}
					else if (query.Status != DnsStatus.Success)
					{
						ExTraceGlobals.RoutingTracer.TraceError((long)this.GetHashCode(), "Request ID={0}: DNS query '{1}' failed with status '{2}'; queryType is '{3}'", new object[]
						{
							this.RequestId,
							query.Domain,
							query.Status,
							this.queryType
						});
						if (EnhancedDnsRequest.IsFatalError(query.Status))
						{
							dnsStatus2 = query.Status;
							ipaddress = query.Server;
						}
						else
						{
							dnsStatus = query.Status;
							ipaddress2 = query.Server;
						}
					}
					else if (this.queryType == EnhancedDnsRequest.QueryType.MXQuery)
					{
						query.Result = Components.UnhealthyTargetFilterComponent.UnhealthyTargetFqdnFilter.FilterOutUnhealthyTargets<EnhancedDnsTargetHost>(query.Result, (EnhancedDnsTargetHost targetHost) => new FqdnPortPair(targetHost.Name, query.Port));
						if (query.Result == null || query.Result.Length == 0)
						{
							throw new InvalidOperationException("DNS status of MX query is success but no results returned");
						}
						num += query.Result.Length;
					}
					else
					{
						if (query.Result[0].IPAddresses == null || query.Result[0].IPAddresses.Count == 0)
						{
							throw new InvalidOperationException("DNS status of A query is success but no results returned");
						}
						num++;
					}
				}
			}
			if (num == 0)
			{
				StringBuilder stringBuilder = new StringBuilder("[Domain:Result] =");
				foreach (EnhancedDnsRequest.DnsQuery dnsQuery in this.queries)
				{
					stringBuilder.AppendFormat(" {0}:{1};", dnsQuery.Domain, dnsQuery.Status.ToString());
				}
				string diagnosticInfo = stringBuilder.ToString();
				DnsStatus dnsStatus3;
				IPAddress ipaddress3;
				if (dnsStatus != DnsStatus.Success)
				{
					dnsStatus3 = dnsStatus;
					ipaddress3 = ipaddress2;
				}
				else
				{
					dnsStatus3 = dnsStatus2;
					ipaddress3 = ipaddress;
				}
				ExTraceGlobals.RoutingTracer.TraceError<int, DnsStatus, IPAddress>((long)this.GetHashCode(), "Request ID={0}: failed to obtain any addresses; completing with status '{1}', reported by {2}", this.RequestId, dnsStatus3, ipaddress3);
				this.asyncResult.InvokeCallback(new EnhancedDnsStatusResult(dnsStatus3, ipaddress3, this.requestContext, diagnosticInfo));
				return;
			}
			EnhancedDnsTargetHost[] hosts = new EnhancedDnsTargetHost[num];
			int i = 0;
			foreach (EnhancedDnsRequest.DnsQuery dnsQuery2 in this.queries)
			{
				if (dnsQuery2.Status == DnsStatus.Success)
				{
					foreach (EnhancedDnsTargetHost enhancedDnsTargetHost in dnsQuery2.Result)
					{
						hosts[i++] = enhancedDnsTargetHost;
						ExTraceGlobals.RoutingTracer.TraceDebug<int, TargetHostTraceWrapper>((long)this.GetHashCode(), "Request ID={0}: adding target host '{1}' for MX result", this.RequestId, new TargetHostTraceWrapper(hosts[i - 1]));
					}
				}
			}
			if (i != num)
			{
				throw new InvalidOperationException("Failure in populating extended DNS results");
			}
			for (i = 0; i < hosts.Length; i++)
			{
				if (hosts[i].IPAddresses.Count > 1)
				{
					bool flag;
					List<IPAddress> list = Components.UnhealthyTargetFilterComponent.UnhealthyTargetIPAddressFilter.FilterOutUnhealthyTargets<IPAddress>(new List<IPAddress>(hosts[i].IPAddresses), (IPAddress address) => new IPAddressPortPair(address, hosts[i].Port), out flag);
					if (list.Count != hosts[i].IPAddresses.Count)
					{
						hosts[i] = new EnhancedDnsTargetHost(hosts[i].Name, list, hosts[i].TimeToLive, hosts[i].Port);
					}
				}
			}
			ExTraceGlobals.RoutingTracer.TraceDebug<int>((long)this.GetHashCode(), "Request ID={0}: invoking user callback", this.RequestId);
			this.asyncResult.InvokeCallback(new EnhancedDnsHostsResult(hosts, this.requestContext));
		}

		private void HandleDnsResult(IAsyncResult ar)
		{
			int index = (int)ar.AsyncState;
			IPAddress server;
			DnsStatus dnsStatus;
			EnhancedDnsTargetHost[] result;
			if (this.queryType == EnhancedDnsRequest.QueryType.MXQuery)
			{
				TargetHost[] array;
				dnsStatus = Dns.EndResolveToMailServers(ar, out array, out server);
				EnhancedDnsTargetHost[] array2 = new EnhancedDnsTargetHost[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = new EnhancedDnsTargetHost(array[i].Name, array[i].IPAddresses, array[i].TimeToLive, this.queries[index].Port);
				}
				result = array2;
				ExTraceGlobals.RoutingTracer.TraceDebug((long)this.GetHashCode(), "Request ID={0}: MX query '{1}' completed with status '{2}' and target hosts: {3}", new object[]
				{
					this.RequestId,
					this.queries[index].Domain,
					dnsStatus,
					new TargetHostArrayTracer(array)
				});
			}
			else
			{
				IPAddress[] array3;
				dnsStatus = Dns.EndResolveToAddresses(ar, out array3, out server);
				result = new EnhancedDnsTargetHost[]
				{
					new EnhancedDnsTargetHost(this.queries[index].Domain, new List<IPAddress>(array3), TimeSpan.FromDays(2.0), this.queries[index].Port)
				};
				ExTraceGlobals.RoutingTracer.TraceDebug((long)this.GetHashCode(), "Request ID={0}: A query '{1}' completed with status '{2}' and IP addresses: {3}", new object[]
				{
					this.RequestId,
					this.queries[index].Domain,
					dnsStatus,
					new IPArrayTracer(array3)
				});
			}
			this.queries[index].Status = dnsStatus;
			this.queries[index].Result = result;
			this.queries[index].Server = server;
			if (Interlocked.Increment(ref this.numResponses) == this.queries.Count)
			{
				ExTraceGlobals.RoutingTracer.TraceDebug<int>((long)this.GetHashCode(), "Request ID={0}: last query response received; completing", this.RequestId);
				this.Complete();
			}
		}

		public readonly int RequestId;

		private IList<EnhancedDnsRequest.DnsQuery> queries;

		private EnhancedDnsRequest.QueryType queryType;

		private LazyAsyncResult asyncResult;

		private int numResponses;

		private EnhancedDnsRequestContext requestContext;

		public enum QueryType
		{
			MXQuery,
			AQuery
		}

		private class DnsQuery
		{
			public DnsQuery(INextHopServer host, int requestId, ushort port, ref int numResponses)
			{
				this.Server = IPAddress.None;
				this.Port = port;
				if (host.IsIPAddress)
				{
					EnhancedDnsTargetHost enhancedDnsTargetHost = new EnhancedDnsTargetHost(host.Address.ToString(), new List<IPAddress>(new IPAddress[]
					{
						host.Address
					}), TimeSpan.FromDays(2.0), port);
					this.Result = new EnhancedDnsTargetHost[]
					{
						enhancedDnsTargetHost
					};
					this.Status = DnsStatus.Success;
					numResponses++;
					ExTraceGlobals.RoutingTracer.TraceDebug<int, IPAddress>((long)this.GetHashCode(), "Request ID={0}: explicit IP address {1} detected", requestId, host.Address);
					return;
				}
				this.Domain = host.Fqdn;
				this.Status = DnsStatus.Pending;
			}

			public readonly string Domain;

			public readonly ushort Port;

			public DnsStatus Status;

			public IPAddress Server;

			public EnhancedDnsTargetHost[] Result;
		}
	}
}
