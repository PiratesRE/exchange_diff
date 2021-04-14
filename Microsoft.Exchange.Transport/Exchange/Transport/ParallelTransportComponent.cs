using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal sealed class ParallelTransportComponent : CompositeTransportComponent
	{
		public ParallelTransportComponent(string description) : base(description)
		{
		}

		public override void Load()
		{
			List<ParallelTransportComponent.TransportComponentStartContext> list = new List<ParallelTransportComponent.TransportComponentStartContext>();
			this.componentsLoading = base.TransportComponents.Count;
			this.finishedLoading = new AutoResetEvent(false);
			foreach (ITransportComponent component in base.TransportComponents)
			{
				ParallelTransportComponent.TransportComponentStartContext transportComponentStartContext = new ParallelTransportComponent.TransportComponentStartContext(component);
				list.Add(transportComponentStartContext);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnLoadChild), transportComponentStartContext);
			}
			try
			{
				this.finishedLoading.WaitOne();
			}
			catch (ThreadInterruptedException inner)
			{
				throw new TransportComponentLoadFailedException("Failed loading component", inner);
			}
			foreach (ParallelTransportComponent.TransportComponentStartContext transportComponentStartContext2 in list)
			{
				if (transportComponentStartContext2.FailureException != null)
				{
					for (int i = list.Count - 1; i >= 0; i--)
					{
						if (list[i].FailureException == null)
						{
							ITransportComponent component2 = list[i].Component;
							CompositeTransportComponent.UnRegisterForDiagnostics(component2);
							component2.Unload();
						}
					}
					string message = Strings.TransportComponentLoadFailedWithName(transportComponentStartContext2.Component.GetType().Name);
					throw new TransportComponentLoadFailedException(message, transportComponentStartContext2.FailureException);
				}
			}
		}

		private void OnLoadChild(object state)
		{
			ParallelTransportComponent.TransportComponentStartContext childContext = (ParallelTransportComponent.TransportComponentStartContext)state;
			try
			{
				ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Loading component {0}.", childContext.Component.GetType().Name);
				childContext.SetStartTime();
				base.BeginTiming(CompositeTransportComponent.Operation.Load, childContext.Component);
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					childContext.Component.Load();
				}, 1);
				if (!adoperationResult.Succeeded)
				{
					throw new TransportComponentLoadFailedException(Strings.ReadingADConfigFailed, adoperationResult.Exception);
				}
				base.EndTiming(CompositeTransportComponent.Operation.Load, childContext.Component);
				childContext.SetEndTime();
				CompositeTransportComponent.RegisterForDiagnostics(childContext.Component);
				ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Loaded component {0}.", childContext.Component.GetType().Name);
			}
			catch (TransportComponentLoadFailedException ex)
			{
				ExTraceGlobals.GeneralTracer.TraceDebug<string, string>(0L, "Failed loading component {0}. {1}", childContext.Component.GetType().Name, ex.Message);
				childContext.Failed(ex);
			}
			if (Interlocked.Decrement(ref this.componentsLoading) == 0)
			{
				this.finishedLoading.Set();
			}
		}

		private int componentsLoading;

		private AutoResetEvent finishedLoading;

		private class TransportComponentStartContext
		{
			public TransportComponentStartContext(ITransportComponent component)
			{
				this.component = component;
			}

			public Exception FailureException
			{
				get
				{
					return this.failureException;
				}
			}

			public TimeSpan TimeElapsed
			{
				get
				{
					return this.stepStopwatch.Elapsed;
				}
			}

			public ITransportComponent Component
			{
				get
				{
					return this.component;
				}
			}

			public void Failed(Exception failureException)
			{
				this.failureException = failureException;
			}

			public void SetStartTime()
			{
				this.stepStopwatch.Start();
			}

			public void SetEndTime()
			{
				this.stepStopwatch.Stop();
			}

			private ITransportComponent component;

			private Exception failureException;

			private Stopwatch stepStopwatch = new Stopwatch();
		}
	}
}
