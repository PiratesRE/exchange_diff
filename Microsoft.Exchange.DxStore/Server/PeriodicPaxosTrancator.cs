using System;
using FUSE.Paxos;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.DxStore.Server
{
	public class PeriodicPaxosTrancator
	{
		public PeriodicPaxosTrancator(DxStoreInstance instance)
		{
			this.instance = instance;
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.TruncatorTracer;
			}
		}

		public bool IsStarted
		{
			get
			{
				bool result;
				lock (this.locker)
				{
					result = (this.timer != null);
				}
				return result;
			}
		}

		public void Start()
		{
			if (this.instance.IsStopping)
			{
				return;
			}
			lock (this.locker)
			{
				if (this.timer != null)
				{
					this.timer.Dispose(true);
					this.timer = null;
				}
				PeriodicPaxosTrancator.Tracer.TraceDebug<string>((long)this.instance.IdentityHash, "{0}: Starting truncator timer", this.instance.GroupConfig.Identity);
				this.timer = new GuardedTimer(delegate(object unused)
				{
					this.TruncateCallback();
				}, null, TimeSpan.Zero, this.instance.GroupConfig.Settings.TruncationPeriodicCheckInterval);
			}
		}

		public void TruncateIfRequired()
		{
			lock (this.locker)
			{
				if (this.instance.StateMachine != null)
				{
					IStorage<string, DxStoreCommand> storage = this.instance.StateMachine.Paxos.Storage;
					if (storage != null)
					{
						int countExecuted = this.instance.StateMachine.CountExecuted;
						int countTruncated = storage.CountTruncated;
						int num = countExecuted - countTruncated;
						PeriodicPaxosTrancator.Tracer.TraceDebug((long)this.instance.IdentityHash, "{0}: CountExecuted: {1} CountTruncated: {2} Limit: {3} PaddingLength: {4}", new object[]
						{
							this.instance.GroupConfig.Identity,
							countExecuted,
							countTruncated,
							this.instance.GroupConfig.Settings.TruncationLimit,
							this.instance.GroupConfig.Settings.TruncationPaddingLength
						});
						if (num > this.instance.GroupConfig.Settings.TruncationLimit + this.instance.GroupConfig.Settings.TruncationPaddingLength)
						{
							int num2 = countExecuted - this.instance.GroupConfig.Settings.TruncationLimit;
							PeriodicPaxosTrancator.Tracer.TraceInformation<string, int, int>(0, (long)this.instance.IdentityHash, "{0}: Starting to truncate upto {1} (Diff: {2})", this.instance.GroupConfig.Identity, num2, num);
							storage.Truncate(num2);
							PeriodicPaxosTrancator.Tracer.TraceInformation<string, int>(0, (long)this.instance.IdentityHash, "{0}: Finished truncating upto {1}", this.instance.GroupConfig.Identity, num2);
						}
					}
					else
					{
						PeriodicPaxosTrancator.Tracer.TraceWarning<string>((long)this.instance.IdentityHash, "{0}: Skipped truncation checks since storage is not initialized yet", this.instance.GroupConfig.Identity);
					}
				}
				else
				{
					PeriodicPaxosTrancator.Tracer.TraceWarning<string>((long)this.instance.IdentityHash, "{0}: Skipped truncation checks since state machine is not initialized yet", this.instance.GroupConfig.Identity);
				}
			}
		}

		public void Stop()
		{
			lock (this.locker)
			{
				if (this.timer != null)
				{
					PeriodicPaxosTrancator.Tracer.TraceDebug<string>((long)this.instance.IdentityHash, "{0}: Stopping truncator timer (disposing)", this.instance.GroupConfig.Identity);
					this.timer.Dispose(true);
					this.timer = null;
				}
			}
		}

		private void TruncateCallback()
		{
			if (!this.instance.IsStopping)
			{
				this.instance.RunBestEffortOperation("TruncateIfRequired", delegate
				{
					this.TruncateIfRequired();
				}, LogOptions.LogException, null, null, null, null);
			}
		}

		private readonly object locker = new object();

		private readonly DxStoreInstance instance;

		private GuardedTimer timer;
	}
}
