using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[Serializable]
	public class RecipientTemplateProvisioningPolicy : TemplateProvisioningPolicy
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return RecipientTemplateProvisioningPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RecipientTemplateProvisioningPolicy.MostDerivedClass;
			}
		}

		internal override ICollection<Type> SupportedPresentationObjectTypes
		{
			get
			{
				return ProvisioningHelper.AllSupportedRecipientTypes;
			}
		}

		internal override IEnumerable<IProvisioningTemplate> ProvisioningTemplateRules
		{
			get
			{
				return RecipientTemplateProvisioningPolicy.provisioningTemplates;
			}
		}

		public RecipientTemplateProvisioningPolicy()
		{
			base.Name = "Recipient Template Policy";
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> DefaultMaxSendSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[RecipientTemplateProvisioningPolicySchema.DefaultMaxSendSize];
			}
			set
			{
				this[RecipientTemplateProvisioningPolicySchema.DefaultMaxSendSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> DefaultMaxReceiveSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[RecipientTemplateProvisioningPolicySchema.DefaultMaxReceiveSize];
			}
			set
			{
				this[RecipientTemplateProvisioningPolicySchema.DefaultMaxReceiveSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> DefaultProhibitSendQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[RecipientTemplateProvisioningPolicySchema.DefaultProhibitSendQuota];
			}
			set
			{
				this[RecipientTemplateProvisioningPolicySchema.DefaultProhibitSendQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> DefaultProhibitSendReceiveQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[RecipientTemplateProvisioningPolicySchema.DefaultProhibitSendReceiveQuota];
			}
			set
			{
				this[RecipientTemplateProvisioningPolicySchema.DefaultProhibitSendReceiveQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> DefaultIssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[RecipientTemplateProvisioningPolicySchema.DefaultIssueWarningQuota];
			}
			set
			{
				this[RecipientTemplateProvisioningPolicySchema.DefaultIssueWarningQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize? DefaultRulesQuota
		{
			get
			{
				return (ByteQuantifiedSize?)this[RecipientTemplateProvisioningPolicySchema.DefaultRulesQuota];
			}
			set
			{
				this[RecipientTemplateProvisioningPolicySchema.DefaultRulesQuota] = value;
			}
		}

		public ADObjectId DefaultDistributionListOU
		{
			get
			{
				return (ADObjectId)this[RecipientTemplateProvisioningPolicySchema.DefaultDistributionListOU];
			}
			set
			{
				this[RecipientTemplateProvisioningPolicySchema.DefaultDistributionListOU] = value;
			}
		}

		internal override void ProvisionCustomDefaultProperties(IConfigurable provisionedDefault)
		{
			base.ProvisionCustomDefaultProperties(provisionedDefault);
			Mailbox mailbox = provisionedDefault as Mailbox;
			if (mailbox != null && (this.DefaultProhibitSendQuota != Unlimited<ByteQuantifiedSize>.UnlimitedValue || this.DefaultProhibitSendReceiveQuota != Unlimited<ByteQuantifiedSize>.UnlimitedValue || this.DefaultIssueWarningQuota != Unlimited<ByteQuantifiedSize>.UnlimitedValue))
			{
				mailbox.UseDatabaseQuotaDefaults = new bool?(false);
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			errors.AddRange(Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
			{
				RecipientTemplateProvisioningPolicySchema.DefaultIssueWarningQuota,
				RecipientTemplateProvisioningPolicySchema.DefaultProhibitSendQuota,
				RecipientTemplateProvisioningPolicySchema.DefaultProhibitSendReceiveQuota
			}, this.Identity));
		}

		internal const string PolicyName = "Recipient Template Policy";

		private static RecipientTemplateProvisioningPolicySchema schema = ObjectSchema.GetInstance<RecipientTemplateProvisioningPolicySchema>();

		private static IProvisioningTemplate[] provisioningTemplates = new IProvisioningTemplate[]
		{
			new ProvisioningPropertyTemplate(RecipientTemplateProvisioningPolicySchema.DefaultMaxSendSize, MailEnabledRecipientSchema.MaxSendSize, null, ProvisioningHelper.AllSupportedRecipientTypes),
			new ProvisioningPropertyTemplate(RecipientTemplateProvisioningPolicySchema.DefaultMaxReceiveSize, MailEnabledRecipientSchema.MaxReceiveSize, null, ProvisioningHelper.AllSupportedRecipientTypes),
			new ProvisioningPropertyTemplate(RecipientTemplateProvisioningPolicySchema.DefaultProhibitSendQuota, MailboxSchema.ProhibitSendQuota, null, typeof(Mailbox)),
			new ProvisioningPropertyTemplate(RecipientTemplateProvisioningPolicySchema.DefaultProhibitSendReceiveQuota, MailboxSchema.ProhibitSendReceiveQuota, null, typeof(Mailbox)),
			new ProvisioningPropertyTemplate(RecipientTemplateProvisioningPolicySchema.DefaultIssueWarningQuota, MailboxSchema.IssueWarningQuota, null, typeof(Mailbox)),
			new ProvisioningPropertyTemplate(RecipientTemplateProvisioningPolicySchema.DefaultRulesQuota, MailboxSchema.RulesQuota, null, typeof(Mailbox)),
			new ProvisioningPropertyTemplate(RecipientTemplateProvisioningPolicySchema.DefaultDistributionListOU, DistributionGroupBaseSchema.DefaultDistributionListOU, null, new Type[]
			{
				typeof(DistributionGroup),
				typeof(DynamicDistributionGroup)
			})
		};

		internal new static string MostDerivedClass = "msExchRecipientTemplatePolicy";
	}
}
