using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[Serializable]
	internal class DlpPolicyMetaData
	{
		internal string Version { get; set; }

		internal RuleState State { get; set; }

		internal RuleMode Mode { get; set; }

		internal string ContentVersion { get; set; }

		internal string PublisherName { get; set; }

		internal string Name { get; set; }

		internal string Description { get; set; }

		internal List<string> Keywords { get; set; }

		internal Guid ImmutableId { get; set; }

		internal List<string> PolicyCommands { get; set; }

		internal DlpPolicyMetaData()
		{
			this.Keywords = new List<string>();
			this.PolicyCommands = new List<string>();
			this.ImmutableId = Guid.NewGuid();
			this.Mode = RuleMode.Audit;
			this.State = RuleState.Enabled;
		}

		internal DlpPolicyMetaData(DlpPolicyTemplateMetaData dlpTemplate, CultureInfo culture = null)
		{
			this.Name = DlpPolicyTemplateMetaData.GetLocalizedStringValue(dlpTemplate.LocalizedNames, culture);
			this.Description = DlpPolicyTemplateMetaData.GetLocalizedStringValue(dlpTemplate.LocalizedDescriptions, culture);
			this.ContentVersion = dlpTemplate.ContentVersion;
			this.Version = dlpTemplate.Version;
			this.Mode = dlpTemplate.Mode;
			this.State = dlpTemplate.State;
			this.PublisherName = dlpTemplate.PublisherName;
			this.ImmutableId = Guid.NewGuid();
			this.Keywords = (from keyword in dlpTemplate.LocalizedKeywords
			select DlpPolicyTemplateMetaData.GetLocalizedStringValue(keyword, culture)).ToArray<string>().ToList<string>();
			this.PolicyCommands = dlpTemplate.PolicyCommands;
		}

		internal void Validate()
		{
			if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(this.Version))
			{
				throw new DlpPolicyParsingException(Strings.DlpPolicyXmlMissingElements);
			}
			if (new Version(this.Version) > DlpUtils.MaxSupportedVersion)
			{
				throw new DlpPolicyParsingException(Strings.DlpPolicyVersionUnsupported);
			}
			if (this.Keywords.Any(new Func<string, bool>(string.IsNullOrEmpty)))
			{
				throw new DlpPolicyParsingException(Strings.DlpPolicyContainsEmptyKeywords);
			}
			DlpPolicyTemplateMetaData.ValidateFieldSize("name", this.Name, 64);
			DlpPolicyTemplateMetaData.ValidateFieldSize("version", this.Version, 16);
			DlpPolicyTemplateMetaData.ValidateFieldSize("contentVersion", this.ContentVersion, 16);
			DlpPolicyTemplateMetaData.ValidateFieldSize("publisherName", this.PublisherName, 256);
			DlpPolicyTemplateMetaData.ValidateFieldSize("description", this.Description, 1024);
			this.Keywords.ForEach(delegate(string x)
			{
				DlpPolicyTemplateMetaData.ValidateFieldSize("keyword", x, 64);
			});
			this.PolicyCommands.ForEach(delegate(string command)
			{
				DlpPolicyTemplateMetaData.ValidateFieldSize("commandBlock", command, 4096);
			});
			this.PolicyCommands.ForEach(delegate(string command)
			{
				DlpPolicyTemplateMetaData.ValidateCmdletParameters(command);
			});
		}

		internal ADComplianceProgram ToAdObject()
		{
			return new ADComplianceProgram
			{
				Name = this.Name,
				Description = this.Description,
				ImmutableId = this.ImmutableId,
				Keywords = this.Keywords.ToArray(),
				PublisherName = this.PublisherName,
				State = DlpUtils.RuleStateToDlpState(this.State, this.Mode),
				TransportRulesXml = new StreamReader(new MemoryStream(DlpPolicyParser.SerializeDlpPolicyInstance(this))).ReadToEnd(),
				Version = this.Version
			};
		}
	}
}
