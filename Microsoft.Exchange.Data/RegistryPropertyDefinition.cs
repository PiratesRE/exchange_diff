using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class RegistryPropertyDefinition : SimpleProviderPropertyDefinition
	{
		public RegistryPropertyDefinition(string name, Type type, object defaultValue) : this(name, type, PropertyDefinitionFlags.None, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None)
		{
		}

		public RegistryPropertyDefinition(string name, Type type, PropertyDefinitionFlags flags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : this(name, type, flags, defaultValue, readConstraints, writeConstraints, ProviderPropertyDefinition.None, null, null, null)
		{
		}

		internal RegistryPropertyDefinition(string name, Type type, PropertyDefinitionFlags flags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints, ProviderPropertyDefinition[] supportingProperties, CustomFilterBuilderDelegate customFilterBuilderDelegate, GetterDelegate getterDelegate, SetterDelegate setterDelegate) : base(name, ExchangeObjectVersion.Exchange2003, type, (defaultValue != null) ? (flags | PropertyDefinitionFlags.PersistDefaultValue) : flags, defaultValue, readConstraints, writeConstraints, supportingProperties, customFilterBuilderDelegate, getterDelegate, setterDelegate)
		{
		}

		public override bool Equals(ProviderPropertyDefinition other)
		{
			return object.ReferenceEquals(other, this) || (base.Equals(other) && (other as RegistryPropertyDefinition).Flags == base.Flags);
		}

		public new static RegistryPropertyDefinition[] None = new RegistryPropertyDefinition[0];
	}
}
