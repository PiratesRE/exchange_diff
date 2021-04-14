using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public abstract class OverallConsecutiveFailuresMonitor : MonitorWorkItem
	{
		protected abstract bool ShouldAlert();

		protected abstract bool HaveInsufficientSamples();

		protected abstract Task SetConsecutiveFailureNumbers(CancellationToken cancellationToken);

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			this.SetConsecutiveFailureNumbers(cancellationToken).ContinueWith(delegate(Task t)
			{
				this.HandleInsufficientSamples(new Func<bool>(this.HaveInsufficientSamples), cancellationToken);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current).ContinueWith(delegate(Task t)
			{
				if (!base.Result.IsAlert)
				{
					base.Result.IsAlert = this.ShouldAlert();
				}
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallConsecutiveFailuresMonitor: Finished analyzing probe results.", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallConsecutiveFailuresMonitor.cs", 71);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current);
		}

		protected virtual void SetStateAttribute6ForScopeMonitoring(double counter)
		{
			if (!string.IsNullOrWhiteSpace(base.Definition.TargetScopes))
			{
				if (!double.IsNaN(counter) || counter < 0.0)
				{
					string paramName = "OverallConsecutiveFailuresMonitor: Counter passed to the scope monitor is invalid";
					throw new ArgumentNullException(paramName);
				}
				base.Result.StateAttribute6 = counter;
			}
		}
	}
}
