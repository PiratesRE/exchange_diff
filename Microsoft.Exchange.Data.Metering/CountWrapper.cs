using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal class CountWrapper<TEntityType, TCountType> : ICount<TEntityType, TCountType> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		public CountWrapper(Count<TEntityType, TCountType> count)
		{
			ArgumentValidator.ThrowIfNull("count", count);
			this.count = count;
		}

		public ICountedEntity<TEntityType> Entity
		{
			get
			{
				return this.count.Entity;
			}
		}

		public ICountedConfig Config
		{
			get
			{
				return this.count.Config;
			}
		}

		public TCountType Measure
		{
			get
			{
				return this.count.Measure;
			}
		}

		public bool IsPromoted
		{
			get
			{
				return this.count.IsPromoted;
			}
		}

		public long Total
		{
			get
			{
				return this.count.Total;
			}
		}

		public long Average
		{
			get
			{
				return this.count.Average;
			}
		}

		public ITrendline Trend
		{
			get
			{
				return this.count.Trend;
			}
		}

		public bool TryGetObject(string key, out object value)
		{
			return this.count.TryGetObject(key, out value);
		}

		public void SetObject(string key, object value)
		{
			this.count.SetObject(key, value);
		}

		public override string ToString()
		{
			return this.count.ToString();
		}

		private Count<TEntityType, TCountType> count;
	}
}
