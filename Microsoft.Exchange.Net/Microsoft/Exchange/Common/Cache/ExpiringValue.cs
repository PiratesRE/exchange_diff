using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Common.Cache
{
	internal abstract class ExpiringValue<T, TProvider> : IExpiringValue where TProvider : IExpirationWindowProvider<T>, new()
	{
		public T Value { get; private set; }

		public DateTime ExpirationTime { get; private set; }

		protected ExpiringValue(T value)
		{
			this.Value = value;
			TProvider tprovider = (default(TProvider) == null) ? Activator.CreateInstance<TProvider>() : default(TProvider);
			this.ExpirationTime = (DateTime)ExDateTime.Now + tprovider.GetExpirationWindow(value);
		}

		public bool Expired
		{
			get
			{
				return this.ExpirationTime < (DateTime)ExDateTime.Now;
			}
		}
	}
}
