using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PersonSchemaProperties
	{
		public PersonSchemaProperties(PropertyDefinition[] extendedProperties, params IEnumerable<PropertyDefinition>[] otherPropertySets)
		{
			if (extendedProperties != null)
			{
				StorePropertyDefinition[] array = new StorePropertyDefinition[extendedProperties.Length];
				for (int i = 0; i < extendedProperties.Length; i++)
				{
					array[i] = (StorePropertyDefinition)extendedProperties[i];
				}
				ApplicationAggregatedProperty item = new ApplicationAggregatedProperty(PersonSchema.ExtendedProperties, PersonPropertyAggregationStrategy.CreateExtendedPropertiesAggregation(array));
				HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
				hashSet.Add(item);
				if (otherPropertySets != null)
				{
					foreach (ICollection<PropertyDefinition> collection in otherPropertySets)
					{
						if (collection != null)
						{
							foreach (PropertyDefinition propertyDefinition in collection)
							{
								if (!object.Equals(propertyDefinition, PersonSchema.ExtendedProperties))
								{
									hashSet.Add(propertyDefinition);
								}
							}
						}
					}
				}
				this.All = new PropertyDefinition[hashSet.Count];
				hashSet.CopyTo(this.All);
				return;
			}
			this.All = PropertyDefinitionCollection.Merge<PropertyDefinition>(otherPropertySets);
		}

		public PropertyDefinition[] All { get; private set; }
	}
}
