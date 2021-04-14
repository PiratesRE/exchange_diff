using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class XsoMailboxConfigurationObjectSchema : UserConfigurationObjectSchema
	{
		public XsoMailboxConfigurationObjectSchema()
		{
			this.xsoPropertyMappings = new Dictionary<PropertyDefinition, XsoDriverPropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in base.AllProperties)
			{
				XsoDriverPropertyDefinition xsoDriverPropertyDefinition = propertyDefinition as XsoDriverPropertyDefinition;
				if (xsoDriverPropertyDefinition != null)
				{
					if (this.xsoPropertyMappings.ContainsKey(xsoDriverPropertyDefinition.StorePropertyDefinition))
					{
						throw new NotSupportedException("One XSO property is mapping to multiple XSO driver property.");
					}
					this.xsoPropertyMappings[xsoDriverPropertyDefinition.StorePropertyDefinition] = xsoDriverPropertyDefinition;
				}
			}
			this.cachedXsoProperties = this.xsoPropertyMappings.Keys.ToArray<PropertyDefinition>();
		}

		public PropertyDefinition[] AllDependentXsoProperties
		{
			get
			{
				return (PropertyDefinition[])this.cachedXsoProperties.Clone();
			}
		}

		public XsoDriverPropertyDefinition GetRelatedWrapperProperty(PropertyDefinition def)
		{
			if (def == null)
			{
				throw new ArgumentNullException("def");
			}
			if (!(def is StorePropertyDefinition))
			{
				throw new NotSupportedException("GetRelatedWrapperProperty: def: " + def.GetType().FullName);
			}
			XsoDriverPropertyDefinition result;
			if (this.xsoPropertyMappings.TryGetValue(def, out result))
			{
				return result;
			}
			return null;
		}

		private Dictionary<PropertyDefinition, XsoDriverPropertyDefinition> xsoPropertyMappings;

		private PropertyDefinition[] cachedXsoProperties;

		public static readonly SimpleProviderPropertyDefinition MailboxOwnerId = new SimpleProviderPropertyDefinition("MailboxOwnerId", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), PropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
