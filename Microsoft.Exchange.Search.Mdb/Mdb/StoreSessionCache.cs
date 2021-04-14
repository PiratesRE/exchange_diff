using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Search.Performance;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Search.Mdb
{
	internal sealed class StoreSessionCache : IDisposeTrackable, IDisposable
	{
		internal StoreSessionCache(string instanceName, int maxNumberOfStoreSessions, int maxNumberOfStoreSessionsPerMailbox, TimeSpan cacheTimeout)
		{
			Util.ThrowOnNullOrEmptyArgument(instanceName, "instanceName");
			this.maxNumberOfStoreSessionsPerMailbox = maxNumberOfStoreSessionsPerMailbox;
			this.cacheTimeout = cacheTimeout;
			this.storeSessionCache = new TimeoutCache<long, StoreSessionCache.StoreSessionContext>(1, maxNumberOfStoreSessions, true);
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("StoreSessionCache", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.MdbSessionCacheTracer, (long)this.GetHashCode());
			this.cachePerfCounter = MdbCachePerfCounters.GetInstance(instanceName);
			this.cachePerfCounter.Reset();
			this.disposeTracker = this.GetDisposeTracker();
		}

		internal Dictionary<StoreSessionCacheKey, List<long>> InnerCacheLookup
		{
			get
			{
				return this.cacheLookup;
			}
		}

		internal TimeoutCache<long, StoreSessionCache.StoreSessionContext> InnerTimeoutCache
		{
			get
			{
				return this.storeSessionCache;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StoreSessionCache>(this);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal StoreSession GetStoreSession(StoreSessionCacheKey cacheKey, MdbItemIdentity identity, bool wantSessionForRMS, bool localOnly)
		{
			return this.GetStoreSession(cacheKey, identity, wantSessionForRMS, true, localOnly);
		}

		internal StoreSession GetWritableStoreSession(StoreSessionCacheKey cacheKey, MdbItemIdentity identity, bool wantSessionForRMS)
		{
			return this.GetStoreSession(cacheKey, identity, wantSessionForRMS, false, true);
		}

		internal void ReturnStoreSession(ref StoreSession storeSession)
		{
			Util.ThrowOnNullArgument(storeSession, "storeSession");
			try
			{
				StoreSessionCacheKey storeSessionCacheKey = new StoreSessionCacheKey(storeSession.MdbGuid, storeSession.MailboxGuid, StoreSessionCache.IsMoveDestinationSession(storeSession));
				this.diagnosticsSession.TraceDebug<StoreSessionCacheKey>("Returning store session for {0}", storeSessionCacheKey);
				this.ReturnStoreSessionContext(storeSessionCacheKey, new StoreSessionCache.StoreSessionContext(storeSessionCacheKey)
				{
					Session = storeSession
				});
			}
			finally
			{
				storeSession = null;
			}
		}

		internal void RegisterAsUnavailableSession(StoreSessionCacheKey cacheKey, StoreSessionCache.StoreSessionContext storeSessionContext, Exception e)
		{
			bool flag = false;
			try
			{
				object obj;
				Monitor.Enter(obj = this.locker, ref flag);
				this.diagnosticsSession.TraceDebug<StoreSessionCacheKey>("Mark session as unavailable for {0}", cacheKey);
				if (storeSessionContext.Session != null)
				{
					storeSessionContext.Session.Dispose();
					storeSessionContext.Session = null;
				}
				storeSessionContext.Exception = e;
				this.RemoveAllFromCache(cacheKey);
				this.ReturnStoreSessionContext(cacheKey, storeSessionContext);
				throw e;
			}
			finally
			{
				if (flag)
				{
					object obj;
					Monitor.Exit(obj);
					goto IL_5D;
				}
				goto IL_5D;
				IL_5D:;
			}
		}

		private static bool IsMoveDestinationSession(StoreSession storeSession)
		{
			return storeSession.ClientInfoString == "Client=CI;Client=CIMoveDestination";
		}

		private StoreSession GetStoreSession(StoreSessionCacheKey cacheKey, MdbItemIdentity identity, bool wantSessionForRMS, bool readOnly, bool localOnly)
		{
			Util.ThrowOnNullArgument(cacheKey, "cacheKey");
			Util.ThrowOnNullArgument(identity, "identity");
			ISearchServiceConfig searchServiceConfig = Factory.Current.CreateSearchServiceConfig();
			readOnly = (readOnly && searchServiceConfig.ReadFromPassiveEnabled);
			localOnly = (localOnly && searchServiceConfig.ReadFromPassiveEnabled);
			StoreSessionCache.StoreSessionContext storeSessionContext = new StoreSessionCache.StoreSessionContext(cacheKey);
			bool flag = false;
			this.diagnosticsSession.TraceDebug<StoreSessionCacheKey>("Getting store session for {0}", cacheKey);
			this.cachePerfCounter.NumberOfRequest.Increment();
			this.cachePerfCounter.CacheHitRatioBase.Increment();
			this.cachePerfCounter.CacheMissRatioBase.Increment();
			try
			{
				lock (this.locker)
				{
					this.CheckDisposed();
					List<long> list;
					if (this.cacheLookup.TryGetValue(cacheKey, out list))
					{
						this.diagnosticsSession.TraceDebug<StoreSessionCacheKey, int>("Found lookup cache for {0}: {1} store session caches", cacheKey, list.Count);
						int i = 0;
						bool flag3 = !searchServiceConfig.ReadFromPassiveEnabled || readOnly;
						if (wantSessionForRMS && flag3)
						{
							flag3 = false;
							while (i < list.Count)
							{
								StoreSessionCache.StoreSessionContext storeSessionContext2;
								if (this.storeSessionCache.TryGetValue(list[i], out storeSessionContext2))
								{
									if (storeSessionContext2.Session == null)
									{
										throw storeSessionContext2.Exception;
									}
									if (StoreSessionManager.IsSessionUsableForRMS(storeSessionContext2.Session))
									{
										flag3 = true;
										break;
									}
								}
								i++;
							}
						}
						if (flag3)
						{
							long num = list[i];
							list.RemoveAt(i);
							if (list.Count == 0)
							{
								this.diagnosticsSession.TraceDebug<StoreSessionCacheKey>("Remove the lookup cache for {0}", cacheKey);
								this.cacheLookup.Remove(cacheKey);
							}
							this.diagnosticsSession.TraceDebug<long, StoreSessionCacheKey>("Checking out the store session cache {0} for {1}", num, cacheKey);
							storeSessionContext = this.storeSessionCache.Remove(num);
							this.cachePerfCounter.NumberOfCacheHit.Increment();
							this.cachePerfCounter.CacheHitRatio.Increment();
						}
					}
				}
				if (storeSessionContext.Session == null)
				{
					this.diagnosticsSession.TraceDebug<StoreSessionCacheKey>("Create a new store session since no store session cache found for {0}", cacheKey);
					this.cachePerfCounter.NumberOfCacheMiss.Increment();
					this.cachePerfCounter.CacheMissRatio.Increment();
					try
					{
						storeSessionContext.Session = this.CreateStoreSession(identity, cacheKey.IsMoveDestination, wantSessionForRMS, localOnly, readOnly);
						goto IL_2AE;
					}
					catch (Exception ex)
					{
						Exception innerException;
						if (ex is AdUserNotFoundException)
						{
							this.RegisterAsUnavailableSession(cacheKey, storeSessionContext, new UnavailableSessionException(cacheKey, ex));
						}
						else if (ex is MailboxInTransitException)
						{
							this.RegisterAsUnavailableSession(cacheKey, storeSessionContext, new MailboxLockedException(cacheKey, ex));
						}
						else if (Util.TryGetExceptionOrInnerOfType<MapiExceptionMailboxQuarantined>(ex, out innerException))
						{
							this.RegisterAsUnavailableSession(cacheKey, storeSessionContext, new MailboxQuarantinedException(cacheKey, innerException));
						}
						else
						{
							if (!Util.TryGetExceptionOrInnerOfType<MapiExceptionLogonFailed>(ex, out innerException))
							{
								throw;
							}
							this.RegisterAsUnavailableSession(cacheKey, storeSessionContext, new MailboxLoginFailedException(cacheKey, innerException));
						}
						goto IL_2AE;
					}
				}
				if (!storeSessionContext.Session.IsConnected)
				{
					storeSessionContext.Session.Connect();
				}
				IL_2AE:
				flag = true;
			}
			finally
			{
				if (storeSessionContext != null && storeSessionContext.Session != null && !flag)
				{
					storeSessionContext.Session.Dispose();
					storeSessionContext.Session = null;
				}
			}
			return storeSessionContext.Session;
		}

		private void ReturnStoreSessionContext(StoreSessionCacheKey cacheKey, StoreSessionCache.StoreSessionContext storeSessionContext)
		{
			bool flag = false;
			try
			{
				lock (this.locker)
				{
					this.CheckDisposed();
					List<long> list;
					if (!this.cacheLookup.TryGetValue(cacheKey, out list))
					{
						this.diagnosticsSession.TraceDebug<StoreSessionCacheKey>("Adding lookup cache for {0}", cacheKey);
						list = new List<long>(this.maxNumberOfStoreSessionsPerMailbox);
						this.cacheLookup.Add(cacheKey, list);
					}
					this.diagnosticsSession.TraceDebug<StoreSessionCacheKey, int>("The lookup cache for {0}: {1} store session caches", cacheKey, list.Count);
					if (storeSessionContext.Session != null && StoreSessionManager.IsSessionUsableForRMS(storeSessionContext.Session) && list.Count >= this.maxNumberOfStoreSessionsPerMailbox)
					{
						int i = 0;
						bool flag3 = false;
						while (i < list.Count)
						{
							StoreSessionCache.StoreSessionContext storeSessionContext2 = this.storeSessionCache.Get(list[i]);
							if (storeSessionContext2.Session != null && !StoreSessionManager.IsSessionUsableForRMS(storeSessionContext2.Session))
							{
								flag3 = true;
								break;
							}
							i++;
						}
						if (flag3)
						{
							StoreSessionCache.StoreSessionContext storeSessionContext3 = this.storeSessionCache.Remove(list[i]);
							storeSessionContext3.Session.Dispose();
							storeSessionContext3.Session = null;
							list.RemoveAt(i);
						}
					}
					if (list.Count < this.maxNumberOfStoreSessionsPerMailbox)
					{
						long num = Interlocked.Increment(ref StoreSessionCache.globalCacheId);
						this.diagnosticsSession.TraceDebug<long, StoreSessionCacheKey>("Checking in store session cache {0} back for {1}", num, cacheKey);
						this.storeSessionCache.AddSliding(num, storeSessionContext, this.cacheTimeout, new RemoveItemDelegate<long, StoreSessionCache.StoreSessionContext>(this.RemoveFromCacheCallback));
						flag = true;
						list.Add(num);
					}
				}
			}
			finally
			{
				if (!flag && storeSessionContext.Session != null)
				{
					storeSessionContext.Session.Dispose();
					storeSessionContext.Session = null;
				}
			}
		}

		private void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				lock (this.locker)
				{
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
					this.disposed = true;
					this.storeSessionCache.Dispose();
					this.cacheLookup.Clear();
				}
			}
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void RemoveFromCacheCallback(long cacheId, StoreSessionCache.StoreSessionContext storeSessionContext, RemoveReason reason)
		{
			if (reason == RemoveReason.Removed)
			{
				return;
			}
			try
			{
				StoreSessionCacheKey cacheKey = storeSessionContext.CacheKey;
				this.diagnosticsSession.TraceDebug<StoreSessionCacheKey, long, RemoveReason>("Store session for {0} is removed from cache {1} due to reason: {2}", cacheKey, cacheId, reason);
				lock (this.locker)
				{
					List<long> list;
					if (this.cacheLookup.TryGetValue(cacheKey, out list))
					{
						this.diagnosticsSession.TraceDebug<StoreSessionCacheKey, int>("Removing from lookup cache for {0}: {1} store session caches", cacheKey, list.Count);
						list.Remove(cacheId);
						if (list.Count == 0)
						{
							this.diagnosticsSession.TraceDebug<StoreSessionCacheKey>("Remove the lookup cache for {0}", cacheKey);
							this.cacheLookup.Remove(cacheKey);
						}
					}
				}
			}
			finally
			{
				if (storeSessionContext.Session != null)
				{
					storeSessionContext.Session.Dispose();
					storeSessionContext.Session = null;
				}
			}
		}

		private StoreSession CreateStoreSession(MdbItemIdentity identity, bool isMoveDestination, bool wantSessionForRMS, bool localOnly, bool readOnly)
		{
			Guid mdbGuid = identity.GetMdbGuid(MdbItemIdentity.Location.ExchangeMdb);
			if (Guid.Empty == mdbGuid)
			{
				throw new ArgumentException("Mdb Guid is empty");
			}
			if (Guid.Empty == identity.MailboxGuid)
			{
				throw new ArgumentException("Mailbox Guid is empty");
			}
			bool isPublicFolderMailbox = false;
			byte[] persistableHint = null;
			if (identity.PersistableTenantId == null)
			{
				DatabaseLocationInfo activeLocationInfo = null;
				if (!localOnly)
				{
					ActiveManager noncachingActiveManagerInstance = ActiveManager.GetNoncachingActiveManagerInstance();
					activeLocationInfo = noncachingActiveManagerInstance.GetServerForDatabase(mdbGuid, GetServerForDatabaseFlags.IgnoreAdSiteBoundary | GetServerForDatabaseFlags.BasicQuery);
				}
				MapiUtil.TranslateMapiExceptions(this.diagnosticsSession, Strings.ConnectionToMailboxFailed(identity.MailboxGuid), delegate
				{
					using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create(isMoveDestination ? "Client=CI;Client=CIMoveDestination" : "Client=CI", localOnly ? LocalServer.GetServer().Fqdn : activeLocationInfo.ServerFqdn, null, null, null))
					{
						Guid b = Guid.Empty;
						PropValue[][] mailboxTableInfo = exRpcAdmin.GetMailboxTableInfo(mdbGuid, identity.MailboxGuid, new PropTag[]
						{
							PropTag.UserGuid,
							PropTag.MailboxType,
							PropTag.PersistableTenantPartitionHint
						});
						foreach (PropValue[] array2 in mailboxTableInfo)
						{
							foreach (PropValue propValue in array2)
							{
								int num = propValue.PropTag.Id();
								if (num == PropTag.UserGuid.Id())
								{
									b = new Guid(propValue.GetBytes());
								}
								else if (num == PropTag.MailboxType.Id())
								{
									isPublicFolderMailbox = StoreSession.IsPublicFolderMailbox(propValue.GetInt());
								}
								else if (num == PropTag.PersistableTenantPartitionHint.Id())
								{
									persistableHint = propValue.GetBytes();
								}
							}
							if (identity.MailboxGuid == b)
							{
								break;
							}
						}
					}
				});
			}
			else
			{
				isPublicFolderMailbox = identity.IsPublicFolder;
				persistableHint = identity.PersistableTenantId;
			}
			ExchangePrincipal exchangePrincipal;
			if (!wantSessionForRMS)
			{
				if (localOnly)
				{
					DatabaseLocationInfo databaseLocationInfo = new DatabaseLocationInfo(LocalServer.GetServer(), false);
					exchangePrincipal = ExchangePrincipal.FromMailboxData(identity.MailboxGuid, mdbGuid, null, Array<CultureInfo>.Empty, RemotingOptions.LocalConnectionsOnly, databaseLocationInfo);
				}
				else
				{
					exchangePrincipal = ExchangePrincipal.FromMailboxData(identity.MailboxGuid, mdbGuid, Array<CultureInfo>.Empty, RemotingOptions.AllowCrossSite);
				}
			}
			else
			{
				OrganizationId currentOrganizationId = ADSessionSettings.FromTenantPartitionHint(TenantPartitionHint.FromPersistablePartitionHint(persistableHint)).CurrentOrganizationId;
				ADSessionSettings adSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(currentOrganizationId);
				if (localOnly)
				{
					exchangePrincipal = ExchangePrincipal.FromLocalServerMailboxGuid(adSettings, mdbGuid, identity.MailboxGuid, true);
				}
				else
				{
					exchangePrincipal = ExchangePrincipal.FromMailboxGuid(adSettings, identity.MailboxGuid, mdbGuid, RemotingOptions.AllowCrossSite, null, true);
				}
			}
			ExAssert.RetailAssert(exchangePrincipal != null, "ExchangePrincipal is null");
			string clientInfoString = isMoveDestination ? "Client=CI;Client=CIMoveDestination" : "Client=CI";
			if (isPublicFolderMailbox)
			{
				return PublicFolderSession.OpenAsSearch(exchangePrincipal, clientInfoString, readOnly);
			}
			StoreSession result;
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				MailboxAccessInfo accessInfo = new MailboxAccessInfo(new WindowsPrincipal(current));
				result = MailboxSession.ConfigurableOpen(exchangePrincipal, accessInfo, CultureInfo.InvariantCulture, clientInfoString, LogonType.SystemService, StoreSessionCache.MailboxProperties, readOnly ? (MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.OverrideHomeMdb | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.UseNamedProperties | MailboxSession.InitializationFlags.DeferDefaultFolderIdInitialization | MailboxSession.InitializationFlags.ReadOnly) : (MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.OverrideHomeMdb | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.UseNamedProperties | MailboxSession.InitializationFlags.DeferDefaultFolderIdInitialization), MailboxSession.FreeDefaultFolders);
			}
			return result;
		}

		private void RemoveAllFromCache(StoreSessionCacheKey cacheKey)
		{
			List<long> list;
			if (this.cacheLookup.TryGetValue(cacheKey, out list))
			{
				foreach (long key in list)
				{
					StoreSessionCache.StoreSessionContext storeSessionContext = this.storeSessionCache.Remove(key);
					if (storeSessionContext.Session != null)
					{
						storeSessionContext.Session.Dispose();
						storeSessionContext.Session = null;
					}
				}
				this.cacheLookup.Remove(cacheKey);
			}
		}

		private const MailboxSession.InitializationFlags MailboxSessionInitFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.OverrideHomeMdb | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.UseNamedProperties | MailboxSession.InitializationFlags.DeferDefaultFolderIdInitialization;

		private const MailboxSession.InitializationFlags MailboxReadOnlySessionInitFlags = MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.UserConfigurationManager | MailboxSession.InitializationFlags.OverrideHomeMdb | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.UseNamedProperties | MailboxSession.InitializationFlags.DeferDefaultFolderIdInitialization | MailboxSession.InitializationFlags.ReadOnly;

		private static readonly PropertyDefinition[] MailboxProperties = new PropertyDefinition[]
		{
			MailboxSchema.LocaleId
		};

		private static long globalCacheId;

		private readonly int maxNumberOfStoreSessionsPerMailbox;

		private readonly TimeSpan cacheTimeout;

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly MdbCachePerfCountersInstance cachePerfCounter;

		private TimeoutCache<long, StoreSessionCache.StoreSessionContext> storeSessionCache;

		private Dictionary<StoreSessionCacheKey, List<long>> cacheLookup = new Dictionary<StoreSessionCacheKey, List<long>>();

		private object locker = new object();

		private bool disposed;

		private DisposeTracker disposeTracker;

		internal class StoreSessionContext
		{
			public StoreSessionContext(StoreSessionCacheKey cacheKey)
			{
				this.CacheKey = cacheKey;
			}

			public StoreSessionCacheKey CacheKey { get; private set; }

			public StoreSession Session { get; set; }

			public Exception Exception { get; set; }
		}
	}
}
