using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class PropertyUpdateXML : XMLSerializableBase
	{
		[XmlElement("Operation")]
		public PropertyUpdateOperation Operation
		{
			get
			{
				return this.operation;
			}
			set
			{
				this.operation = value;
			}
		}

		[XmlElement("Property")]
		public PropertyXML Property
		{
			get
			{
				return this.property;
			}
			set
			{
				this.property = value;
			}
		}

		internal static void Add(List<PropertyUpdateXML> updates, ProviderPropertyDefinition pdef, object value, PropertyUpdateOperation op)
		{
			updates.Add(new PropertyUpdateXML
			{
				Operation = op,
				Property = PropertyXML.Create(pdef, value)
			});
		}

		internal static void Apply(ICollection<PropertyUpdateXML> updates, ConfigurableObject targetObject)
		{
			if (updates == null)
			{
				return;
			}
			foreach (PropertyUpdateXML propertyUpdateXML in updates)
			{
				ProviderPropertyDefinition providerPropertyDefinition = null;
				foreach (PropertyDefinition propertyDefinition in targetObject.ObjectSchema.AllProperties)
				{
					if (string.Compare(propertyDefinition.Name, propertyUpdateXML.Property.PropertyName, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						providerPropertyDefinition = (propertyDefinition as ProviderPropertyDefinition);
						break;
					}
				}
				if (providerPropertyDefinition == null)
				{
					MrsTracer.Common.Warning("Ignoring property update for '{0}', no such property found in the schema.", new object[]
					{
						propertyUpdateXML.Property.PropertyName
					});
				}
				else
				{
					propertyUpdateXML.Property.TryApplyChange(providerPropertyDefinition, targetObject, propertyUpdateXML.Operation);
				}
			}
		}

		private PropertyUpdateOperation operation;

		private PropertyXML property;
	}
}
