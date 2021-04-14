using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class GrammarRecipientHelper
	{
		public static QueryFilter GetUserFilter()
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "person");
			QueryFilter queryFilter2 = new ExistsFilter(ADRecipientSchema.AddressListMembership);
			QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.HiddenFromAddressListsEnabled, false);
			QueryFilter queryFilter4 = new ExistsFilter(ADRecipientSchema.DisplayName);
			QueryFilter queryFilter5 = new ExistsFilter(ADOrgPersonSchema.FirstName);
			QueryFilter queryFilter6 = new ExistsFilter(ADOrgPersonSchema.LastName);
			return new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2,
				queryFilter3,
				new OrFilter(new QueryFilter[]
				{
					queryFilter4,
					queryFilter5,
					queryFilter6
				})
			});
		}

		public static QueryFilter GetDLFilter()
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.HiddenFromAddressListsEnabled, false);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.DynamicDistributionGroup);
			QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUniversalDistributionGroup);
			QueryFilter queryFilter4 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUniversalSecurityGroup);
			QueryFilter queryFilter5 = new OrFilter(new QueryFilter[]
			{
				queryFilter2,
				queryFilter3,
				queryFilter4
			});
			return new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter5
			});
		}

		public static string GetSanitizedDisplayNameForXMLEntry(string displayName)
		{
			StringBuilder stringBuilder = new StringBuilder(displayName.Length);
			foreach (char c in displayName)
			{
				if (c > '\u001f' && c != '￾' && c != '￿')
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetNormalizedEmailAddress(string emailAddress)
		{
			PIIMessage data = PIIMessage.Create(PIIType._EmailAddress, emailAddress);
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, 0, data, "NormalizationHelper.GetNormalizedEmailAddress for EmailAddress='_EmailAddress'", new object[0]);
			string inText = Utils.TrimSpaces(emailAddress);
			return SpeechUtils.SrgsEncode(inText);
		}

		public static Dictionary<string, bool> ApplyExclusionList(string input, RecipientType recipType)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, 0, "NormalizationHelper : ApplyExclusionList - input='{0}', recipType='{1}'", new object[]
			{
				input,
				recipType
			});
			List<Replacement> list = null;
			ExclusionList exclusionList = null;
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			if (!string.IsNullOrEmpty(input))
			{
				try
				{
					exclusionList = ExclusionList.Instance;
				}
				catch (ExclusionListException ex)
				{
					CallIdTracer.TraceError(ExTraceGlobals.UMGrammarGeneratorTracer, 0, "NormalizationHelper: ApplyExclusionList encountered exception while getting exclusionList. Details : {0}", new object[]
					{
						ex
					});
				}
				if (exclusionList != null)
				{
					switch (exclusionList.GetReplacementStrings(input, recipType, out list))
					{
					case MatchResult.NoMatch:
						dictionary.Add(input, true);
						return dictionary;
					case MatchResult.MatchWithReplacements:
						using (List<Replacement>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Replacement replacement = enumerator.Current;
								string value = Utils.TrimSpaces(replacement.ReplacementString);
								if (!string.IsNullOrEmpty(value) && !dictionary.ContainsKey(replacement.ReplacementString))
								{
									dictionary.Add(replacement.ReplacementString, replacement.ShouldNormalize);
								}
							}
							return dictionary;
						}
						break;
					case MatchResult.MatchWithNoReplacements:
					case MatchResult.NotFound:
						return dictionary;
					default:
						return dictionary;
					}
				}
				dictionary.Add(input, true);
			}
			return dictionary;
		}

		public static string CharacterMapReplaceString(string name)
		{
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, name);
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, 0, data, "NormalizationHelper : CharacterMapReplaceString name='_UserDisplayName'", new object[]
			{
				name
			});
			string text = Utils.TrimSpaces(name);
			if (!string.IsNullOrEmpty(text))
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (char c in text)
				{
					string value = null;
					if (GrammarRecipientHelper.GetUnsafeCharacterMap().TryGetValue(c, out value))
					{
						stringBuilder.Append(value);
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				return stringBuilder.ToString();
			}
			return text;
		}

		private static Dictionary<char, string> GetUnsafeCharacterMap()
		{
			if (GrammarRecipientHelper.unsafeCharMap == null)
			{
				lock (GrammarRecipientHelper.staticLock)
				{
					if (GrammarRecipientHelper.unsafeCharMap == null)
					{
						GrammarRecipientHelper.unsafeCharMap = new Dictionary<char, string>(2);
						GrammarRecipientHelper.unsafeCharMap.Add('-', " ");
						GrammarRecipientHelper.unsafeCharMap.Add('\'', " ");
					}
				}
			}
			return GrammarRecipientHelper.unsafeCharMap;
		}

		public const int TenantSizeThreshold = 10;

		private static object staticLock = new object();

		private static Dictionary<char, string> unsafeCharMap;

		public static readonly PropertyDefinition[] LookupProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.PhoneticDisplayName,
			ADRecipientSchema.PrimarySmtpAddress,
			ADObjectSchema.Guid,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.RecipientTypeDetails,
			ADRecipientSchema.UMRecipientDialPlanId,
			ADObjectSchema.WhenChangedUTC,
			ADObjectSchema.DistinguishedName,
			ADRecipientSchema.AddressListMembership
		};

		public enum LookupPropertiesEnum
		{
			DisplayName,
			PhoneticDisplayName,
			PrimarySmtpAddress,
			Guid,
			RecipientType,
			RecipientTypeDetails,
			UMRecipientDialPlanId,
			WhenChangedUTC,
			DistinguishedName,
			AddressListMembership
		}
	}
}
