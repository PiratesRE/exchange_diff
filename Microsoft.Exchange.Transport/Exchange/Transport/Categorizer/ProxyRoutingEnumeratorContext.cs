using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ProxyRoutingEnumeratorContext
	{
		public ProxyRoutingEnumeratorContext(ProxyRoutingContext proxyContext)
		{
			this.proxyContext = proxyContext;
			this.remainingServerCount = proxyContext.MaxTotalHubCount;
			this.remoteSiteRemainingServerCount = Math.Min(proxyContext.MaxRemoteSiteHubCount, this.remainingServerCount);
			this.localSiteRemainingServerCount = Math.Min(proxyContext.MaxLocalSiteHubCount, this.remainingServerCount);
			this.allServersUnhealthy = true;
			this.serverHealthCheckEnabled = true;
			this.allowedServerGuids = new HashSet<Guid>();
		}

		public bool AllServersUnhealthy
		{
			get
			{
				return this.allServersUnhealthy;
			}
		}

		public int RemainingServerCount
		{
			get
			{
				return this.remainingServerCount;
			}
		}

		public int RemoteSiteRemainingServerCount
		{
			get
			{
				return this.remoteSiteRemainingServerCount;
			}
		}

		public int LocalSiteRemainingServerCount
		{
			get
			{
				return this.localSiteRemainingServerCount;
			}
		}

		public bool PreLoadbalanceFilter(RoutingServerInfo serverInfo)
		{
			if (!this.proxyContext.VerifyVersionRestriction(serverInfo))
			{
				RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Server {1} does not satisfy version requirements for being a proxy target", this.proxyContext.Timestamp, serverInfo.Fqdn);
				return false;
			}
			return true;
		}

		public IEnumerable<RoutingServerInfo> PostLoadbalanceFilter(IEnumerable<RoutingServerInfo> servers, bool? inRemoteSite)
		{
			foreach (RoutingServerInfo serverInfo in servers)
			{
				bool inRemoteSiteValue = (inRemoteSite != null) ? inRemoteSite.Value : (!this.proxyContext.RoutingTables.ServerMap.IsInLocalSite(serverInfo));
				if (this.remainingServerCount == 0 || (inRemoteSite != null && inRemoteSiteValue && this.remoteSiteRemainingServerCount == 0) || (inRemoteSite != null && !inRemoteSiteValue && this.localSiteRemainingServerCount == 0))
				{
					RoutingDiag.Tracer.TraceDebug<DateTime>((long)this.GetHashCode(), "[{0}] Proxy enumerator has reached one of the server count limits while more servers are available", this.proxyContext.Timestamp);
					break;
				}
				if (inRemoteSiteValue && this.remoteSiteRemainingServerCount == 0)
				{
					RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Server {1} is in a remote site and the limit for remote servers has been reached; skipping it", this.proxyContext.Timestamp, serverInfo.Fqdn);
				}
				else if (!inRemoteSiteValue && this.localSiteRemainingServerCount == 0)
				{
					RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Server {1} is in a local site and the limit for local servers has been reached; skipping it", this.proxyContext.Timestamp, serverInfo.Fqdn);
				}
				else if (this.allowedServerGuids.Contains(serverInfo.Id.ObjectGuid))
				{
					RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Server {1} has already been returned by this proxy enumerator; skipping it", this.proxyContext.Timestamp, serverInfo.Fqdn);
				}
				else
				{
					ushort port = 25;
					if (serverInfo.IsFrontendAndHubColocatedServer)
					{
						port = 2525;
					}
					if (this.serverHealthCheckEnabled && this.proxyContext.Core.Dependencies.IsUnhealthyFqdn(serverInfo.Fqdn, port))
					{
						if (this.allServersUnhealthy)
						{
							RoutingUtils.AddItemToLazyList<RoutingServerInfo>(serverInfo, ref this.allUnhealthyServerList);
						}
						RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Server FQDN {1} is Unhealthy and will be excluded from the proxy target list", this.proxyContext.Timestamp, serverInfo.Fqdn);
					}
					else
					{
						this.allServersUnhealthy = false;
						this.allUnhealthyServerList = null;
						this.remainingServerCount--;
						if (inRemoteSiteValue || this.remoteSiteRemainingServerCount > this.remainingServerCount)
						{
							this.remoteSiteRemainingServerCount--;
						}
						if (!inRemoteSiteValue || this.localSiteRemainingServerCount > this.remainingServerCount)
						{
							this.localSiteRemainingServerCount--;
						}
						RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] Returning server {1} from proxy enumerator", this.proxyContext.Timestamp, serverInfo.Fqdn);
						this.allowedServerGuids.Add(serverInfo.Id.ObjectGuid);
						yield return serverInfo;
					}
				}
			}
			yield break;
		}

		public IEnumerable<RoutingServerInfo> GetUnhealthyServers()
		{
			if (!this.allServersUnhealthy || this.allUnhealthyServerList == null)
			{
				throw new InvalidOperationException("GetUnhealthyServers() must not be invoked when not all servers are unhealthy");
			}
			IList<RoutingServerInfo> servers = this.allUnhealthyServerList;
			this.allUnhealthyServerList = null;
			this.serverHealthCheckEnabled = false;
			return this.PostLoadbalanceFilter(servers, null);
		}

		private ProxyRoutingContext proxyContext;

		private int remainingServerCount;

		private int remoteSiteRemainingServerCount;

		private int localSiteRemainingServerCount;

		private HashSet<Guid> allowedServerGuids;

		private bool serverHealthCheckEnabled;

		private bool allServersUnhealthy;

		private List<RoutingServerInfo> allUnhealthyServerList;
	}
}
