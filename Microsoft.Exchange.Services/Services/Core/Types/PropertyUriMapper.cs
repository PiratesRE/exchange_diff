using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class PropertyUriMapper
	{
		static PropertyUriMapper()
		{
			Type typeFromHandle = typeof(PropertyUriEnum);
			IEnumerable<FieldInfo> declaredFields = typeFromHandle.GetTypeInfo().DeclaredFields;
			foreach (FieldInfo fieldInfo in declaredFields)
			{
				if (fieldInfo.FieldType == typeof(PropertyUriEnum))
				{
					XmlEnumAttribute xmlEnumAttribute = (XmlEnumAttribute)fieldInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false).ElementAt(0);
					PropertyUriEnum propertyUriEnum = (PropertyUriEnum)fieldInfo.GetValue(null);
					PropertyUriMapper.AddEntry(propertyUriEnum, xmlEnumAttribute.Name);
				}
			}
			typeFromHandle = typeof(DictionaryUriEnum);
			declaredFields = typeFromHandle.GetTypeInfo().DeclaredFields;
			foreach (FieldInfo fieldInfo2 in declaredFields)
			{
				if (fieldInfo2.FieldType == typeof(DictionaryUriEnum))
				{
					XmlEnumAttribute xmlEnumAttribute2 = (XmlEnumAttribute)fieldInfo2.GetCustomAttributes(typeof(XmlEnumAttribute), false).ElementAt(0);
					DictionaryUriEnum dictionaryUriEnum = (DictionaryUriEnum)fieldInfo2.GetValue(null);
					PropertyUriMapper.AddEntry(dictionaryUriEnum, xmlEnumAttribute2.Name);
				}
			}
			typeFromHandle = typeof(ExceptionPropertyUriEnum);
			declaredFields = typeFromHandle.GetTypeInfo().DeclaredFields;
			foreach (FieldInfo fieldInfo3 in declaredFields)
			{
				if (fieldInfo3.FieldType == typeof(ExceptionPropertyUriEnum))
				{
					XmlEnumAttribute xmlEnumAttribute3 = (XmlEnumAttribute)fieldInfo3.GetCustomAttributes(typeof(XmlEnumAttribute), false).ElementAt(0);
					ExceptionPropertyUriEnum exceptionPropertyUriEnum = (ExceptionPropertyUriEnum)fieldInfo3.GetValue(null);
					PropertyUriMapper.AddEntry(exceptionPropertyUriEnum, xmlEnumAttribute3.Name);
				}
			}
		}

		private static void AddEntry(PropertyUriEnum propertyUriEnum, string xmlEnumValue)
		{
			PropertyUriMapper.propUriToUriMap.Add(propertyUriEnum, xmlEnumValue);
			PropertyUriMapper.uriToPropUriMap.Add(xmlEnumValue, propertyUriEnum);
		}

		private static void AddEntry(DictionaryUriEnum dictionaryUriEnum, string xmlEnumValue)
		{
			PropertyUriMapper.dictionaryUriToUriMap.Add(dictionaryUriEnum, xmlEnumValue);
			PropertyUriMapper.uriToDictionaryUriMap.Add(xmlEnumValue, dictionaryUriEnum);
		}

		private static void AddEntry(ExceptionPropertyUriEnum exceptionPropertyUriEnum, string xmlEnumValue)
		{
			PropertyUriMapper.exceptionPropertyUriToUriMap.Add(exceptionPropertyUriEnum, xmlEnumValue);
		}

		public static string GetXmlEnumValue(PropertyUriEnum propertyUriEnum)
		{
			return PropertyUriMapper.propUriToUriMap[propertyUriEnum];
		}

		public static bool TryGetPropertyUriEnum(string xmlEnumValue, out PropertyUriEnum propertyUriEnum)
		{
			return PropertyUriMapper.uriToPropUriMap.TryGetValue(xmlEnumValue, out propertyUriEnum);
		}

		public static string GetXmlEnumValue(DictionaryUriEnum dictionaryUriEnum)
		{
			return PropertyUriMapper.dictionaryUriToUriMap[dictionaryUriEnum];
		}

		public static bool TryGetDictionaryUriEnum(string xmlEnumValue, out DictionaryUriEnum dictionaryUriEnum)
		{
			return PropertyUriMapper.uriToDictionaryUriMap.TryGetValue(xmlEnumValue, out dictionaryUriEnum);
		}

		public static string GetXmlEnumValue(ExceptionPropertyUriEnum dictionaryUriEnum)
		{
			return PropertyUriMapper.exceptionPropertyUriToUriMap[dictionaryUriEnum];
		}

		private static Dictionary<PropertyUriEnum, string> propUriToUriMap = new Dictionary<PropertyUriEnum, string>();

		private static Dictionary<string, PropertyUriEnum> uriToPropUriMap = new Dictionary<string, PropertyUriEnum>();

		private static Dictionary<DictionaryUriEnum, string> dictionaryUriToUriMap = new Dictionary<DictionaryUriEnum, string>();

		private static Dictionary<string, DictionaryUriEnum> uriToDictionaryUriMap = new Dictionary<string, DictionaryUriEnum>();

		private static Dictionary<ExceptionPropertyUriEnum, string> exceptionPropertyUriToUriMap = new Dictionary<ExceptionPropertyUriEnum, string>();
	}
}
