using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class XsoDriverPropertyDefinition : SimpleProviderPropertyDefinition
	{
		public object InitialValue { get; private set; }

		public StorePropertyDefinition StorePropertyDefinition { get; private set; }

		public XsoDriverPropertyDefinition(StorePropertyDefinition storePropertyDefinition, string name, ExchangeObjectVersion versionAdded, PropertyDefinitionFlags flags, object defaultValue, object initialValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : base(name, versionAdded, XsoDriverPropertyDefinition.WrapValueTypeByNullable(storePropertyDefinition.Type), flags, defaultValue, readConstraints, XsoDriverPropertyDefinition.MergeWithXsoConstraints(writeConstraints, storePropertyDefinition))
		{
			this.InitialValue = initialValue;
			this.StorePropertyDefinition = storePropertyDefinition;
		}

		private static Type WrapValueTypeByNullable(Type propType)
		{
			if (null == propType)
			{
				throw new ArgumentNullException("propType");
			}
			if (!propType.IsValueType)
			{
				return propType;
			}
			if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return propType;
			}
			return typeof(Nullable<>).MakeGenericType(new Type[]
			{
				propType
			});
		}

		private static PropertyDefinitionConstraint[] MergeWithXsoConstraints(PropertyDefinitionConstraint[] constraints, StorePropertyDefinition storePropertyDefinition)
		{
			if (storePropertyDefinition.Constraints.Count == 0)
			{
				return constraints;
			}
			if (constraints == null || constraints.Length == 0)
			{
				return new PropertyDefinitionConstraint[]
				{
					new XsoDriverPropertyConstraint(storePropertyDefinition)
				};
			}
			return new List<PropertyDefinitionConstraint>(constraints)
			{
				new XsoDriverPropertyConstraint(storePropertyDefinition)
			}.ToArray();
		}

		public override bool Equals(ProviderPropertyDefinition other)
		{
			if (object.ReferenceEquals(other, this))
			{
				return true;
			}
			XsoDriverPropertyDefinition xsoDriverPropertyDefinition = other as XsoDriverPropertyDefinition;
			return xsoDriverPropertyDefinition != null && object.Equals(this.InitialValue, xsoDriverPropertyDefinition.InitialValue) && this.StorePropertyDefinition.CompareTo(xsoDriverPropertyDefinition.StorePropertyDefinition) == 0 && base.Equals(other);
		}
	}
}
