using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Common;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class MailboxTaskQueue : IComponentData
	{
		private MailboxTaskQueue(StoreDatabase database)
		{
			this.database = database;
		}

		bool IComponentData.DoCleanup(Context context)
		{
			bool result;
			using (LockManager.Lock(this.items, LockManager.LockType.LeafMonitorLock, context.Diagnostics))
			{
				result = (this.items.Count == 0);
			}
			return result;
		}

		internal static IDisposable SetMailboxTaskScheduledTaskTestHook(Action action)
		{
			return MailboxTaskQueue.mailboxTaskScheduledTaskTestHook.SetTestHook(action);
		}

		internal static void Initialize()
		{
			if (MailboxTaskQueue.mailboxStateSlot == -1)
			{
				MailboxTaskQueue.mailboxStateSlot = MailboxState.AllocateComponentDataSlot(false);
			}
		}

		internal static void LaunchMailboxTask<TMailboxTaskContext>(Context context, MailboxTaskQueue.Priority priority, TaskTypeId taskTypeId, MailboxState mailboxState, SecurityIdentifier taskOwnerSid, ClientType taskOwnerClientType, CultureInfo taskOwnerCulture, MailboxTaskQueue.MailboxTaskDelegate mailboxTaskDelegate) where TMailboxTaskContext : MailboxTaskContext, new()
		{
			MailboxTaskQueue.LaunchMailboxTask<TMailboxTaskContext>(context, priority, taskTypeId, mailboxState, taskOwnerSid, taskOwnerClientType, taskOwnerCulture, null, null, null, mailboxTaskDelegate);
		}

		internal static void LaunchMailboxTask<TMailboxTaskContext>(Context context, MailboxTaskQueue.Priority priority, TaskTypeId taskTypeId, MailboxState mailboxState, SecurityIdentifier taskOwnerSid, ClientType taskOwnerClientType, CultureInfo taskOwnerCulture, Action<Context> onBeforeTaskStep, Action<Context> onInsideTaskStep, Action<Context> onAfterTaskStep, MailboxTaskQueue.MailboxTaskDelegate mailboxTaskDelegate) where TMailboxTaskContext : MailboxTaskContext, new()
		{
			MailboxTaskQueue mailboxTaskQueue = MailboxTaskQueue.GetMailboxTaskQueue(context, mailboxState);
			MailboxTaskQueue.MailboxTaskParameters mailboxTaskParameters = new MailboxTaskQueue.MailboxTaskParameters(context.Database, mailboxState.MailboxNumber, taskOwnerSid, taskOwnerClientType, taskOwnerCulture, onBeforeTaskStep, onInsideTaskStep, onAfterTaskStep, mailboxTaskDelegate);
			StoreDatabase database = context.Database;
			Guid userIdentity = context.UserIdentity;
			int mailboxNumber = mailboxState.MailboxNumber;
			Guid clientActivityId = context.Diagnostics.ClientActivityId;
			string clientComponentName = context.Diagnostics.ClientComponentName;
			string clientProtocolName = context.Diagnostics.ClientProtocolName;
			string clientActionString = context.Diagnostics.ClientActionString;
			Func<MailboxTaskContext> executionContextCreator = () => MailboxTaskContext.CreateTaskExecutionContext<TMailboxTaskContext>(taskTypeId, database, mailboxNumber, taskOwnerClientType, clientActivityId, clientComponentName, clientProtocolName, clientActionString, userIdentity, taskOwnerSid, taskOwnerCulture);
			mailboxTaskQueue.QueueTask(context, priority, taskTypeId, mailboxState, executionContextCreator, mailboxTaskParameters);
		}

		internal static MailboxTaskQueue GetMailboxTaskQueueNoCreate(Context context, MailboxState mailboxState)
		{
			return (MailboxTaskQueue)mailboxState.GetComponentData(MailboxTaskQueue.mailboxStateSlot);
		}

		internal static MailboxTaskQueue GetMailboxTaskQueue(Context context, MailboxState mailboxState)
		{
			MailboxTaskQueue mailboxTaskQueue = MailboxTaskQueue.GetMailboxTaskQueueNoCreate(context, mailboxState);
			if (mailboxTaskQueue == null)
			{
				mailboxTaskQueue = new MailboxTaskQueue(context.Database);
				MailboxTaskQueue mailboxTaskQueue2 = (MailboxTaskQueue)mailboxState.CompareExchangeComponentData(MailboxTaskQueue.mailboxStateSlot, null, mailboxTaskQueue);
				if (mailboxTaskQueue2 != null)
				{
					mailboxTaskQueue = mailboxTaskQueue2;
				}
			}
			return mailboxTaskQueue;
		}

		internal void DrainQueue()
		{
			LinkedListNode<MailboxTaskQueue.QueueItem> linkedListNode;
			do
			{
				linkedListNode = null;
				using (LockManager.Lock(this.items, LockManager.LockType.LeafMonitorLock))
				{
					if (this.items.Count != 0)
					{
						linkedListNode = this.items.First;
						this.items.Remove(linkedListNode);
					}
				}
				if (linkedListNode != null)
				{
					linkedListNode.Value.Completed = true;
					linkedListNode.Value.Cleanup();
					linkedListNode.Value.Dispose();
				}
			}
			while (linkedListNode != null);
		}

		internal void ScheduleWorkerTaskIfNeeded()
		{
			bool flag = false;
			using (LockManager.Lock(this.items, LockManager.LockType.LeafMonitorLock))
			{
				if (this.items.Count == 0)
				{
					this.workerTaskScheduled = false;
					return;
				}
				if (this.workerTaskScheduled)
				{
					return;
				}
				this.workerTaskScheduled = true;
				flag = true;
			}
			if (flag)
			{
				Task<MailboxTaskQueue> task = SingleExecutionTask<MailboxTaskQueue>.CreateSingleExecutionTask(this.database.TaskList, new Task<MailboxTaskQueue>.TaskCallback(MailboxTaskQueue.WorkerTaskCallback), this, true);
				if (task != null)
				{
					FaultInjection.InjectFault(MailboxTaskQueue.mailboxTaskScheduledTaskTestHook);
					return;
				}
				this.workerTaskScheduled = false;
			}
		}

		private static void RunMailboxTaskStep(MailboxTaskContext mailboxTaskContext, MailboxTaskQueue.QueueItem queueItem, Func<bool> shouldTaskContinue)
		{
			bool flag = false;
			bool flag2 = true;
			Action action = null;
			ThreadManager.ThreadInfo threadInfo;
			using (ThreadManager.NewMethodFrame(queueItem.TaskParameters.MailboxTaskDelegate, out threadInfo))
			{
				try
				{
					if (queueItem.IsNew)
					{
						mailboxTaskContext.TaskDiagnostics.OnBeforeTask(RopSummaryCollector.GetRopSummaryCollector(mailboxTaskContext.Database));
						queueItem.IsNew = false;
					}
					mailboxTaskContext.TaskDiagnostics.OnBeginMailboxTaskQueueChunk();
					if (queueItem.TaskParameters.OnBeforeTaskStep != null)
					{
						queueItem.TaskParameters.OnBeforeTaskStep(mailboxTaskContext);
					}
					ErrorCode first = mailboxTaskContext.StartMailboxOperation();
					if (first != ErrorCode.NoError)
					{
						return;
					}
					threadInfo.MailboxGuid = mailboxTaskContext.Mailbox.MailboxGuid;
					if (queueItem.TaskParameters.OnInsideTaskStep != null)
					{
						queueItem.TaskParameters.OnInsideTaskStep(mailboxTaskContext);
					}
					DiagnosticContext.TraceLocation((LID)65328U);
					IEnumerator<MailboxTaskQueue.TaskStepResult> taskSteps = queueItem.GetTaskSteps(mailboxTaskContext, shouldTaskContinue);
					DiagnosticContext.TraceLocation((LID)40752U);
					if (taskSteps.MoveNext())
					{
						DiagnosticContext.TraceLocation((LID)57136U);
						MailboxTaskQueue.TaskStepResult taskStepResult = taskSteps.Current;
						action = taskStepResult.PostStepAction;
						flag2 = false;
					}
					DiagnosticContext.TraceLocation((LID)40368U);
					flag = true;
				}
				finally
				{
					DiagnosticContext.TraceLocation((LID)56752U);
					queueItem.Completed = flag2;
					bool flag3 = false;
					try
					{
						if (queueItem.Completed)
						{
							DiagnosticContext.TraceLocation((LID)44464U);
							queueItem.Dispose();
						}
						DiagnosticContext.TraceLocation((LID)60848U);
						flag3 = true;
					}
					finally
					{
						DiagnosticContext.TraceLocation((LID)36272U);
						bool flag4 = false;
						try
						{
							if (mailboxTaskContext.IsMailboxOperationStarted)
							{
								mailboxTaskContext.EndMailboxOperation(flag && flag3, false, !flag2);
							}
							if (queueItem.TaskParameters.OnAfterTaskStep != null)
							{
								queueItem.TaskParameters.OnAfterTaskStep(mailboxTaskContext);
							}
							DiagnosticContext.TraceLocation((LID)38320U);
							if (action != null)
							{
								DiagnosticContext.TraceLocation((LID)42416U);
								action();
							}
							mailboxTaskContext.TaskDiagnostics.OnEndMailboxTaskQueueChunk();
							if (queueItem.Completed)
							{
								mailboxTaskContext.TaskDiagnostics.OnTaskEnd();
							}
							flag4 = true;
						}
						finally
						{
							DiagnosticContext.TraceLocation((LID)58800U);
							if (!flag4 && !queueItem.Completed)
							{
								DiagnosticContext.TraceLocation((LID)54704U);
								queueItem.Completed = true;
								queueItem.Dispose();
							}
						}
					}
				}
			}
			DiagnosticContext.TraceLocation((LID)62896U);
		}

		private static void WorkerTaskCallback(TaskExecutionDiagnosticsProxy diagnosticsContext, MailboxTaskQueue mailboxTaskQueue, Func<bool> shouldCallbackContinue)
		{
			bool flag = false;
			try
			{
				mailboxTaskQueue.WorkerTaskCallback(diagnosticsContext, shouldCallbackContinue);
				flag = true;
			}
			finally
			{
				if (!flag && ExTraceGlobals.TasksTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.TasksTracer.TraceError(34224L, ToStringHelper.GetAsString<byte[]>(DiagnosticContext.PackInfo()));
				}
				else if (ExTraceGlobals.TasksTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.TasksTracer.TraceDebug(52656L, ToStringHelper.GetAsString<byte[]>(DiagnosticContext.PackInfo()));
				}
			}
		}

		private void QueueTask(Context context, MailboxTaskQueue.Priority priority, TaskTypeId taskTypeId, MailboxState mailboxState, Func<MailboxTaskContext> executionContextCreator, MailboxTaskQueue.MailboxTaskParameters mailboxTaskParameters)
		{
			if (mailboxState.Quarantined)
			{
				throw new StoreException((LID)47865U, ErrorCodeValue.MailboxQuarantined, string.Format("Unable to queue mailbox task {0} for mailbox number {1}", taskTypeId, mailboxState.MailboxNumber));
			}
			using (LockManager.Lock(this.items, LockManager.LockType.LeafMonitorLock, context.Diagnostics))
			{
				if (priority == MailboxTaskQueue.Priority.Low)
				{
					this.items.AddLast(new MailboxTaskQueue.QueueItem(context, taskTypeId, mailboxState, executionContextCreator, mailboxTaskParameters));
				}
				else if (priority == MailboxTaskQueue.Priority.High)
				{
					this.items.AddFirst(new MailboxTaskQueue.QueueItem(context, taskTypeId, mailboxState, executionContextCreator, mailboxTaskParameters));
				}
			}
			this.ScheduleWorkerTaskIfNeeded();
		}

		private void WorkerTaskCallback(TaskExecutionDiagnosticsProxy diagnosticsContext, Func<bool> shouldCallbackContinue)
		{
			LinkedListNode<MailboxTaskQueue.QueueItem> first;
			using (LockManager.Lock(this.items, LockManager.LockType.LeafMonitorLock))
			{
				first = this.items.First;
			}
			try
			{
				first.Value.TaskCallback(diagnosticsContext, first.Value, shouldCallbackContinue);
			}
			finally
			{
				using (LockManager.Lock(this.items, LockManager.LockType.LeafMonitorLock))
				{
					if (first.Value.Completed)
					{
						this.items.Remove(first);
					}
					this.workerTaskScheduled = false;
				}
				this.ScheduleWorkerTaskIfNeeded();
				DiagnosticContext.TraceLocation((LID)46512U);
			}
		}

		private static int mailboxStateSlot = -1;

		private static Hookable<Action> mailboxTaskScheduledTaskTestHook = Hookable<Action>.Create(true, null);

		private readonly StoreDatabase database;

		private LinkedList<MailboxTaskQueue.QueueItem> items = new LinkedList<MailboxTaskQueue.QueueItem>();

		private bool workerTaskScheduled;

		internal enum Priority
		{
			Low,
			High
		}

		public delegate IEnumerator<MailboxTaskQueue.TaskStepResult> MailboxTaskDelegate(MailboxTaskContext context, Func<bool> shouldTaskContinue);

		internal struct TaskStepResult
		{
			private TaskStepResult(Action postStepAction)
			{
				this.postStepAction = postStepAction;
			}

			internal Action PostStepAction
			{
				get
				{
					return this.postStepAction;
				}
			}

			internal static MailboxTaskQueue.TaskStepResult Result(Action postStepAction)
			{
				return new MailboxTaskQueue.TaskStepResult(postStepAction);
			}

			private Action postStepAction;
		}

		internal class MailboxTaskParameters
		{
			public MailboxTaskParameters(StoreDatabase mailboxDatabase, int mailboxNumber, SecurityIdentifier userSid, ClientType clientType, CultureInfo culture, Action<Context> onBeforeTaskStep, Action<Context> onInsideTaskStep, Action<Context> onAfterTaskStep, MailboxTaskQueue.MailboxTaskDelegate mailboxTaskDelegate)
			{
				this.mailboxDatabase = mailboxDatabase;
				this.mailboxNumber = mailboxNumber;
				this.userSid = userSid;
				this.clientType = clientType;
				this.culture = culture;
				this.onBeforeTaskStep = onBeforeTaskStep;
				this.onInsideTaskStep = onInsideTaskStep;
				this.onAfterTaskStep = onAfterTaskStep;
				this.mailboxTaskDelegate = mailboxTaskDelegate;
			}

			public StoreDatabase MailboxDatabase
			{
				get
				{
					return this.mailboxDatabase;
				}
			}

			public int MailboxNumber
			{
				get
				{
					return this.mailboxNumber;
				}
			}

			public SecurityIdentifier UserSid
			{
				get
				{
					return this.userSid;
				}
			}

			public ClientType ClientType
			{
				get
				{
					return this.clientType;
				}
			}

			public CultureInfo Culture
			{
				get
				{
					return this.culture;
				}
			}

			public Action<Context> OnBeforeTaskStep
			{
				get
				{
					return this.onBeforeTaskStep;
				}
			}

			public Action<Context> OnInsideTaskStep
			{
				get
				{
					return this.onInsideTaskStep;
				}
			}

			public Action<Context> OnAfterTaskStep
			{
				get
				{
					return this.onAfterTaskStep;
				}
			}

			public MailboxTaskQueue.MailboxTaskDelegate MailboxTaskDelegate
			{
				get
				{
					return this.mailboxTaskDelegate;
				}
			}

			private readonly StoreDatabase mailboxDatabase;

			private readonly int mailboxNumber;

			private readonly SecurityIdentifier userSid;

			private readonly ClientType clientType;

			private readonly CultureInfo culture;

			private readonly Action<Context> onBeforeTaskStep;

			private readonly Action<Context> onInsideTaskStep;

			private readonly Action<Context> onAfterTaskStep;

			private readonly MailboxTaskQueue.MailboxTaskDelegate mailboxTaskDelegate;
		}

		private class QueueItem : DisposableBase
		{
			internal QueueItem(Context context, TaskTypeId taskTypeId, MailboxState mailboxState, Func<MailboxTaskContext> executionContextCreator, MailboxTaskQueue.MailboxTaskParameters taskParameters)
			{
				this.executionContextCreator = executionContextCreator;
				Guid clientActivityId = context.Diagnostics.ClientActivityId;
				string clientComponentName = context.Diagnostics.ClientComponentName;
				string clientProtocolName = context.Diagnostics.ClientProtocolName;
				string clientActionString = context.Diagnostics.ClientActionString;
				this.taskCallback = TaskExecutionWrapper<MailboxTaskQueue.QueueItem>.WrapExecute<MailboxTaskContext>(new TaskDiagnosticInformation(taskTypeId, context.ClientType, context.Database.MdbGuid, mailboxState.MailboxGuid, clientActivityId, clientComponentName, clientProtocolName, clientActionString), new TaskExecutionWrapper<MailboxTaskQueue.QueueItem>.TaskCallback<MailboxTaskContext>(MailboxTaskQueue.RunMailboxTaskStep), new Func<MailboxTaskContext>(this.GetMailboxTaskExecutionContext), new Action<MailboxTaskContext>(this.ReleaseMailboxTaskExecutionContext), true);
				this.taskParameters = taskParameters;
				this.taskSteps = null;
				this.completed = false;
				this.isNew = true;
			}

			internal Task<MailboxTaskQueue.QueueItem>.TaskCallback TaskCallback
			{
				get
				{
					return this.taskCallback;
				}
			}

			internal MailboxTaskQueue.MailboxTaskParameters TaskParameters
			{
				get
				{
					return this.taskParameters;
				}
			}

			internal bool Completed
			{
				get
				{
					return this.completed;
				}
				set
				{
					this.completed = value;
				}
			}

			internal bool IsNew
			{
				get
				{
					return this.isNew;
				}
				set
				{
					this.isNew = value;
				}
			}

			internal void Cleanup()
			{
				if (this.currentMailboxTaskContext != null)
				{
					this.ReleaseMailboxTaskExecutionContext(this.currentMailboxTaskContext);
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<MailboxTaskQueue.QueueItem>(this);
			}

			protected override void InternalDispose(bool calledFromDispose)
			{
				if (calledFromDispose && this.taskSteps != null)
				{
					this.taskSteps.Dispose();
					this.taskSteps = null;
				}
			}

			internal IEnumerator<MailboxTaskQueue.TaskStepResult> GetTaskSteps(MailboxTaskContext mailboxTaskContext, Func<bool> shouldTaskContinue)
			{
				this.currentShouldTaskContinueCallback = shouldTaskContinue;
				if (this.taskSteps == null)
				{
					this.taskSteps = this.taskParameters.MailboxTaskDelegate(mailboxTaskContext, new Func<bool>(this.ShouldTaskContinueCallback));
				}
				return this.taskSteps;
			}

			private MailboxTaskContext GetMailboxTaskExecutionContext()
			{
				if (this.currentMailboxTaskContext == null)
				{
					this.currentMailboxTaskContext = this.executionContextCreator();
				}
				return this.currentMailboxTaskContext;
			}

			private void ReleaseMailboxTaskExecutionContext(MailboxTaskContext mailboxTaskContext)
			{
				if (this.Completed)
				{
					this.currentMailboxTaskContext.Dispose();
					this.currentMailboxTaskContext = null;
				}
			}

			private bool ShouldTaskContinueCallback()
			{
				return this.currentShouldTaskContinueCallback();
			}

			private readonly Func<MailboxTaskContext> executionContextCreator;

			private readonly Task<MailboxTaskQueue.QueueItem>.TaskCallback taskCallback;

			private readonly MailboxTaskQueue.MailboxTaskParameters taskParameters;

			private MailboxTaskContext currentMailboxTaskContext;

			private Func<bool> currentShouldTaskContinueCallback;

			private IEnumerator<MailboxTaskQueue.TaskStepResult> taskSteps;

			private bool completed;

			private bool isNew;
		}
	}
}
