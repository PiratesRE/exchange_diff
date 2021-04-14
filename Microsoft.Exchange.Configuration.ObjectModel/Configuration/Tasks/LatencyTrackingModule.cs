using System;
using Microsoft.Exchange.Diagnostics.CmdletInfra;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class LatencyTrackingModule : ITaskModule, ICriticalFeature
	{
		public LatencyTrackingModule(TaskContext context)
		{
			this.context = context;
			CmdletLatencyTracker.StartInternalTracking(context.UniqueId, "ParameterBinding");
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		void ITaskModule.Init(ITaskEvent task)
		{
			task.PreInit += this.OnPreInit;
			task.InitCompleted += this.OnInitCompleted;
			task.PreIterate += this.OnPreIterate;
			task.IterateCompleted += this.OnIterateCompleted;
			task.PreRelease += this.OnPreRelease;
			task.Release += this.OnRelease;
			task.PreStop += this.OnPreStop;
			task.Stop += this.OnStop;
		}

		void ITaskModule.Dispose()
		{
			CmdletLatencyTracker.DisposeLatencyTracker(this.context.UniqueId);
		}

		private void OnPreInit(object sender, EventArgs eventArgs)
		{
			Guid uniqueId = this.context.UniqueId;
			CmdletLatencyTracker.EndInternalTracking(uniqueId, "ParameterBinding");
			CmdletLatencyTracker.StartInternalTracking(this.context.UniqueId, "BeginProcessing");
		}

		private void OnInitCompleted(object sender, EventArgs eventArgs)
		{
			CmdletLatencyTracker.EndInternalTracking(this.context.UniqueId, "BeginProcessing");
		}

		private void OnPreIterate(object sender, EventArgs eventArgs)
		{
			CmdletLatencyTracker.StartInternalTracking(this.context.UniqueId, "ProcessRecord");
		}

		private void OnIterateCompleted(object sender, EventArgs eventArgs)
		{
			CmdletLatencyTracker.EndInternalTracking(this.context.UniqueId, "ProcessRecord");
		}

		private void OnPreRelease(object sender, EventArgs eventArgs)
		{
			CmdletLatencyTracker.StartInternalTracking(this.context.UniqueId, "EndProcessing");
		}

		private void OnRelease(object sender, EventArgs eventArgs)
		{
			CmdletLatencyTracker.EndInternalTracking(this.context.UniqueId, "EndProcessing");
		}

		private void OnPreStop(object sender, EventArgs eventArgs)
		{
			CmdletLatencyTracker.StartInternalTracking(this.context.UniqueId, "StopProcessing");
		}

		private void OnStop(object sender, EventArgs eventArgs)
		{
			CmdletLatencyTracker.EndInternalTracking(this.context.UniqueId, "StopProcessing");
		}

		private readonly TaskContext context;
	}
}
