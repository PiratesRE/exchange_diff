using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class UnresolvedPropertyDefinition : PropertyDefinition, IComparable<UnresolvedPropertyDefinition>, IComparable
	{
		private UnresolvedPropertyDefinition(PropTag propTag) : base(string.Empty, InternalSchema.ClrTypeFromPropTag(propTag))
		{
			this.propertyTag = (uint)propTag;
		}

		public static UnresolvedPropertyDefinition Create(PropTag propTag)
		{
			PropType propType = propTag.ValueType();
			if (!EnumValidator<PropType>.IsValidValue(propType))
			{
				throw new EnumArgumentException(string.Format("Invalid property type {0}", propType), "propTag");
			}
			return new UnresolvedPropertyDefinition(propTag);
		}

		public uint PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public override int GetHashCode()
		{
			return (int)this.propertyTag;
		}

		public override string ToString()
		{
			return string.Format("[{0:x8}] {1}", this.propertyTag, base.Name);
		}

		public int CompareTo(UnresolvedPropertyDefinition other)
		{
			if (other == null)
			{
				throw new ArgumentException(ServerStrings.ObjectMustBeOfType(base.GetType().Name));
			}
			return this.PropertyTag.CompareTo(other.PropertyTag);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			UnresolvedPropertyDefinition other = obj as UnresolvedPropertyDefinition;
			return this.CompareTo(other);
		}

		private readonly uint propertyTag;
	}
}
