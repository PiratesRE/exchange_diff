using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal class LazyMember<T>
	{
		public LazyMember(Func<T> initializationDelegate)
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
							this.initialized = true;
						}
					}
				}
				return this.lazyMember;
			}
		}

		public override string ToString()
		{
			string result = null;
			if (this.Member != null)
			{
				T member = this.Member;
				result = member.ToString();
			}
			return result;
		}

		private T lazyMember;

		private Func<T> initializationDelegate;

		private object lockObject = new object();

		private bool initialized;
	}
}
