using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	[Serializable]
	internal class HygienePropertyDefinition : ADPropertyDefinition
	{
		public HygienePropertyDefinition(string name, Type type) : this(name, type, type.IsValueType ? Activator.CreateInstance(type) : ((type == typeof(string)) ? string.Empty : null), ADPropertyDefinitionFlags.None)
		{
		}

		public HygienePropertyDefinition(string name, Type type, object defaultValue, ADPropertyDefinitionFlags flags = ADPropertyDefinitionFlags.PersistDefaultValue) : this(name, type, defaultValue, ExchangeObjectVersion.Exchange2010, flags)
		{
		}

		public HygienePropertyDefinition(string name, Type type, object defaultValue, ExchangeObjectVersion version, ADPropertyDefinitionFlags flags = ADPropertyDefinitionFlags.PersistDefaultValue) : base(name, version, type, name, flags, defaultValue, new PropertyDefinitionConstraint[0], new PropertyDefinitionConstraint[0], null, null)
		{
		}

		public HygienePropertyDefinition(string name, Type type, object defaultValue, ExchangeObjectVersion version, string displayName, ADPropertyDefinitionFlags flags) : base(name, version, type, displayName, flags, defaultValue, new PropertyDefinitionConstraint[0], new PropertyDefinitionConstraint[0], null, null)
		{
		}
	}
}
