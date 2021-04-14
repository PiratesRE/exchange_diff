using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[Serializable]
	public class DlpPolicyTemplate : DlpPolicyPresentationBase
	{
		private CultureInfo CurrentCulture { get; set; }

		public string Name
		{
			get
			{
				return this.dlpTemplateMetaData.Name;
			}
			set
			{
				this.dlpTemplateMetaData.Name = value;
			}
		}

		public string Version
		{
			get
			{
				return this.dlpTemplateMetaData.Version;
			}
		}

		public string ContentVersion
		{
			get
			{
				return this.dlpTemplateMetaData.ContentVersion;
			}
		}

		public Guid ImmutableId
		{
			get
			{
				return this.dlpTemplateMetaData.ImmutableId;
			}
			set
			{
				this.dlpTemplateMetaData.ImmutableId = value;
			}
		}

		[LocDescription(Strings.IDs.DlpPolicyStateDescription)]
		[LocDisplayName(Strings.IDs.DlpPolicyStateDisplayName)]
		public RuleState State
		{
			get
			{
				return this.dlpTemplateMetaData.State;
			}
		}

		[LocDescription(Strings.IDs.DlpPolicyModeDescription)]
		[LocDisplayName(Strings.IDs.DlpPolicyModeDisplayName)]
		public RuleMode Mode
		{
			get
			{
				return this.dlpTemplateMetaData.Mode;
			}
		}

		[LocDisplayName(Strings.IDs.DlpPolicyDescriptionDisplayName)]
		[LocDescription(Strings.IDs.DlpPolicyDescriptionDescription)]
		public string Description
		{
			get
			{
				return DlpPolicyTemplateMetaData.GetLocalizedStringValue(this.dlpTemplateMetaData.LocalizedDescriptions, this.CurrentCulture);
			}
		}

		public string PublisherName
		{
			get
			{
				return this.dlpTemplateMetaData.PublisherName;
			}
		}

		public string LocalizedName
		{
			get
			{
				return DlpPolicyTemplateMetaData.GetLocalizedStringValue(this.dlpTemplateMetaData.LocalizedNames, this.CurrentCulture);
			}
		}

		public string[] Keywords
		{
			get
			{
				return (from keyword in this.dlpTemplateMetaData.LocalizedKeywords
				select DlpPolicyTemplateMetaData.GetLocalizedStringValue(keyword, this.CurrentCulture)).ToArray<string>();
			}
		}

		public MultiValuedProperty<string> RuleParameters
		{
			get
			{
				return (from parameter in this.dlpTemplateMetaData.RuleParameters
				select parameter.ToString(this.CurrentCulture)).ToArray<string>();
			}
		}

		public DlpPolicyTemplate() : base(new ADComplianceProgram())
		{
			this.CurrentCulture = DlpPolicyTemplateMetaData.DefaultCulture;
		}

		public DlpPolicyTemplate(ADComplianceProgram dlpPolicy, CultureInfo culture) : base(dlpPolicy)
		{
			if (base.AdDlpPolicy != null)
			{
				base.AdDlpPolicy = base.AdDlpPolicy;
				this.dlpTemplateMetaData = DlpPolicyParser.ParseDlpPolicyTemplate(base.AdDlpPolicy.TransportRulesXml);
			}
			else
			{
				base.AdDlpPolicy = new ADComplianceProgram();
				this.dlpTemplateMetaData = new DlpPolicyTemplateMetaData();
			}
			this.CurrentCulture = culture;
		}

		private ObjectSchema ObjectSchema
		{
			get
			{
				return DlpPolicyTemplate.schema;
			}
		}

		private readonly DlpPolicyTemplateMetaData dlpTemplateMetaData;

		private static readonly DlpPolicyTemplateSchema schema = ObjectSchema.GetInstance<DlpPolicyTemplateSchema>();
	}
}
