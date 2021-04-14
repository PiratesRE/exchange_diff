using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public class Worker
	{
		public Worker(WorkBroker[] brokers) : this(brokers, null)
		{
		}

		public Worker(WorkBroker[] brokers, Action onStopNotification) : this(brokers, onStopNotification, false)
		{
		}

		public Worker(WorkBroker[] brokers, Action onStopNotification, bool perfCountersExist)
		{
			this.traceContext = new TracingContext(null)
			{
				LId = this.GetHashCode(),
				Id = base.GetType().GetHashCode()
			};
			this.controller = new Controller(brokers, this.traceContext, perfCountersExist);
			this.controllerExitCallback = onStopNotification;
			this.cancellationSource = new CancellationTokenSource();
			TaskScheduler.UnobservedTaskException += delegate(object sender, UnobservedTaskExceptionEventArgs eventArgs)
			{
				eventArgs.SetObserved();
				eventArgs.Exception.Handle(delegate(Exception ex)
				{
					WTFDiagnostics.TraceDebug<Exception>(WTFLog.Core, this.traceContext, "[Worker.Worker]: unobserved exception encountered.\n{0}", ex, null, ".ctor", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 110);
					return true;
				});
			};
			this.packageSets = new BlockingCollection<string>[brokers.Count<WorkBroker>()];
			for (int i = 0; i < brokers.Count<WorkBroker>(); i++)
			{
				this.packageSets[i] = brokers[i].AsyncGetWorkItemPackages(this.cancellationSource.Token);
			}
			WTFDiagnostics.TraceInformation<Type>(WTFLog.Core, this.traceContext, "[Worker.Worker]: {0} created.", base.GetType(), null, ".ctor", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 121);
		}

		public RestartRequest RestartRequest
		{
			get
			{
				return this.controller.RestartRequest;
			}
		}

		public List<string> GetWorkItemPackages()
		{
			List<string> list = new List<string>();
			int millisecondsTimeout = 50;
			int num = this.packageSets.Count<BlockingCollection<string>>();
			int i = 0;
			while (i < num)
			{
				Thread.Sleep(millisecondsTimeout);
				if (this.controller.RestartRequest != null)
				{
					WTFDiagnostics.TraceError(WTFLog.Core, this.traceContext, "[Worker.GetWorkItemPackages]: Restart was requested before work item packages could be discovered.", null, "GetWorkItemPackages", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 159);
					break;
				}
				string item;
				if (BlockingCollection<string>.TryTakeFromAny(this.packageSets, out item, 15) != -1)
				{
					list.Add(item);
					i++;
				}
			}
			return list;
		}

		public void Start()
		{
			if (this.cancellationSource.IsCancellationRequested)
			{
				this.cancellationSource.Dispose();
				this.cancellationSource = new CancellationTokenSource();
			}
			Task task = Task.Factory.StartNew(delegate()
			{
				WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Worker.Start]: Starting Controller.", null, "Start", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 188);
				this.controller.QueueWork(this.cancellationSource.Token);
			}, this.cancellationSource.Token, TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning, TaskScheduler.Current);
			this.workTask = task.ContinueWith(delegate(Task t)
			{
				if (this.cancellationSource.IsCancellationRequested)
				{
					WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Worker.Start]: we come here because of cancellation (request from host process).", null, "Start", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 202);
					if (this.controller.RestartRequest != null)
					{
						WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Worker.Start]: clearing pending controller restart request.", null, "Start", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 209);
						this.controller.RestartRequest = null;
					}
					if (t.Exception != null)
					{
						WTFDiagnostics.TraceInformation<AggregateException>(WTFLog.Core, this.traceContext, "[Worker.Start]: controller task is concelled but finished with exception: {0}", t.Exception, null, "Start", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 215);
					}
				}
				else if (this.controller.RestartRequest == null)
				{
					WTFDiagnostics.TraceError<AggregateException>(WTFLog.Core, this.traceContext, "[Worker.Start]: controller task finished with exception: {0}", t.Exception, null, "Start", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 223);
					this.controller.RestartRequest = RestartRequest.CreateUnknownRestartRequest(t.Exception);
				}
				else
				{
					WTFDiagnostics.TraceError<RestartRequest>(WTFLog.Core, this.traceContext, "[Worker.Start]: controller task finished with RestartRequest: {0}", this.controller.RestartRequest, null, "Start", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 228);
				}
				if (this.controllerExitCallback != null)
				{
					WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Worker.Start]: Starting controllerExitCallback.", null, "Start", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 233);
					Task.Factory.StartNew(delegate()
					{
						this.controllerExitCallback();
					});
				}
			});
		}

		public void Stop()
		{
			WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Worker.Stop]: Cancelling controller execution loop.", null, "Stop", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 246);
			this.cancellationSource.Cancel();
		}

		public void Wait()
		{
			WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Worker.Wait]: Waiting for Controller to stop.", null, "Wait", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 256);
			if (this.workTask != null)
			{
				this.workTask.Wait();
				return;
			}
			WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Worker.Wait]: Controller is not started yet. No action is required.", null, "Wait", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 264);
		}

		public void WaitWithRestartSLA(TimeSpan waitSpan)
		{
			WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Worker.Wait]: Waiting for Controller to stop.", null, "WaitWithRestartSLA", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 281);
			if (this.workTask != null)
			{
				this.controller.ControllerExitingEvent.WaitOne();
				this.workTask.Wait(waitSpan);
				return;
			}
			WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Worker.Wait]: Controller is not started yet. No action is required.", null, "WaitWithRestartSLA", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Worker.cs", 291);
		}

		private Controller controller;

		private CancellationTokenSource cancellationSource;

		private Task workTask;

		private TracingContext traceContext;

		private BlockingCollection<string>[] packageSets;

		private Action controllerExitCallback;
	}
}
