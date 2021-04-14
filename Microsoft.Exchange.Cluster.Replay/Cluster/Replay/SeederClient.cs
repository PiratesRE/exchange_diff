using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SeederClient : IDisposeTrackable, IDisposable
	{
		private SeederClient(string serverName, ServerVersion serverVersion, string databaseName, string sourceName, ReplayRpcClient client)
		{
			this.m_disposeTracker = this.GetDisposeTracker();
			this.m_serverName = serverName;
			this.m_serverVersion = serverVersion;
			this.m_databaseName = databaseName;
			this.m_sourceName = sourceName;
			this.m_client = client;
		}

		~SeederClient()
		{
			this.Dispose(false);
		}

		public string ServerName
		{
			get
			{
				return this.m_serverName;
			}
		}

		public static SeederClient Create(Server server, string databaseName, Server sourceServer)
		{
			if (server == null)
			{
				throw new ArgumentException("server cannot be null.", "server");
			}
			return SeederClient.Create(server.Fqdn, databaseName, (sourceServer == null) ? null : sourceServer.Fqdn, server.AdminDisplayVersion);
		}

		public static SeederClient Create(string serverName, string databaseName, string sourceName, ServerVersion serverVersion)
		{
			if (!ReplayRpcVersionControl.IsSeedRpcSupported(serverVersion))
			{
				throw new SeederRpcUnsupportedException(serverName, serverVersion.ToString(), ReplayRpcVersionControl.SeedRpcSupportVersion.ToString());
			}
			ExTraceGlobals.SeederClientTracer.TraceDebug<string, ServerVersion>(0L, "SeederClient is now being created for server '{0}' with version '0x{1:x}'.", serverName, serverVersion);
			ReplayRpcClient rpcClient = null;
			SeederRpcExceptionWrapper.Instance.ClientRetryableOperation(serverName, delegate
			{
				rpcClient = new ReplayRpcClient(serverName);
			});
			return new SeederClient(serverName, serverVersion, databaseName, sourceName, rpcClient);
		}

		public void Dispose()
		{
			if (!this.m_fDisposed)
			{
				if (this.m_disposeTracker != null)
				{
					this.m_disposeTracker.Dispose();
				}
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_fDisposed)
				{
					if (disposing)
					{
						this.m_client.Dispose();
						this.m_client = null;
					}
					this.m_fDisposed = true;
				}
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SeederClient>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Suppress();
			}
		}

		public void PrepareDbSeedAndBegin(Guid dbGuid, bool fDeleteExistingLogs, bool fSafeDeleteExistingFiles, bool fAutoSuspend, bool fManualResume, bool fSeedDatabase, bool fSeedCiFiles, string networkId, string seedingPath, string sourceName, bool? compressOverride, bool? encryptOverride, SeederRpcFlags flags = SeederRpcFlags.None)
		{
			if (dbGuid == Guid.Empty)
			{
				throw new ArgumentException("An invalid Database Guid was specified.", "dbGuid");
			}
			RpcSeederArgs args = new RpcSeederArgs(dbGuid, fDeleteExistingLogs, fAutoSuspend, seedingPath, null, networkId, false, sourceName, null, fManualResume, fSeedDatabase, fSeedCiFiles, compressOverride, encryptOverride, 0, fSafeDeleteExistingFiles, flags);
			this.ValidateArgs(args);
			ExTraceGlobals.SeederClientTracer.TraceDebug<RpcSeederArgs>((long)this.GetHashCode(), "PrepareDbSeedAndBegin(): Constructed RpcSeederArgs: {0}", args);
			RpcErrorExceptionInfo errorInfo = null;
			RpcSeederStatus seedStatus = null;
			ServerVersion version = this.GetTestHookServerVersion();
			bool isSafeDeleteSupported = ReplayRpcVersionControl.IsSeedRpcSafeDeleteSupported(version);
			bool isSeedV5Supported = ReplayRpcVersionControl.IsSeedRpcV5Supported(version);
			SeederRpcExceptionWrapper.Instance.ClientRetryableOperation(this.m_serverName, delegate
			{
				if (isSeedV5Supported)
				{
					errorInfo = this.m_client.RpccPrepareDatabaseSeedAndBegin5(args, ref seedStatus);
					return;
				}
				if (isSafeDeleteSupported)
				{
					errorInfo = this.m_client.RpccPrepareDatabaseSeedAndBegin4(args, ref seedStatus);
					return;
				}
				if (!fSafeDeleteExistingFiles)
				{
					errorInfo = this.m_client.RpccPrepareDatabaseSeedAndBegin(args, ref seedStatus);
					return;
				}
				ExTraceGlobals.SeederClientTracer.TraceError<string, ServerVersion, ServerVersion>((long)this.GetHashCode(), "PrepareDbSeedAndBegin(): Server '{0}' does not support SafeDeleteExistingFiles RPC. Server version: {1}. Minimum supported version: {2}", this.m_serverName, version, ReplayRpcVersionControl.SeedRpcSafeDeleteSupportVersion);
				throw new SeederRpcSafeDeleteUnsupportedException(this.m_serverName, version.ToString(), ReplayRpcVersionControl.SeedRpcSafeDeleteSupportVersion.ToString());
			});
			SeederRpcExceptionWrapper.Instance.ClientRethrowIfFailed(this.m_databaseName, this.m_serverName, errorInfo);
		}

		public void BeginServerLevelSeed(bool fDeleteExistingLogs, bool fSafeDeleteExistingFiles, int maxSeedsInParallel, bool fAutoSuspend, bool fManualResume, bool fSeedDatabase, bool fSeedCiFiles, bool? compressOverride, bool? encryptOverride, SeederRpcFlags flags = SeederRpcFlags.None)
		{
			RpcSeederArgs args = new RpcSeederArgs(Guid.Empty, fDeleteExistingLogs, fAutoSuspend, null, null, string.Empty, false, string.Empty, null, fManualResume, fSeedDatabase, fSeedCiFiles, compressOverride, encryptOverride, maxSeedsInParallel, fSafeDeleteExistingFiles, flags);
			this.ValidateArgs(args);
			ExTraceGlobals.SeederClientTracer.TraceDebug<RpcSeederArgs>((long)this.GetHashCode(), "BeginServerLevelSeed(): Constructed RpcSeederArgs: {0}", args);
			RpcErrorExceptionInfo errorInfo = null;
			RpcSeederStatus seedStatus = null;
			ServerVersion version = this.GetTestHookServerVersion();
			SeederRpcExceptionWrapper.Instance.ClientRetryableOperation(this.m_serverName, delegate
			{
				if (ReplayRpcVersionControl.IsSeedRpcV5Supported(version))
				{
					errorInfo = this.m_client.RpccPrepareDatabaseSeedAndBegin5(args, ref seedStatus);
					return;
				}
				if (ReplayRpcVersionControl.IsSeedRpcSafeDeleteSupported(version))
				{
					errorInfo = this.m_client.RpccPrepareDatabaseSeedAndBegin4(args, ref seedStatus);
					return;
				}
				ExTraceGlobals.SeederClientTracer.TraceError<string, ServerVersion, ServerVersion>((long)this.GetHashCode(), "BeginServerLevelSeed(): Server '{0}' does not support server-level reseed RPC. Server version: {1}. Minimum supported version: {2}", this.m_serverName, version, ReplayRpcVersionControl.SeedRpcSafeDeleteSupportVersion);
				throw new SeederRpcServerLevelUnsupportedException(this.m_serverName, version.ToString(), ReplayRpcVersionControl.SeedRpcSafeDeleteSupportVersion.ToString());
			});
			SeederRpcExceptionWrapper.Instance.ClientRethrowIfFailed(this.m_databaseName, this.m_serverName, errorInfo);
		}

		public RpcSeederStatus GetDatabaseSeedStatus(Guid dbGuid)
		{
			if (dbGuid == Guid.Empty)
			{
				throw new ArgumentException("An invalid Database Guid was specified.", "dbGuid");
			}
			ExTraceGlobals.SeederClientTracer.TraceDebug<Guid>((long)this.GetHashCode(), "GetDatabaseSeedStatus(): calling server RPC for guid ({0}).", dbGuid);
			RpcErrorExceptionInfo errorExceptionInfo = null;
			RpcSeederStatus seedStatus = null;
			SeederRpcExceptionWrapper.Instance.ClientRetryableOperation(this.m_serverName, delegate
			{
				errorExceptionInfo = this.m_client.RpccGetDatabaseSeedStatus(dbGuid, ref seedStatus);
			});
			SeederRpcExceptionWrapper.Instance.ClientRethrowIfFailed(this.m_databaseName, this.m_serverName, errorExceptionInfo);
			return seedStatus;
		}

		public void CancelDbSeed(Guid dbGuid)
		{
			if (dbGuid == Guid.Empty)
			{
				throw new ArgumentException("An invalid Database Guid was specified.", "dbGuid");
			}
			RpcErrorExceptionInfo errorInfo = null;
			ExTraceGlobals.SeederClientTracer.TraceDebug<Guid>((long)this.GetHashCode(), "CancelDbSeed(): calling server RPC for guid ({0}).", dbGuid);
			SeederRpcExceptionWrapper.Instance.ClientRetryableOperation(this.m_serverName, delegate
			{
				errorInfo = this.m_client.CancelDbSeed(dbGuid);
			});
			SeederRpcExceptionWrapper.Instance.ClientRethrowIfFailed(this.m_databaseName, this.m_serverName, errorInfo);
		}

		public void EndDbSeed(Guid dbGuid)
		{
			if (dbGuid == Guid.Empty)
			{
				throw new ArgumentException("An invalid Database Guid was specified.", "dbGuid");
			}
			RpcErrorExceptionInfo errorInfo = null;
			ExTraceGlobals.SeederClientTracer.TraceDebug<Guid>((long)this.GetHashCode(), "EndDbSeed(): calling server RPC for guid ({0}).", dbGuid);
			SeederRpcExceptionWrapper.Instance.ClientRetryableOperation(this.m_serverName, delegate
			{
				errorInfo = this.m_client.EndDbSeed(dbGuid);
			});
			SeederRpcExceptionWrapper.Instance.ClientRethrowIfFailed(this.m_databaseName, this.m_serverName, errorInfo);
		}

		private void ValidateArgs(RpcSeederArgs args)
		{
			if (!args.SeedDatabase && !args.SeedCiFiles)
			{
				throw new ArgumentException("One of SeedDatabase and SeedCiFiles must be specified.");
			}
			if (args.SafeDeleteExistingFiles && args.DeleteExistingFiles)
			{
				throw new ArgumentException("Only one of SafeDeleteExistingFiles and DeleteExistingFiles can be specified at once.");
			}
		}

		private ServerVersion GetTestHookServerVersion()
		{
			ServerVersion serverVersion = this.m_serverVersion;
			if (RegistryTestHook.TargetServerVersionOverride > 0)
			{
				serverVersion = new ServerVersion(RegistryTestHook.TargetServerVersionOverride);
				ExTraceGlobals.SeederClientTracer.TraceDebug<string, ServerVersion>(0L, "GetTestHookServerVersion( {0} ) is returning TargetServerVersionOverride registry override of '{1}'.", this.m_serverName, serverVersion);
			}
			return serverVersion;
		}

		private ReplayRpcClient m_client;

		private string m_serverName;

		private string m_databaseName;

		private string m_sourceName;

		private ServerVersion m_serverVersion;

		private bool m_fDisposed;

		private DisposeTracker m_disposeTracker;

		private delegate void SeederRpcOperation();
	}
}
