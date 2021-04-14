using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public sealed class ThreadManager : IStoreSimpleQueryTarget<ThreadManager.ThreadDiagnosticInfo>, IStoreQueryTargetBase<ThreadManager.ThreadDiagnosticInfo>, IStoreSimpleQueryTarget<ThreadManager.ProcessThreadInfo>, IStoreQueryTargetBase<ThreadManager.ProcessThreadInfo>
	{
		private ThreadManager()
		{
		}

		public static ThreadManager Instance
		{
			get
			{
				return ThreadManager.instance;
			}
		}

		internal static void Initialize()
		{
			ThreadManager.threadHangDetectionTimeout = Hookable<TimeSpan>.Create(false, ConfigurationSchema.ThreadHangDetectionTimeout.Value);
			ThreadManager.instance = new ThreadManager();
			ThreadManager.instance.StartScavengerTask();
		}

		internal static void Terminate()
		{
			if (ThreadManager.instance != null)
			{
				ThreadManager.instance.StopScavengerTask();
			}
		}

		public static IDisposable SetThreadHangDetectionTimeoutTestHook(TimeSpan newTimeout)
		{
			return ThreadManager.threadHangDetectionTimeout.SetTestHook(newTimeout);
		}

		public static ThreadManager.MethodFrame NewMethodFrame(string methodName)
		{
			ThreadManager.ThreadInfo threadInfo;
			return ThreadManager.NewMethodFrame(methodName, out threadInfo);
		}

		public static ThreadManager.MethodFrame NewMethodFrame(string methodName, ThreadManager.TimeoutDefinition timeoutDefinition)
		{
			return new ThreadManager.MethodFrame(methodName, timeoutDefinition);
		}

		public static ThreadManager.MethodFrame NewMethodFrame(string methodName, out ThreadManager.ThreadInfo currentThreadInfo)
		{
			ThreadManager.MethodFrame result = new ThreadManager.MethodFrame(methodName);
			currentThreadInfo = result.CurrentThreadInfo;
			return result;
		}

		public static ThreadManager.MethodFrame NewMethodFrame(Delegate methodDelegate)
		{
			ThreadManager.ThreadInfo threadInfo;
			return ThreadManager.NewMethodFrame(methodDelegate, out threadInfo);
		}

		public static ThreadManager.MethodFrame NewMethodFrame(Delegate methodDelegate, out ThreadManager.ThreadInfo currentThreadInfo)
		{
			return ThreadManager.NewMethodFrame(ThreadManager.GetMethodNameFromDelegate(methodDelegate), out currentThreadInfo);
		}

		public static void MarkCurrentThreadAsLongRunning()
		{
			bool flag;
			ThreadManager.ThreadInfo orCreateCurrentThreadDiagnosticInfo = ThreadManager.Instance.GetOrCreateCurrentThreadDiagnosticInfo(out flag);
			Globals.AssertRetail(!flag, "This method should be called only when we already have ThreadInfo");
			orCreateCurrentThreadDiagnosticInfo.LongRunningThread = true;
		}

		public ThreadManager.ThreadInfo GetOrCreateCurrentThreadDiagnosticInfo(out bool created)
		{
			ThreadManager.ThreadInfo threadInfo = new ThreadManager.ThreadInfo();
			ThreadManager.ThreadInfo orAdd = this.threadList.GetOrAdd(Environment.CurrentManagedThreadId, threadInfo);
			created = (orAdd == threadInfo);
			return orAdd;
		}

		public void RemoveCurrentThreadDiagnosticInfo()
		{
			ThreadManager.ThreadInfo threadInfo;
			bool assertCondition = this.threadList.TryRemove(Environment.CurrentManagedThreadId, out threadInfo);
			Globals.AssertRetail(assertCondition, "Current ThreadInfo doesn't exist");
		}

		internal void ExecuteScavengerForTest()
		{
			this.ThreadListScavenger(null, null, () => true);
		}

		private static string GetMethodNameFromDelegate(Delegate delegateMethod)
		{
			return delegateMethod.GetMethodInfo().Name;
		}

		private void StartScavengerTask()
		{
			if (this.threadListScavengerTask == null)
			{
				this.threadListScavengerTask = new RecurringTask<object>(new Task<object>.TaskCallback(this.ThreadListScavenger), null, ThreadManager.ThreadListScavengerInterval);
				this.threadListScavengerTask.Start();
			}
		}

		private void StopScavengerTask()
		{
			if (this.threadListScavengerTask != null)
			{
				this.threadListScavengerTask.Stop();
				this.threadListScavengerTask.Dispose();
				this.threadListScavengerTask = null;
			}
		}

		private void ThreadListScavenger(TaskExecutionDiagnosticsProxy diagnosticsContext, object unused, Func<bool> shouldCallbackContinue)
		{
			if (!shouldCallbackContinue() || ErrorHelper.IsDebuggerAttached())
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			foreach (KeyValuePair<int, ThreadManager.ThreadInfo> keyValuePair in this.threadList)
			{
				ThreadManager.ThreadInfo value = keyValuePair.Value;
				TimeSpan timeSpan = utcNow - value.StartUtcTime;
				if (!value.LongRunningThread)
				{
					if (value.TimeoutDefinition != null && timeSpan >= value.TimeoutDefinition.TimeoutValue)
					{
						value.TimeoutDefinition.TimeoutCrashAction(value);
						Globals.AssertRetail(false, "We should not reach this point");
					}
					if (timeSpan >= ThreadManager.threadHangDetectionTimeout.Value)
					{
						throw new InvalidOperationException(string.Format("Possible hang detected. Operation: {0}. Execution time: {1}. Client: {2}. MailboxGuid: {3}", new object[]
						{
							value.MethodName,
							timeSpan,
							value.Client,
							value.MailboxGuid
						}));
					}
				}
			}
		}

		string IStoreQueryTargetBase<ThreadManager.ThreadDiagnosticInfo>.Name
		{
			get
			{
				return "ThreadDiagnosticInfo";
			}
		}

		Type[] IStoreQueryTargetBase<ThreadManager.ThreadDiagnosticInfo>.ParameterTypes
		{
			get
			{
				return Array<Type>.Empty;
			}
		}

		IEnumerable<ThreadManager.ThreadDiagnosticInfo> IStoreSimpleQueryTarget<ThreadManager.ThreadDiagnosticInfo>.GetRows(object[] parameters)
		{
			foreach (ThreadManager.ThreadInfo threadInfo in this.threadList.Values)
			{
				Thread thread = threadInfo.Thread;
				Globals.AssertRetail(thread.IsAlive, "If this thread is not alive then how it is still in the collection?");
				threadInfo.Priority = thread.Priority;
				threadInfo.Status = thread.ThreadState;
				yield return threadInfo;
			}
			yield break;
		}

		string IStoreQueryTargetBase<ThreadManager.ProcessThreadInfo>.Name
		{
			get
			{
				return "ProcessThreadInfo";
			}
		}

		Type[] IStoreQueryTargetBase<ThreadManager.ProcessThreadInfo>.ParameterTypes
		{
			get
			{
				return Array<Type>.Empty;
			}
		}

		IEnumerable<ThreadManager.ProcessThreadInfo> IStoreSimpleQueryTarget<ThreadManager.ProcessThreadInfo>.GetRows(object[] parameters)
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				ProcessThreadCollection threadCollection = currentProcess.Threads;
				foreach (object obj in threadCollection)
				{
					ProcessThread thread = (ProcessThread)obj;
					yield return new ThreadManager.ProcessThreadInfo(thread);
				}
			}
			yield break;
		}

		private const int ThreadListInitialCapacity = 50;

		private static ThreadManager instance;

		private static readonly TimeSpan ThreadListScavengerInterval = TimeSpan.FromMinutes(15.0);

		private static Hookable<TimeSpan> threadHangDetectionTimeout;

		private ConcurrentDictionary<int, ThreadManager.ThreadInfo> threadList = new ConcurrentDictionary<int, ThreadManager.ThreadInfo>(Environment.ProcessorCount, 50);

		private RecurringTask<object> threadListScavengerTask;

		internal sealed class ProcessThreadInfo
		{
			public int NativeId { get; private set; }

			public int BasePriority { get; private set; }

			public int CurrentPriority { get; private set; }

			public System.Diagnostics.ThreadState State { get; private set; }

			public DateTime StartTime { get; private set; }

			public TimeSpan TotalProcessorTime { get; private set; }

			public TimeSpan UserProcessorTime { get; private set; }

			public ProcessThreadInfo(ProcessThread processThread)
			{
				try
				{
					this.NativeId = processThread.Id;
					this.BasePriority = processThread.BasePriority;
					this.CurrentPriority = processThread.CurrentPriority;
					this.StartTime = processThread.StartTime;
					this.State = processThread.ThreadState;
					this.TotalProcessorTime = processThread.TotalProcessorTime;
					this.UserProcessorTime = processThread.UserProcessorTime;
				}
				catch (Win32Exception exception)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
				}
			}
		}

		public class ThreadDiagnosticInfo
		{
			[Queryable(Index = 0)]
			public int NativeId { get; internal set; }

			[Queryable]
			public int ThreadId { get; internal set; }

			[Queryable]
			public ThreadPriority Priority { get; internal set; }

			[Queryable]
			public string MethodName { get; internal set; }

			[Queryable]
			public string Client
			{
				get
				{
					return this.clientInformation.Client;
				}
				internal set
				{
					this.clientInformation.Client = value;
				}
			}

			[Queryable(Visibility = Visibility.Redacted)]
			public string User
			{
				get
				{
					return this.clientInformation.User;
				}
				internal set
				{
					this.clientInformation.User = value;
				}
			}

			[Queryable]
			public Guid UserGuid
			{
				get
				{
					return this.clientInformation.UserGuid;
				}
				internal set
				{
					this.clientInformation.UserGuid = value;
				}
			}

			[Queryable(Visibility = Visibility.Redacted)]
			public string Mailbox
			{
				get
				{
					return this.clientInformation.Mailbox;
				}
				internal set
				{
					this.clientInformation.Mailbox = value;
				}
			}

			[Queryable]
			public Guid MailboxGuid
			{
				get
				{
					return this.clientInformation.MailboxGuid;
				}
				internal set
				{
					this.clientInformation.MailboxGuid = value;
				}
			}

			[Queryable]
			public System.Threading.ThreadState Status { get; internal set; }

			[Queryable]
			public DateTime StartUtcTime { get; internal set; }

			[Queryable]
			public TimeSpan Duration
			{
				get
				{
					return DateTime.UtcNow - this.StartUtcTime;
				}
			}

			public ThreadDiagnosticInfo()
			{
				this.ResetClientInformation();
				this.NativeId = ThreadManager.ThreadDiagnosticInfo.GetCurrentWinThreadId();
				this.ThreadId = Thread.CurrentThread.ManagedThreadId;
				this.StartUtcTime = DateTime.UtcNow;
			}

			internal ThreadManager.ThreadDiagnosticInfo.ClientInformation ClientInfo
			{
				get
				{
					return this.clientInformation;
				}
				set
				{
					this.clientInformation = value;
				}
			}

			internal void SetClientInformation(ThreadManager.ThreadDiagnosticInfo.ClientInformation clientInformation)
			{
				this.clientInformation = clientInformation;
			}

			internal void ResetClientInformation()
			{
				this.Client = string.Empty;
				this.User = string.Empty;
				this.UserGuid = Guid.Empty;
				this.Mailbox = string.Empty;
				this.MailboxGuid = Guid.Empty;
			}

			[DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
			public static extern int GetCurrentWinThreadId();

			private ThreadManager.ThreadDiagnosticInfo.ClientInformation clientInformation;

			internal struct ClientInformation
			{
				public string Client { get; internal set; }

				public string User { get; internal set; }

				public Guid UserGuid { get; internal set; }

				public string Mailbox { get; internal set; }

				public Guid MailboxGuid { get; internal set; }
			}
		}

		public sealed class ThreadInfo : ThreadManager.ThreadDiagnosticInfo
		{
			public Thread Thread
			{
				get
				{
					return this.thread;
				}
			}

			public bool LongRunningThread { get; set; }

			public ThreadManager.TimeoutDefinition TimeoutDefinition
			{
				get
				{
					return this.timeoutDefinition;
				}
				set
				{
					this.timeoutDefinition = value;
				}
			}

			public ThreadInfo()
			{
				this.thread = Thread.CurrentThread;
			}

			private Thread thread;

			private ThreadManager.TimeoutDefinition timeoutDefinition;
		}

		public class TimeoutDefinition
		{
			internal TimeoutDefinition(TimeSpan timeoutValue, Action<ThreadManager.ThreadInfo> timeoutCrashAction)
			{
				this.timeoutValue = timeoutValue;
				this.timeoutCrashAction = timeoutCrashAction;
			}

			internal TimeSpan TimeoutValue
			{
				get
				{
					return this.timeoutValue;
				}
			}

			internal Action<ThreadManager.ThreadInfo> TimeoutCrashAction
			{
				get
				{
					return this.timeoutCrashAction;
				}
			}

			private readonly TimeSpan timeoutValue;

			private readonly Action<ThreadManager.ThreadInfo> timeoutCrashAction;
		}

		public struct MethodFrame : IDisposable
		{
			internal MethodFrame(string methodName)
			{
				this = new ThreadManager.MethodFrame(methodName, null);
			}

			internal MethodFrame(string methodName, ThreadManager.TimeoutDefinition timeoutDefinition)
			{
				this.threadInfo = ThreadManager.Instance.GetOrCreateCurrentThreadDiagnosticInfo(out this.topLevelMethod);
				this.originalMethodName = this.threadInfo.MethodName;
				this.originalClientInformation = this.threadInfo.ClientInfo;
				this.originalTimeoutDefinition = this.threadInfo.TimeoutDefinition;
				this.threadInfo.MethodName = methodName;
				this.threadInfo.TimeoutDefinition = timeoutDefinition;
			}

			internal ThreadManager.ThreadInfo CurrentThreadInfo
			{
				get
				{
					return this.threadInfo;
				}
			}

			public void Dispose()
			{
				if (this.threadInfo != null)
				{
					if (this.topLevelMethod)
					{
						ThreadManager.Instance.RemoveCurrentThreadDiagnosticInfo();
					}
					else
					{
						this.threadInfo.MethodName = this.originalMethodName;
						this.threadInfo.ClientInfo = this.originalClientInformation;
						this.threadInfo.TimeoutDefinition = this.originalTimeoutDefinition;
					}
					this.threadInfo = null;
				}
			}

			private ThreadManager.ThreadInfo threadInfo;

			private bool topLevelMethod;

			private string originalMethodName;

			private ThreadManager.TimeoutDefinition originalTimeoutDefinition;

			private ThreadManager.ThreadDiagnosticInfo.ClientInformation originalClientInformation;
		}
	}
}
