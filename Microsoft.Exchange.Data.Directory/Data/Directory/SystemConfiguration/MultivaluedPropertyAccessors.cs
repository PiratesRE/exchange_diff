using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MultivaluedPropertyAccessors
	{
		internal static string GetStringValueFromMultivaluedProperty(string name, IEnumerable<string> property, string defaultValue)
		{
			string text = property.SingleOrDefault((string s) => s.StartsWith(name + ":"));
			if (text == null)
			{
				return defaultValue;
			}
			if (text.Length <= name.Length + ":".Length)
			{
				return string.Empty;
			}
			return text.Substring(name.Length + ":".Length);
		}

		internal static int GetIntValueFromMultivaluedProperty(string name, IEnumerable<string> property, int defaultValue)
		{
			string stringValueFromMultivaluedProperty = MultivaluedPropertyAccessors.GetStringValueFromMultivaluedProperty(name, property, null);
			if (string.IsNullOrEmpty(stringValueFromMultivaluedProperty))
			{
				return defaultValue;
			}
			int result;
			if (int.TryParse(stringValueFromMultivaluedProperty, out result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static Version GetVersionValueFromMultivaluedProperty(string name, IEnumerable<string> property, Version defaultValue)
		{
			string stringValueFromMultivaluedProperty = MultivaluedPropertyAccessors.GetStringValueFromMultivaluedProperty(name, property, null);
			if (string.IsNullOrEmpty(stringValueFromMultivaluedProperty))
			{
				return defaultValue;
			}
			Version result;
			if (Version.TryParse(stringValueFromMultivaluedProperty, out result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static ByteQuantifiedSize GetByteQuantifiedValueFromMultivaluedProperty(string name, IEnumerable<string> property, ByteQuantifiedSize defaultValue)
		{
			string stringValueFromMultivaluedProperty = MultivaluedPropertyAccessors.GetStringValueFromMultivaluedProperty(name, property, null);
			if (string.IsNullOrEmpty(stringValueFromMultivaluedProperty))
			{
				return defaultValue;
			}
			ByteQuantifiedSize result;
			if (ByteQuantifiedSize.TryParse(stringValueFromMultivaluedProperty, out result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static EnhancedTimeSpan GetTimespanValueFromMultivaluedProperty(string name, IEnumerable<string> property, EnhancedTimeSpan defaultValue)
		{
			string stringValueFromMultivaluedProperty = MultivaluedPropertyAccessors.GetStringValueFromMultivaluedProperty(name, property, null);
			if (string.IsNullOrEmpty(stringValueFromMultivaluedProperty))
			{
				return defaultValue;
			}
			EnhancedTimeSpan result;
			if (EnhancedTimeSpan.TryParse(stringValueFromMultivaluedProperty, out result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static void UpdateMultivaluedProperty<T>(T value, string name, MultiValuedProperty<string> property)
		{
			string text = name + ":" + value.ToString();
			int num = property.ToList<string>().FindIndex((string s) => s.StartsWith(name + ":"));
			if (num < 0)
			{
				property.Add(text);
				return;
			}
			property[num] = text;
		}

		private const string NameValueSeparator = ":";
	}
}
