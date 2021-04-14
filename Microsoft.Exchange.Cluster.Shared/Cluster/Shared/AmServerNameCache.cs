using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class AmServerNameCache : IAmServerNameLookup
	{
		public static IAmServerNameLookup Instance
		{
			get
			{
				return SharedDependencies.AmServerNameLookup;
			}
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AmServerNameCacheTracer;
			}
		}

		private TimeSpan TimeToLive
		{
			get
			{
				return new TimeSpan(0, 0, RegistryParameters.AmServerNameCacheTTLInSec);
			}
		}

		internal static string GetFqdnWithLocalDomainSuffix(string shortNodeName)
		{
			return string.Format("{0}.{1}", shortNodeName, SharedDependencies.ManagementClassHelper.LocalDomainName);
		}

		public static string ResolveFqdn(string shortNodeName, bool throwException)
		{
			string fqdn = null;
			Exception ex = SharedHelper.RunADOperationEx(delegate(object param0, EventArgs param1)
			{
				IADToplogyConfigurationSession iadtoplogyConfigurationSession = ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
				iadtoplogyConfigurationSession.UseConfigNC = false;
				iadtoplogyConfigurationSession.UseGlobalCatalog = true;
				IADComputer iadcomputer = iadtoplogyConfigurationSession.FindComputerByHostName(shortNodeName);
				if (iadcomputer != null && !string.IsNullOrEmpty(iadcomputer.DnsHostName))
				{
					fqdn = iadcomputer.DnsHostName;
					return;
				}
				if (throwException)
				{
					throw new AmGetFqdnFailedNotFoundException(shortNodeName);
				}
				AmTrace.Error("FQDN resolution of the short name {0} failed. Could not find valid DNS hostname from ADComputer query.", new object[]
				{
					shortNodeName
				});
				fqdn = AmServerNameCache.GetFqdnWithLocalDomainSuffix(shortNodeName);
			});
			if (ex != null)
			{
				if (throwException)
				{
					throw new AmGetFqdnFailedADErrorException(shortNodeName, ex.Message, ex);
				}
				AmTrace.Error("FQDN resolution of the short name {0} failed. Error: {1}", new object[]
				{
					shortNodeName,
					ex.Message
				});
				fqdn = AmServerNameCache.GetFqdnWithLocalDomainSuffix(shortNodeName);
			}
			return fqdn;
		}

		public void Enable()
		{
			try
			{
				this.m_rwLock.EnterWriteLock();
				if (this.m_cache == null)
				{
					this.m_cache = new Dictionary<string, AmServerNameCache.CacheEntry>(16, MachineName.Comparer);
				}
			}
			finally
			{
				try
				{
					this.m_rwLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		public string GetFqdn(string shortNodeName)
		{
			return this.GetFqdn(shortNodeName, true);
		}

		public virtual string GetFqdn(string shortNodeName, bool throwException)
		{
			if (SharedHelper.StringIEquals(shortNodeName, "localhost") || SharedHelper.StringIEquals(shortNodeName, MachineName.Local))
			{
				return AmServerName.LocalComputerName.Fqdn;
			}
			AmServerNameCache.CacheEntry cacheEntry = null;
			bool flag = false;
			try
			{
				this.m_rwLock.EnterReadLock();
				if (this.m_cache != null)
				{
					flag = true;
					if (this.m_cache.TryGetValue(shortNodeName, out cacheEntry) && cacheEntry.ExpiryTime > ExDateTime.Now)
					{
						return cacheEntry.Fqdn;
					}
				}
			}
			finally
			{
				try
				{
					this.m_rwLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			string text = AmServerNameCache.ResolveFqdn(shortNodeName, throwException);
			if (!flag)
			{
				return text;
			}
			try
			{
				this.m_rwLock.EnterWriteLock();
				if (!this.m_cache.TryGetValue(shortNodeName, out cacheEntry))
				{
					cacheEntry = new AmServerNameCache.CacheEntry();
					this.m_cache.Add(shortNodeName, cacheEntry);
				}
				cacheEntry.ExpiryTime = ExDateTime.Now + this.TimeToLive;
				cacheEntry.Fqdn = text;
			}
			finally
			{
				try
				{
					this.m_rwLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return text;
		}

		public void RemoveEntry(string serverName)
		{
			string nodeNameFromFqdn = SharedHelper.GetNodeNameFromFqdn(serverName);
			try
			{
				this.m_rwLock.EnterWriteLock();
				this.m_cache.Remove(nodeNameFromFqdn);
			}
			finally
			{
				try
				{
					this.m_rwLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		public void PopulateForDag()
		{
			Dictionary<string, AmServerNameCache.CacheEntry> dictionary = new Dictionary<string, AmServerNameCache.CacheEntry>(16, MachineName.Comparer);
			Exception ex = SharedHelper.RunADOperationEx(delegate(object param0, EventArgs param1)
			{
				IADToplogyConfigurationSession iadtoplogyConfigurationSession = ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
				Exception ex2;
				IADDatabaseAvailabilityGroup localServerDatabaseAvailabilityGroup = DirectoryHelper.GetLocalServerDatabaseAvailabilityGroup(iadtoplogyConfigurationSession, out ex2);
				if (ex2 != null)
				{
					AmServerNameCache.Tracer.TraceError<Exception>(0L, "PopulateForDag failed to get DAG: {0}", ex2);
					return;
				}
				if (localServerDatabaseAvailabilityGroup != null)
				{
					iadtoplogyConfigurationSession.UseConfigNC = false;
					iadtoplogyConfigurationSession.UseGlobalCatalog = true;
					MultiValuedProperty<ADObjectId> servers = localServerDatabaseAvailabilityGroup.Servers;
					if (servers != null)
					{
						foreach (ADObjectId adobjectId in servers)
						{
							IADComputer iadcomputer = iadtoplogyConfigurationSession.FindComputerByHostName(adobjectId.Name);
							if (iadcomputer != null && !string.IsNullOrEmpty(iadcomputer.DnsHostName))
							{
								AmServerNameCache.CacheEntry cacheEntry = new AmServerNameCache.CacheEntry();
								cacheEntry.ExpiryTime = ExDateTime.Now + this.TimeToLive;
								cacheEntry.Fqdn = iadcomputer.DnsHostName;
							}
							else
							{
								AmServerNameCache.Tracer.TraceError<ADObjectId>(0L, "PopulateForDag could not map server {0}", adobjectId);
							}
						}
					}
				}
			});
			if (ex != null)
			{
				AmServerNameCache.Tracer.TraceError<Exception>(0L, "PopulateForDag did not replace cache because: {0}", ex);
				return;
			}
			if (dictionary != null)
			{
				AmServerNameCache.Tracer.TraceDebug(0L, "PopulateForDag replaced the cache");
				try
				{
					this.m_rwLock.EnterWriteLock();
					this.m_cache = dictionary;
				}
				finally
				{
					try
					{
						this.m_rwLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
		}

		private ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();

		private Dictionary<string, AmServerNameCache.CacheEntry> m_cache;

		private class CacheEntry
		{
			public string Fqdn;

			public ExDateTime ExpiryTime;
		}
	}
}
