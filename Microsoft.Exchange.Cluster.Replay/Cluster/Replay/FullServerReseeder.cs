using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FullServerReseeder : ChangePoller
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.SeederServerTracer;
			}
		}

		public FullServerReseeder(RpcSeederArgs seederArgs) : base(true)
		{
			this.m_args = seederArgs;
			this.m_resumesAttempted = new Dictionary<Guid, CopyStatusClientCachedEntry>(20);
			this.m_reseedsAttempted = new Dictionary<Guid, CopyStatusClientCachedEntry>(20);
		}

		protected override void PollerThread()
		{
			Thread.CurrentThread.Name = "FullServerReseeder";
			FullServerReseeder.Tracer.TraceDebug((long)this.GetHashCode(), "FullServerReseeder: Starting the main seeding thread.");
			ReplayCrimsonEvents.FullServerSeedStarted.Log();
			while (!this.m_fShutdown)
			{
				IMonitoringADConfigProvider monitoringADConfigProvider = Dependencies.MonitoringADConfigProvider;
				ICopyStatusClientLookup monitoringCopyStatusClientLookup = Dependencies.MonitoringCopyStatusClientLookup;
				try
				{
					AmServerName localComputerName = AmServerName.LocalComputerName;
					IMonitoringADConfig config = monitoringADConfigProvider.GetConfig(true);
					this.m_localServer = config.TargetMiniServer;
					IEnumerable<CopyStatusClientCachedEntry> copyStatusesByServer = monitoringCopyStatusClientLookup.GetCopyStatusesByServer(localComputerName, config.DatabaseMap[localComputerName], CopyStatusClientLookupFlags.None);
					IEnumerable<CopyStatusClientCachedEntry> enumerable = from status in copyStatusesByServer
					where this.IsCopyReseedable(status)
					select status;
					IEnumerable<CopyStatusClientCachedEntry> source = from status in copyStatusesByServer
					where this.IsCopyReseeding(status)
					select status;
					List<CopyStatusClientCachedEntry> list = new List<CopyStatusClientCachedEntry>(20);
					List<CopyStatusClientCachedEntry> list2 = new List<CopyStatusClientCachedEntry>(20);
					foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in enumerable)
					{
						if (!this.m_resumesAttempted.ContainsKey(copyStatusClientCachedEntry.DbGuid))
						{
							list.Add(copyStatusClientCachedEntry);
						}
						else if (!this.m_reseedsAttempted.ContainsKey(copyStatusClientCachedEntry.DbGuid))
						{
							list2.Add(copyStatusClientCachedEntry);
						}
					}
					if (list.Count == 0 && list2.Count == 0)
					{
						FullServerReseeder.Tracer.TraceDebug<int, int>((long)this.GetHashCode(), "FullServerReseeder: Exiting because there is nothing more to do. {0} copies resumed, and {1} copies reseeded.", this.m_resumesAttempted.Count, this.m_reseedsAttempted.Count);
						ReplayCrimsonEvents.FullServerSeedCompleted.Log<int, int>(this.m_resumesAttempted.Count, this.m_reseedsAttempted.Count);
						this.PrepareToStop();
						ThreadPool.QueueUserWorkItem(delegate(object state)
						{
							this.Stop();
						});
						break;
					}
					list.ForEach(delegate(CopyStatusClientCachedEntry status)
					{
						this.TryResumeCopy(status);
					});
					int num = source.Count<CopyStatusClientCachedEntry>();
					FullServerReseeder.Tracer.TraceDebug<int>((long)this.GetHashCode(), "FullServerReseeder: Currently, {0} seeds are concurrently running.", num);
					foreach (CopyStatusClientCachedEntry status2 in list2)
					{
						if (num >= this.m_args.MaxSeedsInParallel)
						{
							FullServerReseeder.Tracer.TraceDebug<int>((long)this.GetHashCode(), "FullServerReseeder: Number of concurrent reseeds exceeds MaxSeedsInParallel ({0}). Waiting for seeds to complete before starting new seeds.", this.m_args.MaxSeedsInParallel);
							break;
						}
						num++;
						this.TryReseedCopy(status2);
					}
				}
				catch (MonitoringADConfigException ex)
				{
					FullServerReseeder.Tracer.TraceError<MonitoringADConfigException>((long)this.GetHashCode(), "FullServerReseeder: Encountered an error when querying AD config: {0}", ex);
					ReplayCrimsonEvents.FullServerSeedError.Log<string>(ex.Message);
				}
				FullServerReseeder.Tracer.TraceDebug<int>((long)this.GetHashCode(), "FullServerReseeder: Sleeping for {0} secs and retrying.", this.RetryIntervalSecs);
				if (this.m_shutdownEvent.WaitOne(this.RetryIntervalSecs * 1000, false))
				{
					break;
				}
			}
			if (this.m_fShutdown)
			{
				FullServerReseeder.Tracer.TraceDebug((long)this.GetHashCode(), "FullServerReseeder: Exiting because of PrepareToStop() called!");
			}
		}

		private void TryResumeCopy(CopyStatusClientCachedEntry status)
		{
			Exception ex = null;
			string dbname = status.CopyStatus.DBName;
			string netbiosName = status.ServerContacted.NetbiosName;
			try
			{
				FullServerReseeder.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "FullServerReseeder: First attempting to resume database copy '{0}\\{1}' to avoid a reseed if possible.", dbname, netbiosName);
				ReplayCrimsonEvents.FullServerCopyResuming.Log<string, string, Guid>(dbname, netbiosName, status.DbGuid);
				this.m_resumesAttempted.Add(status.DbGuid, status);
				DatabaseCopyActionFlags flags = DatabaseCopyActionFlags.Replication | DatabaseCopyActionFlags.Activation | DatabaseCopyActionFlags.SkipSettingResumeAutoReseedState;
				Dependencies.ReplayRpcClientWrapper.RequestResume2(AmServerName.LocalComputerName.Fqdn, status.DbGuid, (uint)flags);
			}
			catch (TaskServerException ex2)
			{
				ex = ex2;
			}
			catch (TaskServerTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				FullServerReseeder.Tracer.TraceError<string, string, Exception>((long)this.GetHashCode(), "FullServerReseeder: Resuming database copy '{0}\\{1}' failed. Error: {2}", dbname, netbiosName, ex);
				ReplayCrimsonEvents.FullServerCopyResumeFailed.Log<string, string, Guid, string>(dbname, netbiosName, status.DbGuid, ex.Message);
			}
		}

		private void TryReseedCopy(CopyStatusClientCachedEntry status)
		{
			Exception ex = null;
			string dbname = status.CopyStatus.DBName;
			string netbiosName = status.ServerContacted.NetbiosName;
			try
			{
				FullServerReseeder.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "FullServerReseeder: Next, attempting to reseed database copy '{0}\\{1}'.", dbname, netbiosName);
				ReplayCrimsonEvents.FullServerCopyReseeding.Log<string, string, Guid>(dbname, netbiosName, status.DbGuid);
				this.m_reseedsAttempted.Add(status.DbGuid, status);
				using (SeederClient seederClient = SeederClient.Create(AmServerName.LocalComputerName.Fqdn, dbname, null, this.m_localServer.AdminDisplayVersion))
				{
					if (this.m_args.SeedCiFiles)
					{
						try
						{
							FullServerReseeder.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "FullServerReseeder: Trying to cancel/end a previously running seed for database copy '{0}\\{1}'", dbname, netbiosName);
							seederClient.CancelDbSeed(status.DbGuid);
							seederClient.EndDbSeed(status.DbGuid);
						}
						catch (SeederInstanceNotFoundException arg)
						{
							FullServerReseeder.Tracer.TraceDebug<string, Guid, SeederInstanceNotFoundException>((long)this.GetHashCode(), "FullServerReseeder: CancelDbSeed() or EndDbSeed() failed for database '{0}' ({1}). Error: {2}", dbname, status.DbGuid, arg);
						}
					}
					seederClient.PrepareDbSeedAndBegin(status.DbGuid, this.m_args.DeleteExistingFiles, this.m_args.SafeDeleteExistingFiles, this.m_args.AutoSuspend, this.m_args.ManualResume, this.m_args.SeedDatabase, this.m_args.SeedCiFiles, string.Empty, null, string.Empty, this.m_args.CompressOverride, this.m_args.EncryptOverride, this.m_args.Flags | SeederRpcFlags.SkipSettingReseedAutoReseedState);
				}
			}
			catch (SeederServerException ex2)
			{
				ex = ex2;
			}
			catch (SeederServerTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				FullServerReseeder.Tracer.TraceError<string, string, Exception>((long)this.GetHashCode(), "FullServerReseeder: Reseeding database copy '{0}\\{1}' failed. Error: {2}", dbname, netbiosName, ex);
				ReplayCrimsonEvents.FullServerCopyReseedFailed.Log<string, string, Guid, string>(dbname, netbiosName, status.DbGuid, ex.Message);
			}
		}

		private bool IsCopyReseedable(CopyStatusClientCachedEntry status)
		{
			return status.Result == CopyStatusRpcResult.Success && !status.IsActive && ((status.CopyStatus.CopyStatus == CopyStatusEnum.Suspended || status.CopyStatus.CopyStatus == CopyStatusEnum.FailedAndSuspended) && status.CopyStatus.ActionInitiator != ActionInitiatorType.Administrator);
		}

		private bool IsCopyReseeding(CopyStatusClientCachedEntry status)
		{
			return status.Result == CopyStatusRpcResult.Success && !status.IsActive && status.CopyStatus.CopyStatus == CopyStatusEnum.Seeding;
		}

		private readonly int RetryIntervalSecs = RegistryParameters.FullServerReseedRetryIntervalInSec;

		private IADServer m_localServer;

		private RpcSeederArgs m_args;

		private Dictionary<Guid, CopyStatusClientCachedEntry> m_resumesAttempted;

		private Dictionary<Guid, CopyStatusClientCachedEntry> m_reseedsAttempted;
	}
}
