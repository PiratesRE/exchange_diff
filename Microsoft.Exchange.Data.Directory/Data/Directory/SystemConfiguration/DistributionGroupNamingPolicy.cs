using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class DistributionGroupNamingPolicy
	{
		static DistributionGroupNamingPolicy()
		{
			DistributionGroupNamingPolicy.placeHolders.Add("<GroupName>");
			DistributionGroupNamingPolicy.placeHolders.Add("<<");
			DistributionGroupNamingPolicy.placeHolders.Add(">>");
			string[] array = DistributionGroupNamingPolicy.placeHolders.ToArray();
			DistributionGroupNamingPolicy.placeHoldersString = string.Join(", ", array);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("^(([^<>]*)");
			foreach (string text in array)
			{
				if (text == "<GroupName>")
				{
					text = "?<originalName>" + text;
				}
				stringBuilder.AppendFormat("|({0})", text);
			}
			stringBuilder.Append(")*$");
			DistributionGroupNamingPolicy.namingPolicyValidationRegex = new Regex(stringBuilder.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant);
			string pattern = string.Join("|", array);
			DistributionGroupNamingPolicy.placeHoldersReplacementRegex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
			StringBuilder stringBuilder2 = new StringBuilder();
			List<string> list = DistributionGroupNamingPolicy.propertyMapping.Keys.ToList<string>();
			list.Add("<GroupName>");
			for (int j = 0; j < list.Count; j++)
			{
				string arg = array[j];
				stringBuilder2.AppendFormat("({0}(?!>))", arg);
				if (j < list.Count - 1)
				{
					stringBuilder2.Append("|");
				}
			}
			DistributionGroupNamingPolicy.splitRegex = new Regex(stringBuilder2.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant);
		}

		private DistributionGroupNamingPolicy(string namingPolicy)
		{
			this.namingPolicyString = namingPolicy;
		}

		public static DistributionGroupNamingPolicy Parse(string namingPolicy)
		{
			DistributionGroupNamingPolicy result = null;
			if (!DistributionGroupNamingPolicy.TryParse(namingPolicy, out result))
			{
				throw new FormatException(DirectoryStrings.InvalidDistributionGroupNamingPolicyFormat(namingPolicy, DistributionGroupNamingPolicy.placeHoldersString));
			}
			return result;
		}

		public static bool TryParse(string namingPolicy, out DistributionGroupNamingPolicy instance)
		{
			if (string.IsNullOrEmpty(namingPolicy))
			{
				instance = new DistributionGroupNamingPolicy(namingPolicy);
				return true;
			}
			if (char.IsWhiteSpace(namingPolicy[0]) || char.IsWhiteSpace(namingPolicy[namingPolicy.Length - 1]))
			{
				instance = null;
				return false;
			}
			Match match = DistributionGroupNamingPolicy.namingPolicyValidationRegex.Match(namingPolicy);
			if (!match.Success || match.Groups["originalName"].Captures.Count != 1)
			{
				instance = null;
				return false;
			}
			instance = new DistributionGroupNamingPolicy(namingPolicy);
			return true;
		}

		public string GetAppliedName(string originalName, ADUser user)
		{
			if (this.IsEmpty)
			{
				return originalName;
			}
			return DistributionGroupNamingPolicy.placeHoldersReplacementRegex.Replace(this.namingPolicyString, delegate(Match match)
			{
				string value = match.Value;
				if (value == "<GroupName>")
				{
					return originalName;
				}
				if (value == "<<")
				{
					return "<";
				}
				if (value == ">>")
				{
					return ">";
				}
				if (!DistributionGroupNamingPolicy.propertyMapping.ContainsKey(value))
				{
					return match.Value;
				}
				if (value == "<CountryCode>")
				{
					string result = string.Empty;
					if (user.CountryOrRegion != null)
					{
						result = user.CountryOrRegion.CountryCode.ToString();
					}
					return result;
				}
				return this.GetAttributeInString(user[DistributionGroupNamingPolicy.propertyMapping[value]]);
			});
		}

		private static string UnescapeText(string text)
		{
			return text.Replace("<<", "<").Replace(">>", ">");
		}

		private string GetAttributeInString(object attribute)
		{
			if (attribute == null)
			{
				return string.Empty;
			}
			return attribute.ToString();
		}

		public static string GroupNameLocString
		{
			get
			{
				return DirectoryStrings.GroupNameInNamingPolicy;
			}
		}

		public string[] PrefixElements
		{
			get
			{
				this.Split();
				return this.prefix;
			}
		}

		public string[] SuffixElements
		{
			get
			{
				this.Split();
				return this.suffix;
			}
		}

		private void Split()
		{
			if (this.IsEmpty)
			{
				return;
			}
			if (this.prefix != null || this.suffix != null)
			{
				return;
			}
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			bool flag = true;
			foreach (string text in DistributionGroupNamingPolicy.splitRegex.Split(this.namingPolicyString))
			{
				if (!string.IsNullOrEmpty(text))
				{
					if (text == "<GroupName>")
					{
						flag = false;
					}
					else if (flag)
					{
						list.Add(text);
					}
					else
					{
						list2.Add(text);
					}
				}
			}
			this.prefix = list.ToArray();
			this.suffix = list2.ToArray();
		}

		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.namingPolicyString);
			}
		}

		public override string ToString()
		{
			return this.namingPolicyString;
		}

		public string ToLocalizedString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}<{1}>{2}", this.GetLocalizedString(this.PrefixElements), DistributionGroupNamingPolicy.GroupNameLocString, this.GetLocalizedString(this.SuffixElements));
			return stringBuilder.ToString();
		}

		private string GetLocalizedString(string[] elements)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in elements)
			{
				if (text.Length > 1 && text[0] == '<' && text[text.Length - 1] == '>')
				{
					string value = text.Substring(1, text.Length - 2);
					if (Enum.IsDefined(typeof(GroupNamingPolicyAttributeEnum), value))
					{
						GroupNamingPolicyAttributeEnum groupNamingPolicyAttributeEnum = (GroupNamingPolicyAttributeEnum)Enum.Parse(typeof(GroupNamingPolicyAttributeEnum), value);
						stringBuilder.AppendFormat("<{0}>", LocalizedDescriptionAttribute.FromEnum(typeof(GroupNamingPolicyAttributeEnum), groupNamingPolicyAttributeEnum));
					}
					else
					{
						stringBuilder.Append(DistributionGroupNamingPolicy.UnescapeText(text));
					}
				}
				else
				{
					stringBuilder.Append(DistributionGroupNamingPolicy.UnescapeText(text));
				}
			}
			return stringBuilder.ToString();
		}

		private const string OriginalGroupNamePlaceHolder = "<GroupName>";

		private static readonly Dictionary<string, ADPropertyDefinition> propertyMapping = new Dictionary<string, ADPropertyDefinition>
		{
			{
				"<Department>",
				ADOrgPersonSchema.Department
			},
			{
				"<Company>",
				ADOrgPersonSchema.Company
			},
			{
				"<Office>",
				ADOrgPersonSchema.Office
			},
			{
				"<City>",
				ADOrgPersonSchema.City
			},
			{
				"<StateOrProvince>",
				ADOrgPersonSchema.StateOrProvince
			},
			{
				"<CountryOrRegion>",
				ADOrgPersonSchema.CountryOrRegion
			},
			{
				"<CountryCode>",
				ADOrgPersonSchema.CountryCode
			},
			{
				"<Title>",
				ADOrgPersonSchema.Title
			},
			{
				"<CustomAttribute1>",
				ADRecipientSchema.CustomAttribute1
			},
			{
				"<CustomAttribute2>",
				ADRecipientSchema.CustomAttribute2
			},
			{
				"<CustomAttribute3>",
				ADRecipientSchema.CustomAttribute3
			},
			{
				"<CustomAttribute4>",
				ADRecipientSchema.CustomAttribute4
			},
			{
				"<CustomAttribute5>",
				ADRecipientSchema.CustomAttribute5
			},
			{
				"<CustomAttribute6>",
				ADRecipientSchema.CustomAttribute6
			},
			{
				"<CustomAttribute7>",
				ADRecipientSchema.CustomAttribute7
			},
			{
				"<CustomAttribute8>",
				ADRecipientSchema.CustomAttribute8
			},
			{
				"<CustomAttribute9>",
				ADRecipientSchema.CustomAttribute9
			},
			{
				"<CustomAttribute10>",
				ADRecipientSchema.CustomAttribute10
			},
			{
				"<CustomAttribute11>",
				ADRecipientSchema.CustomAttribute11
			},
			{
				"<CustomAttribute12>",
				ADRecipientSchema.CustomAttribute12
			},
			{
				"<CustomAttribute13>",
				ADRecipientSchema.CustomAttribute13
			},
			{
				"<CustomAttribute14>",
				ADRecipientSchema.CustomAttribute14
			},
			{
				"<CustomAttribute15>",
				ADRecipientSchema.CustomAttribute15
			},
			{
				"<ExtensionCustomAttribute1>",
				ADRecipientSchema.ExtensionCustomAttribute1
			},
			{
				"<ExtensionCustomAttribute2>",
				ADRecipientSchema.ExtensionCustomAttribute2
			},
			{
				"<ExtensionCustomAttribute3>",
				ADRecipientSchema.ExtensionCustomAttribute3
			},
			{
				"<ExtensionCustomAttribute4>",
				ADRecipientSchema.ExtensionCustomAttribute4
			},
			{
				"<ExtensionCustomAttribute5>",
				ADRecipientSchema.ExtensionCustomAttribute5
			}
		};

		private static readonly List<string> placeHolders = DistributionGroupNamingPolicy.propertyMapping.Keys.ToList<string>();

		private static readonly string placeHoldersString;

		private static readonly Regex namingPolicyValidationRegex;

		private static readonly Regex placeHoldersReplacementRegex;

		private static readonly Regex splitRegex;

		private string namingPolicyString;

		private string[] prefix;

		private string[] suffix;
	}
}
