using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Inference.Performance;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.AsyncTask;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.EventLog;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	internal sealed class Pipeline : ContainerComponent, IPipeline, IDocumentProcessor, IStartStop, IDisposable, IDiagnosable, INotifyFailed
	{
		static Pipeline()
		{
			ComponentRegistry.Register<Pipeline>();
		}

		internal Pipeline(PipelineDefinition definition, string instance) : this(definition, instance, null, null)
		{
		}

		internal Pipeline(PipelineDefinition definition, string instance, IPipelineContext context, IPipelineErrorHandler errorHandler)
		{
			Util.ThrowOnNullArgument(definition, "definition");
			Util.ThrowOnNullOrEmptyArgument(instance, "instance");
			base.DiagnosticsSession.ComponentName = instance;
			base.DiagnosticsSession.Tracer = ExTraceGlobals.CorePipelineTracer;
			this.definition = definition;
			this.instanceName = instance;
			this.context = context;
			this.errorHandler = errorHandler;
			this.poisonComponentThreshold = ((this.definition.PoisonComponentThreshold > 0) ? this.definition.PoisonComponentThreshold : Pipeline.DefaultPoisonComponentThreshold);
			this.pipelinePerfCounter = PipelineCounters.GetInstance(this.instanceName);
			this.pipelinePerfCounter.Reset();
		}

		public int MaxConcurrency
		{
			get
			{
				return this.definition.MaxConcurrency;
			}
		}

		public IAsyncResult BeginProcess(IDocument document, AsyncCallback callback, object context)
		{
			base.CheckDisposed();
			Util.ThrowOnNullArgument(document, "document");
			base.DiagnosticsSession.IncrementCounter(this.pipelinePerfCounter.NumberOfIncomingDocuments);
			AsyncResult asyncResult = new AsyncResult(callback, context);
			this.BeginDispatchProcessDocSignal(document, asyncResult, delegate(IAsyncResult ar)
			{
				try
				{
					this.EndDispatchProcessDocSignal(ar);
				}
				catch (ComponentException asCompleted)
				{
					asyncResult.SetAsCompleted(asCompleted);
				}
			}, null);
			return asyncResult;
		}

		public void EndProcess(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			((AsyncResult)asyncResult).End();
		}

		public override string ToString()
		{
			return this.instanceName;
		}

		public void ProcessDocument(IDocument document, object context)
		{
			ComponentException ex = null;
			ComponentException ex2 = null;
			PerfCounterSampleCollector sampleCollector = this.StartProcess(document);
			for (int i = 0; i < this.components.Count; i++)
			{
				if (!this.components.IsPoisonComponent(i))
				{
					IPipelineComponent pipelineComponent = this.components[i];
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, document.Identity, "Processing document through component: {0} ({1})", new object[]
					{
						i,
						pipelineComponent.Name
					});
					PipelineCountersInstance instance = PipelineCounters.GetInstance(this.GetComponentInstanceName(pipelineComponent));
					base.DiagnosticsSession.IncrementCounter(instance.NumberOfIncomingDocuments);
					PerfCounterSampleCollector perfCounterSampleCollector = new PerfCounterSampleCollector(instance, base.DiagnosticsSession);
					perfCounterSampleCollector.Start();
					try
					{
						pipelineComponent.ProcessDocument(document, context);
					}
					catch (ComponentException ex3)
					{
						ex = ex3;
					}
					perfCounterSampleCollector.Stop(ex == null);
					if (!this.ContinueAfterException(document, i, ex, out ex2))
					{
						ex = ex2;
						break;
					}
					ex = null;
				}
			}
			this.CompleteProcess(document, ex, null, sampleCollector);
			if (ex != null)
			{
				throw ex;
			}
		}

		protected override XElement InternalGetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = base.InternalGetDiagnosticInfo(parameters);
			for (int i = 0; i < this.components.Count; i++)
			{
				IPipelineComponent pipelineComponent = this.components[i];
				XElement xelement2 = new XElement(string.Format("Component{0}", i + 1));
				xelement2.Add(new XElement("Name", pipelineComponent.Name));
				xelement2.Add(new XElement("Description", pipelineComponent.Description));
				IDiagnosable diagnosable = pipelineComponent as IDiagnosable;
				if (diagnosable != null)
				{
					xelement2.Add(diagnosable.GetDiagnosticInfo(parameters));
				}
				if (this.nestedPipelines.ContainsKey(i))
				{
					IDiagnosable diagnosable2 = this.nestedPipelines[i];
					xelement2.Add(diagnosable2.GetDiagnosticInfo(parameters));
				}
				xelement.Add(xelement2);
			}
			return xelement;
		}

		protected override void CreateChildren()
		{
			this.CreateComponents();
			this.componentsMonitor = new PipelineComponentMonitor(this.components);
			for (int i = 0; i < this.components.Count; i++)
			{
				string componentInstanceName = this.GetComponentInstanceName(this.components[i]);
				PipelineCounters.ResetInstance(componentInstanceName);
			}
		}

		protected override void PrepareToStartChildrenAsync()
		{
			List<AsyncTask> list = new List<AsyncTask>(this.components.Count);
			if (this.components.HasStartStopComponent)
			{
				for (int i = 0; i < this.components.Count; i++)
				{
					IPipelineComponent pipelineComponent = this.components[i];
					if (this.nestedPipelines.ContainsKey(i))
					{
						IPipeline component = this.nestedPipelines[i];
						base.DiagnosticsSession.TraceDebug<Pipeline>("{0}: Adding AsyncTaskSequence for AsyncPrepareToStart of nested pipeline and the component", this);
						list.Add(new AsyncTaskSequence(new AsyncTask[]
						{
							new AsyncPrepareToStart(component),
							new AsyncPrepareToStart((IStartStopPipelineComponent)pipelineComponent)
						}));
					}
					else if (pipelineComponent is IStartStopPipelineComponent)
					{
						base.DiagnosticsSession.TraceDebug<Pipeline>("{0}: Adding AsyncPrepareToStart for the component that supports start/stop", this);
						list.Add(new AsyncPrepareToStart((IStartStopPipelineComponent)pipelineComponent));
					}
				}
			}
			base.DiagnosticsSession.TraceDebug<Pipeline, int>("{0}: Found {1} nested pipelines", this, this.nestedPipelines.Count);
			base.DiagnosticsSession.TraceDebug<Pipeline, int>("{0}: Found {1} components in this pipeline that support start/stop", this, list.Count);
			list.Add(new AsyncPrepareToStart(this.componentsMonitor));
			base.DiagnosticsSession.TraceDebug<Pipeline>("{0}: Preparing to start nested pipelines, components and monitor in parallel", this);
			new AsyncTaskParallel(list).Execute(delegate(AsyncTask asyncTask)
			{
				base.CompletePrepareToStart(asyncTask.Exception);
			});
		}

		protected override void StartChildrenAsync()
		{
			List<AsyncTask> list = new List<AsyncTask>();
			List<AsyncTask> list2 = new List<AsyncTask>(this.nestedPipelines.Count);
			foreach (IPipeline component in this.nestedPipelines.Values)
			{
				list2.Add(new AsyncStart(component));
			}
			base.DiagnosticsSession.TraceDebug<Pipeline, int>("{0}: Found {1} nested pipelines", this, this.nestedPipelines.Count);
			if (list2.Count > 0)
			{
				list.Add(new AsyncTaskParallel(list2));
			}
			List<AsyncTask> list3 = new List<AsyncTask>(this.components.Count);
			if (this.components.HasStartStopComponent)
			{
				for (int i = 0; i < this.components.Count; i++)
				{
					IPipelineComponent pipelineComponent = this.components[i];
					if (pipelineComponent is IStartStopPipelineComponent)
					{
						list3.Add(new AsyncStart((IStartStopPipelineComponent)pipelineComponent));
					}
				}
			}
			base.DiagnosticsSession.TraceDebug<Pipeline, int>("{0}: Found {1} components in this pipeline that support start/stop", this, list3.Count);
			if (list3.Count > 0)
			{
				list.Add(new AsyncTaskParallel(list3));
			}
			list.Add(new AsyncStart(this.componentsMonitor));
			base.DiagnosticsSession.TraceDebug<Pipeline>("{0}: Starting nested pipelines, components, monitor in sequence.", this);
			new AsyncTaskSequence(list).Execute(delegate(AsyncTask asyncTask)
			{
				base.CompleteStart(asyncTask.Exception);
			});
		}

		protected override void StopChildrenAsync()
		{
			List<AsyncTask> list = new List<AsyncTask>(this.components.Count + this.nestedPipelines.Count);
			if (this.components.HasStartStopComponent)
			{
				for (int i = 0; i < this.components.Count; i++)
				{
					IPipelineComponent pipelineComponent = this.components[i];
					if (pipelineComponent is IStartStopPipelineComponent)
					{
						list.Add(new AsyncStop((IStartStopPipelineComponent)pipelineComponent));
					}
				}
			}
			base.DiagnosticsSession.TraceDebug<Pipeline, int>("{0}: Found {1} components in this pipeline that support start/stop", this, list.Count);
			foreach (IPipeline component in this.nestedPipelines.Values)
			{
				list.Add(new AsyncStop(component));
			}
			base.DiagnosticsSession.TraceDebug<Pipeline, int>("{0}: Found {1} nested pipelines", this, this.nestedPipelines.Count);
			List<AsyncTask> list2 = new List<AsyncTask>();
			list2.Add(new AsyncStop(this.componentsMonitor));
			if (list.Count > 0)
			{
				list2.Add(new AsyncTaskParallel(list));
			}
			base.DiagnosticsSession.TraceDebug<Pipeline>("{0}: Stopping monitor first, and then components and nested pipelines in parallel", this);
			new AsyncTaskSequence(list2).Execute(delegate(AsyncTask asyncTask)
			{
				base.DiagnosticsSession.TraceDebug<Pipeline>("{0}: Pipeline children components are stopped.", this);
				this.BeginDispatchDoneStoppingChildrenSignal(asyncTask.Exception, new AsyncCallback(this.EndDispatchDoneStoppingChildrenSignal), null);
			});
		}

		protected override void AtFail(ComponentFailedException reason)
		{
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Failing due to exception: {0}.", new object[]
			{
				reason
			});
			if (this.outstandingDocuments == 0)
			{
				base.AtFail(reason);
			}
		}

		protected override void DisposeChildren()
		{
			if (this.componentsMonitor != null)
			{
				this.componentsMonitor.Dispose();
				this.componentsMonitor = null;
			}
			if (this.components != null)
			{
				this.components.Dispose();
				this.components = null;
			}
			if (this.nestedPipelines.Count > 0)
			{
				foreach (IPipeline pipeline in this.nestedPipelines.Values)
				{
					pipeline.Dispose();
				}
				this.nestedPipelines.Clear();
			}
			if (this.documentSampleCollectorMap != null)
			{
				this.documentSampleCollectorMap.Clear();
			}
		}

		private void CreateComponents()
		{
			this.components = new PipelineComponentList(this.definition.Components.Length, this.poisonComponentThreshold);
			for (int i = 0; i < this.definition.Components.Length; i++)
			{
				PipelineComponentDefinition componentDefinition = this.definition.Components[i];
				int index = componentDefinition.Order - 1;
				if (componentDefinition.Pipeline != null)
				{
					string nestedPipelineInstanceName = this.GetNestedPipelineInstanceName(componentDefinition.Pipeline);
					base.DiagnosticsSession.TraceDebug<Pipeline, string>("{0}: Creating a nested pipeline of instance name of {1}", this, nestedPipelineInstanceName);
					IPipeline nestedPipeline = new Pipeline(componentDefinition.Pipeline, nestedPipelineInstanceName, this.context, this.errorHandler);
					this.nestedPipelines.Add(index, nestedPipeline);
					this.components.Insert(index, delegate
					{
						this.DiagnosticsSession.TraceDebug<Pipeline, int>("{0}: Creating component of index {1} with the nested pipeline", this, index);
						return componentDefinition.CreateComponent(this.context, nestedPipeline);
					});
				}
				else
				{
					this.components.Insert(index, delegate
					{
						this.DiagnosticsSession.TraceDebug<Pipeline, int>("{0}: Creating component of index {1}", this, index);
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();
						IPipelineComponent result = componentDefinition.CreateComponent(this.context, null);
						stopwatch.Stop();
						this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, "Component {0} creation timespan: {1} ms", new object[]
						{
							componentDefinition.Name,
							stopwatch.ElapsedMilliseconds
						});
						return result;
					});
				}
			}
		}

		private string GetComponentInstanceName(IPipelineComponent component)
		{
			return string.Format("{0} - {1}", this.instanceName, component.Name);
		}

		private string GetNestedPipelineInstanceName(PipelineDefinition nestedPipeline)
		{
			return string.Format("{0} - {1}", this.instanceName, nestedPipeline.Name);
		}

		private PerfCounterSampleCollector StartProcess(IDocument document)
		{
			PerfCounterSampleCollector perfCounterSampleCollector = new PerfCounterSampleCollector(this.pipelinePerfCounter, base.DiagnosticsSession);
			perfCounterSampleCollector.Start();
			base.DiagnosticsSession.TraceDebug<Pipeline, int>("{0}: There are {1} outstanding documents in the pipeline", this, this.outstandingDocuments);
			base.DiagnosticsSession.TraceDebug<Pipeline, IDocument>("{0}: About to process document {1} through the pipeline", this, document);
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, document.Identity, "Beginning to process document", new object[0]);
			Interlocked.Increment(ref this.outstandingDocuments);
			return perfCounterSampleCollector;
		}

		private void AtProcessDoc(IDocument document, AsyncResult asyncResult)
		{
			PerfCounterSampleCollector value = this.StartProcess(document);
			this.documentSampleCollectorMap.Add(document, value);
			this.BeginDispatchProcessDocInComponentSignal(document, 0, asyncResult, new AsyncCallback(this.EndDispatchProcessDocInComponentSignal), null);
		}

		private void AtCannotProcessDoc(IDocument document, AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug<Pipeline, int>("{0}: There are {1} outstanding documents in the pipeline", this, this.outstandingDocuments);
			base.DiagnosticsSession.TraceDebug<Pipeline, IDocument>("{0}: Unable to process document {1} through the pipeline", this, document);
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, document.Identity, "Cannot process document", new object[0]);
			base.DiagnosticsSession.IncrementCounter(this.pipelinePerfCounter.NumberOfRejectedDocuments);
			asyncResult.SetAsCompleted(new CannotProcessDocException());
		}

		private void AtProcessDocInComponent(IDocument document, int index, AsyncResult asyncResult)
		{
			IPipelineComponent component = null;
			if (this.components.IsPoisonComponent(index))
			{
				this.AdvanceToNextComponent(document, index, asyncResult, null);
				return;
			}
			component = this.components[index];
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, document.Identity, "Processing document through component: {0} ({1})", new object[]
			{
				index,
				component.Name
			});
			PipelineCountersInstance instance = PipelineCounters.GetInstance(this.GetComponentInstanceName(component));
			base.DiagnosticsSession.IncrementCounter(instance.NumberOfIncomingDocuments);
			PerfCounterSampleCollector componentSampleCollector = new PerfCounterSampleCollector(instance, base.DiagnosticsSession);
			componentSampleCollector.Start();
			component.BeginProcess(document, delegate(IAsyncResult componentAsyncResult)
			{
				ComponentException ex = null;
				try
				{
					component.EndProcess(componentAsyncResult);
				}
				catch (ComponentException ex2)
				{
					ex = ex2;
				}
				componentSampleCollector.Stop(ex == null);
				this.AdvanceToNextComponent(document, index, asyncResult, ex);
			}, asyncResult.AsyncState);
		}

		private bool ContinueAfterException(IDocument document, int index, ComponentException exception, out ComponentException exceptionToReport)
		{
			bool result = true;
			exceptionToReport = null;
			if (exception != null)
			{
				IPipelineComponent pipelineComponent = this.components[index];
				base.DiagnosticsSession.TraceError("{0}: Exception occurred in component {1} for document {2}: {3}", new object[]
				{
					this,
					index,
					document,
					exception
				});
				base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, document.Identity, "Exception occurred in component {0} ({1}): {2}", new object[]
				{
					index,
					pipelineComponent.Name,
					exception
				});
				DocumentResolution documentResolution = DocumentResolution.CompleteError;
				if (this.errorHandler != null)
				{
					documentResolution = this.errorHandler.HandleException(pipelineComponent, exception);
				}
				switch (documentResolution)
				{
				case DocumentResolution.IgnoreAndContinue:
					base.DiagnosticsSession.TraceDebug<Pipeline, IDocument>("{0}: Ignore and continue processing of document {1} in the pipeline", this, document);
					break;
				case DocumentResolution.CompleteError:
					base.DiagnosticsSession.TraceDebug<Pipeline, IDocument>("{0}: Abort processing of document {1} in the pipeline", this, document);
					result = false;
					exceptionToReport = exception;
					break;
				case DocumentResolution.CompleteSuccess:
					base.DiagnosticsSession.TraceDebug<Pipeline, IDocument>("{0}: Skip processing of document {1} in the pipeline", this, document);
					result = false;
					break;
				case DocumentResolution.PoisonComponentAndContinue:
					base.DiagnosticsSession.TraceDebug<Pipeline, IDocument>("{0}: Ignore and continue processing of document {1} in the pipeline", this, document);
					this.HandlePoisonComponent(document, index, exception);
					break;
				}
			}
			return result;
		}

		private void HandlePoisonComponent(IDocument document, int index, ComponentException exception)
		{
			string name = this.components[index].Name;
			if (this.components.TrackPoisonComponent(index))
			{
				PipelineCountersInstance instance = PipelineCounters.GetInstance(this.instanceName);
				base.DiagnosticsSession.IncrementCounter(instance.NumberOfComponentsPoisoned);
				base.DiagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_ComponentPoisoned, new object[]
				{
					name,
					this.instanceName,
					this.poisonComponentThreshold,
					exception.InnerException
				});
				base.DiagnosticsSession.TraceDebug<Pipeline, IDocument, string>("{0}: Poisoned component {2} during processing of document {1}", this, document, name);
			}
		}

		private void AdvanceToNextComponent(IDocument document, int index, AsyncResult asyncResult, ComponentException exception)
		{
			ComponentException result = null;
			if (this.ContinueAfterException(document, index, exception, out result) && index + 1 < this.components.Count)
			{
				this.BeginDispatchProcessDocInComponentSignal(document, index + 1, asyncResult, new AsyncCallback(this.EndDispatchProcessDocInComponentSignal), null);
				return;
			}
			this.BeginDispatchDoneProcessingDocSignal(document, result, asyncResult, new AsyncCallback(this.EndDispatchDoneProcessingDocSignal), null);
		}

		private void AtDoneProcessingDoc(IDocument document, ComponentException result, AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug<Pipeline, IDocument>("{0}: Complete processing of document {1} in the pipeline", this, document);
			this.CompleteProcess(document, result, asyncResult);
		}

		private void AtCancelledProcessDoc(IDocument document, int index, AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug<Pipeline, IDocument>("{0}: Processing of document {1} is cancelled in stopping", this, document);
			this.CompleteProcess(document, new DocProcessCanceledException(), asyncResult);
			this.SafeStop();
		}

		private void AtDoneProcessingDocInStopping(IDocument document, ComponentException result, AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug<Pipeline, IDocument>("{0}: Complete processing of document {1} in stopping", this, document);
			this.CompleteProcess(document, result, asyncResult);
			this.SafeStop();
		}

		private void AtDoneStoppingChildren(ComponentException result)
		{
			base.DiagnosticsSession.TraceDebug<Pipeline>("{0}: Completes stopping of children components in stopping", this);
			this.stoppingChildrenException = result;
			this.stoppingChildrenIsDone = true;
			this.SafeStop();
		}

		private void AtCancelledProcessDocInFailing(IDocument document, int index, AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug<Pipeline, IDocument>("{0}: Processing of document {1} is cancelled during failing", this, document);
			this.CompleteProcess(document, new DocProcessCanceledException(), asyncResult);
			if (this.outstandingDocuments == 0)
			{
				base.AtFail(base.LastFailedReason);
			}
		}

		private void AtDoneProcessingDocInFailing(IDocument document, ComponentException result, AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug<Pipeline, IDocument>("{0}: Complete processing of document {1} during failing", this, document);
			this.CompleteProcess(document, result, asyncResult);
			if (this.outstandingDocuments == 0)
			{
				base.AtFail(base.LastFailedReason);
			}
		}

		private void CompleteProcess(IDocument document, ComponentException result, AsyncResult asyncResult)
		{
			PerfCounterSampleCollector sampleCollector = this.documentSampleCollectorMap[document];
			this.documentSampleCollectorMap.Remove(document);
			this.CompleteProcess(document, result, asyncResult, sampleCollector);
		}

		private void CompleteProcess(IDocument document, ComponentException result, AsyncResult asyncResult, PerfCounterSampleCollector sampleCollector)
		{
			sampleCollector.Stop(result == null);
			Interlocked.Decrement(ref this.outstandingDocuments);
			base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, document.Identity, "Completed processing of document{0}", new object[]
			{
				(result == null) ? string.Empty : string.Format(" with exception {0}", result)
			});
			base.DiagnosticsSession.TraceDebug<Pipeline, int>("{0}: There are {1} outstanding documents in the pipeline", this, this.outstandingDocuments);
			if (asyncResult != null)
			{
				asyncResult.SetAsCompleted(result);
			}
		}

		private void SafeStop()
		{
			bool flag = true;
			if (this.outstandingDocuments != 0)
			{
				flag = false;
				base.DiagnosticsSession.TraceDebug<Pipeline>("{0}: Waiting for outstanding documents to be processed", this);
			}
			if (!this.stoppingChildrenIsDone)
			{
				flag = false;
				base.DiagnosticsSession.TraceDebug<Pipeline>("{0}: Waiting for stopping of children component to be done", this);
			}
			if (flag)
			{
				base.CompleteStop(this.stoppingChildrenException);
			}
		}

		private void OnComponentsMonitorFailed(object sender, FailedEventArgs e)
		{
			ComponentFailedException reason = e.Reason;
			base.DiagnosticsSession.TraceDebug<Pipeline, ComponentFailedException>("{0}: The components monitor failed to revive component. Exception={1}", this, reason);
			base.BeginDispatchFailSignal(reason, new AsyncCallback(base.EndDispatchFailSignal), null);
		}

		private static void RegisterComponent(ComponentInfo componentInfo)
		{
			componentInfo.RegisterSignal(Pipeline.Signal.ProcessDoc, SignalPriority.Medium);
			componentInfo.RegisterSignal(Pipeline.Signal.ProcessDocInComponent, SignalPriority.Medium);
			componentInfo.RegisterSignal(Pipeline.Signal.DoneProcessingDoc, SignalPriority.Medium);
			componentInfo.RegisterSignal(Pipeline.Signal.DoneStoppingChildren, SignalPriority.Medium);
			componentInfo.RegisterState(Pipeline.State.QueueFull);
			componentInfo.RegisterTransition(6U, 9U, 6U, new ConditionMethod(Pipeline.Condition_InlineCondition__0), new ActionMethod(Pipeline.Transition_AtProcessDoc));
			componentInfo.RegisterTransition(6U, 9U, 10U, new ConditionMethod(Pipeline.Condition_InlineCondition__1), new ActionMethod(Pipeline.Transition_AtProcessDoc));
			componentInfo.RegisterTransition(6U, 10U, 6U, null, new ActionMethod(Pipeline.Transition_AtProcessDocInComponent));
			componentInfo.RegisterTransition(6U, 11U, 6U, null, new ActionMethod(Pipeline.Transition_AtDoneProcessingDoc));
			componentInfo.RegisterTransition(10U, 9U, 10U, null, new ActionMethod(Pipeline.Transition_AtCannotProcessDoc));
			componentInfo.RegisterTransition(10U, 10U, 10U, null, new ActionMethod(Pipeline.Transition_AtProcessDocInComponent));
			componentInfo.RegisterTransition(10U, 11U, 6U, null, new ActionMethod(Pipeline.Transition_AtDoneProcessingDoc));
			componentInfo.RegisterTransition(10U, 5U, 3U, null, new ActionMethod(StartStopComponent.Transition_AtStop));
			componentInfo.RegisterTransition(3U, 9U, 3U, null, new ActionMethod(Pipeline.Transition_AtCannotProcessDoc));
			componentInfo.RegisterTransition(2U, 9U, 2U, null, new ActionMethod(Pipeline.Transition_AtCannotProcessDoc));
			componentInfo.RegisterTransition(3U, 10U, 3U, null, new ActionMethod(Pipeline.Transition_AtCancelledProcessDoc));
			componentInfo.RegisterTransition(3U, 11U, 3U, null, new ActionMethod(Pipeline.Transition_AtDoneProcessingDocInStopping));
			componentInfo.RegisterTransition(9U, 10U, 9U, null, new ActionMethod(Pipeline.Transition_AtCancelledProcessDocInFailing));
			componentInfo.RegisterTransition(9U, 11U, 9U, null, new ActionMethod(Pipeline.Transition_AtDoneProcessingDocInFailing));
			componentInfo.RegisterTransition(3U, 12U, 3U, null, new ActionMethod(Pipeline.Transition_AtDoneStoppingChildren));
		}

		internal static void Transition_AtProcessDoc(object component, params object[] args)
		{
			((Pipeline)component).AtProcessDoc((IDocument)args[0], (AsyncResult)args[1]);
		}

		internal static void Transition_AtProcessDocInComponent(object component, params object[] args)
		{
			((Pipeline)component).AtProcessDocInComponent((IDocument)args[0], (int)args[1], (AsyncResult)args[2]);
		}

		internal static void Transition_AtDoneProcessingDoc(object component, params object[] args)
		{
			((Pipeline)component).AtDoneProcessingDoc((IDocument)args[0], (ComponentException)args[1], (AsyncResult)args[2]);
		}

		internal static void Transition_AtCannotProcessDoc(object component, params object[] args)
		{
			((Pipeline)component).AtCannotProcessDoc((IDocument)args[0], (AsyncResult)args[1]);
		}

		internal static void Transition_AtCancelledProcessDoc(object component, params object[] args)
		{
			((Pipeline)component).AtCancelledProcessDoc((IDocument)args[0], (int)args[1], (AsyncResult)args[2]);
		}

		internal static void Transition_AtDoneProcessingDocInStopping(object component, params object[] args)
		{
			((Pipeline)component).AtDoneProcessingDocInStopping((IDocument)args[0], (ComponentException)args[1], (AsyncResult)args[2]);
		}

		internal static void Transition_AtCancelledProcessDocInFailing(object component, params object[] args)
		{
			((Pipeline)component).AtCancelledProcessDocInFailing((IDocument)args[0], (int)args[1], (AsyncResult)args[2]);
		}

		internal static void Transition_AtDoneProcessingDocInFailing(object component, params object[] args)
		{
			((Pipeline)component).AtDoneProcessingDocInFailing((IDocument)args[0], (ComponentException)args[1], (AsyncResult)args[2]);
		}

		internal static void Transition_AtDoneStoppingChildren(object component, params object[] args)
		{
			((Pipeline)component).AtDoneStoppingChildren((ComponentException)args[0]);
		}

		private static bool Condition_InlineCondition__0(object component)
		{
			return ((Pipeline)component).outstandingDocuments < ((Pipeline)component).definition.MaxConcurrency - 1;
		}

		private static bool Condition_InlineCondition__1(object component)
		{
			return ((Pipeline)component).outstandingDocuments == ((Pipeline)component).definition.MaxConcurrency - 1;
		}

		internal IAsyncResult BeginDispatchProcessDocSignal(IDocument document, AsyncResult asyncResult, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 9U, callback, context, TimeSpan.Zero, new object[]
			{
				document,
				asyncResult
			});
		}

		internal IAsyncResult BeginDispatchProcessDocSignal(IDocument document, AsyncResult asyncResult, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 9U, callback, context, delayInTimespan, new object[]
			{
				document,
				asyncResult
			});
		}

		internal void EndDispatchProcessDocSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		internal IAsyncResult BeginDispatchProcessDocInComponentSignal(IDocument document, int index, AsyncResult asyncResult, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 10U, callback, context, TimeSpan.Zero, new object[]
			{
				document,
				index,
				asyncResult
			});
		}

		internal IAsyncResult BeginDispatchProcessDocInComponentSignal(IDocument document, int index, AsyncResult asyncResult, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 10U, callback, context, delayInTimespan, new object[]
			{
				document,
				index,
				asyncResult
			});
		}

		internal void EndDispatchProcessDocInComponentSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		internal IAsyncResult BeginDispatchDoneProcessingDocSignal(IDocument document, ComponentException result, AsyncResult asyncResult, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 11U, callback, context, TimeSpan.Zero, new object[]
			{
				document,
				result,
				asyncResult
			});
		}

		internal IAsyncResult BeginDispatchDoneProcessingDocSignal(IDocument document, ComponentException result, AsyncResult asyncResult, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 11U, callback, context, delayInTimespan, new object[]
			{
				document,
				result,
				asyncResult
			});
		}

		internal void EndDispatchDoneProcessingDocSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		internal IAsyncResult BeginDispatchDoneStoppingChildrenSignal(ComponentException result, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 12U, callback, context, TimeSpan.Zero, new object[]
			{
				result
			});
		}

		internal IAsyncResult BeginDispatchDoneStoppingChildrenSignal(ComponentException result, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 12U, callback, context, delayInTimespan, new object[]
			{
				result
			});
		}

		internal void EndDispatchDoneStoppingChildrenSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		private static readonly int DefaultPoisonComponentThreshold = 3;

		private readonly PipelineDefinition definition;

		private readonly IPipelineContext context;

		private readonly IPipelineErrorHandler errorHandler;

		private readonly string instanceName;

		private readonly int poisonComponentThreshold;

		private readonly PipelineCountersInstance pipelinePerfCounter;

		private readonly Dictionary<IDocument, PerfCounterSampleCollector> documentSampleCollectorMap = new Dictionary<IDocument, PerfCounterSampleCollector>();

		private readonly SortedList<int, IPipeline> nestedPipelines = new SortedList<int, IPipeline>();

		private PipelineComponentList components;

		private PipelineComponentMonitor componentsMonitor;

		private int outstandingDocuments;

		private bool stoppingChildrenIsDone;

		private ComponentException stoppingChildrenException;

		internal new enum Signal : uint
		{
			ProcessDoc = 9U,
			ProcessDocInComponent,
			DoneProcessingDoc,
			DoneStoppingChildren,
			Max
		}

		internal new enum State : uint
		{
			QueueFull = 10U,
			Max
		}
	}
}
