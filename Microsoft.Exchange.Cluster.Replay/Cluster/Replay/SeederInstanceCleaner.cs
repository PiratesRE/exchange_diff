using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SeederInstanceCleaner : TimerComponent
	{
		public SeederInstanceCleaner(SeederInstances instances) : base(TimeSpan.Zero, TimeSpan.FromMilliseconds((double)SeederInstanceCleaner.retryIntervalMilliSecs), "SeederInstanceCleaner")
		{
			this.m_instances = instances;
		}

		internal SeederInstanceCleaner(SeederInstances instances, int maxDurationMs) : this(instances)
		{
			SeederInstanceCleaner.maxDurationMilliSecs = maxDurationMs;
		}

		protected override void TimerCallbackInternal()
		{
			SeederInstanceContainer[] allInstances = this.m_instances.GetAllInstances();
			foreach (SeederInstanceContainer seederInstanceContainer in allInstances)
			{
				if (base.PrepareToStopCalled)
				{
					return;
				}
				SeederState seedState = seederInstanceContainer.SeedState;
				if (seedState == SeederState.SeedSuccessful || seedState == SeederState.SeedCancelled || seedState == SeederState.SeedFailed)
				{
					DateTime completedTimeUtc = seederInstanceContainer.CompletedTimeUtc;
					long num = (long)Math.Ceiling(DateTime.UtcNow.Subtract(completedTimeUtc).TotalMilliseconds);
					if (num >= (long)SeederInstanceCleaner.maxDurationMilliSecs)
					{
						this.m_instances.RemoveInstance(seederInstanceContainer);
						ExTraceGlobals.SeederServerTracer.TraceDebug<string, SeederState, long>((long)this.GetHashCode(), "SeederInstanceCleaner: Removed stale seed instance '{0}' in state '{1}' of age {2} secs.", seederInstanceContainer.Identity, seedState, num / 1000L);
						ReplayEventLogConstants.Tuple_SeedInstanceCleanupStale.LogEvent(null, new object[]
						{
							seederInstanceContainer.Name,
							num / 1000L
						});
					}
				}
			}
		}

		private static int maxDurationMilliSecs = RegistryParameters.SeederInstanceStaleDuration;

		private static int retryIntervalMilliSecs = Math.Min(SeederInstanceCleaner.maxDurationMilliSecs, 30000);

		private SeederInstances m_instances;
	}
}
