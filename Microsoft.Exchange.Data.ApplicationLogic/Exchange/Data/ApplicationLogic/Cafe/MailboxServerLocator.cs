using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.ServerLocator;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;
using www.outlook.com.highavailability.ServerLocator.v1;

namespace Microsoft.Exchange.Data.ApplicationLogic.Cafe
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxServerLocator : DisposeTrackableBase
	{
		private Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.CafeTracer;
			}
		}

		public Guid DatabaseGuid { get; private set; }

		public Dictionary<Guid, BackEndServer> AvailabilityGroupServers { get; private set; }

		public string ResourceForestFqdn { get; private set; }

		public string DomainController { get; private set; }

		public long Latency { get; private set; }

		public List<long> GlsLatencies
		{
			get
			{
				return this.glsLatencies;
			}
		}

		public List<long> DirectoryLatencies
		{
			get
			{
				return this.resourceForestLatencies;
			}
		}

		public string[] LocatorServiceHosts
		{
			get
			{
				return this.contactedServers.ConvertAll<string>((ADObjectId x) => x.Name).ToArray();
			}
		}

		public bool BatchRequest { get; private set; }

		public bool IsSourceCachedData { get; private set; }

		internal bool SkipServerLocatorQuery { get; set; }

		private MailboxServerLocator(Guid databaseGuid, string tenantAcceptedDomain) : this(databaseGuid, tenantAcceptedDomain, true)
		{
		}

		private MailboxServerLocator(Guid databaseGuid, string tenantAcceptedDomain, bool batchRequest)
		{
			this.glsLatencies = new List<long>();
			this.resourceForestLatencies = new List<long>();
			this.lockObject = new object();
			this.contactedServers = new List<ADObjectId>();
			base..ctor();
			if (databaseGuid.Equals(Guid.Empty))
			{
				throw new ArgumentNullException("databaseGuid");
			}
			this.DatabaseGuid = databaseGuid;
			if (!string.IsNullOrEmpty(tenantAcceptedDomain))
			{
				this.ResourceForestFqdn = this.GetResourceForestFqdnByAcceptedDomainName(tenantAcceptedDomain);
				this.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "[MailboxServerLocator.Ctor] Resolved resource forest {0} from tenant accepted domain {1}.", this.ResourceForestFqdn, tenantAcceptedDomain);
			}
			this.BatchRequest = batchRequest;
			this.IsSourceCachedData = this.BatchRequest;
			this.InitializeConfigSession();
		}

		private MailboxServerLocator(Guid databaseGuid, Fqdn resourceForestFqdn) : this(databaseGuid, resourceForestFqdn, true)
		{
		}

		private MailboxServerLocator(Guid databaseGuid, Fqdn resourceForestFqdn, bool batchRequest)
		{
			this.glsLatencies = new List<long>();
			this.resourceForestLatencies = new List<long>();
			this.lockObject = new object();
			this.contactedServers = new List<ADObjectId>();
			base..ctor();
			if (databaseGuid.Equals(Guid.Empty))
			{
				throw new ArgumentNullException("databaseGuid");
			}
			this.DatabaseGuid = databaseGuid;
			this.ResourceForestFqdn = resourceForestFqdn;
			this.BatchRequest = batchRequest;
			this.IsSourceCachedData = this.BatchRequest;
			this.InitializeConfigSession();
		}

		internal static MailboxServerLocator CreateWithDomainName(Guid databaseGuid, string domainName)
		{
			return new MailboxServerLocator(databaseGuid, domainName);
		}

		internal static MailboxServerLocator CreateWithResourceForestFqdn(Guid databaseGuid, Fqdn resourceForestFqdn)
		{
			return new MailboxServerLocator(databaseGuid, resourceForestFqdn);
		}

		internal static MailboxServerLocator Create(Guid databaseGuid, string domainName, string resourceForest)
		{
			return MailboxServerLocator.Create(databaseGuid, domainName, resourceForest, true);
		}

		internal static MailboxServerLocator Create(Guid databaseGuid, string domainName, string resourceForest, bool batchRequest)
		{
			if (MailboxServerLocator.UseResourceForest.Value && !string.IsNullOrEmpty(resourceForest))
			{
				return new MailboxServerLocator(databaseGuid, new Fqdn(resourceForest), batchRequest);
			}
			return new MailboxServerLocator(databaseGuid, domainName, batchRequest);
		}

		public IAsyncResult BeginGetServer(AsyncCallback callback, object asyncState)
		{
			if (this.serverLocator != null)
			{
				throw new InvalidOperationException("BeginGetServerForDatabase executing in progress.");
			}
			this.ResolveMasterServerOrDag();
			if (this.masterServer == null)
			{
				this.Tracer.TraceError<Guid>((long)this.GetHashCode(), "[MailboxServerLocator.BeginGetServerForDatabase] Cannot find any available mailbox server for database {0}.", this.DatabaseGuid);
				throw new MailboxServerLocatorException(this.DatabaseGuid.ToString());
			}
			IAsyncResult result2;
			lock (this.lockObject)
			{
				this.stopWatch = Stopwatch.StartNew();
				if (this.masterServer.VersionNumber < Server.E15MinVersion || this.SkipServerLocatorQuery || MailboxServerLocator.InjectRemoteForestDownLevelServerException.Value)
				{
					MailboxServerLocator.DummyAsyncResult result = new MailboxServerLocator.DummyAsyncResult
					{
						AsyncState = asyncState
					};
					if (callback != null)
					{
						ThreadPool.QueueUserWorkItem(delegate(object o)
						{
							callback(result);
						});
					}
					this.IsSourceCachedData = false;
					result2 = result;
				}
				else
				{
					this.lazyAsyncResult = new LazyAsyncResult(this, asyncState, callback);
					this.ServerLocatorBeginGetServerList(this.BatchRequest);
					result2 = this.lazyAsyncResult;
				}
			}
			return result2;
		}

		public BackEndServer EndGetServer(IAsyncResult result)
		{
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			lock (this.lockObject)
			{
				base.CheckDisposed();
			}
			BackEndServer result2;
			try
			{
				MailboxServerLocator.DummyAsyncResult dummyAsyncResult = result as MailboxServerLocator.DummyAsyncResult;
				if (dummyAsyncResult != null)
				{
					this.CheckDownLevelServerForest();
					BackEndServer backEndServer = null;
					if (this.masterServer.VersionNumber < Server.E14MinVersion || this.SkipServerLocatorQuery)
					{
						backEndServer = new BackEndServer(this.masterServer.Fqdn, this.masterServer.VersionNumber);
					}
					else
					{
						lock (this.lockObject)
						{
							backEndServer = this.GetLegacyServerForDatabase(this.DatabaseGuid);
						}
					}
					if (backEndServer != null)
					{
						this.AvailabilityGroupServers = new Dictionary<Guid, BackEndServer>(1);
						this.AvailabilityGroupServers.Add(this.DatabaseGuid, backEndServer);
					}
					result2 = backEndServer;
				}
				else
				{
					lock (this.lockObject)
					{
						if (this.serverLocator == null)
						{
							throw new InvalidOperationException("BeginGetServerForDatabase() was not executed.");
						}
						if (!object.ReferenceEquals(result, this.lazyAsyncResult))
						{
							throw new InvalidOperationException("Calling with the wrong instance of IAsyncResult.");
						}
					}
					this.lazyAsyncResult.InternalWaitForCompletion();
					lock (this.lockObject)
					{
						if (this.asyncException != null)
						{
							this.Tracer.TraceError<Exception>((long)this.GetHashCode(), "[MailboxServerLocator.EndGetServer] Throwing async exception {0}.", this.asyncException);
							throw this.asyncException;
						}
						result2 = this.AvailabilityGroupServers[this.DatabaseGuid];
					}
				}
			}
			finally
			{
				this.Latency = this.stopWatch.ElapsedMilliseconds;
			}
			return result2;
		}

		public BackEndServer GetServer()
		{
			return this.EndGetServer(this.BeginGetServer(null, null));
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxServerLocator>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				lock (this.lockObject)
				{
					if (this.serverLocator != null)
					{
						this.serverLocator.Dispose();
						this.serverLocator = null;
					}
				}
			}
		}

		private void InitializeConfigSession()
		{
			PartitionId partitionId = PartitionId.LocalForest;
			if (this.ResourceForestFqdn != null)
			{
				partitionId = new PartitionId(this.ResourceForestFqdn);
			}
			this.configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 650, "InitializeConfigSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\MailboxServerLocator.cs");
		}

		private void ServerLocatorBeginGetServerList(bool batchRequest)
		{
			this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[MailboxServerLocator.ServerLocatorBeginGetServerList] Calling ServerLocatorService for on host {0}.", this.masterServer.Fqdn);
			if (this.serverLocator != null)
			{
				this.serverLocator.Dispose();
				this.serverLocator = null;
			}
			this.serverLocator = ServerLocatorServiceClient.Create(this.masterServer.Fqdn, MailboxServerLocator.ServerLocatorCloseTimeout.Value, MailboxServerLocator.ServerLocatorOpenTimeout.Value, MailboxServerLocator.ServerLocatorReceiveTimeout.Value, MailboxServerLocator.ServerLocatorSendTimeout.Value);
			this.IsSourceCachedData = batchRequest;
			if (batchRequest)
			{
				this.serverLocator.BeginGetActiveCopiesForDatabaseAvailabilityGroup(new AsyncCallback(this.ServerLocatorAsyncCallback), batchRequest);
				return;
			}
			this.serverLocator.BeginGetServerForDatabase(this.DatabaseGuid, new AsyncCallback(this.ServerLocatorAsyncCallback), batchRequest);
		}

		private void ServerLocatorAsyncCallback(IAsyncResult locatorAsyncResult)
		{
			lock (this.lockObject)
			{
				this.AvailabilityGroupServers = null;
				this.asyncException = null;
				Exception ex = null;
				bool flag2 = (bool)locatorAsyncResult.AsyncState;
				bool flag3 = false;
				try
				{
					if (flag2)
					{
						try
						{
							DatabaseServerInformation[] backEndServerList = this.serverLocator.EndGetActiveCopiesForDatabaseAvailabilityGroup(locatorAsyncResult);
							this.AvailabilityGroupServers = this.ProcessBatchResults(backEndServerList);
							this.lazyAsyncResult.InvokeCallback();
							return;
						}
						catch (MissingRequestedDatabaseException)
						{
							flag3 = true;
							flag2 = false;
							goto IL_93;
						}
						goto IL_66;
						IL_93:
						goto IL_C0;
					}
					IL_66:
					DatabaseServerInformation backEndServerInfo = this.serverLocator.EndGetServerForDatabase(locatorAsyncResult);
					this.AvailabilityGroupServers = this.ProcessSingleResult(backEndServerInfo);
					this.lazyAsyncResult.InvokeCallback();
					return;
				}
				catch (Exception ex2)
				{
					this.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "[MailboxServerLocator.ServerLocatorAsyncCallback] ServerLocatorService returned error when contacting host {0}. Exception: {1}", this.masterServer.Fqdn, ex2);
					ex = ex2;
				}
				IL_C0:
				if (!(ex is ServerLocatorClientTransientException) && !(ex is ServerLocatorClientException))
				{
					if (!flag3)
					{
						goto IL_116;
					}
				}
				try
				{
					if (!flag3)
					{
						this.masterServer = null;
						this.ResolveMasterServerOrDag();
					}
					if (this.masterServer != null)
					{
						this.ServerLocatorBeginGetServerList(flag2);
						return;
					}
				}
				catch (Exception ex3)
				{
					ex = ex3;
					this.Tracer.TraceError<Exception>((long)this.GetHashCode(), "[MailboxServerLocator.ServerLocatorAsyncCallback] Error occurred during retry. Exception: {0}", ex3);
				}
				IL_116:
				this.asyncException = ex;
				this.lazyAsyncResult.InvokeCallback();
			}
		}

		private void ResolveMasterServerOrDag()
		{
			if (this.serversList == null)
			{
				Database database = this.InvokeResourceForest<Database>(() => this.configSession.FindDatabaseByGuid<Database>(this.DatabaseGuid));
				if (database == null)
				{
					throw new DatabaseNotFoundException(this.DatabaseGuid.ToString());
				}
				ADObjectId masterServerOrDag = null;
				if (database.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
				{
					masterServerOrDag = database.Server;
				}
				else
				{
					masterServerOrDag = database.MasterServerOrAvailabilityGroup;
				}
				if (masterServerOrDag == null)
				{
					return;
				}
				if (database.MasterType == MasterType.DatabaseAvailabilityGroup)
				{
					DatabaseAvailabilityGroup databaseAvailabilityGroup = this.InvokeResourceForest<DatabaseAvailabilityGroup>(() => this.configSession.Read<DatabaseAvailabilityGroup>(masterServerOrDag));
					this.Tracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "[MailboxServerLocator.ResolveMasterServerOrDag] Database {0} is mastered by DatabaseAvailabilityGroup {1}.", this.DatabaseGuid, databaseAvailabilityGroup.Id.ToString());
					this.serversList = new List<ADObjectId>(databaseAvailabilityGroup.Servers);
				}
				else
				{
					this.serversList = new List<ADObjectId>
					{
						masterServerOrDag
					};
				}
			}
			this.masterServer = this.InvokeResourceForest<MiniServer>(() => this.SelectServerFromDag(this.configSession, this.serversList, this.contactedServers));
			this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[MailboxServerLocator.ResolveMasterServerOrDag] To ServerLocatorService host to use is {0}.", (this.masterServer != null) ? this.masterServer.Fqdn : null);
		}

		private Dictionary<Guid, BackEndServer> ProcessSingleResult(DatabaseServerInformation backEndServerInfo)
		{
			if (backEndServerInfo == null)
			{
				this.Tracer.TraceError<string, Guid>((long)this.GetHashCode(), "[MailboxServerLocator.ProcessSingleResult] ServerLocatorServiceClient did not return active server info from master server {0} for database {1}.", this.masterServer.Fqdn, this.DatabaseGuid);
				throw new MailboxServerLocatorException(this.DatabaseGuid.ToString());
			}
			if (string.IsNullOrEmpty(backEndServerInfo.ServerFqdn))
			{
				this.Tracer.TraceError<string, Guid>((long)this.GetHashCode(), "[MailboxServerLocator.ProcessSingleResult] ServerLocatorServiceClient returned empty active server FQDN from master server {0} for database {1}.", this.masterServer.Fqdn, this.DatabaseGuid);
				throw new MailboxServerLocatorException(this.DatabaseGuid.ToString());
			}
			if (backEndServerInfo.ServerVersion == 0)
			{
				this.Tracer.TraceWarning<string, Guid>((long)this.GetHashCode(), "[MailboxServerLocator.ProcessSingleResult] ServerLocatorServiceClient returned empty active server version from master server {0} for database {1}.", this.masterServer.Fqdn, this.DatabaseGuid);
				backEndServerInfo.ServerVersion = this.masterServer.VersionNumber;
			}
			this.Tracer.TraceDebug((long)this.GetHashCode(), "[MailboxServerLocator.ProcessSingleResult] ServerLocatorServiceClient returned active server {2} with version {3} from master server {0} for database {1}.", new object[]
			{
				this.masterServer.Fqdn,
				this.DatabaseGuid,
				backEndServerInfo.ServerFqdn,
				new ServerVersion(backEndServerInfo.ServerVersion)
			});
			return new Dictionary<Guid, BackEndServer>(1)
			{
				{
					this.DatabaseGuid,
					new BackEndServer(backEndServerInfo.ServerFqdn, backEndServerInfo.ServerVersion)
				}
			};
		}

		private Dictionary<Guid, BackEndServer> ProcessBatchResults(DatabaseServerInformation[] backEndServerList)
		{
			if (backEndServerList == null || backEndServerList.Length == 0)
			{
				this.Tracer.TraceError<string, Guid>((long)this.GetHashCode(), "[MailboxServerLocator.ProcessBatchResults] ServerLocatorServiceClient did not return active server info from master server {0} for database {1}.", this.masterServer.Fqdn, this.DatabaseGuid);
				throw new MailboxServerLocatorException(this.DatabaseGuid.ToString());
			}
			this.Tracer.TraceDebug<int, string, Guid>((long)this.GetHashCode(), "[MailboxServerLocator.ProcessBatchResults] ServerLocatorServiceClient returned {0} servers from master server {0} for database {1}.", backEndServerList.Length, this.masterServer.Fqdn, this.DatabaseGuid);
			Dictionary<Guid, BackEndServer> dictionary = new Dictionary<Guid, BackEndServer>(backEndServerList.Length);
			foreach (DatabaseServerInformation databaseServerInformation in backEndServerList)
			{
				if (string.IsNullOrEmpty(databaseServerInformation.ServerFqdn))
				{
					this.Tracer.TraceWarning<string, Guid>((long)this.GetHashCode(), "[MailboxServerLocator.ProcessBatchResults] ServerLocatorServiceClient returned empty active server FQDN from master server {0} for database {1}.", this.masterServer.Fqdn, databaseServerInformation.DatabaseGuid);
				}
				else
				{
					if (databaseServerInformation.ServerVersion == 0)
					{
						this.Tracer.TraceWarning<string, string, Guid>((long)this.GetHashCode(), "[MailboxServerLocator.ProcessBatchResults] ServerLocatorServiceClient returned empty active server version of server {0} from master server {1} for database {2}.", databaseServerInformation.ServerFqdn, this.masterServer.Fqdn, databaseServerInformation.DatabaseGuid);
						databaseServerInformation.ServerVersion = this.masterServer.VersionNumber;
						if (!MailboxServerLocator.NoServiceTopologyTryGetServerVersion)
						{
							ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\MailboxServerLocator.cs", "ProcessBatchResults", 985);
							int serverVersion;
							if (currentServiceTopology.TryGetServerVersion(databaseServerInformation.ServerFqdn, out serverVersion, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Cafe\\MailboxServerLocator.cs", "ProcessBatchResults", 986))
							{
								databaseServerInformation.ServerVersion = serverVersion;
							}
						}
					}
					this.Tracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "[MailboxServerLocator.ProcessBatchResults] The active server of database {0} is {1}.", databaseServerInformation.DatabaseGuid, databaseServerInformation.ServerFqdn);
					dictionary[databaseServerInformation.DatabaseGuid] = new BackEndServer(databaseServerInformation.ServerFqdn, databaseServerInformation.ServerVersion);
				}
			}
			if (!dictionary.ContainsKey(this.DatabaseGuid))
			{
				this.Tracer.TraceWarning<string, Guid>((long)this.GetHashCode(), "[MailboxServerLocator.ProcessBatchResults] ServerLocatorServiceClient did not return the active server of database {1} from master server {0}.", this.masterServer.Fqdn, this.DatabaseGuid);
				throw new MissingRequestedDatabaseException(this.DatabaseGuid.ToString());
			}
			return dictionary;
		}

		private BackEndServer GetLegacyServerForDatabase(Guid databaseGuid)
		{
			this.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "[MailboxServerLocator.GetLegacyServerForDatabase] Resolving legacy server for database {0}.", databaseGuid);
			ActiveManager cachingActiveManagerInstance = ActiveManager.GetCachingActiveManagerInstance();
			DatabaseLocationInfo serverForDatabase = cachingActiveManagerInstance.GetServerForDatabase(databaseGuid);
			if (serverForDatabase != null)
			{
				this.Tracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "[MailboxServerLocator.GetLegacyServerForDatabase] Active manager returns server {1} for database {0}.", databaseGuid, serverForDatabase.ServerFqdn);
				return new BackEndServer(serverForDatabase.ServerFqdn, serverForDatabase.ServerVersion);
			}
			throw new MailboxServerLocatorException(this.DatabaseGuid.ToString());
		}

		private MiniServer SelectServerFromDag(ITopologyConfigurationSession configSession, List<ADObjectId> dagServers, List<ADObjectId> contactedServers)
		{
			if (dagServers.Count == 0 || dagServers.Count == contactedServers.Count)
			{
				return null;
			}
			int num = MailboxServerLocator.random.Next(dagServers.Count);
			int num2 = 0;
			ADObjectId adobjectId;
			MiniServer activeServer;
			for (;;)
			{
				adobjectId = dagServers[num];
				num2++;
				if (!contactedServers.Contains(adobjectId))
				{
					activeServer = this.GetActiveServer(configSession, adobjectId, num2 < dagServers.Count);
					if (activeServer != null)
					{
						break;
					}
				}
				num++;
				if (num >= dagServers.Count)
				{
					num = 0;
				}
				if (num2 >= dagServers.Count * 2)
				{
					goto Block_5;
				}
			}
			contactedServers.Add(adobjectId);
			return activeServer;
			Block_5:
			return null;
		}

		private T InvokeResourceForest<T>(Func<T> adCall)
		{
			Stopwatch stopwatch = new Stopwatch();
			T result = default(T);
			try
			{
				stopwatch.Start();
				result = adCall();
			}
			finally
			{
				stopwatch.Stop();
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				this.resourceForestLatencies.Add(elapsedMilliseconds);
				this.DomainController = this.configSession.DomainController;
			}
			return result;
		}

		private string GetResourceForestFqdnByAcceptedDomainName(string tenantAcceptedDomain)
		{
			Stopwatch stopwatch = new Stopwatch();
			string resourceForestFqdnByAcceptedDomainName;
			try
			{
				stopwatch.Start();
				resourceForestFqdnByAcceptedDomainName = ADAccountPartitionLocator.GetResourceForestFqdnByAcceptedDomainName(tenantAcceptedDomain);
			}
			finally
			{
				stopwatch.Stop();
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				this.glsLatencies.Add(elapsedMilliseconds);
			}
			return resourceForestFqdnByAcceptedDomainName;
		}

		private void CheckDownLevelServerForest()
		{
			if (!HttpProxyBackEndHelper.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.NoCrossForestServerLocate.Enabled)
			{
				return;
			}
			if (MailboxServerLocator.InjectRemoteForestDownLevelServerException.Value)
			{
				throw new RemoteForestDownLevelServerException(this.DatabaseGuid.ToString(), this.ResourceForestFqdn);
			}
			if (this.masterServer.VersionNumber >= Server.E15MinVersion)
			{
				return;
			}
			if (string.Equals(LocalServer.GetServer().Id.DomainId.DistinguishedName, this.masterServer.Id.DomainId.DistinguishedName, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			this.Tracer.TraceWarning<Guid, string>((long)this.GetHashCode(), "[MailboxServerLocator.CheckDownLevelServerForest] Master server {1} for down level database {0} is not in local forest.", this.DatabaseGuid, this.masterServer.Fqdn);
			throw new RemoteForestDownLevelServerException(this.DatabaseGuid.ToString(), this.ResourceForestFqdn);
		}

		private MiniServer GetActiveServer(ITopologyConfigurationSession configSession, ADObjectId serverId, bool skipForMaintenanceMode)
		{
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.CheckServerLocatorServersForMaintenanceMode.Enabled;
			bool enabled2 = VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.CheckServerOnlineForActiveServer.Enabled;
			ADPropertyDefinition[] properties;
			if (enabled)
			{
				properties = new ADPropertyDefinition[]
				{
					ActiveDirectoryServerSchema.DatabaseCopyAutoActivationPolicy,
					ServerSchema.ComponentStates,
					ActiveDirectoryServerSchema.DatabaseCopyActivationDisabledAndMoveNow
				};
			}
			else if (enabled2)
			{
				properties = new ADPropertyDefinition[]
				{
					ActiveDirectoryServerSchema.DatabaseCopyAutoActivationPolicy,
					ServerSchema.ComponentStates
				};
			}
			else
			{
				properties = new ADPropertyDefinition[]
				{
					ActiveDirectoryServerSchema.DatabaseCopyAutoActivationPolicy
				};
			}
			MiniServer miniServer = configSession.ReadMiniServer(serverId, properties);
			if (miniServer == null)
			{
				this.Tracer.TraceDebug((long)this.GetHashCode(), "[MailboxServerLocator.GetActiveServer] return null. Server is NULL.");
				return null;
			}
			if (skipForMaintenanceMode)
			{
				if (miniServer.DatabaseCopyAutoActivationPolicy == DatabaseCopyAutoActivationPolicyType.Blocked)
				{
					this.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "[MailboxServerLocator.GetActiveServer] return null. Server {0} DatabaseCopyAutoActivationPolicy {1}.", miniServer.ToString(), miniServer.DatabaseCopyAutoActivationPolicy.ToString());
					return null;
				}
				if (enabled && miniServer.DatabaseCopyActivationDisabledAndMoveNow)
				{
					this.Tracer.TraceDebug<MiniServer, bool>((long)this.GetHashCode(), "[MailboxServerLocator.GetActiveServer] return null. Server {0} DatabaseCopyActivationDisabledAndMoveNow is {1}.", miniServer, miniServer.DatabaseCopyActivationDisabledAndMoveNow);
					return null;
				}
				if (enabled2 || enabled)
				{
					MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)miniServer[ServerSchema.ComponentStates];
					if (!ServerComponentStates.IsServerOnline(multiValuedProperty))
					{
						this.Tracer.TraceDebug<MiniServer, string>((long)this.GetHashCode(), "[MailboxServerLocator.GetActiveServer] return null. Server {0} ComponentStates {1}.", miniServer, (multiValuedProperty == null) ? "<NULL>" : string.Join(",", multiValuedProperty.ToArray()));
						return null;
					}
				}
			}
			return miniServer;
		}

		private static readonly TimeSpanAppSettingsEntry ServerLocatorCloseTimeout = new TimeSpanAppSettingsEntry("MailboxServerLocator.ServerLocatorCloseTimeout", TimeSpanUnit.Seconds, TimeSpan.FromSeconds(10.0), ExTraceGlobals.CafeTracer);

		private static readonly TimeSpanAppSettingsEntry ServerLocatorOpenTimeout = new TimeSpanAppSettingsEntry("MailboxServerLocator.ServerLocatorOpenTimeout", TimeSpanUnit.Seconds, TimeSpan.FromSeconds(10.0), ExTraceGlobals.CafeTracer);

		private static readonly TimeSpanAppSettingsEntry ServerLocatorReceiveTimeout = new TimeSpanAppSettingsEntry("MailboxServerLocator.ServerLocatorReceiveTimeout", TimeSpanUnit.Seconds, TimeSpan.FromSeconds(10.0), ExTraceGlobals.CafeTracer);

		private static readonly TimeSpanAppSettingsEntry ServerLocatorSendTimeout = new TimeSpanAppSettingsEntry("MailboxServerLocator.ServerLocatorSendTimeout", TimeSpanUnit.Seconds, TimeSpan.FromSeconds(10.0), ExTraceGlobals.CafeTracer);

		private static readonly BoolAppSettingsEntry InjectRemoteForestDownLevelServerException = new BoolAppSettingsEntry("MailboxServerLocator.InjectRemoteForestDownLevelServerException", false, ExTraceGlobals.CafeTracer);

		public static readonly BoolAppSettingsEntry UseResourceForest = new BoolAppSettingsEntry("MailboxServerLocator.UseResourceForest", VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.UseResourceForest.Enabled, ExTraceGlobals.CafeTracer);

		private static readonly bool NoServiceTopologyTryGetServerVersion = VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.NoServiceTopologyTryGetServerVersion.Enabled;

		private static readonly Random random = new Random();

		private readonly List<long> glsLatencies;

		private readonly List<long> resourceForestLatencies;

		private object lockObject;

		private ITopologyConfigurationSession configSession;

		private MiniServer masterServer;

		private List<ADObjectId> serversList;

		private List<ADObjectId> contactedServers;

		private ServerLocatorServiceClient serverLocator;

		private LazyAsyncResult lazyAsyncResult;

		private Exception asyncException;

		private Stopwatch stopWatch;

		private class DummyAsyncResult : IAsyncResult
		{
			public object AsyncState { get; internal set; }

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					lock (this.lockObject)
					{
						if (this.waitEvent == null)
						{
							this.waitEvent = new ManualResetEvent(true);
						}
					}
					return this.waitEvent;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return true;
				}
			}

			private object lockObject = new object();

			private ManualResetEvent waitEvent;
		}
	}
}
