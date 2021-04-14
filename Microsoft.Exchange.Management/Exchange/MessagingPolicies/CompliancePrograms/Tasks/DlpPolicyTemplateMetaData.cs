using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[Serializable]
	internal class DlpPolicyTemplateMetaData
	{
		internal string Name
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(this.name))
				{
					return this.name;
				}
				if (this.LocalizedNames.Any<KeyValuePair<string, string>>())
				{
					return this.LocalizedNames.First<KeyValuePair<string, string>>().Value;
				}
				return string.Empty;
			}
			set
			{
				this.name = value;
			}
		}

		internal string Version { get; set; }

		internal RuleState State { get; set; }

		internal RuleMode Mode { get; set; }

		internal Guid ImmutableId { get; set; }

		internal string ContentVersion { get; set; }

		internal string PublisherName { get; set; }

		internal Dictionary<string, string> LocalizedNames { get; set; }

		internal Dictionary<string, string> LocalizedDescriptions { get; set; }

		internal List<Dictionary<string, string>> LocalizedKeywords { get; set; }

		internal List<DlpTemplateRuleParameter> RuleParameters { get; set; }

		internal List<string> PolicyCommands { get; set; }

		internal Dictionary<string, Dictionary<string, string>> LocalizedPolicyCommandResources { get; set; }

		internal void Validate()
		{
			if (this.LocalizedNames == null || !this.LocalizedNames.Any<KeyValuePair<string, string>>() || string.IsNullOrEmpty(this.Version) || this.ImmutableId == Guid.Empty || string.IsNullOrEmpty(this.ContentVersion) || string.IsNullOrEmpty(this.PublisherName) || this.LocalizedDescriptions == null || !this.LocalizedDescriptions.Any<KeyValuePair<string, string>>() || this.PolicyCommands == null || !this.PolicyCommands.Any<string>())
			{
				throw new DlpPolicyParsingException(Strings.DlpPolicyXmlMissingElements);
			}
			if (new Version(this.Version) > DlpUtils.MaxSupportedVersion)
			{
				throw new DlpPolicyParsingException(Strings.DlpPolicyVersionUnsupported);
			}
			if (this.LocalizedKeywords.Any((Dictionary<string, string> keywords) => keywords.Any((KeyValuePair<string, string> keyword) => string.IsNullOrEmpty(keyword.Value))))
			{
				throw new DlpPolicyParsingException(Strings.DlpPolicyContainsEmptyKeywords);
			}
			DlpPolicyTemplateMetaData.ValidateFieldSize("version", this.Version, 16);
			DlpPolicyTemplateMetaData.ValidateFieldSize("contentVersion", this.ContentVersion, 16);
			DlpPolicyTemplateMetaData.ValidateFieldSize("publisherName", this.PublisherName, 256);
			this.LocalizedNames.Values.ToList<string>().ForEach(delegate(string x)
			{
				DlpPolicyTemplateMetaData.ValidateFieldSize("name", x, 64);
			});
			this.LocalizedDescriptions.Values.ToList<string>().ForEach(delegate(string x)
			{
				DlpPolicyTemplateMetaData.ValidateFieldSize("description", x, 1024);
			});
			this.LocalizedKeywords.ToList<Dictionary<string, string>>().ForEach(delegate(Dictionary<string, string> keywords)
			{
				keywords.Values.ToList<string>().ForEach(delegate(string keyword)
				{
					DlpPolicyTemplateMetaData.ValidateFieldSize("keyword", keyword, 64);
				});
			});
			List<DlpTemplateRuleParameter> list = this.RuleParameters.ToList<DlpTemplateRuleParameter>();
			list.ForEach(delegate(DlpTemplateRuleParameter x)
			{
				DlpPolicyTemplateMetaData.ValidateFieldSize("type", x.Type, 32);
			});
			list.ForEach(delegate(DlpTemplateRuleParameter x)
			{
				DlpPolicyTemplateMetaData.ValidateFieldSize("token", x.Token, 32);
			});
			list.ForEach(delegate(DlpTemplateRuleParameter x)
			{
				x.LocalizedDescriptions.ToList<KeyValuePair<string, string>>().ForEach(delegate(KeyValuePair<string, string> y)
				{
					DlpPolicyTemplateMetaData.ValidateFieldSize("description", y.Value, 1024);
				});
			});
			this.LocalizedPolicyCommandResources.Values.ToList<Dictionary<string, string>>().ForEach(delegate(Dictionary<string, string> resources)
			{
				resources.Values.ToList<string>().ForEach(delegate(string resource)
				{
					DlpPolicyTemplateMetaData.ValidateFieldSize("resource", resource, 1024);
				});
			});
			List<string> list2 = this.PolicyCommands.ToList<string>();
			list2.ForEach(delegate(string x)
			{
				DlpPolicyTemplateMetaData.ValidateFieldSize("commandBlock", x, 4096);
			});
			list2.ForEach(delegate(string command)
			{
				DlpPolicyTemplateMetaData.ValidateCmdletParameters(command);
			});
		}

		internal static void ValidateFieldSize(string fieldName, string value, int sizeLimit)
		{
			if (value.Length > sizeLimit)
			{
				throw new DlpPolicyParsingException(Strings.DlpPolicyFieldLengthsExceedsLimit(fieldName, sizeLimit));
			}
		}

		internal static void ValidateCmdletParameters(string cmdlet)
		{
			CmdletValidator cmdletValidator = new CmdletValidator(DlpPolicyTemplateMetaData.AllowedCommands, DlpPolicyTemplateMetaData.RequiredParams, null);
			ScriptParseResult scriptParseResult = cmdletValidator.ParseCmdletScript(cmdlet);
			if (!scriptParseResult.IsSuccessful)
			{
				throw new DlpPolicyParsingException(Strings.DlpPolicyNotSupportedCmdlet(cmdlet));
			}
		}

		internal ADComplianceProgram ToAdObject()
		{
			string transportRulesXml;
			using (MemoryStream memoryStream = new MemoryStream(DlpPolicyParser.SerializeDlpPolicyTemplate(this)))
			{
				StreamReader streamReader = new StreamReader(memoryStream);
				transportRulesXml = streamReader.ReadToEnd();
			}
			ADComplianceProgram adcomplianceProgram = new ADComplianceProgram();
			adcomplianceProgram.Name = DlpPolicyTemplateMetaData.GetLocalizedStringValue(this.LocalizedNames, null);
			adcomplianceProgram.Description = DlpPolicyTemplateMetaData.GetLocalizedStringValue(this.LocalizedDescriptions, null);
			adcomplianceProgram.ImmutableId = this.ImmutableId;
			adcomplianceProgram.Keywords = (from keyword in this.LocalizedKeywords
			select DlpPolicyTemplateMetaData.GetLocalizedStringValue(keyword, DlpPolicyTemplateMetaData.DefaultCulture)).ToArray<string>();
			adcomplianceProgram.PublisherName = this.PublisherName;
			adcomplianceProgram.State = DlpUtils.RuleStateToDlpState(this.State, this.Mode);
			adcomplianceProgram.TransportRulesXml = transportRulesXml;
			adcomplianceProgram.Version = this.Version;
			return adcomplianceProgram;
		}

		internal static string GetLocalizedStringValue(IDictionary<string, string> localizedStrings, CultureInfo culture = null)
		{
			if (culture == null)
			{
				culture = DlpPolicyTemplateMetaData.DefaultCulture;
			}
			if (!localizedStrings.Any<KeyValuePair<string, string>>())
			{
				return string.Empty;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>(localizedStrings, StringComparer.CurrentCultureIgnoreCase);
			int num = 5;
			while (num-- > 0)
			{
				if (dictionary.ContainsKey(culture.Name))
				{
					return dictionary[culture.Name];
				}
				culture = culture.Parent;
				if (culture == CultureInfo.InvariantCulture)
				{
					break;
				}
			}
			if (dictionary.ContainsKey(DlpPolicyTemplateMetaData.DefaultCulture.Name))
			{
				return dictionary[DlpPolicyTemplateMetaData.DefaultCulture.Name];
			}
			return dictionary.FirstOrDefault<KeyValuePair<string, string>>().Value;
		}

		internal static IEnumerable<string> LocalizeCmdlets(IEnumerable<string> policyCommands, Dictionary<string, Dictionary<string, string>> policyCommandResources, CultureInfo culture)
		{
			IDictionary<string, string> localizedResources = (from localizedResource in policyCommandResources
			select new KeyValuePair<string, string>(localizedResource.Key, DlpPolicyTemplateMetaData.GetLocalizedStringValue(localizedResource.Value, culture))).ToDictionary((KeyValuePair<string, string> pair) => pair.Key, (KeyValuePair<string, string> pair) => pair.Value, StringComparer.OrdinalIgnoreCase);
			return from cmdlet in policyCommands
			select localizedResources.Aggregate(cmdlet, (string current, KeyValuePair<string, string> parameter) => current.Replace(parameter.Key, parameter.Value).Trim());
		}

		public static readonly CultureInfo DefaultCulture = new CultureInfo("en");

		public static readonly HashSet<string> AllowedCommands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"New-TransportRule"
		};

		public static readonly Dictionary<string, HashSet<string>> RequiredParams = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"New-TransportRule",
				new HashSet<string>(StringComparer.OrdinalIgnoreCase)
				{
					"-DlpPolicy"
				}
			}
		};

		private string name;
	}
}
