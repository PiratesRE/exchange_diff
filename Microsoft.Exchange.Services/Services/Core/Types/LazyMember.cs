using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class LazyMember<T>
	{
		public LazyMember(InitializeLazyMember<T> initializationDelegate)
		{
			this.initializationDelegate = initializationDelegate;
		}

		public T Member
		{
			get
			{
				if (!this.initialized)
				{
					lock (this.lockObject)
					{
						if (!this.initialized)
						{
							this.lazyMember = this.initializationDelegate();
						}
						this.initialized = true;
					}
				}
				return this.lazyMember;
			}
		}

		private T lazyMember;

		private InitializeLazyMember<T> initializationDelegate;

		private object lockObject = new object();

		private bool initialized;
	}
}
