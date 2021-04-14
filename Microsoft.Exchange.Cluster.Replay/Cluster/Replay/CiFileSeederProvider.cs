using System;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.EseRepl;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Search.Fast;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CiFileSeederProvider
	{
		internal CiFileSeederProvider(string sourceServerName, string targetServerName, Guid databaseGuid)
		{
			this.sourceServerFqdn = sourceServerName;
			this.targetServerFqdn = targetServerName;
			this.databaseGuid = databaseGuid;
		}

		internal string NetworkName { get; set; }

		internal bool? CompressOverride { get; set; }

		internal bool? EncryptOverride { get; set; }

		public override string ToString()
		{
			return string.Format("[CiFileSeederProvider: sourceServer={0}, targetServer={1}, databaseGuid={2}]", this.sourceServerFqdn, this.targetServerFqdn, this.databaseGuid);
		}

		internal void Close()
		{
			this.CloseChannel();
		}

		internal void SeedCatalog(string endpoint, IReplicaSeederCallback callback, string reason)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "SeedCatalog {0} to {1}.", this.databaseGuid, endpoint);
			this.GetChannel();
			if (this.DoesSourceSupportExtensibleSeedingRequests())
			{
				SeedCiFileRequest2 seedCiFileRequest = new SeedCiFileRequest2(this.channel, this.databaseGuid, endpoint, reason);
				seedCiFileRequest.Send();
			}
			else
			{
				SeedCiFileRequest seedCiFileRequest2 = new SeedCiFileRequest(this.channel, this.databaseGuid, endpoint);
				seedCiFileRequest2.Send();
			}
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			replayStopwatch.Start();
			NetworkChannelMessage message = this.channel.GetMessage();
			SeedCiFileReply seedCiFileReply = message as SeedCiFileReply;
			if (seedCiFileReply == null)
			{
				this.channel.ThrowUnexpectedMessage(message);
			}
			ExTraceGlobals.SeederServerTracer.TraceDebug<long>((long)this.GetHashCode(), "SeedCiFile response took: {0}ms", replayStopwatch.ElapsedMilliseconds);
			string handle = seedCiFileReply.Handle;
			ExTraceGlobals.SeederServerTracer.TraceDebug<string>((long)this.GetHashCode(), "Get seeding handle: {0}", handle);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2911251773U);
			this.UpdateProgress(handle, callback);
			replayStopwatch.Stop();
			ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "{0}: SeedCatalog succeeded: {1} for {2} after {3} ms", new object[]
			{
				ExDateTime.Now,
				endpoint,
				this.databaseGuid,
				replayStopwatch.ElapsedMilliseconds
			});
		}

		internal void CancelSeeding()
		{
			try
			{
				ExTraceGlobals.SeederServerTracer.TraceError<Guid>((long)this.GetHashCode(), "CancelSeeding catalog for {0}.", this.databaseGuid);
				lock (this)
				{
					this.isAborted = true;
					if (this.channel != null)
					{
						this.channel.Abort();
					}
				}
			}
			finally
			{
				ReplayCrimsonEvents.SeedingErrorOnTarget.Log<Guid, string, LocalizedString>(this.databaseGuid, string.Empty, ReplayStrings.SeederOperationAborted);
			}
		}

		private void UpdateProgress(string seedingHandle, IReplicaSeederCallback callback)
		{
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			ProgressCiFileRequest progressCiFileRequest = new ProgressCiFileRequest(this.channel, this.databaseGuid, seedingHandle);
			int num = -1;
			TimeSpan timeout = TimeSpan.FromSeconds((double)RegistryParameters.SeedCatalogProgressIntervalInSec);
			int progress;
			for (;;)
			{
				progressCiFileRequest.Send();
				replayStopwatch.Restart();
				NetworkChannelMessage message = this.channel.GetMessage();
				ProgressCiFileReply progressCiFileReply = message as ProgressCiFileReply;
				if (progressCiFileReply == null)
				{
					this.channel.ThrowUnexpectedMessage(message);
				}
				ExTraceGlobals.SeederServerTracer.TraceDebug<long>((long)this.GetHashCode(), "ProgressCiFile response took: {0}ms", replayStopwatch.ElapsedMilliseconds);
				progress = progressCiFileReply.Progress;
				ExTraceGlobals.SeederServerTracer.TraceDebug<int>((long)this.GetHashCode(), "Get seeding progress: {0}", progress);
				if (callback != null && callback.IsBackupCancelled())
				{
					break;
				}
				if (progress < 0)
				{
					goto Block_4;
				}
				if (progress > num)
				{
					ExTraceGlobals.SeederServerTracer.TraceDebug<Guid, int, bool>((long)this.GetHashCode(), "Updating progress for catalog '{0}'. Percent = {1}%. Callback = {2}", this.databaseGuid, progress, callback != null);
					if (callback != null)
					{
						callback.ReportProgress("IndexSystem", 102400L, (long)progress * 1024L, (long)progress * 1024L);
					}
					num = progress;
				}
				if (progress == 100)
				{
					return;
				}
				Thread.Sleep(timeout);
			}
			ExTraceGlobals.SeederServerTracer.TraceDebug<int>((long)this.GetHashCode(), "The seeding was cancelled at {0}%", num);
			throw new SeederOperationAbortedException();
			Block_4:
			Exception innerException = new CiSeederGenericException(this.sourceServerFqdn, this.targetServerFqdn, ReplayStrings.CiSeederExchangeSearchTransientException(string.Format("{0}", progress)));
			throw new PerformingFastOperationException(innerException);
		}

		private void GetChannel()
		{
			lock (this)
			{
				if (this.isAborted)
				{
					throw new SeederOperationAbortedException();
				}
				if (this.channel == null)
				{
					NetworkPath networkPath = NetworkManager.ChooseNetworkPath(this.sourceServerFqdn, this.NetworkName, NetworkPath.ConnectionPurpose.Seeding);
					if (this.CompressOverride != null || this.EncryptOverride != null || !string.IsNullOrEmpty(this.NetworkName))
					{
						networkPath.Compress = (this.CompressOverride ?? networkPath.Compress);
						networkPath.Encrypt = (this.EncryptOverride ?? networkPath.Encrypt);
						networkPath.NetworkChoiceIsMandatory = true;
					}
					this.channel = NetworkChannel.Connect(networkPath, TcpChannel.GetDefaultTimeoutInMs(), false);
				}
			}
		}

		private void CloseChannel()
		{
			lock (this)
			{
				if (this.channel != null)
				{
					this.channel.Close();
					this.channel = null;
				}
			}
		}

		private bool DoesSourceSupportExtensibleSeedingRequests()
		{
			AmServerName serverName = new AmServerName(this.sourceServerFqdn);
			Exception ex;
			IADServer miniServer = AmBestCopySelectionHelper.GetMiniServer(serverName, out ex);
			return miniServer != null && ServerVersion.Compare(miniServer.AdminDisplayVersion, CiFileSeederProvider.FirstVersionSupportingExtensibleSeedingRequests) >= 0;
		}

		private static readonly ServerVersion FirstVersionSupportingExtensibleSeedingRequests = new ServerVersion(15, 0, 963, 0);

		private readonly string sourceServerFqdn;

		private readonly string targetServerFqdn;

		private readonly Guid databaseGuid;

		private NetworkChannel channel;

		private bool isAborted;
	}
}
