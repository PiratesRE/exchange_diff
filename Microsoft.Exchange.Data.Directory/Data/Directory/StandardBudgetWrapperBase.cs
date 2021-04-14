using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class StandardBudgetWrapperBase<T> : BudgetWrapper<T>, IStandardBudget, IBudget, IDisposable where T : StandardBudget
	{
		internal StandardBudgetWrapperBase(T innerBudget) : base(innerBudget)
		{
		}

		public CostHandle StartConnection(string callerInfo)
		{
			CostHandle result;
			lock (this.instanceLock)
			{
				if (this.connectionCostHandle != null)
				{
					throw new InvalidOperationException("You can only have a single connection open against a budget wrapper at a time.");
				}
				this.connectionCostHandle = this.InternalStartConnection(callerInfo);
				base.AddAction(this.connectionCostHandle);
				result = this.connectionCostHandle;
			}
			return result;
		}

		protected virtual CostHandle InternalStartConnection(string callerInfo)
		{
			T innerBudget = base.GetInnerBudget();
			return innerBudget.StartConnection(new Action<CostHandle>(base.HandleCostHandleRelease), callerInfo);
		}

		public virtual void EndConnection()
		{
			lock (this.instanceLock)
			{
				if (this.connectionCostHandle != null)
				{
					this.connectionCostHandle.Dispose();
					this.connectionCostHandle = null;
				}
			}
		}

		private volatile CostHandle connectionCostHandle;
	}
}
