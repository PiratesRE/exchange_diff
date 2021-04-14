using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[Serializable]
	public class DlpPolicy : DlpPolicyPresentationBase
	{
		public string Name
		{
			get
			{
				return this.dlpPolicyMetaData.Name;
			}
			set
			{
				this.dlpPolicyMetaData.Name = value;
			}
		}

		public string Version
		{
			get
			{
				return this.dlpPolicyMetaData.Version;
			}
		}

		public string ContentVersion
		{
			get
			{
				return this.dlpPolicyMetaData.ContentVersion;
			}
		}

		public Guid ImmutableId
		{
			get
			{
				return this.dlpPolicyMetaData.ImmutableId;
			}
			set
			{
				this.dlpPolicyMetaData.ImmutableId = value;
			}
		}

		[LocDisplayName(Strings.IDs.DlpPolicyStateDisplayName)]
		[LocDescription(Strings.IDs.DlpPolicyStateDescription)]
		public RuleState State
		{
			get
			{
				return this.dlpPolicyMetaData.State;
			}
		}

		[LocDescription(Strings.IDs.DlpPolicyModeDescription)]
		[LocDisplayName(Strings.IDs.DlpPolicyModeDisplayName)]
		public RuleMode Mode
		{
			get
			{
				return this.dlpPolicyMetaData.Mode;
			}
		}

		[LocDescription(Strings.IDs.DlpPolicyDescriptionDescription)]
		[LocDisplayName(Strings.IDs.DlpPolicyDescriptionDisplayName)]
		public string Description
		{
			get
			{
				return this.dlpPolicyMetaData.Description;
			}
		}

		public string PublisherName
		{
			get
			{
				return this.dlpPolicyMetaData.PublisherName;
			}
		}

		public string[] Keywords
		{
			get
			{
				return this.dlpPolicyMetaData.Keywords.ToArray();
			}
		}

		public DlpPolicy() : this(null)
		{
		}

		internal DlpPolicy(ADComplianceProgram adDlpPolicy) : base(adDlpPolicy)
		{
			if (base.AdDlpPolicy != null)
			{
				base.AdDlpPolicy = base.AdDlpPolicy;
				this.dlpPolicyMetaData = DlpPolicyParser.ParseDlpPolicyInstance(base.AdDlpPolicy.TransportRulesXml);
				return;
			}
			base.AdDlpPolicy = new ADComplianceProgram();
			this.dlpPolicyMetaData = new DlpPolicyMetaData();
		}

		internal ObjectSchema Schema
		{
			get
			{
				return DlpPolicy.schema;
			}
		}

		internal void SetAdDlpPolicyWithNoDlpXml(ADComplianceProgram adDlpPolicy)
		{
			base.AdDlpPolicy = adDlpPolicy;
		}

		internal override void SuppressPiiData(PiiMap piiMap)
		{
			base.SuppressPiiData(piiMap);
			this.dlpPolicyMetaData.Name = (SuppressingPiiProperty.TryRedact(DlpPolicySchemaBase.Name, this.dlpPolicyMetaData.Name, piiMap) as string);
			this.dlpPolicyMetaData.Description = SuppressingPiiProperty.TryRedactValue<string>(DlpPolicySchemaBase.Description, this.dlpPolicyMetaData.Description);
			this.dlpPolicyMetaData.PublisherName = SuppressingPiiProperty.TryRedactValue<string>(DlpPolicySchemaBase.PublisherName, this.dlpPolicyMetaData.PublisherName);
			this.dlpPolicyMetaData.Keywords = SuppressingPiiProperty.TryRedactValue<List<string>>(DlpPolicySchemaBase.Keywords, this.dlpPolicyMetaData.Keywords);
		}

		private readonly DlpPolicyMetaData dlpPolicyMetaData;

		private static readonly DlpPolicySchemaBase schema = ObjectSchema.GetInstance<DlpPolicySchemaBase>();
	}
}
