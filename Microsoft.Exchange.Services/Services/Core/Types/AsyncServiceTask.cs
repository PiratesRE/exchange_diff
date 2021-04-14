using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class AsyncServiceTask<T> : ServiceTask<T>
	{
		internal AsyncServiceTask(BaseRequest request, CallContext callContext, ServiceAsyncResult<T> serviceAsyncResult) : base(request, callContext, serviceAsyncResult)
		{
			this.requestStart = ExDateTime.UtcNow;
			this.budgetKey = base.CallContext.Budget.Owner;
		}

		protected internal override void InternalComplete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			if (this.executionException == null)
			{
				this.queueAndDelayTime = queueAndDelayTime;
				return;
			}
			this.FinishRequest("[CWE]", queueAndDelayTime, ExDateTime.UtcNow - this.requestStart, this.executionException);
		}

		protected internal override TaskExecuteResult InternalExecute(TimeSpan queueAndDelay, TimeSpan totalTime)
		{
			IAsyncServiceCommand asyncServiceCommand = base.Request.ServiceCommand as IAsyncServiceCommand;
			asyncServiceCommand.CompleteRequestAsyncCallback = new CompleteRequestAsyncCallback(this.CompleteRequestCallback);
			TaskExecuteResult result = base.InternalExecute(queueAndDelay, totalTime);
			if (this.budget != null)
			{
				this.budget.LogEndStateToIIS();
				this.budget.Dispose();
				this.budget = null;
			}
			return result;
		}

		protected internal override void InternalTimeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			base.InternalTimeout(queueAndDelayTime, totalTime);
			if (this.budget != null)
			{
				this.budget.LogEndStateToIIS();
				this.budget.Dispose();
				this.budget = null;
			}
		}

		protected internal override void InternalCancel()
		{
			base.InternalCancel();
			if (this.budget != null)
			{
				this.budget.LogEndStateToIIS();
				this.budget.Dispose();
				this.budget = null;
			}
		}

		private void CompleteRequestCallback(Exception exception)
		{
			ExTraceGlobals.ThrottlingTracer.TraceDebug<string>((long)this.GetHashCode(), "[AsyncServiceTask.CompleteRequestCallback] Hanging request completed for task [{0}]", base.Description);
			this.FinishRequest("[C]", this.queueAndDelayTime, ExDateTime.UtcNow - this.requestStart, exception);
		}

		public override IBudget Budget
		{
			get
			{
				IBudget budget = null;
				try
				{
					budget = base.Budget;
				}
				catch (ObjectDisposedException)
				{
				}
				if (budget == null)
				{
					this.budget = EwsBudget.Acquire(this.budgetKey);
					budget = this.budget;
				}
				return budget;
			}
		}

		protected override void WriteThrottlingDiagnostics(string logType, TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
		}

		private ExDateTime requestStart;

		private TimeSpan queueAndDelayTime;

		private BudgetKey budgetKey;

		private IEwsBudget budget;
	}
}
