using System;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal sealed class Delayed<T>
	{
		public Delayed(Func<T> initializer)
		{
			this.initializer = initializer;
		}

		public T Value
		{
			get
			{
				this.InitializeIfNeeded();
				return this.value;
			}
		}

		public static implicit operator T(Delayed<T> t)
		{
			return t.Value;
		}

		private void InitializeIfNeeded()
		{
			if (!this.initialized)
			{
				lock (this.locker)
				{
					if (!this.initialized)
					{
						this.value = this.initializer();
						this.initialized = true;
					}
				}
			}
		}

		private T value;

		private Func<T> initializer;

		private bool initialized;

		private object locker = new object();
	}
}
