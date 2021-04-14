using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class StringExtension
	{
		public static void FaultIfNullOrEmpty(this string value, string errorMessage)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new FaultException(errorMessage);
			}
		}

		private static void FaultIfEmptyPassword(this string password)
		{
			if (string.IsNullOrEmpty(password))
			{
				throw new FaultException(Strings.PasswordNotSetError);
			}
		}

		public static bool IsBindingExpression(this string expression)
		{
			return !string.IsNullOrEmpty(expression) && expression.StartsWith("{") && expression.EndsWith("}");
		}

		public static string NullIfEmpty(this string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return null;
		}

		public static SecureString ToSecureString(this string password)
		{
			password.FaultIfEmptyPassword();
			return password.ConvertToSecureString();
		}

		public static PSCredential ToPSCredential(this string userName, SecureString password)
		{
			userName.FaultIfNullOrEmpty(Strings.UserNameNotSetError);
			return new PSCredential(userName, password);
		}

		public static string ToRecipeintFilterString(this object searializedObj, string additionalFilter)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			string text = (searializedObj is DBNull) ? null : ((string)searializedObj);
			if (!string.IsNullOrEmpty(text))
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				object[] array = javaScriptSerializer.Deserialize<object[]>(text);
				new StringBuilder();
				foreach (object obj in array)
				{
					object[] array3 = (object[])obj;
					string text2 = (string)array3[0];
					if (text2.Equals("ANR", StringComparison.InvariantCultureIgnoreCase))
					{
						object[] array4 = (object[])array3[2];
						for (int j = 0; j < array4.Length; j++)
						{
							object obj2 = array4[j];
							string anrItemString = (string)obj2;
							if (!string.IsNullOrEmpty(anrItemString))
							{
								IEnumerable<string> source = from enumName in Enum.GetNames(typeof(RecipientTypeDetails))
								where enumName.StartsWith(anrItemString, StringComparison.InvariantCultureIgnoreCase) && !enumName.Equals(RecipientTypeDetails.AllUniqueRecipientTypes.ToString(), StringComparison.InvariantCultureIgnoreCase)
								select enumName;
								List<string> list2 = source.ToList<string>();
								list2.Remove("AllUniqueRecipientTypes");
								new StringBuilder();
								List<QueryFilter> list3 = (from name in list2
								select new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, Enum.Parse(typeof(RecipientTypeDetails), name))).ToList<QueryFilter>();
								list3.Add(new TextFilter(ADRecipientSchema.DisplayName, anrItemString, MatchOptions.Prefix, MatchFlags.Default));
								list3.Add(new TextFilter(ADRecipientSchema.Alias, anrItemString, MatchOptions.Prefix, MatchFlags.Default));
								list3.Add(new TextFilter(ADUserSchema.FirstName, anrItemString, MatchOptions.Prefix, MatchFlags.Default));
								list3.Add(new TextFilter(ADUserSchema.LastName, anrItemString, MatchOptions.Prefix, MatchFlags.Default));
								list3.Add(new TextFilter(ADUserSchema.Department, anrItemString, MatchOptions.Prefix, MatchFlags.Default));
								list3.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, anrItemString));
								list.Add(new OrFilter(list3.ToArray()));
							}
						}
					}
					else if (array3[1].ToString().ToLowerInvariant() == "equals")
					{
						if (StringExtension.QueryNameToPropertyDef[(string)array3[0]].Type == typeof(bool) && !bool.Parse((string)array3[2]))
						{
							list.Add(new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, StringExtension.QueryNameToPropertyDef[(string)array3[0]], true)));
						}
						else
						{
							list.Add(new ComparisonFilter(ComparisonOperator.Equal, StringExtension.QueryNameToPropertyDef[(string)array3[0]], array3[2]));
						}
					}
					else
					{
						if (!(array3[1].ToString().ToLowerInvariant() == "startswith"))
						{
							throw new ArgumentOutOfRangeException(string.Format("Operator {0} is not reconized.", array3[1]));
						}
						list.Add(new TextFilter(StringExtension.QueryNameToPropertyDef[(string)array3[0]], (string)array3[2], MatchOptions.Prefix, MatchFlags.Default));
					}
				}
			}
			if (list.Count <= 0)
			{
				return additionalFilter;
			}
			return string.Format("{0} -and {1}", new AndFilter(list.ToArray()).GenerateInfixString(FilterLanguage.Monad), additionalFilter);
		}

		public static string ToFFORecipientFilterString(this object searializedObj)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			string text = (searializedObj is DBNull) ? null : ((string)searializedObj);
			if (!string.IsNullOrEmpty(text))
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				object[] array = javaScriptSerializer.Deserialize<object[]>(text);
				new StringBuilder();
				foreach (object obj in array)
				{
					object[] array3 = (object[])obj;
					string text2 = (string)array3[0];
					if (text2.Equals("ANR", StringComparison.InvariantCultureIgnoreCase))
					{
						foreach (object obj2 in (object[])array3[2])
						{
							string text3 = (string)obj2;
							if (!string.IsNullOrEmpty(text3))
							{
								list.Add(new OrFilter(new List<QueryFilter>
								{
									new TextFilter(ADRecipientSchema.DisplayName, text3, MatchOptions.Prefix, MatchFlags.Default),
									new TextFilter(ADRecipientSchema.Alias, text3, MatchOptions.Prefix, MatchFlags.Default),
									new TextFilter(ADUserSchema.FirstName, text3, MatchOptions.Prefix, MatchFlags.Default),
									new TextFilter(ADUserSchema.LastName, text3, MatchOptions.Prefix, MatchFlags.Default),
									new TextFilter(ADUserSchema.Department, text3, MatchOptions.Prefix, MatchFlags.Default),
									new TextFilter(ADRecipientSchema.EmailAddresses, text3, MatchOptions.Prefix, MatchFlags.Default)
								}.ToArray()));
							}
						}
					}
					else if (array3[1].ToString().ToLowerInvariant() == "equals")
					{
						if (StringExtension.QueryNameToPropertyDef[(string)array3[0]].Type == typeof(bool) && !bool.Parse((string)array3[2]))
						{
							list.Add(new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, StringExtension.QueryNameToPropertyDef[(string)array3[0]], true)));
						}
						else
						{
							list.Add(new ComparisonFilter(ComparisonOperator.Equal, StringExtension.QueryNameToPropertyDef[(string)array3[0]], array3[2]));
						}
					}
					else
					{
						if (!(array3[1].ToString().ToLowerInvariant() == "startswith"))
						{
							throw new ArgumentOutOfRangeException(string.Format("Operator {0} is not reconized.", array3[1]));
						}
						if (!text2.Equals("recipienttypedetails", StringComparison.InvariantCultureIgnoreCase))
						{
							list.Add(new TextFilter(StringExtension.QueryNameToPropertyDef[(string)array3[0]], (string)array3[2], MatchOptions.Prefix, MatchFlags.Default));
						}
					}
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return new AndFilter(list.ToArray()).GenerateInfixString(FilterLanguage.Monad);
		}

		public static string[] ToFFOGroupTypeFilter(this object searializedObj, string originalFilter)
		{
			string text = (searializedObj is DBNull) ? null : ((string)searializedObj);
			if (!string.IsNullOrEmpty(text))
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				object[] array = javaScriptSerializer.Deserialize<object[]>(text);
				new StringBuilder();
				foreach (object obj in array)
				{
					object[] array3 = (object[])obj;
					string text2 = (string)array3[0];
					if (!text2.Equals("ANR", StringComparison.InvariantCultureIgnoreCase))
					{
						string commaSeparatedString = (string)array3[2];
						if (text2.Equals("recipienttypedetails", StringComparison.InvariantCultureIgnoreCase))
						{
							return commaSeparatedString.ToArrayOfStrings();
						}
					}
				}
			}
			return originalFilter.ToArrayOfStrings();
		}

		public static string ToPropertiesString(this object propertyList, object customProperties, string defaultValue)
		{
			if (!DBNull.Value.Equals(propertyList) && !string.IsNullOrEmpty((string)propertyList))
			{
				return (string)propertyList;
			}
			if (!DBNull.Value.Equals(customProperties) && !string.IsNullOrEmpty((string)customProperties))
			{
				return (string)customProperties + ',' + defaultValue;
			}
			return defaultValue;
		}

		public static string ToGroupFilterString(this object searchText, string additionalFilter)
		{
			QueryFilter queryFilter = null;
			string text = (searchText == DBNull.Value) ? string.Empty : ((string)searchText).Trim();
			if (!string.IsNullOrEmpty(text))
			{
				queryFilter = new OrFilter(new List<QueryFilter>
				{
					new TextFilter(ADRecipientSchema.DisplayName, text, MatchOptions.Prefix, MatchFlags.Default),
					new TextFilter(ADObjectSchema.Name, text, MatchOptions.Prefix, MatchFlags.Default)
				}.ToArray());
			}
			if (queryFilter == null)
			{
				return additionalFilter;
			}
			return string.Format("{0} -and {1}", new AndFilter(new QueryFilter[]
			{
				queryFilter
			}).GenerateInfixString(FilterLanguage.Monad), additionalFilter);
		}

		public static string ANRToMailboxFilterString(this object anr)
		{
			string text = (anr == DBNull.Value) ? string.Empty : ((string)anr);
			if (text.Length <= 0)
			{
				return string.Empty;
			}
			return new OrFilter(new QueryFilter[]
			{
				new TextFilter(ADObjectSchema.Name, text, MatchOptions.Prefix, MatchFlags.Default),
				new TextFilter(ADRecipientSchema.DisplayName, text, MatchOptions.Prefix, MatchFlags.Default),
				new TextFilter(ADRecipientSchema.Alias, text, MatchOptions.Prefix, MatchFlags.Default)
			}).GenerateInfixString(FilterLanguage.Monad);
		}

		public static string ToSourceMailboxFilterString(this object anr)
		{
			QueryFilter queryFilter = StringExtension.ToRecipientTypeDetailsFilter(new RecipientTypeDetails[]
			{
				RecipientTypeDetails.UserMailbox,
				RecipientTypeDetails.LinkedMailbox,
				RecipientTypeDetails.TeamMailbox,
				RecipientTypeDetails.SharedMailbox,
				RecipientTypeDetails.LegacyMailbox,
				RecipientTypeDetails.RoomMailbox,
				RecipientTypeDetails.EquipmentMailbox,
				(RecipientTypeDetails)((ulong)int.MinValue),
				RecipientTypeDetails.RemoteRoomMailbox,
				RecipientTypeDetails.RemoteEquipmentMailbox,
				RecipientTypeDetails.RemoteTeamMailbox,
				RecipientTypeDetails.RemoteSharedMailbox
			}, new RecipientTypeDetails[]
			{
				RecipientTypeDetails.MailUniversalDistributionGroup,
				RecipientTypeDetails.MailUniversalSecurityGroup,
				RecipientTypeDetails.MailNonUniversalGroup,
				RecipientTypeDetails.DynamicDistributionGroup
			}, ExchangeObjectVersion.Exchange2010);
			string text = (anr == DBNull.Value) ? string.Empty : ((string)anr);
			if (text.Length > 0)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					StringExtension.ANRToQueryFilter(text),
					queryFilter
				});
			}
			return LdapFilterBuilder.LdapFilterFromQueryFilter(queryFilter);
		}

		public static string ANRToRecipientFilterString(this object anr, object additionalFilter)
		{
			string text = (anr is DBNull) ? string.Empty : ((string)anr);
			string text2 = (additionalFilter is DBNull) ? "RecipientTypeDetails -eq 'User,DisabledUser'" : ((string)additionalFilter);
			string text3 = (text.Length > 0) ? StringExtension.ANRToQueryFilter(text).GenerateInfixString(FilterLanguage.Monad) : string.Empty;
			if (!string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text2))
			{
				return string.Format("({0}) -and ({1})", text3, text2);
			}
			if (!string.IsNullOrEmpty(text3) && string.IsNullOrEmpty(text2))
			{
				return text3;
			}
			if (string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text2))
			{
				return text2;
			}
			throw new ArgumentOutOfRangeException("Return null or empty filter will cause DDI service fail because -Filter:\"\" will fail in cmdlet.");
		}

		public static string FFOANRToRecipientFilterString(this object anr)
		{
			string text = (anr is DBNull) ? string.Empty : ((string)anr);
			return (text.Length > 0) ? StringExtension.FFOANRToQueryFilter(text).GenerateInfixString(FilterLanguage.Monad) : string.Empty;
		}

		public static string ToSuggestionFilterString(this object input, string additionalFilter)
		{
			List<string> list = new List<string>();
			string text = (input is DBNull) ? null : ((string)input);
			string arg = string.Empty;
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Replace("'", "''");
				list.Add(string.Format("Alias -like '{0}*'", text));
				list.Add(string.Format("DisplayName -like '{0}*'", text));
				list.Add(string.Format("Department -like '{0}*'", text));
				list.Add(string.Format("FirstName -like '{0}*'", text));
				list.Add(string.Format("LastName -like '{0}*'", text));
				list.Add(string.Format("EmailAddresses -like 'SMTP:{0}*'", text));
				list.Add(string.Format("ExternalEmailAddress -like 'SMTP:{0}*'", text));
				arg = string.Join(" -or ", list.ToArray());
				return string.Format("({0} -and ({1}))", additionalFilter, arg);
			}
			return string.Empty;
		}

		public static string ToSecurityGroupPickerFilterString(this object input)
		{
			string text = (input is DBNull) ? null : ((string)input);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Replace("'", "''");
				return string.Format("RecipientTypeDetails -eq 'UniversalSecurityGroup,MailUniversalSecurityGroup' -and Name -like '{0}*'", text);
			}
			return "RecipientTypeDetails -eq 'UniversalSecurityGroup,MailUniversalSecurityGroup'";
		}

		public static string ToArbitrationMailboxPickerFilterString(this object input)
		{
			string text = (input is DBNull) ? string.Empty : ((string)input);
			if (!string.IsNullOrEmpty(text))
			{
				return new OrFilter(new QueryFilter[]
				{
					new TextFilter(ADObjectSchema.Name, text, MatchOptions.Prefix, MatchFlags.Default),
					new TextFilter(ADRecipientSchema.DisplayName, text, MatchOptions.Prefix, MatchFlags.Default),
					new TextFilter(ADRecipientSchema.Alias, text, MatchOptions.Prefix, MatchFlags.Default)
				}).GenerateInfixString(FilterLanguage.Monad);
			}
			return string.Empty;
		}

		public static string[] ToArrayOfStrings(this string commaSeparatedString)
		{
			if (commaSeparatedString == null)
			{
				return null;
			}
			StringArrayConverter stringArrayConverter = new StringArrayConverter();
			return (from stringValue in ((string[])stringArrayConverter.ConvertFrom(commaSeparatedString)).AsEnumerable<string>()
			where stringValue != null && stringValue != string.Empty
			select stringValue).ToArray<string>();
		}

		public static string ToLogString<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> a)
		{
			StringBuilder stringBuilder = new StringBuilder("[");
			if (a != null)
			{
				foreach (KeyValuePair<T1, T2> keyValuePair in a)
				{
					stringBuilder.Append('[');
					StringBuilder stringBuilder2 = stringBuilder;
					T1 key = keyValuePair.Key;
					stringBuilder2.Append(key.ToString());
					stringBuilder.Append(", ");
					StringBuilder stringBuilder3 = stringBuilder;
					T2 value = keyValuePair.Value;
					stringBuilder3.Append(value.ToString());
					stringBuilder.Append("],");
				}
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		public static string ToLogString<T>(this IEnumerable<T> a)
		{
			StringBuilder stringBuilder = new StringBuilder("[");
			if (a != null)
			{
				foreach (T t in a)
				{
					stringBuilder.Append(t.ToString());
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		public static string RemoveAccelerator(this string value)
		{
			if (StringExtension.removeDBCSAcceleratorRegEx.IsMatch(value))
			{
				return StringExtension.removeDBCSAcceleratorRegEx.Replace(value, string.Empty);
			}
			return StringExtension.removeAcceleratorRegEx.Replace(value, "$1");
		}

		public static string SurrogateSubstring(this string value, int startIndex, int length)
		{
			if (startIndex < 0 || length < 0 || startIndex + length > value.Length)
			{
				throw new IndexOutOfRangeException();
			}
			if (startIndex > 0 && char.IsLowSurrogate(value, startIndex))
			{
				startIndex--;
			}
			if (length > 0 && char.IsHighSurrogate(value, startIndex + length - 1))
			{
				length--;
			}
			return value.Substring(startIndex, length);
		}

		internal static QueryFilter ANRToQueryFilter(string strAnr)
		{
			return new OrFilter(new QueryFilter[]
			{
				new TextFilter(ADObjectSchema.Name, strAnr, MatchOptions.Prefix, MatchFlags.Default),
				new TextFilter(ADRecipientSchema.DisplayName, strAnr, MatchOptions.Prefix, MatchFlags.Default),
				new TextFilter(ADRecipientSchema.Alias, strAnr, MatchOptions.Prefix, MatchFlags.Default),
				new TextFilter(ADUserSchema.FirstName, strAnr, MatchOptions.Prefix, MatchFlags.Default),
				new TextFilter(ADUserSchema.LastName, strAnr, MatchOptions.Prefix, MatchFlags.Default)
			});
		}

		internal static QueryFilter FFOANRToQueryFilter(string strAnr)
		{
			return new OrFilter(new List<QueryFilter>
			{
				new TextFilter(ADRecipientSchema.DisplayName, strAnr, MatchOptions.Prefix, MatchFlags.Default),
				new TextFilter(ADRecipientSchema.Alias, strAnr, MatchOptions.Prefix, MatchFlags.Default),
				new TextFilter(ADUserSchema.FirstName, strAnr, MatchOptions.Prefix, MatchFlags.Default),
				new TextFilter(ADUserSchema.LastName, strAnr, MatchOptions.Prefix, MatchFlags.Default),
				new TextFilter(ADRecipientSchema.EmailAddresses, strAnr, MatchOptions.Prefix, MatchFlags.Default)
			}.ToArray());
		}

		internal static QueryFilter ToRecipientTypeDetailsFilter(RecipientTypeDetails[] typeDetailsWithVersionCheck, RecipientTypeDetails[] typeDetailsWithoutVersionCheck, ExchangeObjectVersion minVersion)
		{
			QueryFilter queryFilter = null;
			if (typeDetailsWithVersionCheck != null && minVersion != null)
			{
				QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.ExchangeVersion, minVersion);
				QueryFilter recipientTypeDetailsFilter = RecipientIdParameter.GetRecipientTypeDetailsFilter(typeDetailsWithVersionCheck);
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter2,
					recipientTypeDetailsFilter
				});
			}
			if (typeDetailsWithoutVersionCheck != null)
			{
				QueryFilter recipientTypeDetailsFilter2 = RecipientIdParameter.GetRecipientTypeDetailsFilter(typeDetailsWithoutVersionCheck);
				queryFilter = ((queryFilter != null) ? new OrFilter(new QueryFilter[]
				{
					queryFilter,
					recipientTypeDetailsFilter2
				}) : recipientTypeDetailsFilter2);
			}
			return queryFilter;
		}

		internal static Dictionary<string, PropertyDefinition> QueryNameToPropertyDef = new Dictionary<string, PropertyDefinition>(StringComparer.InvariantCultureIgnoreCase)
		{
			{
				"DisplayName",
				ADRecipientSchema.DisplayName
			},
			{
				"Alias",
				ADRecipientSchema.Alias
			},
			{
				"Department",
				ADUserSchema.Department
			},
			{
				"EmailAddresses",
				ADRecipientSchema.EmailAddresses
			},
			{
				"ExternalEmailAddress",
				ADRecipientSchema.ExternalEmailAddress
			},
			{
				"FirstName",
				ADUserSchema.FirstName
			},
			{
				"LastName",
				ADUserSchema.LastName
			},
			{
				"RecipientTypeDetails",
				ADRecipientSchema.RecipientTypeDetails
			},
			{
				"City",
				ADUserSchema.City
			},
			{
				"Company",
				ADUserSchema.Company
			},
			{
				"CountryOrRegion",
				ADUserSchema.CountryOrRegion
			},
			{
				"CustomAttribute1",
				ADRecipientSchema.CustomAttribute1
			},
			{
				"CustomAttribute2",
				ADRecipientSchema.CustomAttribute2
			},
			{
				"CustomAttribute3",
				ADRecipientSchema.CustomAttribute3
			},
			{
				"CustomAttribute4",
				ADRecipientSchema.CustomAttribute4
			},
			{
				"CustomAttribute5",
				ADRecipientSchema.CustomAttribute5
			},
			{
				"CustomAttribute6",
				ADRecipientSchema.CustomAttribute6
			},
			{
				"CustomAttribute7",
				ADRecipientSchema.CustomAttribute7
			},
			{
				"CustomAttribute8",
				ADRecipientSchema.CustomAttribute8
			},
			{
				"CustomAttribute9",
				ADRecipientSchema.CustomAttribute9
			},
			{
				"CustomAttribute10",
				ADRecipientSchema.CustomAttribute10
			},
			{
				"CustomAttribute11",
				ADRecipientSchema.CustomAttribute11
			},
			{
				"CustomAttribute12",
				ADRecipientSchema.CustomAttribute12
			},
			{
				"CustomAttribute13",
				ADRecipientSchema.CustomAttribute13
			},
			{
				"CustomAttribute14",
				ADRecipientSchema.CustomAttribute14
			},
			{
				"CustomAttribute15",
				ADRecipientSchema.CustomAttribute15
			},
			{
				"Database",
				ADMailboxRecipientSchema.Database
			},
			{
				"EmailAddressPolicyEnabled",
				ADRecipientSchema.EmailAddressPolicyEnabled
			},
			{
				"LitigationHoldEnabled",
				ADUserSchema.LitigationHoldEnabled
			},
			{
				"ManagedBy",
				ADGroupSchema.ManagedBy
			},
			{
				"Manager",
				ADUserSchema.Manager
			},
			{
				"MemberOfGroup",
				ADRecipientSchema.MemberOfGroup
			},
			{
				"Office",
				ADUserSchema.Office
			},
			{
				"Location",
				ADUserSchema.Office
			},
			{
				"ServerName",
				ADMailboxRecipientSchema.ServerName
			},
			{
				"StateOrProvince",
				ADUserSchema.StateOrProvince
			},
			{
				"Title",
				ADUserSchema.Title
			},
			{
				"UMEnabled",
				ADUserSchema.UMEnabled
			},
			{
				"UMMailboxPolicy",
				ADUserSchema.UMMailboxPolicy
			}
		};

		private static Regex removeAcceleratorRegEx = new Regex("&([^&])", RegexOptions.Compiled);

		private static Regex removeDBCSAcceleratorRegEx = new Regex("\\(&([^&])\\)", RegexOptions.Compiled);
	}
}
