using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class Operation
	{
		public Operation(string debugText, Action action, IExecutionInfo executionInfo, int maxTransientExceptions = 5)
		{
			Operation <>4__this = this;
			ArgumentValidator.ThrowIfNullOrEmpty("debugText", debugText);
			ArgumentValidator.ThrowIfNull("action", action);
			ArgumentValidator.ThrowIfNull("executionInfo", executionInfo);
			ArgumentValidator.ThrowIfZeroOrNegative("maxTransientExceptions", maxTransientExceptions);
			this.debugText = debugText;
			this.executionInfo = executionInfo;
			this.maxTransientExceptions = maxTransientExceptions;
			this.action = delegate()
			{
				<>4__this.ActionWrapper(action);
			};
		}

		public IExecutionInfo ExecutionInfo
		{
			get
			{
				return this.executionInfo;
			}
		}

		public static async Task InvokeOperationsAsync(IEnumerable<Operation> operations, TimeSpan timeout)
		{
			ArgumentValidator.ThrowIfNull("operations", operations);
			ArgumentValidator.ThrowIfInvalidValue<TimeSpan>("timeout", timeout, (TimeSpan timeoutPeriod) => timeoutPeriod > TimeSpan.Zero);
			foreach (Operation operation3 in operations)
			{
				if (operation3 != null)
				{
					operation3.Start();
				}
			}
			Task allOperationsTask = Task.WhenAll(from operation in operations
			where operation != null && operation.task != null
			select operation into op
			select op.task);
			Task timeoutTask = Task.Delay(timeout);
			await Task.WhenAny(new Task[]
			{
				allOperationsTask,
				timeoutTask
			});
			List<Exception> exceptions = new List<Exception>();
			foreach (Operation operation2 in operations)
			{
				if (operation2 != null)
				{
					Exception ex = operation2.End();
					if (ex != null)
					{
						exceptions.Add(ex);
					}
				}
			}
			if (exceptions.Any<Exception>())
			{
				throw new AggregateException(exceptions);
			}
		}

		private void ActionWrapper(Action originalAction)
		{
			this.executionInfo.OnStart();
			try
			{
				originalAction();
			}
			catch (TransientException ex)
			{
				this.exceptionList.Add(ex);
				this.executionInfo.OnException(ex);
			}
			finally
			{
				this.executionInfo.OnFinish();
			}
		}

		private void Start()
		{
			if (this.ShouldInvoke())
			{
				this.task = Task.Run(this.action);
			}
		}

		private Exception End()
		{
			if (this.task == null)
			{
				return null;
			}
			if (this.task.Status == TaskStatus.Running)
			{
				throw new TimeoutException(this.debugText);
			}
			if (this.exceptionList.Count >= this.maxTransientExceptions)
			{
				throw new AggregateException("Too many transient exceptions were thrown. Debug info:" + this.debugText, this.exceptionList);
			}
			return this.task.Exception;
		}

		private bool ShouldInvoke()
		{
			return this.task == null || this.task.IsCompleted;
		}

		private readonly int maxTransientExceptions;

		private readonly List<Exception> exceptionList = new List<Exception>();

		private readonly Action action;

		private readonly IExecutionInfo executionInfo;

		private readonly string debugText;

		private Task task;
	}
}
