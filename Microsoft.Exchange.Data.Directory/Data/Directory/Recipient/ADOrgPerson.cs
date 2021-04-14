using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal static class ADOrgPerson
	{
		internal static object CountryOrRegionGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ADOrgPersonSchema.C];
			int countryCode = (int)propertyBag[ADOrgPersonSchema.CountryCode];
			string displayName = (string)propertyBag[ADOrgPersonSchema.Co];
			CountryInfo result = null;
			if (text != null && text.Length == 2)
			{
				try
				{
					result = CountryInfo.Parse(text, countryCode, displayName);
				}
				catch (InvalidCountryOrRegionException ex)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("CountryOrRegion", ex.Message), ADOrgPersonSchema.CountryOrRegion, propertyBag[ADOrgPersonSchema.C]), ex);
				}
			}
			return result;
		}

		internal static void CountryOrRegionSetter(object value, IPropertyBag propertyBag)
		{
			CountryInfo countryInfo = value as CountryInfo;
			if (countryInfo != null)
			{
				propertyBag[ADOrgPersonSchema.C] = countryInfo.Name;
				propertyBag[ADOrgPersonSchema.Co] = countryInfo.DisplayName;
				propertyBag[ADOrgPersonSchema.CountryCode] = countryInfo.CountryCode;
				return;
			}
			propertyBag[ADOrgPersonSchema.C] = null;
			propertyBag[ADOrgPersonSchema.Co] = null;
			propertyBag[ADOrgPersonSchema.CountryCode] = 0;
		}

		internal static QueryFilter CountryOrRegionFilterBuilder(SinglePropertyFilter filter)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			CountryInfo countryInfo = (CountryInfo)comparisonFilter.PropertyValue;
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, ADOrgPersonSchema.C, countryInfo.Name);
		}

		internal static object SanitizedPhoneNumbersGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			foreach (ProviderPropertyDefinition propertyDefinition in ADOrgPersonSchema.SanitizedPhoneNumbers.SupportingProperties)
			{
				ICollection<string> collection = propertyBag[propertyDefinition] as ICollection<string>;
				if (collection != null)
				{
					using (IEnumerator<string> enumerator2 = collection.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							string property = enumerator2.Current;
							ADOrgPerson.SanitizedPhoneNumber(multiValuedProperty, property);
						}
						continue;
					}
				}
				ADOrgPerson.SanitizedPhoneNumber(multiValuedProperty, propertyBag[propertyDefinition] as string);
			}
			return multiValuedProperty;
		}

		internal static object IndexedPhoneNumbersGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.UMDtmfMap];
			foreach (string text in multiValuedProperty2)
			{
				if (!string.IsNullOrEmpty(text) && text.StartsWith("reversedPhone:", StringComparison.OrdinalIgnoreCase) && text.Length > "reversedPhone:".Length)
				{
					string text2 = text.Substring("reversedPhone:".Length);
					if (!multiValuedProperty.Contains(text2))
					{
						multiValuedProperty.Add(DtmfString.Reverse(text2));
					}
				}
			}
			return multiValuedProperty;
		}

		internal static QueryFilter IndexedPhoneNumbersGetterFilterBuilder(SinglePropertyFilter filter)
		{
			TextFilter textFilter = filter as TextFilter;
			if (textFilter == null)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(TextFilter)));
			}
			if (MatchOptions.Prefix == textFilter.MatchOptions)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedMatchOptionsForProperty(textFilter.Property.Name, textFilter.MatchOptions.ToString()));
			}
			MatchOptions matchOptions;
			if (MatchOptions.Suffix == textFilter.MatchOptions)
			{
				matchOptions = MatchOptions.Prefix;
			}
			else
			{
				matchOptions = textFilter.MatchOptions;
			}
			string text = DtmfString.Reverse(textFilter.Text);
			text = "reversedPhone:" + text;
			return new TextFilter(ADRecipientSchema.UMDtmfMap, text, matchOptions, textFilter.MatchFlags);
		}

		internal static object LanguagesGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ADOrgPersonSchema.LanguagesRaw];
			MultiValuedProperty<CultureInfo> multiValuedProperty = new MultiValuedProperty<CultureInfo>();
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					CultureInfo cultureInfo = null;
					try
					{
						cultureInfo = new CultureInfo(array[i].Trim());
					}
					catch (ArgumentException ex)
					{
						ExTraceGlobals.ADReadDetailsTracer.TraceDebug<string, string>(0L, "ADOrgPerson::LanguagesGetter - Invalid culture {0} ignored. Exception: {1}", array[i], ex.Message);
					}
					if (cultureInfo != null)
					{
						multiValuedProperty.Add(cultureInfo);
					}
				}
			}
			return multiValuedProperty;
		}

		internal static void LanguagesSetter(object value, IPropertyBag propertyBag)
		{
			StringBuilder stringBuilder = new StringBuilder(80);
			if (value != null)
			{
				bool flag = true;
				MultiValuedProperty<CultureInfo> multiValuedProperty = (MultiValuedProperty<CultureInfo>)value;
				foreach (CultureInfo cultureInfo in multiValuedProperty)
				{
					stringBuilder.AppendFormat("{0}{1}", flag ? string.Empty : ",", cultureInfo.ToString());
					flag = false;
				}
			}
			propertyBag[ADOrgPersonSchema.LanguagesRaw] = stringBuilder.ToString();
		}

		internal static void SanitizedPhoneNumber(MultiValuedProperty<string> calculatedValue, string property)
		{
			string text = DtmfString.SanitizePhoneNumber(property);
			if (!string.IsNullOrEmpty(text) && !calculatedValue.Contains(text))
			{
				calculatedValue.Add(text);
			}
		}

		internal static object[][] GetManagementChainView(IRecipientSession adSession, IADOrgPerson person, bool getPeers, params PropertyDefinition[] returnProperties)
		{
			if (returnProperties == null)
			{
				throw new ArgumentNullException("returnProperties");
			}
			int? num = null;
			for (int i = 0; i < returnProperties.Length; i++)
			{
				if (returnProperties[i] == ADOrgPersonSchema.ViewDepth)
				{
					num = new int?(i);
					break;
				}
			}
			PropertyDefinition[] array = new PropertyDefinition[returnProperties.Length + 1];
			returnProperties.CopyTo(array, 1);
			array[0] = ADOrgPersonSchema.Manager;
			Collection<IADRecipient[]> collection = new Collection<IADRecipient[]>();
			Collection<Guid> collection2 = new Collection<Guid>();
			int num2 = 0;
			if (getPeers)
			{
				ADObjectId adobjectId = person.Manager;
				while (adobjectId != null && num2 < 100)
				{
					if (collection2.Contains(adobjectId.ObjectGuid))
					{
						break;
					}
					QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADOrgPersonSchema.Manager, adobjectId);
					IADRecipient[] item = adSession.Find(adSession.SearchRoot, QueryScope.SubTree, filter, null, 0);
					collection2.Add(adobjectId.ObjectGuid);
					collection.Add(item);
					IADOrgPerson iadorgPerson = adSession.Read(adobjectId) as IADOrgPerson;
					if (iadorgPerson == null)
					{
						adobjectId = null;
					}
					else if (iadorgPerson.Manager == null || iadorgPerson.Manager.ObjectGuid == adobjectId.ObjectGuid)
					{
						item = new IADRecipient[]
						{
							iadorgPerson
						};
						collection.Add(item);
						num2++;
						adobjectId = null;
					}
					else
					{
						adobjectId = iadorgPerson.Manager;
					}
					num2++;
				}
			}
			else
			{
				ADObjectId adobjectId = person.Id;
				while (adobjectId != null && num2 < 100 && !collection2.Contains(adobjectId.ObjectGuid))
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, adobjectId);
					IADOrgPerson iadorgPerson2 = adSession.Read(adobjectId) as IADOrgPerson;
					collection2.Add(adobjectId.ObjectGuid);
					if (iadorgPerson2 == null)
					{
						adobjectId = null;
					}
					else
					{
						collection.Add(new IADRecipient[]
						{
							iadorgPerson2
						});
						if (iadorgPerson2.Manager != null && iadorgPerson2.Manager.ObjectGuid == adobjectId.ObjectGuid)
						{
							adobjectId = null;
						}
						else
						{
							adobjectId = iadorgPerson2.Manager;
						}
						num2++;
					}
				}
			}
			int count = collection.Count;
			int num3 = 0;
			for (int j = 0; j < count; j++)
			{
				num3 += collection[j].Length;
			}
			object[][] array2 = new object[num3][];
			int k = count - 1;
			int num4 = 0;
			while (k >= 0)
			{
				for (int l = 0; l < collection[k].Length; l++)
				{
					array2[num4] = collection[k][l].GetProperties(returnProperties);
					if (num != null)
					{
						array2[num4][num.Value] = count - 1 - k;
					}
					num4++;
				}
				k--;
			}
			return array2;
		}

		internal static object[][] GetDirectReportsView(IRecipientSession adSession, IADOrgPerson person, params PropertyDefinition[] returnProperties)
		{
			if (returnProperties == null)
			{
				throw new ArgumentNullException("returnProperties");
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADOrgPersonSchema.Manager, person.Id);
			ADPagedReader<ADRawEntry> adpagedReader = adSession.FindPagedADRawEntry(adSession.SearchRoot, QueryScope.SubTree, filter, null, 0, returnProperties);
			ADRawEntry[] recipients = adpagedReader.ReadAllPages();
			return ADSession.ConvertToView(recipients, returnProperties);
		}

		private const int MaxHierarchyDepth = 100;
	}
}
