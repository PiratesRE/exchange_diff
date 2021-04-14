using System;
using System.Threading;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class Task<T> : Task
	{
		public Task(Task<T>.TaskCallback callback, T context, bool autoStart) : this(callback, context, ThreadPriority.Normal, 0, (TaskFlags)(2 | (autoStart ? 1 : 0)))
		{
		}

		public Task(Task<T>.TaskCallback callback, T context, ThreadPriority priority, int stackSizeInKilobytes, TaskFlags taskFlags)
		{
			this.callback = callback;
			this.context = context;
			this.priority = priority;
			this.stackSizeInKilobytes = stackSizeInKilobytes;
			this.taskFlags = taskFlags;
			this.taskComplete = new ManualResetEvent(true);
			this.state = Task<T>.TaskState.Ready;
			if ((byte)(taskFlags & TaskFlags.AutoStart) != 0)
			{
				this.Start();
			}
		}

		protected Task<T>.TaskCallback CallbackDelegate
		{
			get
			{
				return this.callback;
			}
			set
			{
				this.callback = value;
			}
		}

		protected T Context
		{
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
			}
		}

		protected object StateLock
		{
			get
			{
				return this.stateLockObject;
			}
		}

		internal uint ExecutionCount
		{
			get
			{
				return this.executionCount;
			}
		}

		protected Task<T>.TaskState State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
				switch (value)
				{
				case Task<T>.TaskState.Ready:
					this.taskComplete.Set();
					return;
				case Task<T>.TaskState.Starting:
					this.taskComplete.Reset();
					return;
				case Task<T>.TaskState.Running:
					this.taskComplete.Reset();
					return;
				case Task<T>.TaskState.StopRequested:
				case Task<T>.TaskState.DisposingStopRequested:
					this.taskComplete.Reset();
					return;
				case Task<T>.TaskState.Complete:
				case Task<T>.TaskState.DisposingComplete:
					this.taskComplete.Set();
					return;
				default:
					return;
				}
			}
		}

		public override void Start()
		{
			base.CheckDisposed();
			FaultInjection.InjectFault(this.StartLockEnterTestHook);
			using (LockManager.Lock(this.StateLock))
			{
				FaultInjection.InjectFault(this.StartLockEnteredTestHook);
				if ((this.State == Task<T>.TaskState.Ready || this.State == Task<T>.TaskState.Complete) && (!this.RunOnceOnly() || this.executionCount == 0U))
				{
					this.State = Task<T>.TaskState.Starting;
					if ((byte)(this.taskFlags & TaskFlags.UseThreadPoolThread) != 0)
					{
						WaitCallback callBack = delegate(object stateParameter)
						{
							this.Worker();
						};
						if (!ThreadPool.QueueUserWorkItem(callBack))
						{
							this.State = Task<T>.TaskState.Ready;
						}
					}
					else
					{
						ThreadStart start = new ThreadStart(this.Worker);
						Thread thread;
						if (this.stackSizeInKilobytes == 0)
						{
							thread = new Thread(start);
						}
						else
						{
							thread = new Thread(start, this.stackSizeInKilobytes * 1024);
						}
						thread.Start();
					}
				}
			}
		}

		public override void Stop()
		{
			base.CheckDisposed();
			FaultInjection.InjectFault(this.StopLockEnterTestHook);
			using (LockManager.Lock(this.StateLock))
			{
				FaultInjection.InjectFault(this.StopLockEnteredTestHook);
				if (this.State == Task<T>.TaskState.Starting || this.State == Task<T>.TaskState.Running)
				{
					this.State = Task<T>.TaskState.StopRequested;
				}
			}
		}

		public bool RunOnceOnly()
		{
			return (byte)(this.taskFlags & TaskFlags.RunOnceOnly) == 4;
		}

		public override bool WaitForCompletion(TimeSpan delay)
		{
			base.CheckDisposed();
			return this.taskComplete == null || this.taskComplete.WaitOne(delay);
		}

		public override bool Finished()
		{
			return this.WaitForCompletion(Task.NoDelay);
		}

		public bool ShouldCallbackContinue()
		{
			return this.ShouldCallbackContinueImplementation();
		}

		protected virtual bool ShouldCallbackContinueImplementation()
		{
			base.CheckDisposed();
			bool result;
			using (LockManager.Lock(this.StateLock))
			{
				result = (this.State == Task<T>.TaskState.Starting || this.State == Task<T>.TaskState.Running);
			}
			return result;
		}

		protected virtual void Invoke()
		{
			LockManager.AssertNoLocksHeld();
			if (!Task.testDisabledInvoke)
			{
				Interlocked.Increment(ref Task.invokeCount);
				try
				{
					using (this.executionDiagnosticsContext.NewDiagnosticsScope())
					{
						FaultInjection.InjectFault(Task.invokeTestHook);
						FaultInjection.InjectFault(this.InvokeLock1EnterTestHook);
						using (LockManager.Lock(this.StateLock))
						{
							FaultInjection.InjectFault(this.InvokeLock1EnteredTestHook);
							switch (this.State)
							{
							case Task<T>.TaskState.Starting:
								this.State = Task<T>.TaskState.Running;
								goto IL_AD;
							case Task<T>.TaskState.StopRequested:
								this.State = Task<T>.TaskState.Complete;
								break;
							case Task<T>.TaskState.DisposingStopRequested:
								this.State = Task<T>.TaskState.DisposingComplete;
								break;
							}
							return;
						}
						IL_AD:
						base.CheckDisposed();
						Thread currentThread = Thread.CurrentThread;
						ThreadPriority threadPriority = currentThread.Priority;
						try
						{
							if (currentThread.Priority != this.priority)
							{
								currentThread.Priority = this.priority;
							}
						}
						catch (ThreadStateException exception)
						{
							NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
						}
						this.executionCount += 1U;
						try
						{
							using (ThreadManager.NewMethodFrame(this.CallbackDelegate))
							{
								this.CallbackDelegate(this.executionDiagnosticsContext, this.context, new Func<bool>(this.ShouldCallbackContinue));
							}
						}
						finally
						{
							try
							{
								if (currentThread.Priority != threadPriority)
								{
									currentThread.Priority = threadPriority;
								}
							}
							catch (ThreadStateException exception2)
							{
								NullExecutionDiagnostics.Instance.OnExceptionCatch(exception2);
							}
							FaultInjection.InjectFault(this.InvokeLock2EnterTestHook);
							using (LockManager.Lock(this.StateLock))
							{
								FaultInjection.InjectFault(this.InvokeLock2EnteredTestHook);
								switch (this.State)
								{
								case Task<T>.TaskState.Running:
								case Task<T>.TaskState.StopRequested:
									this.State = Task<T>.TaskState.Complete;
									break;
								case Task<T>.TaskState.DisposingStopRequested:
									this.State = Task<T>.TaskState.DisposingComplete;
									break;
								}
							}
						}
					}
				}
				finally
				{
					Interlocked.Decrement(ref Task.invokeCount);
					LockManager.AssertNoLocksHeld();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Task<T>>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				FaultInjection.InjectFault(this.DisposeLock1EnterTestHook);
				using (LockManager.Lock(this.StateLock))
				{
					FaultInjection.InjectFault(this.DisposeLock1EnteredTestHook);
					Task<T>.TaskState taskState = this.State;
					if (taskState != Task<T>.TaskState.Ready)
					{
						switch (taskState)
						{
						case Task<T>.TaskState.Complete:
							break;
						case Task<T>.TaskState.DisposingComplete:
							goto IL_55;
						default:
							this.State = Task<T>.TaskState.DisposingStopRequested;
							goto IL_55;
						}
					}
					this.State = Task<T>.TaskState.DisposingComplete;
					IL_55:;
				}
				base.WaitForCompletion();
				FaultInjection.InjectFault(this.DisposeLock2EnterTestHook);
				using (LockManager.Lock(this.StateLock))
				{
					FaultInjection.InjectFault(this.DisposeLock2EnteredTestHook);
					if (this.taskComplete != null)
					{
						this.taskComplete.Dispose();
						this.taskComplete = null;
					}
				}
			}
		}

		protected void Worker()
		{
			WatsonOnUnhandledException.Guard(this.executionDiagnosticsContext, new TryDelegate(this, (UIntPtr)ldftn(<Worker>b__2)));
		}

		private Task<T>.TaskState state;

		private TaskFlags taskFlags;

		private ManualResetEvent taskComplete;

		private T context;

		private Task<T>.TaskCallback callback;

		private ThreadPriority priority;

		private int stackSizeInKilobytes;

		private object stateLockObject = new object();

		private uint executionCount;

		private TaskExecutionDiagnosticsProxy executionDiagnosticsContext = new TaskExecutionDiagnosticsProxy();

		public delegate void Callback(T context, Func<bool> shouldCallbackContinue);

		public delegate void TaskCallback(TaskExecutionDiagnosticsProxy diagnosticsContext, T context, Func<bool> shouldCallbackContinue);

		public enum TaskState
		{
			Ready,
			Starting,
			Running,
			StopRequested,
			DisposingStopRequested,
			Complete,
			DisposingComplete
		}
	}
}
