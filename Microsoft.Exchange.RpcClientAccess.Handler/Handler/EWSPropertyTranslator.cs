using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal static class EWSPropertyTranslator
	{
		internal static IEnumerable<ExtendedProperty> ConvertPropsToEWSProps(PropertyValue[] propValues, StoreSession session, out Dictionary<PropertyTag, ExtendedPropertyDefinition> mapiToEWSPropertyMap, out List<PropertyProblem> propertyProblems)
		{
			PropertyTag[] array = new PropertyTag[propValues.Length];
			for (int i = 0; i < propValues.Length; i++)
			{
				array[i] = propValues[i].PropertyTag;
			}
			mapiToEWSPropertyMap = EWSPropertyTranslator.GetPropTagsToDefinitionMapping(array, session, out propertyProblems);
			List<ExtendedProperty> list = new List<ExtendedProperty>(propValues.Length);
			foreach (PropertyValue propertyValue in propValues)
			{
				ExtendedPropertyDefinition extendedPropertyDefinition = null;
				if (mapiToEWSPropertyMap.TryGetValue(propertyValue.PropertyTag, out extendedPropertyDefinition) && !(extendedPropertyDefinition == null))
				{
					ExtendedProperty extendedProperty = new ExtendedProperty(extendedPropertyDefinition);
					switch (extendedProperty.PropertyDefinition.MapiType)
					{
					case 23:
						extendedProperty.Value = (DateTime)((ExDateTime)propertyValue.Value);
						break;
					case 24:
						extendedProperty.Value = ExDateTime.ToDateTimeArray((ExDateTime[])propertyValue.Value);
						break;
					default:
						extendedProperty.Value = propertyValue.Value;
						break;
					}
					list.Add(extendedProperty);
				}
			}
			return list;
		}

		internal static IEnumerable<ExtendedPropertyDefinition> ConvertPropTagsToDefinitions(PropertyTag[] propTags, StoreSession session, out Dictionary<PropertyTag, ExtendedPropertyDefinition> mapiToEWSPropertyMap, out List<PropertyProblem> propertyProblems)
		{
			mapiToEWSPropertyMap = EWSPropertyTranslator.GetPropTagsToDefinitionMapping(propTags, session, out propertyProblems);
			List<ExtendedPropertyDefinition> list = new List<ExtendedPropertyDefinition>(propTags.Length);
			foreach (KeyValuePair<PropertyTag, ExtendedPropertyDefinition> keyValuePair in mapiToEWSPropertyMap)
			{
				if (keyValuePair.Value != null)
				{
					list.Add(keyValuePair.Value);
				}
			}
			return list;
		}

		private static Dictionary<PropertyTag, ExtendedPropertyDefinition> GetPropTagsToDefinitionMapping(PropertyTag[] propertyTags, StoreSession session, out List<PropertyProblem> propertyProblems)
		{
			Dictionary<PropertyTag, ExtendedPropertyDefinition> dictionary = new Dictionary<PropertyTag, ExtendedPropertyDefinition>();
			propertyProblems = new List<PropertyProblem>(propertyTags.Length);
			List<ushort> list = new List<ushort>(propertyTags.Length);
			foreach (PropertyTag propertyTag in propertyTags)
			{
				if (propertyTag.IsNamedProperty)
				{
					list.Add((ushort)propertyTag.PropertyId);
				}
			}
			NamedPropertyDefinition.NamedPropertyKey[] namesFromIds = NamedPropConverter.GetNamesFromIds(session, list);
			int num = 0;
			ushort num2 = 0;
			while ((int)num2 < propertyTags.Length)
			{
				PropertyTag propertyTag2 = propertyTags[(int)num2];
				MapiPropertyType propertyType = EWSPropertyTranslator.GetPropertyType(propertyTag2.PropertyType);
				if (propertyType == -1)
				{
					ExTraceGlobals.FolderTracer.TraceDebug<PropertyTag, PropertyType>(Activity.TraceId, "Property {0} will be skipped. Invalid PropertyType : {0}", propertyTag2, propertyTag2.PropertyType);
					propertyProblems.Add(new PropertyProblem(num2, propertyTag2, (ErrorCode)2147746562U));
				}
				else
				{
					ExtendedPropertyDefinition value = null;
					if (!propertyTag2.IsNamedProperty)
					{
						value = new ExtendedPropertyDefinition((int)propertyTag2.PropertyId, propertyType);
					}
					else
					{
						NamedPropertyDefinition.NamedPropertyKey namedPropertyKey = namesFromIds[num++];
						if (namedPropertyKey == null)
						{
							ExTraceGlobals.FolderTracer.TraceDebug<PropertyTag>(Activity.TraceId, "Missing named property definition for property tag {0}", propertyTag2);
							propertyProblems.Add(new PropertyProblem(num2, propertyTag2, (ErrorCode)2147746564U));
							goto IL_17B;
						}
						GuidIdPropertyDefinition.GuidIdKey guidIdKey = namedPropertyKey as GuidIdPropertyDefinition.GuidIdKey;
						if (guidIdKey != null)
						{
							value = new ExtendedPropertyDefinition(guidIdKey.PropertyGuid, guidIdKey.PropertyId, propertyType);
						}
						else
						{
							GuidNamePropertyDefinition.GuidNameKey guidNameKey = namedPropertyKey as GuidNamePropertyDefinition.GuidNameKey;
							if (guidNameKey != null)
							{
								value = new ExtendedPropertyDefinition(guidNameKey.PropertyGuid, guidNameKey.PropertyName, propertyType);
							}
						}
					}
					if (!dictionary.ContainsKey(propertyTag2))
					{
						dictionary[propertyTag2] = value;
					}
				}
				IL_17B:
				num2 += 1;
			}
			return dictionary;
		}

		private static MapiPropertyType GetPropertyType(PropertyType propType)
		{
			if (propType <= PropertyType.Binary)
			{
				if (propType <= PropertyType.Unicode)
				{
					switch (propType)
					{
					case PropertyType.Null:
						return 18;
					case PropertyType.Int16:
						return 21;
					case PropertyType.Int32:
						return 14;
					case PropertyType.Float:
						return 12;
					case PropertyType.Double:
						return 9;
					case PropertyType.Currency:
						return 7;
					case PropertyType.AppTime:
						return 0;
					case (PropertyType)8:
					case (PropertyType)9:
					case (PropertyType)12:
					case (PropertyType)14:
					case (PropertyType)15:
					case (PropertyType)16:
					case (PropertyType)17:
					case (PropertyType)18:
					case (PropertyType)19:
						break;
					case PropertyType.Error:
						return 11;
					case PropertyType.Bool:
						return 4;
					case PropertyType.Object:
						return 19;
					case PropertyType.Int64:
						return 16;
					default:
						switch (propType)
						{
						case PropertyType.String8:
						case PropertyType.Unicode:
							return 25;
						}
						break;
					}
				}
				else
				{
					if (propType == PropertyType.SysTime)
					{
						return 23;
					}
					if (propType == PropertyType.Guid)
					{
						return 5;
					}
					if (propType == PropertyType.Binary)
					{
						return 2;
					}
				}
			}
			else if (propType <= PropertyType.MultiValueUnicode)
			{
				switch (propType)
				{
				case PropertyType.MultiValueInt16:
					return 22;
				case PropertyType.MultiValueInt32:
					return 15;
				case PropertyType.MultiValueFloat:
					return 13;
				case PropertyType.MultiValueDouble:
					return 10;
				case PropertyType.MultiValueCurrency:
					return 8;
				case PropertyType.MultiValueAppTime:
					return 1;
				default:
					if (propType == PropertyType.MultiValueInt64)
					{
						return 17;
					}
					switch (propType)
					{
					case PropertyType.MultiValueString8:
					case PropertyType.MultiValueUnicode:
						return 26;
					}
					break;
				}
			}
			else
			{
				if (propType == PropertyType.MultiValueSysTime)
				{
					return 24;
				}
				if (propType == PropertyType.MultiValueGuid)
				{
					return 6;
				}
				if (propType == PropertyType.MultiValueBinary)
				{
					return 3;
				}
			}
			return -1;
		}
	}
}
