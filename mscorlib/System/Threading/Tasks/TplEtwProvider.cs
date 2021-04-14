using System;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Threading.Tasks
{
	[EventSource(Name = "System.Threading.Tasks.TplEventSource", Guid = "2e5dba47-a3d2-4d16-8ee0-6671ffdcd7b5", LocalizationResources = "mscorlib")]
	internal sealed class TplEtwProvider : EventSource
	{
		protected override void OnEventCommand(EventCommandEventArgs command)
		{
			if (command.Command == EventCommand.Enable)
			{
				AsyncCausalityTracer.EnableToETW(true);
			}
			else if (command.Command == EventCommand.Disable)
			{
				AsyncCausalityTracer.EnableToETW(false);
			}
			if (base.IsEnabled(EventLevel.Informational, (EventKeywords)128L))
			{
				ActivityTracker.Instance.Enable();
			}
			else
			{
				this.TasksSetActivityIds = base.IsEnabled(EventLevel.Informational, (EventKeywords)65536L);
			}
			this.Debug = base.IsEnabled(EventLevel.Informational, (EventKeywords)131072L);
			this.DebugActivityId = base.IsEnabled(EventLevel.Informational, (EventKeywords)262144L);
		}

		private TplEtwProvider()
		{
		}

		[SecuritySafeCritical]
		[Event(1, Level = EventLevel.Informational, ActivityOptions = EventActivityOptions.Recursive, Task = (EventTask)1, Opcode = EventOpcode.Start)]
		public unsafe void ParallelLoopBegin(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID, TplEtwProvider.ForkJoinOperationType OperationType, long InclusiveFrom, long ExclusiveTo)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)4L))
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)6) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->Size = 4;
				ptr->DataPointer = (IntPtr)((void*)(&OriginatingTaskSchedulerID));
				ptr[1].Size = 4;
				ptr[1].DataPointer = (IntPtr)((void*)(&OriginatingTaskID));
				ptr[2].Size = 4;
				ptr[2].DataPointer = (IntPtr)((void*)(&ForkJoinContextID));
				ptr[3].Size = 4;
				ptr[3].DataPointer = (IntPtr)((void*)(&OperationType));
				ptr[4].Size = 8;
				ptr[4].DataPointer = (IntPtr)((void*)(&InclusiveFrom));
				ptr[5].Size = 8;
				ptr[5].DataPointer = (IntPtr)((void*)(&ExclusiveTo));
				base.WriteEventCore(1, 6, ptr);
			}
		}

		[SecuritySafeCritical]
		[Event(2, Level = EventLevel.Informational, Task = (EventTask)1, Opcode = EventOpcode.Stop)]
		public unsafe void ParallelLoopEnd(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID, long TotalIterations)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)4L))
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)4) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->Size = 4;
				ptr->DataPointer = (IntPtr)((void*)(&OriginatingTaskSchedulerID));
				ptr[1].Size = 4;
				ptr[1].DataPointer = (IntPtr)((void*)(&OriginatingTaskID));
				ptr[2].Size = 4;
				ptr[2].DataPointer = (IntPtr)((void*)(&ForkJoinContextID));
				ptr[3].Size = 8;
				ptr[3].DataPointer = (IntPtr)((void*)(&TotalIterations));
				base.WriteEventCore(2, 4, ptr);
			}
		}

		[SecuritySafeCritical]
		[Event(3, Level = EventLevel.Informational, ActivityOptions = EventActivityOptions.Recursive, Task = (EventTask)2, Opcode = EventOpcode.Start)]
		public unsafe void ParallelInvokeBegin(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID, TplEtwProvider.ForkJoinOperationType OperationType, int ActionCount)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)4L))
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)5) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->Size = 4;
				ptr->DataPointer = (IntPtr)((void*)(&OriginatingTaskSchedulerID));
				ptr[1].Size = 4;
				ptr[1].DataPointer = (IntPtr)((void*)(&OriginatingTaskID));
				ptr[2].Size = 4;
				ptr[2].DataPointer = (IntPtr)((void*)(&ForkJoinContextID));
				ptr[3].Size = 4;
				ptr[3].DataPointer = (IntPtr)((void*)(&OperationType));
				ptr[4].Size = 4;
				ptr[4].DataPointer = (IntPtr)((void*)(&ActionCount));
				base.WriteEventCore(3, 5, ptr);
			}
		}

		[Event(4, Level = EventLevel.Informational, Task = (EventTask)2, Opcode = EventOpcode.Stop)]
		public void ParallelInvokeEnd(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)4L))
			{
				base.WriteEvent(4, OriginatingTaskSchedulerID, OriginatingTaskID, ForkJoinContextID);
			}
		}

		[Event(5, Level = EventLevel.Verbose, ActivityOptions = EventActivityOptions.Recursive, Task = (EventTask)5, Opcode = EventOpcode.Start)]
		public void ParallelFork(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Verbose, (EventKeywords)4L))
			{
				base.WriteEvent(5, OriginatingTaskSchedulerID, OriginatingTaskID, ForkJoinContextID);
			}
		}

		[Event(6, Level = EventLevel.Verbose, Task = (EventTask)5, Opcode = EventOpcode.Stop)]
		public void ParallelJoin(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ForkJoinContextID)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Verbose, (EventKeywords)4L))
			{
				base.WriteEvent(6, OriginatingTaskSchedulerID, OriginatingTaskID, ForkJoinContextID);
			}
		}

		[SecuritySafeCritical]
		[Event(7, Task = (EventTask)6, Version = 1, Opcode = EventOpcode.Send, Level = EventLevel.Informational, Keywords = (EventKeywords)3L)]
		public unsafe void TaskScheduled(int OriginatingTaskSchedulerID, int OriginatingTaskID, int TaskID, int CreatingTaskID, int TaskCreationOptions, int appDomain)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)3L))
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)5) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->Size = 4;
				ptr->DataPointer = (IntPtr)((void*)(&OriginatingTaskSchedulerID));
				ptr[1].Size = 4;
				ptr[1].DataPointer = (IntPtr)((void*)(&OriginatingTaskID));
				ptr[2].Size = 4;
				ptr[2].DataPointer = (IntPtr)((void*)(&TaskID));
				ptr[3].Size = 4;
				ptr[3].DataPointer = (IntPtr)((void*)(&CreatingTaskID));
				ptr[4].Size = 4;
				ptr[4].DataPointer = (IntPtr)((void*)(&TaskCreationOptions));
				if (this.TasksSetActivityIds)
				{
					Guid guid = TplEtwProvider.CreateGuidForTaskID(TaskID);
					base.WriteEventWithRelatedActivityIdCore(7, &guid, 5, ptr);
					return;
				}
				base.WriteEventCore(7, 5, ptr);
			}
		}

		[Event(8, Level = EventLevel.Informational, Keywords = (EventKeywords)2L)]
		public void TaskStarted(int OriginatingTaskSchedulerID, int OriginatingTaskID, int TaskID)
		{
			if (base.IsEnabled(EventLevel.Informational, (EventKeywords)2L))
			{
				base.WriteEvent(8, OriginatingTaskSchedulerID, OriginatingTaskID, TaskID);
			}
		}

		[SecuritySafeCritical]
		[Event(9, Version = 1, Level = EventLevel.Informational, Keywords = (EventKeywords)64L)]
		public unsafe void TaskCompleted(int OriginatingTaskSchedulerID, int OriginatingTaskID, int TaskID, bool IsExceptional)
		{
			if (base.IsEnabled(EventLevel.Informational, (EventKeywords)2L))
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)4) * (UIntPtr)sizeof(EventSource.EventData))];
				int num = IsExceptional ? 1 : 0;
				ptr->Size = 4;
				ptr->DataPointer = (IntPtr)((void*)(&OriginatingTaskSchedulerID));
				ptr[1].Size = 4;
				ptr[1].DataPointer = (IntPtr)((void*)(&OriginatingTaskID));
				ptr[2].Size = 4;
				ptr[2].DataPointer = (IntPtr)((void*)(&TaskID));
				ptr[3].Size = 4;
				ptr[3].DataPointer = (IntPtr)((void*)(&num));
				base.WriteEventCore(9, 4, ptr);
			}
		}

		[SecuritySafeCritical]
		[Event(10, Version = 3, Task = (EventTask)4, Opcode = EventOpcode.Send, Level = EventLevel.Informational, Keywords = (EventKeywords)3L)]
		public unsafe void TaskWaitBegin(int OriginatingTaskSchedulerID, int OriginatingTaskID, int TaskID, TplEtwProvider.TaskWaitBehavior Behavior, int ContinueWithTaskID, int appDomain)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)3L))
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)5) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->Size = 4;
				ptr->DataPointer = (IntPtr)((void*)(&OriginatingTaskSchedulerID));
				ptr[1].Size = 4;
				ptr[1].DataPointer = (IntPtr)((void*)(&OriginatingTaskID));
				ptr[2].Size = 4;
				ptr[2].DataPointer = (IntPtr)((void*)(&TaskID));
				ptr[3].Size = 4;
				ptr[3].DataPointer = (IntPtr)((void*)(&Behavior));
				ptr[4].Size = 4;
				ptr[4].DataPointer = (IntPtr)((void*)(&ContinueWithTaskID));
				if (this.TasksSetActivityIds)
				{
					Guid guid = TplEtwProvider.CreateGuidForTaskID(TaskID);
					base.WriteEventWithRelatedActivityIdCore(10, &guid, 5, ptr);
					return;
				}
				base.WriteEventCore(10, 5, ptr);
			}
		}

		[Event(11, Level = EventLevel.Verbose, Keywords = (EventKeywords)2L)]
		public void TaskWaitEnd(int OriginatingTaskSchedulerID, int OriginatingTaskID, int TaskID)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Verbose, (EventKeywords)2L))
			{
				base.WriteEvent(11, OriginatingTaskSchedulerID, OriginatingTaskID, TaskID);
			}
		}

		[Event(13, Level = EventLevel.Verbose, Keywords = (EventKeywords)64L)]
		public void TaskWaitContinuationComplete(int TaskID)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Verbose, (EventKeywords)2L))
			{
				base.WriteEvent(13, TaskID);
			}
		}

		[Event(19, Level = EventLevel.Verbose, Keywords = (EventKeywords)64L)]
		public void TaskWaitContinuationStarted(int TaskID)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Verbose, (EventKeywords)2L))
			{
				base.WriteEvent(19, TaskID);
			}
		}

		[SecuritySafeCritical]
		[Event(12, Task = (EventTask)7, Opcode = EventOpcode.Send, Level = EventLevel.Informational, Keywords = (EventKeywords)3L)]
		public unsafe void AwaitTaskContinuationScheduled(int OriginatingTaskSchedulerID, int OriginatingTaskID, int ContinuwWithTaskId)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)3L))
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)3) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->Size = 4;
				ptr->DataPointer = (IntPtr)((void*)(&OriginatingTaskSchedulerID));
				ptr[1].Size = 4;
				ptr[1].DataPointer = (IntPtr)((void*)(&OriginatingTaskID));
				ptr[2].Size = 4;
				ptr[2].DataPointer = (IntPtr)((void*)(&ContinuwWithTaskId));
				if (this.TasksSetActivityIds)
				{
					Guid guid = TplEtwProvider.CreateGuidForTaskID(ContinuwWithTaskId);
					base.WriteEventWithRelatedActivityIdCore(12, &guid, 3, ptr);
					return;
				}
				base.WriteEventCore(12, 3, ptr);
			}
		}

		[SecuritySafeCritical]
		[Event(14, Version = 1, Level = EventLevel.Informational, Keywords = (EventKeywords)8L)]
		public unsafe void TraceOperationBegin(int TaskID, string OperationName, long RelatedContext)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)8L))
			{
				fixed (string text = OperationName)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					EventSource.EventData* ptr2 = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)3) * (UIntPtr)sizeof(EventSource.EventData))];
					ptr2->Size = 4;
					ptr2->DataPointer = (IntPtr)((void*)(&TaskID));
					ptr2[1].Size = (OperationName.Length + 1) * 2;
					ptr2[1].DataPointer = (IntPtr)((void*)ptr);
					ptr2[2].Size = 8;
					ptr2[2].DataPointer = (IntPtr)((void*)(&RelatedContext));
					base.WriteEventCore(14, 3, ptr2);
				}
			}
		}

		[SecuritySafeCritical]
		[Event(16, Version = 1, Level = EventLevel.Informational, Keywords = (EventKeywords)16L)]
		public void TraceOperationRelation(int TaskID, CausalityRelation Relation)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)16L))
			{
				base.WriteEvent(16, TaskID, (int)Relation);
			}
		}

		[SecuritySafeCritical]
		[Event(15, Version = 1, Level = EventLevel.Informational, Keywords = (EventKeywords)8L)]
		public void TraceOperationEnd(int TaskID, AsyncCausalityStatus Status)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)8L))
			{
				base.WriteEvent(15, TaskID, (int)Status);
			}
		}

		[SecuritySafeCritical]
		[Event(17, Version = 1, Level = EventLevel.Informational, Keywords = (EventKeywords)32L)]
		public void TraceSynchronousWorkBegin(int TaskID, CausalitySynchronousWork Work)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)32L))
			{
				base.WriteEvent(17, TaskID, (int)Work);
			}
		}

		[SecuritySafeCritical]
		[Event(18, Version = 1, Level = EventLevel.Informational, Keywords = (EventKeywords)32L)]
		public unsafe void TraceSynchronousWorkEnd(CausalitySynchronousWork Work)
		{
			if (base.IsEnabled() && base.IsEnabled(EventLevel.Informational, (EventKeywords)32L))
			{
				EventSource.EventData* ptr = stackalloc EventSource.EventData[checked(unchecked((UIntPtr)1) * (UIntPtr)sizeof(EventSource.EventData))];
				ptr->Size = 4;
				ptr->DataPointer = (IntPtr)((void*)(&Work));
				base.WriteEventCore(18, 1, ptr);
			}
		}

		[NonEvent]
		[SecuritySafeCritical]
		public unsafe void RunningContinuation(int TaskID, object Object)
		{
			this.RunningContinuation(TaskID, (long)((ulong)(*(IntPtr*)((void*)JitHelpers.UnsafeCastToStackPointer<object>(ref Object)))));
		}

		[Event(20, Keywords = (EventKeywords)131072L)]
		private void RunningContinuation(int TaskID, long Object)
		{
			if (this.Debug)
			{
				base.WriteEvent(20, (long)TaskID, Object);
			}
		}

		[NonEvent]
		[SecuritySafeCritical]
		public unsafe void RunningContinuationList(int TaskID, int Index, object Object)
		{
			this.RunningContinuationList(TaskID, Index, (long)((ulong)(*(IntPtr*)((void*)JitHelpers.UnsafeCastToStackPointer<object>(ref Object)))));
		}

		[Event(21, Keywords = (EventKeywords)131072L)]
		public void RunningContinuationList(int TaskID, int Index, long Object)
		{
			if (this.Debug)
			{
				base.WriteEvent(21, (long)TaskID, (long)Index, Object);
			}
		}

		[Event(22, Keywords = (EventKeywords)131072L)]
		public void DebugMessage(string Message)
		{
			base.WriteEvent(22, Message);
		}

		[Event(23, Keywords = (EventKeywords)131072L)]
		public void DebugFacilityMessage(string Facility, string Message)
		{
			base.WriteEvent(23, Facility, Message);
		}

		[Event(24, Keywords = (EventKeywords)131072L)]
		public void DebugFacilityMessage1(string Facility, string Message, string Value1)
		{
			base.WriteEvent(24, Facility, Message, Value1);
		}

		[Event(25, Keywords = (EventKeywords)262144L)]
		public void SetActivityId(Guid NewId)
		{
			if (this.DebugActivityId)
			{
				base.WriteEvent(25, new object[]
				{
					NewId
				});
			}
		}

		[Event(26, Keywords = (EventKeywords)131072L)]
		public void NewID(int TaskID)
		{
			if (this.Debug)
			{
				base.WriteEvent(26, TaskID);
			}
		}

		internal static Guid CreateGuidForTaskID(int taskID)
		{
			uint s_currentPid = EventSource.s_currentPid;
			int domainID = Thread.GetDomainID();
			return new Guid(taskID, (short)domainID, (short)(domainID >> 16), (byte)s_currentPid, (byte)(s_currentPid >> 8), (byte)(s_currentPid >> 16), (byte)(s_currentPid >> 24), byte.MaxValue, 220, 215, 181);
		}

		internal bool TasksSetActivityIds;

		internal bool Debug;

		private bool DebugActivityId;

		public static TplEtwProvider Log = new TplEtwProvider();

		private const EventKeywords ALL_KEYWORDS = EventKeywords.All;

		private const int PARALLELLOOPBEGIN_ID = 1;

		private const int PARALLELLOOPEND_ID = 2;

		private const int PARALLELINVOKEBEGIN_ID = 3;

		private const int PARALLELINVOKEEND_ID = 4;

		private const int PARALLELFORK_ID = 5;

		private const int PARALLELJOIN_ID = 6;

		private const int TASKSCHEDULED_ID = 7;

		private const int TASKSTARTED_ID = 8;

		private const int TASKCOMPLETED_ID = 9;

		private const int TASKWAITBEGIN_ID = 10;

		private const int TASKWAITEND_ID = 11;

		private const int AWAITTASKCONTINUATIONSCHEDULED_ID = 12;

		private const int TASKWAITCONTINUATIONCOMPLETE_ID = 13;

		private const int TASKWAITCONTINUATIONSTARTED_ID = 19;

		private const int TRACEOPERATIONSTART_ID = 14;

		private const int TRACEOPERATIONSTOP_ID = 15;

		private const int TRACEOPERATIONRELATION_ID = 16;

		private const int TRACESYNCHRONOUSWORKSTART_ID = 17;

		private const int TRACESYNCHRONOUSWORKSTOP_ID = 18;

		public enum ForkJoinOperationType
		{
			ParallelInvoke = 1,
			ParallelFor,
			ParallelForEach
		}

		public enum TaskWaitBehavior
		{
			Synchronous = 1,
			Asynchronous
		}

		public class Tasks
		{
			public const EventTask Loop = (EventTask)1;

			public const EventTask Invoke = (EventTask)2;

			public const EventTask TaskExecute = (EventTask)3;

			public const EventTask TaskWait = (EventTask)4;

			public const EventTask ForkJoin = (EventTask)5;

			public const EventTask TaskScheduled = (EventTask)6;

			public const EventTask AwaitTaskContinuationScheduled = (EventTask)7;

			public const EventTask TraceOperation = (EventTask)8;

			public const EventTask TraceSynchronousWork = (EventTask)9;
		}

		public class Keywords
		{
			public const EventKeywords TaskTransfer = (EventKeywords)1L;

			public const EventKeywords Tasks = (EventKeywords)2L;

			public const EventKeywords Parallel = (EventKeywords)4L;

			public const EventKeywords AsyncCausalityOperation = (EventKeywords)8L;

			public const EventKeywords AsyncCausalityRelation = (EventKeywords)16L;

			public const EventKeywords AsyncCausalitySynchronousWork = (EventKeywords)32L;

			public const EventKeywords TaskStops = (EventKeywords)64L;

			public const EventKeywords TasksFlowActivityIds = (EventKeywords)128L;

			public const EventKeywords TasksSetActivityIds = (EventKeywords)65536L;

			public const EventKeywords Debug = (EventKeywords)131072L;

			public const EventKeywords DebugActivityId = (EventKeywords)262144L;
		}
	}
}
