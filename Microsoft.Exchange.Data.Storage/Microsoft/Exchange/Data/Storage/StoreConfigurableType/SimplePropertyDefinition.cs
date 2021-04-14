using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.StoreConfigurableType
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SimplePropertyDefinition : ProviderPropertyDefinition, IEquatable<SimplePropertyDefinition>
	{
		public SimplePropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, object initialValue) : base(name, versionAdded, type, defaultValue)
		{
			this.PropertyDefinitionFlags = flags;
			this.InitialValue = initialValue;
		}

		public SimplePropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, object initialValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : base(name, versionAdded, type, defaultValue, readConstraints, writeConstraints)
		{
			this.PropertyDefinitionFlags = flags;
			this.InitialValue = initialValue;
		}

		public SimplePropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, object initialValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints, ProviderPropertyDefinition[] supportingProperties, CustomFilterBuilderDelegate customFilterBuilderDelegate, GetterDelegate getterDelegate, SetterDelegate setterDelegate) : base(name, versionAdded, type, defaultValue, readConstraints, writeConstraints, supportingProperties, customFilterBuilderDelegate, getterDelegate, setterDelegate)
		{
			this.PropertyDefinitionFlags = flags;
			this.InitialValue = initialValue;
		}

		public override bool IsMultivalued
		{
			get
			{
				return PropertyDefinitionFlags.None != (PropertyDefinitionFlags.MultiValued & this.PropertyDefinitionFlags);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return PropertyDefinitionFlags.None != (PropertyDefinitionFlags.ReadOnly & this.PropertyDefinitionFlags);
			}
		}

		public override bool IsCalculated
		{
			get
			{
				return PropertyDefinitionFlags.None != (PropertyDefinitionFlags.Calculated & this.PropertyDefinitionFlags);
			}
		}

		public override bool IsFilterOnly
		{
			get
			{
				return PropertyDefinitionFlags.None != (PropertyDefinitionFlags.FilterOnly & this.PropertyDefinitionFlags);
			}
		}

		public override bool IsMandatory
		{
			get
			{
				return PropertyDefinitionFlags.None != (PropertyDefinitionFlags.Mandatory & this.PropertyDefinitionFlags);
			}
		}

		public override bool PersistDefaultValue
		{
			get
			{
				return PropertyDefinitionFlags.None != (PropertyDefinitionFlags.PersistDefaultValue & this.PropertyDefinitionFlags);
			}
		}

		public override bool IsWriteOnce
		{
			get
			{
				return PropertyDefinitionFlags.None != (PropertyDefinitionFlags.WriteOnce & this.PropertyDefinitionFlags);
			}
		}

		public override bool IsBinary
		{
			get
			{
				return false;
			}
		}

		public PropertyDefinitionFlags PropertyDefinitionFlags { get; private set; }

		public object InitialValue { get; private set; }

		public override int GetHashCode()
		{
			if (!string.IsNullOrEmpty(base.Name))
			{
				return base.Name.GetHashCode();
			}
			return string.Empty.GetHashCode();
		}

		public bool Equals(SimplePropertyDefinition other)
		{
			return !object.ReferenceEquals(null, other) && string.Equals(base.Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SimplePropertyDefinition);
		}
	}
}
