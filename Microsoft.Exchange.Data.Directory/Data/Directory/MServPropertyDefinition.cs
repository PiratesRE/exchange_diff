using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class MServPropertyDefinition : SimpleProviderPropertyDefinition
	{
		internal MServPropertyDefinition(string name, Type type, PropertyDefinitionFlags flags, object defaultValue, ProviderPropertyDefinition[] supportingProperties, GetterDelegate getterDelegate = null, SetterDelegate setterDelegate = null) : this(name, ExchangeObjectVersion.Exchange2003, type, flags, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, supportingProperties, getterDelegate, setterDelegate)
		{
		}

		internal MServPropertyDefinition(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints, ProviderPropertyDefinition[] supportingProperties, GetterDelegate getterDelegate, SetterDelegate setterDelegate) : base(name, versionAdded, type, flags, defaultValue, readConstraints, writeConstraints, supportingProperties, null, getterDelegate, setterDelegate)
		{
		}

		public override int GetHashCode()
		{
			if (this.hashcode == 0)
			{
				this.hashcode = base.Name.GetHashCodeCaseInsensitive();
			}
			return this.hashcode;
		}

		internal static MServPropertyDefinition RawRecordPropertyDefinition(string name, PropertyDefinitionFlags flags = PropertyDefinitionFlags.None)
		{
			return new MServPropertyDefinition(name, ExchangeObjectVersion.Exchange2003, typeof(MservRecord), flags, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null);
		}
	}
}
