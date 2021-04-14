using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SourceSeedTable
	{
		internal static SourceSeedTable Instance
		{
			get
			{
				return SourceSeedTable.s_mgr;
			}
		}

		public SeederServerContext TryGetContext(Guid guid)
		{
			SeederServerContext result = null;
			lock (this.locker)
			{
				if (this.activeSeeds.TryGetValue(guid, out result))
				{
					return result;
				}
			}
			return null;
		}

		public void RegisterSeed(SeederServerContext newCtx)
		{
			SeederServerContext seederServerContext = null;
			lock (this.locker)
			{
				SourceSeedTable.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "RegisterSeed {0}", newCtx.DatabaseGuid);
				if (this.activeSeeds.TryGetValue(newCtx.DatabaseGuid, out seederServerContext) && seederServerContext != null)
				{
					ReplayCrimsonEvents.SeedingSourceError.Log<Guid, string, string, string>(newCtx.DatabaseGuid, string.Empty, seederServerContext.TargetServerName, "RegisterSeed:SeedCtx already exists");
					throw new SeedingAnotherServerException(seederServerContext.TargetServerName, newCtx.TargetServerName);
				}
				this.activeSeeds[newCtx.DatabaseGuid] = newCtx;
			}
		}

		public void DeregisterSeed(SeederServerContext oldCtx)
		{
			SeederServerContext seederServerContext = null;
			SourceSeedTable.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "DeregisterSeed {0}", oldCtx.DatabaseGuid);
			lock (this.locker)
			{
				if (this.activeSeeds.TryGetValue(oldCtx.DatabaseGuid, out seederServerContext))
				{
					if (seederServerContext == oldCtx)
					{
						this.activeSeeds[oldCtx.DatabaseGuid] = null;
						SourceSeedTable.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "DeregisterSeed {0} successful.", oldCtx.DatabaseGuid);
					}
					else
					{
						SourceSeedTable.Tracer.TraceError<Guid>((long)this.GetHashCode(), "DeregisterSeed {0} ignored mismatached ctx.", oldCtx.DatabaseGuid);
					}
				}
			}
		}

		public void CancelSeedingIfAppropriate(SourceSeedTable.CancelReason reason, Guid dbGuid)
		{
			SeederServerContext seederServerContext = null;
			lock (this.locker)
			{
				if (this.activeSeeds.TryGetValue(dbGuid, out seederServerContext) && seederServerContext != null)
				{
					SourceSeedTable.Tracer.TraceDebug<string, SourceSeedTable.CancelReason>((long)this.GetHashCode(), "CancelSeeding {0} : {1}", seederServerContext.DatabaseName, reason);
					if (reason == SourceSeedTable.CancelReason.ConfigChanged && seederServerContext.IsCatalogSeed)
					{
						SourceSeedTable.Tracer.TraceDebug<string>((long)this.GetHashCode(), "CancelSeeding skipped for {0} because catalog is seeding", seederServerContext.DatabaseName);
						return;
					}
					this.activeSeeds[seederServerContext.DatabaseGuid] = null;
				}
			}
			if (seederServerContext != null)
			{
				LocalizedString message;
				if (reason == SourceSeedTable.CancelReason.CopyFailed)
				{
					message = ReplayStrings.CancelSeedingDueToFailed(seederServerContext.DatabaseName, Environment.MachineName);
				}
				else
				{
					message = ReplayStrings.CancelSeedingDueToConfigChangeOrServiceShutdown(seederServerContext.DatabaseName, Environment.MachineName, reason.ToString());
				}
				seederServerContext.CancelSeeding(message);
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.SeederServerTracer;

		private static SourceSeedTable s_mgr = new SourceSeedTable();

		private Dictionary<Guid, SeederServerContext> activeSeeds = new Dictionary<Guid, SeederServerContext>();

		private object locker = new object();

		public enum CancelReason
		{
			CopyRemoved = 1,
			CopySuspended,
			CopyFailed,
			ConfigChanged,
			ServiceShutdown
		}
	}
}
