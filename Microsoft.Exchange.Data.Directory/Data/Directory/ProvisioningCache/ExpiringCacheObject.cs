using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	[Serializable]
	internal class ExpiringCacheObject
	{
		public ExpiringCacheObject(TimeSpan expirationTime)
		{
			this.expirationTime = expirationTime;
		}

		public ExpiringCacheObject(TimeSpan expirationTime, object data) : this(expirationTime)
		{
			this.Data = data;
		}

		public object Data
		{
			get
			{
				return this.data;
			}
			set
			{
				IDisposable disposable = value as IDisposable;
				if (disposable != null)
				{
					throw new ArgumentException("IDisposable type object is not allowed to be cached in ProvisioningCache.");
				}
				this.data = value;
				this.hasValue = true;
				this.lastUpdateTime = ExDateTime.UtcNow;
			}
		}

		public bool IsExpired
		{
			get
			{
				if (!this.hasValue)
				{
					return true;
				}
				ExDateTime utcNow = ExDateTime.UtcNow;
				return utcNow - this.lastUpdateTime > this.expirationTime;
			}
		}

		private TimeSpan expirationTime = TimeSpan.MinValue;

		private object data;

		private bool hasValue;

		private ExDateTime lastUpdateTime = ExDateTime.MinValue;
	}
}
