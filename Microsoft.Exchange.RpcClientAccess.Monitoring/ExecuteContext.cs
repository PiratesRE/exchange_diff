using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExecuteContext : BaseObject
	{
		public ExecuteContext(ITask task, ExecuteAsyncResult executeAsyncResult)
		{
			this.executeAsyncResult = executeAsyncResult;
			this.Push(task);
			this.taskResult = TaskResult.Undefined;
		}

		private ExecuteContext.TaskStateMachine Top
		{
			get
			{
				if (this.taskStack.Count <= 0)
				{
					return null;
				}
				return this.taskStack.Peek();
			}
		}

		public void Begin()
		{
			this.Resume();
		}

		public TaskResult End()
		{
			this.executeAsyncResult.WaitForCompletion();
			if (this.exception != null)
			{
				throw new TaskException(this.exception);
			}
			return this.taskResult;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ExecuteContext>(this);
		}

		protected override void InternalDispose()
		{
			foreach (ExecuteContext.TaskStateMachine taskStateMachine in this.taskStack)
			{
				taskStateMachine.Dispose();
			}
			base.InternalDispose();
		}

		private void Resume()
		{
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				((ExecuteContext)state).Process();
			}, this);
		}

		private void Process()
		{
			lock (this.processLock)
			{
				if (this.isFinished)
				{
					return;
				}
				if (this.isProcessing)
				{
					this.continueProcessing = true;
					return;
				}
				this.isProcessing = true;
				goto IL_49;
			}
			try
			{
				for (;;)
				{
					IL_49:
					ExecuteContext.TaskStateMachine top = this.Top;
					while (top != null)
					{
						bool flag2 = false;
						ITask task = top.Step();
						if (task != null)
						{
							if (!object.ReferenceEquals(task, top.Task))
							{
								this.Push(task);
								top = this.Top;
							}
							else
							{
								flag2 = true;
							}
						}
						else
						{
							this.Pop(top);
							top = this.Top;
						}
						if (flag2)
						{
							break;
						}
					}
					lock (this.processLock)
					{
						this.isProcessing = false;
						if (top == null)
						{
							this.isFinished = true;
						}
						else if (this.continueProcessing)
						{
							this.isProcessing = true;
							this.continueProcessing = false;
							continue;
						}
					}
					break;
				}
			}
			catch (Exception ex)
			{
				this.exception = ex;
				this.isFinished = true;
			}
			if (this.isFinished)
			{
				this.executeAsyncResult.InvokeCallback();
			}
		}

		private void Push(ITask uninitializedTask)
		{
			ExecuteContext.TaskStateMachine item = new ExecuteContext.TaskStateMachine(uninitializedTask, new Action(this.Resume));
			this.taskStack.Push(item);
		}

		private void Pop(ExecuteContext.TaskStateMachine doneStateMachine)
		{
			using (ExecuteContext.TaskStateMachine taskStateMachine = this.taskStack.Pop())
			{
				this.taskResult = taskStateMachine.Task.Result;
			}
			doneStateMachine.Task.OnCompleted();
		}

		private readonly Stack<ExecuteContext.TaskStateMachine> taskStack = new Stack<ExecuteContext.TaskStateMachine>();

		private readonly ExecuteAsyncResult executeAsyncResult;

		private readonly object processLock = new object();

		private bool continueProcessing;

		private bool isProcessing;

		private bool isFinished;

		private TaskResult taskResult;

		private Exception exception;

		[ClassAccessLevel(AccessLevel.MSInternal)]
		private sealed class TaskStateMachine : BaseObject
		{
			public TaskStateMachine(ITask uninitializedTask, Action resumeDelegate)
			{
				Util.ThrowOnNullArgument(uninitializedTask, "task");
				Util.ThrowOnNullArgument(resumeDelegate, "resumeDelegate");
				this.task = uninitializedTask;
				uninitializedTask.Initialize(resumeDelegate);
				this.stateMachine = uninitializedTask.Process();
			}

			public ITask Task
			{
				get
				{
					return this.task;
				}
			}

			[DebuggerStepThrough]
			public ITask Step()
			{
				if (this.stateMachine.MoveNext())
				{
					return this.stateMachine.Current ?? this.task;
				}
				return null;
			}

			protected override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<ExecuteContext.TaskStateMachine>(this);
			}

			protected override void InternalDispose()
			{
				Util.DisposeIfPresent(this.stateMachine);
				base.InternalDispose();
			}

			private readonly ITask task;

			private readonly IEnumerator<ITask> stateMachine;
		}
	}
}
