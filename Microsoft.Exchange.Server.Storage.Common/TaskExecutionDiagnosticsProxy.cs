using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public sealed class TaskExecutionDiagnosticsProxy : IExecutionDiagnostics
	{
		public IExecutionDiagnostics ExecutionDiagnostics
		{
			get
			{
				return this.executionDiagnostics;
			}
			set
			{
				this.executionDiagnostics = value;
			}
		}

		internal static IDisposable SetUnhandledExceptionTestHook(Action action)
		{
			return TaskExecutionDiagnosticsProxy.unhandledExceptionTestHook.SetTestHook(action);
		}

		public string DiagnosticInformationForWatsonReport
		{
			get
			{
				if (this.ExecutionDiagnostics == null)
				{
					return string.Empty;
				}
				return this.ExecutionDiagnostics.DiagnosticInformationForWatsonReport;
			}
		}

		int IExecutionDiagnostics.MailboxNumber
		{
			get
			{
				if (this.ExecutionDiagnostics == null)
				{
					return 0;
				}
				return this.ExecutionDiagnostics.MailboxNumber;
			}
		}

		byte IExecutionDiagnostics.OperationId
		{
			get
			{
				if (this.ExecutionDiagnostics == null)
				{
					return 0;
				}
				return this.ExecutionDiagnostics.OperationId;
			}
		}

		byte IExecutionDiagnostics.OperationType
		{
			get
			{
				if (this.ExecutionDiagnostics == null)
				{
					return 0;
				}
				return this.ExecutionDiagnostics.OperationType;
			}
		}

		byte IExecutionDiagnostics.ClientType
		{
			get
			{
				if (this.ExecutionDiagnostics == null)
				{
					return 0;
				}
				return this.ExecutionDiagnostics.ClientType;
			}
		}

		byte IExecutionDiagnostics.OperationFlags
		{
			get
			{
				if (this.ExecutionDiagnostics == null)
				{
					return 0;
				}
				return this.ExecutionDiagnostics.OperationFlags;
			}
		}

		int IExecutionDiagnostics.CorrelationId
		{
			get
			{
				if (this.ExecutionDiagnostics == null)
				{
					return 0;
				}
				return this.ExecutionDiagnostics.CorrelationId;
			}
		}

		public void OnExceptionCatch(Exception exception)
		{
			this.OnExceptionCatch(exception, null);
		}

		public void OnExceptionCatch(Exception exception, object diagnosticData)
		{
			if (this.ExecutionDiagnostics != null)
			{
				this.ExecutionDiagnostics.OnExceptionCatch(exception, diagnosticData);
				return;
			}
			ErrorHelper.OnExceptionCatch(0, 0, 0, 0, -1, exception, diagnosticData);
		}

		public void OnUnhandledException(Exception exception)
		{
			FaultInjection.InjectFault(TaskExecutionDiagnosticsProxy.unhandledExceptionTestHook);
			if (this.ExecutionDiagnostics != null)
			{
				this.ExecutionDiagnostics.OnUnhandledException(exception);
			}
		}

		public TaskExecutionDiagnosticsProxy.TaskExecutionDiagnosticsScope NewDiagnosticsScope()
		{
			return new TaskExecutionDiagnosticsProxy.TaskExecutionDiagnosticsScope(this);
		}

		private static Hookable<Action> unhandledExceptionTestHook = Hookable<Action>.Create(true, null);

		private IExecutionDiagnostics executionDiagnostics;

		public struct TaskExecutionDiagnosticsScope : IDisposable
		{
			internal TaskExecutionDiagnosticsScope(TaskExecutionDiagnosticsProxy proxyExecutionContext)
			{
				this.proxyContext = proxyExecutionContext;
				this.savedExecutionDiagnostics = proxyExecutionContext.ExecutionDiagnostics;
			}

			public void Dispose()
			{
				if (this.proxyContext != null)
				{
					this.proxyContext.ExecutionDiagnostics = this.savedExecutionDiagnostics;
				}
			}

			private TaskExecutionDiagnosticsProxy proxyContext;

			private IExecutionDiagnostics savedExecutionDiagnostics;
		}
	}
}
