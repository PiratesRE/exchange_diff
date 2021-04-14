using System;
using System.DirectoryServices.Protocols;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class RangedPropertyHelper
	{
		public static ADPropertyDefinition CreateRangedProperty(ADPropertyDefinition originalProperty, IntRange range)
		{
			if (range == null || range.LowerBound < 0 || range.LowerBound > range.UpperBound)
			{
				throw new ArgumentException("range");
			}
			return new ADPropertyDefinition(originalProperty.Name, originalProperty.VersionAdded, originalProperty.Type, originalProperty.FormatProvider, originalProperty.LdapDisplayName + RangedPropertyHelper.GetADRangeSuffix(range), originalProperty.Flags | ADPropertyDefinitionFlags.Ranged | ADPropertyDefinitionFlags.ReadOnly, originalProperty.DefaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
		}

		public static DirectoryAttribute GetRangedPropertyValue(ADPropertyDefinition propertyDefinition, SearchResultAttributeCollection attributeCollection, out IntRange returnedRange)
		{
			DirectoryAttribute result = null;
			returnedRange = null;
			string text;
			IntRange propertyRangeFromLdapName = RangedPropertyHelper.GetPropertyRangeFromLdapName(propertyDefinition.LdapDisplayName, out text);
			if (attributeCollection.Contains(propertyDefinition.LdapDisplayName))
			{
				returnedRange = propertyRangeFromLdapName;
				result = attributeCollection[propertyDefinition.LdapDisplayName];
			}
			else
			{
				string attributeNameWithRange = ADSession.GetAttributeNameWithRange(text, propertyRangeFromLdapName.LowerBound.ToString(), "*");
				if (attributeCollection.Contains(attributeNameWithRange))
				{
					returnedRange = new IntRange(propertyRangeFromLdapName.LowerBound, int.MaxValue);
					result = attributeCollection[attributeNameWithRange];
				}
				else
				{
					string value = string.Format(CultureInfo.InvariantCulture, "{0};{1}{2}-", new object[]
					{
						text,
						"range=",
						propertyRangeFromLdapName.LowerBound
					});
					foreach (object obj in attributeCollection.AttributeNames)
					{
						string text2 = (string)obj;
						if (text2.StartsWith(value, StringComparison.OrdinalIgnoreCase))
						{
							result = attributeCollection[text2];
							returnedRange = RangedPropertyHelper.GetPropertyRangeFromLdapName(text2, out text);
						}
					}
				}
			}
			return result;
		}

		private static string GetADRangeSuffix(IntRange range)
		{
			return string.Format(CultureInfo.InvariantCulture, ";{0}{1}-{2}", new object[]
			{
				"range=",
				range.LowerBound,
				(range.UpperBound == int.MaxValue) ? "*" : range.UpperBound.ToString(CultureInfo.InvariantCulture)
			});
		}

		internal static IntRange GetPropertyRangeFromLdapName(string rangedPropertyName, out string ldapNameWithoutRange)
		{
			int num = rangedPropertyName.LastIndexOf(";");
			if (num < 1)
			{
				throw new FormatException(DirectoryStrings.RangePropertyResponseDoesNotContainRangeInformation(rangedPropertyName));
			}
			IntRange result = RangedPropertyHelper.ParsePropertyValueRange(rangedPropertyName.Substring(num + 1));
			ldapNameWithoutRange = rangedPropertyName.Substring(0, num);
			return result;
		}

		private static IntRange ParsePropertyValueRange(string value)
		{
			int num = value.IndexOf('-');
			if (num < 7)
			{
				throw new FormatException(DirectoryStrings.RangeInformationFormatInvalid(value));
			}
			int lowerBound = RangedPropertyHelper.ParsePropertyValueRangeBound(value.Substring(6, num - 6), false);
			int upperBound = RangedPropertyHelper.ParsePropertyValueRangeBound(value.Substring(num + 1), true);
			return new IntRange(lowerBound, upperBound);
		}

		private static int ParsePropertyValueRangeBound(string value, bool allowWildcard)
		{
			if (allowWildcard && value == "*")
			{
				return int.MaxValue;
			}
			return int.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture);
		}

		private const string RangeWildcard = "*";

		private const string RangePrefix = "range=";

		private const int RangePrefixLength = 6;

		internal static readonly IntRange AllValuesRange = new IntRange(0, int.MaxValue);
	}
}
