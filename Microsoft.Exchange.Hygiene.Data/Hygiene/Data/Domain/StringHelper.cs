using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	internal static class StringHelper
	{
		public static string ConvertToString(this ConfigurablePropertyBag configurableObj)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}- ", configurableObj.GetType().Name);
			foreach (PropertyDefinition propertyDefinition in configurableObj.GetPropertyDefinitions(false))
			{
				if (propertyDefinition == DomainSchema.PropertiesAsId)
				{
					stringBuilder.Append("Properties(EntityId:PropertyId:PropertyValue)=");
					Dictionary<int, Dictionary<int, string>> dictionary = configurableObj[DomainSchema.PropertiesAsId] as Dictionary<int, Dictionary<int, string>>;
					if (dictionary != null)
					{
						stringBuilder.Append(dictionary.ConvertToString());
					}
				}
				else
				{
					stringBuilder.AppendFormat("{0}={1}, ", propertyDefinition.Name, configurableObj[propertyDefinition]);
				}
			}
			return stringBuilder.ToString();
		}

		public static string ConvertToString<T>(this IEnumerable<T> items)
		{
			return string.Join<T>(";", items);
		}

		public static string ConvertToString(this Dictionary<int, Dictionary<int, string>> properties)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int num in properties.Keys)
			{
				if (properties[num] != null && properties[num].Count > 0)
				{
					using (Dictionary<int, string>.KeyCollection.Enumerator enumerator2 = properties[num].Keys.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							int num2 = enumerator2.Current;
							stringBuilder.AppendFormat("{0}:{1}:{2}; ", num, num2, properties[num][num2]);
						}
						continue;
					}
				}
				stringBuilder.AppendFormat("{0}:null:null; ", num);
			}
			return stringBuilder.ToString();
		}
	}
}
