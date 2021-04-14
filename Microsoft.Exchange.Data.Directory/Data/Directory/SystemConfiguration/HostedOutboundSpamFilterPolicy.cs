using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class HostedOutboundSpamFilterPolicy : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return HostedOutboundSpamFilterPolicy.schema;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return HostedOutboundSpamFilterPolicy.parentPath;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return HostedOutboundSpamFilterPolicy.ldapName;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.NotifyOutboundSpam && (this.NotifyOutboundSpamRecipients == null || this.NotifyOutboundSpamRecipients.Count == 0))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.NotifyOutboundSpamRecipientsRequired, HostedOutboundSpamFilterPolicySchema.NotifyOutboundSpamRecipients, this.NotifyOutboundSpamRecipients));
			}
			if (this.BccSuspiciousOutboundMail && (this.BccSuspiciousOutboundAdditionalRecipients == null || this.BccSuspiciousOutboundAdditionalRecipients.Count == 0))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.BccSuspiciousOutboundAdditionalRecipientsRequired, HostedOutboundSpamFilterPolicySchema.BccSuspiciousOutboundAdditionalRecipients, this.BccSuspiciousOutboundAdditionalRecipients));
			}
		}

		[Parameter]
		public new string AdminDisplayName
		{
			get
			{
				return (string)this[ADConfigurationObjectSchema.AdminDisplayName];
			}
			set
			{
				this[ADConfigurationObjectSchema.AdminDisplayName] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> NotifyOutboundSpamRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[HostedOutboundSpamFilterPolicySchema.NotifyOutboundSpamRecipients];
			}
			set
			{
				this[HostedOutboundSpamFilterPolicySchema.NotifyOutboundSpamRecipients] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmtpAddress> BccSuspiciousOutboundAdditionalRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[HostedOutboundSpamFilterPolicySchema.BccSuspiciousOutboundAdditionalRecipients];
			}
			set
			{
				this[HostedOutboundSpamFilterPolicySchema.BccSuspiciousOutboundAdditionalRecipients] = value;
			}
		}

		[Parameter]
		public bool BccSuspiciousOutboundMail
		{
			get
			{
				return (bool)this[HostedOutboundSpamFilterPolicySchema.BccSuspiciousOutboundMail];
			}
			set
			{
				this[HostedOutboundSpamFilterPolicySchema.BccSuspiciousOutboundMail] = value;
			}
		}

		[Parameter]
		public bool NotifyOutboundSpam
		{
			get
			{
				return (bool)this[HostedOutboundSpamFilterPolicySchema.NotifyOutboundSpam];
			}
			set
			{
				this[HostedOutboundSpamFilterPolicySchema.NotifyOutboundSpam] = value;
			}
		}

		private static readonly string ldapName = "msExchHygieneConfiguration";

		private static readonly ADObjectId parentPath = new ADObjectId("CN=Outbound Spam Filter,CN=Transport Settings");

		private static readonly HostedOutboundSpamFilterPolicySchema schema = ObjectSchema.GetInstance<HostedOutboundSpamFilterPolicySchema>();
	}
}
