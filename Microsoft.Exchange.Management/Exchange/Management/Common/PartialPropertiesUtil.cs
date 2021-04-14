using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	internal static class PartialPropertiesUtil
	{
		public static IDictionary<string, PropertyDefinition> GetOutputPropertiesToDefinitionDict(ObjectSchema objectSchema, Type publicObjectType, IDictionary<string, string> propertyNameToDefinitionNameDict)
		{
			IDictionary<string, PropertyDefinition> dictionary = new Dictionary<string, PropertyDefinition>(StringComparer.OrdinalIgnoreCase);
			PropertyInfo[] properties = publicObjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			Dictionary<string, PropertyDefinition> dictionary2 = objectSchema.AllProperties.ToDictionary((PropertyDefinition property) => property.Name, StringComparer.OrdinalIgnoreCase);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanRead)
				{
					string name = propertyInfo.Name;
					if (!dictionary.ContainsKey(propertyInfo.Name))
					{
						string key;
						if (propertyNameToDefinitionNameDict == null || !propertyNameToDefinitionNameDict.TryGetValue(name, out key))
						{
							key = name;
						}
						PropertyDefinition propertyDefinition;
						if (dictionary2.TryGetValue(key, out propertyDefinition) && propertyDefinition is ADPropertyDefinition)
						{
							dictionary.Add(name, propertyDefinition);
						}
					}
				}
			}
			return dictionary;
		}

		public static IList<string> ParseUserSpecifiedProperties(string[] userSpecifiedProperties)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if (userSpecifiedProperties != null)
			{
				if (userSpecifiedProperties.Length == 1)
				{
					userSpecifiedProperties = userSpecifiedProperties[0].Split(new char[]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries);
				}
				foreach (string text in userSpecifiedProperties)
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						hashSet.TryAdd(text.Trim());
					}
				}
			}
			return hashSet.ToList<string>();
		}

		public static bool ValidateProperties(IList<string> userSpecifiedProperties, IDictionary<string, PropertyDefinition> outputPropertiesToDefinitionDict, out LocalizedString errorMessage)
		{
			string[] array = (from property in userSpecifiedProperties
			where !outputPropertiesToDefinitionDict.ContainsKey(property)
			select property).ToArray<string>();
			errorMessage = LocalizedString.Empty;
			if (array.Length > 0)
			{
				errorMessage = Strings.ErrorPropertiesInvalid(string.Join(",", array));
				return false;
			}
			return true;
		}

		public static List<PropertyDefinition> CalculatePropertiesToRead(IDictionary<string, PropertyDefinition> outputPropertiesToDefinitionDic, IList<string> userSpecifiedProperties, IList<PropertyDefinition> mandatoryOutputProperties, IDictionary<PropertyDefinition, IList<PropertyDefinition>> propertyRelationship, IList<PropertyDefinition> specialPropertiesLeadToAllRead, WriteVerboseDelegate verboseDelegate)
		{
			List<PropertyDefinition> propertiesToRead = (from property in userSpecifiedProperties
			select outputPropertiesToDefinitionDic[property]).ToList<PropertyDefinition>();
			if (mandatoryOutputProperties != null)
			{
				propertiesToRead.AddRange(from property in mandatoryOutputProperties
				where !propertiesToRead.Contains(property)
				select property);
			}
			if (propertyRelationship != null)
			{
				IEnumerable<IList<PropertyDefinition>> enumerable = from property in propertyRelationship
				where propertiesToRead.Contains(property.Key)
				select property into keyValue
				select keyValue.Value;
				foreach (IList<PropertyDefinition> list in enumerable)
				{
					PropertyDefinition[] source = (PropertyDefinition[])list;
					propertiesToRead.AddRange(from property in source
					where !propertiesToRead.Contains(property)
					select property);
				}
			}
			if (specialPropertiesLeadToAllRead != null)
			{
				foreach (PropertyDefinition propertyDefinition in specialPropertiesLeadToAllRead)
				{
					if (propertiesToRead.Contains(propertyDefinition))
					{
						if (verboseDelegate != null)
						{
							verboseDelegate(Strings.VerboseAllPropertiesAreRead(propertyDefinition.Name));
						}
						return null;
					}
				}
			}
			if (propertiesToRead != null && verboseDelegate != null)
			{
				verboseDelegate(Strings.VerbosePropertiesRead(string.Join<PropertyDefinition>(",", propertiesToRead)));
			}
			return propertiesToRead;
		}
	}
}
