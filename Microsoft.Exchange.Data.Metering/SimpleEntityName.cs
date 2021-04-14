using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal class SimpleEntityName<TEntityType> : IEntityName<TEntityType>, IEquatable<IEntityName<TEntityType>> where TEntityType : struct, IConvertible
	{
		public SimpleEntityName(TEntityType type, string value)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("value", value);
			this.Value = value;
			this.Type = type;
		}

		public TEntityType Type { get; private set; }

		public string Value { get; private set; }

		public bool Equals(IEntityName<TEntityType> other)
		{
			if (other == null)
			{
				return false;
			}
			SimpleEntityName<TEntityType> simpleEntityName = other as SimpleEntityName<TEntityType>;
			if (simpleEntityName == null)
			{
				return false;
			}
			TEntityType type = this.Type;
			return type.Equals(other.Type) && this.Value.Equals(simpleEntityName.Value);
		}

		public override bool Equals(object obj)
		{
			return obj is IEntityName<TEntityType> && this.Equals(obj as IEntityName<TEntityType>);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}-{1}", this.Type, this.Value);
		}
	}
}
