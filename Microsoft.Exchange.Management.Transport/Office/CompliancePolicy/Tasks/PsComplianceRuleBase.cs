using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public class PsComplianceRuleBase : ADPresentationObject
	{
		public PsComplianceRuleBase()
		{
		}

		public PsComplianceRuleBase(RuleStorage ruleStorage) : base(ruleStorage)
		{
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return PsComplianceRuleBase.schema;
			}
		}

		internal Guid MasterIdentity
		{
			get
			{
				return (Guid)this[PsComplianceRuleBaseSchema.MasterIdentity];
			}
			set
			{
				this[PsComplianceRuleBaseSchema.MasterIdentity] = value;
			}
		}

		public bool ReadOnly { get; internal set; }

		public string ExternalIdentity
		{
			get
			{
				if (this.MasterIdentity != Guid.Empty)
				{
					return this.MasterIdentity.ToString();
				}
				return string.Empty;
			}
		}

		internal string RuleBlob
		{
			get
			{
				return (string)this[PsComplianceRuleBaseSchema.RuleBlob];
			}
			set
			{
				this[PsComplianceRuleBaseSchema.RuleBlob] = value;
			}
		}

		public Workload Workload
		{
			get
			{
				return (Workload)this[PsComplianceRuleBaseSchema.Workload];
			}
			set
			{
				this[PsComplianceRuleBaseSchema.Workload] = value;
			}
		}

		public Guid Policy
		{
			get
			{
				return (Guid)this[PsComplianceRuleBaseSchema.Policy];
			}
			set
			{
				this[PsComplianceRuleBaseSchema.Policy] = value;
			}
		}

		public string Comment
		{
			get
			{
				return (string)this[PsComplianceRuleBaseSchema.Comment];
			}
			set
			{
				this[PsComplianceRuleBaseSchema.Comment] = value;
			}
		}

		protected bool Enabled
		{
			get
			{
				return (bool)this[PsComplianceRuleBaseSchema.Enabled];
			}
			set
			{
				this[PsComplianceRuleBaseSchema.Enabled] = value;
			}
		}

		public bool Disabled
		{
			get
			{
				return !this.Enabled;
			}
			set
			{
				this.Enabled = !value;
			}
		}

		public Mode Mode
		{
			get
			{
				return (Mode)this[PsComplianceRuleBaseSchema.Mode];
			}
			set
			{
				this[PsComplianceRuleBaseSchema.Mode] = value;
			}
		}

		public Guid ObjectVersion
		{
			get
			{
				return (Guid)this[PsComplianceRuleBaseSchema.ObjectVersion];
			}
		}

		public string ContentMatchQuery
		{
			get
			{
				return (string)this[PsComplianceRuleBaseSchema.ContentMatchQuery];
			}
			set
			{
				this[PsComplianceRuleBaseSchema.ContentMatchQuery] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		public string CreatedBy { get; protected set; }

		public string LastModifiedBy { get; protected set; }

		internal virtual string GetRuleXmlFromPolicyRule(PolicyRule policyRule)
		{
			return new RuleSerializer().SaveRuleToString(policyRule);
		}

		internal virtual void PopulateTaskProperties(Task task, IConfigurationSession configurationSession)
		{
			RuleStorage ruleStorage = base.DataObject as RuleStorage;
			ADUser userObjectByExternalDirectoryObjectId = Utils.GetUserObjectByExternalDirectoryObjectId(ruleStorage.CreatedBy, configurationSession);
			ADUser userObjectByExternalDirectoryObjectId2 = Utils.GetUserObjectByExternalDirectoryObjectId(ruleStorage.LastModifiedBy, configurationSession);
			this.CreatedBy = ((!Utils.ExecutingUserIsForestWideAdmin(task) && userObjectByExternalDirectoryObjectId != null) ? userObjectByExternalDirectoryObjectId.DisplayName : ruleStorage.CreatedBy);
			this.LastModifiedBy = ((!Utils.ExecutingUserIsForestWideAdmin(task) && userObjectByExternalDirectoryObjectId2 != null) ? userObjectByExternalDirectoryObjectId2.DisplayName : ruleStorage.LastModifiedBy);
		}

		internal virtual void UpdateStorageProperties(Task task, IConfigurationSession configurationSession, bool isNewRule)
		{
			if (!Utils.ExecutingUserIsForestWideAdmin(task))
			{
				ADObjectId objectId;
				task.TryGetExecutingUserId(out objectId);
				ADUser userObjectByObjectId = Utils.GetUserObjectByObjectId(objectId, configurationSession);
				if (userObjectByObjectId != null)
				{
					RuleStorage ruleStorage = base.DataObject as RuleStorage;
					ruleStorage.LastModifiedBy = userObjectByObjectId.ExternalDirectoryObjectId;
					if (isNewRule)
					{
						ruleStorage.CreatedBy = userObjectByObjectId.ExternalDirectoryObjectId;
					}
				}
			}
		}

		internal virtual PolicyRule GetPolicyRuleFromRuleBlob()
		{
			throw new NotImplementedException("GetPolicyRuleFromRuleBlob must be implemented by the derived class");
		}

		internal void SuppressPiiData(PiiMap piiMap)
		{
			base.Name = (SuppressingPiiProperty.TryRedact(ADObjectSchema.Name, base.Name, piiMap) as string);
		}

		private static readonly PsComplianceRuleBaseSchema schema = ObjectSchema.GetInstance<PsComplianceRuleBaseSchema>();
	}
}
