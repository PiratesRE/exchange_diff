using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal class EmptyCount<TEntityType, TCountType> : ICount<TEntityType, TCountType>, IEquatable<EmptyCount<TEntityType, TCountType>> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		public EmptyCount(ICountedEntity<TEntityType> entity, TCountType measure)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			this.Entity = entity;
			this.Measure = measure;
		}

		public ICountedEntity<TEntityType> Entity { get; private set; }

		public ICountedConfig Config
		{
			get
			{
				return null;
			}
		}

		public TCountType Measure { get; private set; }

		public bool IsPromoted
		{
			get
			{
				return false;
			}
		}

		public long Total
		{
			get
			{
				return 0L;
			}
		}

		public long Average
		{
			get
			{
				return 0L;
			}
		}

		public ITrendline Trend
		{
			get
			{
				return null;
			}
		}

		public bool TryGetObject(string key, out object value)
		{
			value = null;
			return false;
		}

		public void SetObject(string key, object value)
		{
		}

		public bool Equals(EmptyCount<TEntityType, TCountType> other)
		{
			if (other == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			if (this.Entity.Equals(other.Entity))
			{
				TCountType measure = this.Measure;
				return measure.Equals(other.Measure);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			return obj != null && (object.ReferenceEquals(this, obj) || (obj is EmptyCount<TEntityType, TCountType> && this.Equals((EmptyCount<TEntityType, TCountType>)obj)));
		}

		public override int GetHashCode()
		{
			int num = this.Entity.GetHashCode() * 397;
			TCountType measure = this.Measure;
			return num ^ measure.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("Entity:{0}, Measure:{1}", this.Entity, this.Measure);
		}
	}
}
