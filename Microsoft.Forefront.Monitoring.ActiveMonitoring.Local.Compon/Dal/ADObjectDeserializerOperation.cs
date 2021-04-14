using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class ADObjectDeserializerOperation : DeserializerOperation
	{
		protected override object DeserializedValue(Type parameterType, Type[] additionalTypes)
		{
			IConfigurable configurable = (IConfigurable)Activator.CreateInstance(parameterType);
			IEnumerable<PropertyDefinition> source = ADObjectDeserializerOperation.GetPropertyDefinitions(configurable).ToArray<PropertyDefinition>();
			using (IEnumerator<XElement> enumerator = base.DataObject.Elements().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XElement xElement = enumerator.Current;
					PropertyDefinition propertyDefinition = source.FirstOrDefault((PropertyDefinition pd) => string.Equals(pd.Name, xElement.Name.LocalName, StringComparison.OrdinalIgnoreCase));
					if (propertyDefinition != null)
					{
						DalHelper.SetPropertyValue(xElement.Value, propertyDefinition, configurable);
					}
				}
			}
			return configurable;
		}

		private static IEnumerable<PropertyDefinition> GetPropertyDefinitions(IConfigurable iconfigObj)
		{
			ConfigurableObject configurableObject = iconfigObj as ConfigurableObject;
			if (configurableObject != null)
			{
				return configurableObject.ObjectSchema.AllProperties;
			}
			ConfigurablePropertyBag configurablePropertyBag = (ConfigurablePropertyBag)iconfigObj;
			return configurablePropertyBag.GetPropertyDefinitions(false);
		}
	}
}
