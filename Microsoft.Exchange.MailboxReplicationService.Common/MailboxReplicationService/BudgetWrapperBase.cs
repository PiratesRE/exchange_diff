using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class BudgetWrapperBase<T> : DisposableWrapper<T> where T : class, IDisposable
	{
		public BudgetWrapperBase(T wrappedObject, bool ownsObject, Func<IDisposable> createCostHandleDelegate, Action<uint> chargeDelegate) : base(wrappedObject, ownsObject)
		{
			this.CreateCostHandle = createCostHandleDelegate;
			this.Charge = chargeDelegate;
		}

		public Func<IDisposable> CreateCostHandle { get; private set; }

		public Action<uint> Charge { get; private set; }
	}
}
