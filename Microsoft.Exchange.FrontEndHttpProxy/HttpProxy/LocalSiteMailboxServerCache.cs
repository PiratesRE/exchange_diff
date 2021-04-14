using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class LocalSiteMailboxServerCache
	{
		public static LocalSiteMailboxServerCache Instance
		{
			get
			{
				if (LocalSiteMailboxServerCache.instance == null)
				{
					lock (LocalSiteMailboxServerCache.staticLock)
					{
						if (LocalSiteMailboxServerCache.instance == null)
						{
							LocalSiteMailboxServerCache.instance = new LocalSiteMailboxServerCache();
						}
					}
				}
				return LocalSiteMailboxServerCache.instance;
			}
		}

		public BackEndServer TryGetRandomE15Server(IRequestContext requestContext)
		{
			if (!LocalSiteMailboxServerCache.CacheLocalSiteLiveE15Servers)
			{
				return null;
			}
			Guid[] array = null;
			try
			{
				this.localSiteServersLock.Wait();
				array = this.localSiteLiveE15Servers.ToArray();
			}
			finally
			{
				this.localSiteServersLock.Release();
			}
			if (array.Length > 0)
			{
				int num = this.random.Next(array.Length);
				int num2 = num;
				BackEndServer backEndServer;
				for (;;)
				{
					Guid database = array[num];
					if (MailboxServerCache.Instance.TryGet(database, requestContext, out backEndServer))
					{
						break;
					}
					num2++;
					if (num2 >= array.Length)
					{
						num2 = 0;
					}
					if (num2 == num)
					{
						goto IL_90;
					}
				}
				ExTraceGlobals.VerboseTracer.TraceDebug<BackEndServer>((long)this.GetHashCode(), "[LocalSiteMailboxServerCache::TryGetRandomE15Server]: Found server {0} from local site E15 server list.", backEndServer);
				return backEndServer;
			}
			IL_90:
			return null;
		}

		internal void Add(Guid database, BackEndServer backEndServer, string resourceForest)
		{
			if (LocalSiteMailboxServerCache.CacheLocalSiteLiveE15Servers && this.IsLocalSiteE15MailboxServer(backEndServer, resourceForest))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<Guid, BackEndServer>((long)this.GetHashCode(), "[LocalSiteMailboxServerCache::Add]: Adding Database {0} on Server {1} to local E15 mailbox server collection.", database, backEndServer);
				try
				{
					this.localSiteServersLock.Wait();
					if (!this.localSiteLiveE15Servers.Contains(database))
					{
						this.localSiteLiveE15Servers.Add(database);
					}
				}
				finally
				{
					this.localSiteServersLock.Release();
				}
				this.UpdateLocalSiteMailboxServerListCounter();
			}
		}

		internal void Remove(Guid database)
		{
			if (LocalSiteMailboxServerCache.CacheLocalSiteLiveE15Servers)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<Guid>((long)this.GetHashCode(), "[LocalSiteMailboxServerCache::Remove]: Removing Database {0} from the local E15 mailbox server collection.", database);
				try
				{
					this.localSiteServersLock.Wait();
					this.localSiteLiveE15Servers.Remove(database);
				}
				finally
				{
					this.localSiteServersLock.Release();
				}
				this.UpdateLocalSiteMailboxServerListCounter();
			}
		}

		private bool IsLocalSiteE15MailboxServer(BackEndServer server, string resourceForest)
		{
			if (!server.IsE15OrHigher)
			{
				return false;
			}
			if ((!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.NoCrossForestServerLocate.Enabled) || string.IsNullOrEmpty(resourceForest) || string.Equals(HttpProxyGlobals.LocalMachineForest.Member, resourceForest, StringComparison.OrdinalIgnoreCase))
			{
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\Cache\\LocalSiteMailboxServerCache.cs", "IsLocalSiteE15MailboxServer", 226);
				Site other = null;
				try
				{
					other = currentServiceTopology.GetSite(server.Fqdn, "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\Cache\\LocalSiteMailboxServerCache.cs", "IsLocalSiteE15MailboxServer", 231);
				}
				catch (ServerNotFoundException)
				{
					return false;
				}
				catch (ServerNotInSiteException)
				{
					return false;
				}
				if (HttpProxyGlobals.LocalSite.Member.Equals(other))
				{
					return true;
				}
				return false;
			}
			return false;
		}

		private void UpdateLocalSiteMailboxServerListCounter()
		{
			PerfCounters.HttpProxyCacheCountersInstance.BackEndServerCacheLocalServerListCount.RawValue = (long)this.localSiteLiveE15Servers.Count;
		}

		private static readonly bool CacheLocalSiteLiveE15Servers = VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.CacheLocalSiteLiveE15Servers.Enabled;

		private static LocalSiteMailboxServerCache instance;

		private static object staticLock = new object();

		private List<Guid> localSiteLiveE15Servers = new List<Guid>();

		private SemaphoreSlim localSiteServersLock = new SemaphoreSlim(1);

		private Random random = new Random();
	}
}
