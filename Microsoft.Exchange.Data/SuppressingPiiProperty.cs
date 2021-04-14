using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Data
{
	internal static class SuppressingPiiProperty
	{
		public static bool Initialized { get; private set; }

		public static string Initialize(string redactionConfigFile)
		{
			string result;
			using (StreamReader streamReader = new StreamReader(redactionConfigFile))
			{
				result = SuppressingPiiProperty.Initialize(streamReader);
			}
			return result;
		}

		public static string Initialize(TextReader reader)
		{
			if (!SuppressingPiiProperty.Initialized)
			{
				lock (SuppressingPiiProperty.syncObject)
				{
					if (!SuppressingPiiProperty.Initialized)
					{
						SuppressingPiiProperty.Initialized = true;
						SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(SuppressingPiiConfig));
						SuppressingPiiProperty.piiDataRedaction = (SuppressingPiiConfig)safeXmlSerializer.Deserialize(reader);
					}
				}
			}
			if (SuppressingPiiProperty.piiDataRedaction != null)
			{
				return SuppressingPiiProperty.piiDataRedaction.DeserializationError;
			}
			return null;
		}

		public static bool IsExcludedSchemaType(Type type)
		{
			return SuppressingPiiProperty.piiDataRedaction != null && SuppressingPiiProperty.piiDataRedaction.ExceptionSchemaTypes != null && SuppressingPiiProperty.piiDataRedaction.ExceptionSchemaTypes.Contains(type);
		}

		public static object TryRedact(PropertyDefinition property, object original)
		{
			return SuppressingPiiProperty.TryRedact(property, original, null);
		}

		public static object TryRedact(PropertyDefinition property, object original, PiiMap piiMap)
		{
			if (original == null)
			{
				return original;
			}
			if (!SuppressingPiiProperty.Initialized || SuppressingPiiProperty.piiDataRedaction == null)
			{
				Type type = original.GetType();
				if (type.IsValueType)
				{
					return Activator.CreateInstance(type);
				}
				return null;
			}
			else
			{
				if (!SuppressingPiiProperty.piiDataRedaction.Enable)
				{
					return original;
				}
				object result = original;
				MethodInfo redactor;
				if (SuppressingPiiProperty.piiDataRedaction.TryGetRedactor(property, out redactor))
				{
					if (piiMap != null && !SuppressingPiiProperty.piiDataRedaction.NeedAddIntoPiiMap(property, original))
					{
						piiMap = null;
					}
					if (property.Type.IsArray)
					{
						result = SuppressingPiiProperty.RedactArray(original, redactor, piiMap);
					}
					else
					{
						result = SuppressingPiiProperty.RedactSingleOrListValue(original, redactor, piiMap);
					}
				}
				return result;
			}
		}

		public static T TryRedactValue<T>(PropertyDefinition property, T original)
		{
			return (T)((object)SuppressingPiiProperty.TryRedact(property, original, null));
		}

		public static int[] GetPiiStringParams(string fullName)
		{
			if (SuppressingPiiProperty.piiDataRedaction != null)
			{
				return SuppressingPiiProperty.piiDataRedaction.GetPiiStringParams(fullName);
			}
			return null;
		}

		private static object RedactSingleOrListValue(object original, MethodInfo redactor, PiiMap piiMap)
		{
			IList list = original as IList;
			if (list != null)
			{
				bool flag = list is MultiValuedPropertyBase;
				if (list.IsReadOnly)
				{
					if (!flag)
					{
						return null;
					}
					((MultiValuedPropertyBase)list).SetIsReadOnly(false, null);
				}
				for (int i = 0; i < list.Count; i++)
				{
					object value = SuppressingPiiProperty.RedactSingleValue(list[i], redactor, piiMap);
					if (flag && !(original is ProxyAddressCollection))
					{
						list.RemoveAt(i);
						list.Insert(i, value);
					}
					else
					{
						list[i] = value;
					}
				}
			}
			else
			{
				original = SuppressingPiiProperty.RedactSingleValue(original, redactor, piiMap);
			}
			return original;
		}

		private static object RedactSingleValue(object original, MethodInfo redactor, PiiMap piiMap)
		{
			object[] array = new object[3];
			array[0] = original;
			object[] array2 = array;
			object result = redactor.Invoke(null, array2);
			string value = (string)array2[1];
			string key = (string)array2[2];
			if (piiMap != null && !string.IsNullOrEmpty(value))
			{
				piiMap[key] = value;
			}
			return result;
		}

		private static object RedactArray(object original, MethodInfo redactor, PiiMap piiMap)
		{
			object[] array = new object[3];
			array[0] = original;
			object[] array2 = array;
			object result = redactor.Invoke(null, array2);
			string[] array3 = (string[])array2[1];
			string[] array4 = (string[])array2[2];
			if (piiMap != null && array3 != null && array4 != null)
			{
				for (int i = 0; i < array3.Length; i++)
				{
					if (!string.IsNullOrEmpty(array3[i]))
					{
						piiMap[array4[i]] = array3[i];
					}
				}
			}
			return result;
		}

		private static SuppressingPiiConfig piiDataRedaction;

		private static object syncObject = new object();
	}
}
