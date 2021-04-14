using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ResourceHealthTracker : DisposeTrackableBase
	{
		public ResourceHealthTracker(ReservationBase reservation)
		{
			this.Reservation = reservation;
			this.openedContexts = new Stack<ResourceHealthTracker.BudgetCostHandle>();
			this.openedContexts.Push(ResourceHealthTracker.OuterContext);
		}

		public ReservationBase Reservation { get; private set; }

		public IDisposable Start()
		{
			return this.Start(CallChargeType.Include);
		}

		public IDisposable StartExclusive()
		{
			return this.Start(CallChargeType.Exclude);
		}

		public IDisposable Start(CallChargeType callChargeType)
		{
			return new ResourceHealthTracker.BudgetCostHandle(callChargeType, this);
		}

		public void Charge(uint bytes)
		{
			if (this.Reservation != null)
			{
				foreach (ResourceBase resourceBase in this.Reservation.ReservedResources)
				{
					resourceBase.Charge(bytes);
				}
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				lock (this.syncRoot)
				{
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ResourceHealthTracker>(this);
		}

		private void EnterContext(ResourceHealthTracker.BudgetCostHandle context)
		{
			lock (this.syncRoot)
			{
				ResourceHealthTracker.BudgetCostHandle currentContext = this.openedContexts.Peek();
				this.openedContexts.Push(context);
				this.OpenOrCloseCostHandles(currentContext, context);
			}
		}

		private void ExitContext(ResourceHealthTracker.BudgetCostHandle context)
		{
			lock (this.syncRoot)
			{
				this.openedContexts.Pop();
				ResourceHealthTracker.BudgetCostHandle newContext = this.openedContexts.Peek();
				this.OpenOrCloseCostHandles(context, newContext);
			}
		}

		private void StartCharging()
		{
			this.startChargeTimestamp = ExDateTime.UtcNow;
		}

		private void StopCharging()
		{
			double totalMilliseconds = (ExDateTime.UtcNow - this.startChargeTimestamp).TotalMilliseconds;
			this.startChargeTimestamp = ExDateTime.MinValue;
		}

		private void OpenOrCloseCostHandles(ResourceHealthTracker.BudgetCostHandle currentContext, ResourceHealthTracker.BudgetCostHandle newContext)
		{
			if (!currentContext.IsInclusive && newContext.IsInclusive)
			{
				this.StartCharging();
				return;
			}
			if (currentContext.IsInclusive && !newContext.IsInclusive)
			{
				this.StopCharging();
			}
		}

		private static readonly ResourceHealthTracker.BudgetCostHandle OuterContext = new ResourceHealthTracker.BudgetCostHandle(CallChargeType.Exclude, null);

		private readonly object syncRoot = new object();

		private ExDateTime startChargeTimestamp;

		private Stack<ResourceHealthTracker.BudgetCostHandle> openedContexts;

		private class BudgetCostHandle : DisposeTrackableBase
		{
			public BudgetCostHandle(CallChargeType callChargeType, ResourceHealthTracker ownerTracker)
			{
				this.callChargeType = callChargeType;
				this.ownerTracker = ownerTracker;
				if (this.ownerTracker != null)
				{
					this.ownerTracker.EnterContext(this);
				}
			}

			public bool IsInclusive
			{
				get
				{
					return this.callChargeType == CallChargeType.Include;
				}
			}

			protected override void InternalDispose(bool calledFromDispose)
			{
				if (calledFromDispose && this.ownerTracker != null)
				{
					this.ownerTracker.ExitContext(this);
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<ResourceHealthTracker.BudgetCostHandle>(this);
			}

			private CallChargeType callChargeType;

			private ResourceHealthTracker ownerTracker;
		}
	}
}
