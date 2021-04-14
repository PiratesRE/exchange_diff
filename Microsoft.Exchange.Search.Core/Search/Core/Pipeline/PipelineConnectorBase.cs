using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.AsyncTask;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	internal abstract class PipelineConnectorBase : ContainerComponent, IPipelineConnector, IStartStop, IDisposable, IDiagnosable
	{
		protected PipelineConnectorBase()
		{
			base.DiagnosticsSession.ComponentName = "PipelineConnectorBase";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.CorePipelineTracer;
		}

		protected abstract void CreateFeeding(IPipeline pipeline);

		protected abstract void DisposeFeeding();

		protected abstract AsyncTask PrepareFeeding(IPipeline pipeline);

		protected abstract AsyncTask StartFeeding();

		protected abstract AsyncTask StopFeeding();

		protected abstract Pipeline CreatePipeline();

		protected sealed override void CreateChildren()
		{
			base.DiagnosticsSession.TraceDebug("Creating pipeline and feeding.", new object[0]);
			this.pipeline = this.CreatePipeline();
			base.AddComponent(this.pipeline);
			this.CreateFeeding(this.pipeline);
		}

		protected sealed override void PrepareToStartChildrenAsync()
		{
			AsyncTask asyncTask2 = this.PrepareFeeding(this.pipeline);
			base.DiagnosticsSession.TraceDebug("Preparing to start pipeline and feeding in parallel.", new object[0]);
			new AsyncTaskParallel(new AsyncTask[]
			{
				new AsyncPrepareToStart(this.pipeline),
				asyncTask2
			}).Execute(delegate(AsyncTask asyncTask)
			{
				base.CompletePrepareToStart(asyncTask.Exception);
			});
		}

		protected sealed override void StartChildrenAsync()
		{
			this.pipeline.Failed += this.OnPipelineFailed;
			AsyncTask asyncTask2 = this.StartFeeding();
			base.DiagnosticsSession.TraceDebug("Starting the pipeline and feeding in sequence.", new object[0]);
			new AsyncTaskSequence(new AsyncTask[]
			{
				new AsyncStart(this.pipeline),
				asyncTask2
			}).Execute(delegate(AsyncTask asyncTask)
			{
				base.CompleteStart(asyncTask.Exception);
			});
		}

		protected sealed override void StopChildrenAsync()
		{
			if (this.pipeline != null)
			{
				this.pipeline.Failed -= this.OnPipelineFailed;
				AsyncTask asyncTask2 = this.StopFeeding();
				base.DiagnosticsSession.TraceDebug("Stopping the pipeline and feeding in parallel.", new object[0]);
				new AsyncTaskParallel(new AsyncTask[]
				{
					new AsyncStop(this.pipeline),
					asyncTask2
				}).Execute(delegate(AsyncTask asyncTask)
				{
					base.CompleteStop(asyncTask.Exception);
				});
				return;
			}
			base.CompleteStop(null);
		}

		protected override void AtFail(ComponentFailedException reason)
		{
			base.DiagnosticsSession.TraceDebug("Pipeline connector is failing and is stopping feeding.", new object[0]);
			this.pipeline.Failed -= this.OnPipelineFailed;
			this.StopFeeding().Execute(new TaskCompleteCallback(this.AtFailCallback));
		}

		protected sealed override void DisposeChildren()
		{
			if (this.pipeline != null)
			{
				base.RemoveComponent(this.pipeline);
				this.pipeline.Dispose();
				this.pipeline = null;
			}
			this.DisposeFeeding();
		}

		private void AtFailCallback(AsyncTask asyncTask)
		{
			base.DiagnosticsSession.TraceDebug("Pipeline connector is failed.", new object[0]);
			base.AtFail(base.LastFailedReason);
		}

		private void OnPipelineFailed(object sender, FailedEventArgs e)
		{
			ComponentFailedPermanentException ex = (ComponentFailedPermanentException)e.Reason;
			base.DiagnosticsSession.TraceDebug<ComponentFailedPermanentException>("The pipeline failed due to error: {0}", ex);
			base.BeginDispatchFailSignal(ex, new AsyncCallback(base.EndDispatchFailSignal), null);
		}

		private Pipeline pipeline;
	}
}
