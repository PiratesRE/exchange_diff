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
	public sealed class TeamMailboxProvisioningPolicy : MailboxPolicy
	{
		internal static QueryFilter IsDefaultFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(MailboxPolicySchema.MailboxPolicyFlags, 1UL));
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return TeamMailboxProvisioningPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return TeamMailboxProvisioningPolicy.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return TeamMailboxProvisioningPolicy.parentPath;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override bool CheckForAssociatedUsers()
		{
			return false;
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (0 < this.IssueWarningQuota.CompareTo(this.ProhibitSendReceiveQuota))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorProperty1GtProperty2(TeamMailboxProvisioningPolicySchema.IssueWarningQuota.Name, this.IssueWarningQuota.ToString(), TeamMailboxProvisioningPolicySchema.ProhibitSendReceiveQuota.Name, this.ProhibitSendReceiveQuota.ToString()), this.Identity, string.Empty));
			}
		}

		public override bool IsDefault
		{
			get
			{
				return (bool)this[TeamMailboxProvisioningPolicySchema.IsDefault];
			}
			set
			{
				this[TeamMailboxProvisioningPolicySchema.IsDefault] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MaxReceiveSize
		{
			get
			{
				return (ByteQuantifiedSize)this[TeamMailboxProvisioningPolicySchema.MaxReceiveSize];
			}
			set
			{
				this[TeamMailboxProvisioningPolicySchema.MaxReceiveSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize IssueWarningQuota
		{
			get
			{
				return (ByteQuantifiedSize)this[TeamMailboxProvisioningPolicySchema.IssueWarningQuota];
			}
			set
			{
				this[TeamMailboxProvisioningPolicySchema.IssueWarningQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ProhibitSendReceiveQuota
		{
			get
			{
				return (ByteQuantifiedSize)this[TeamMailboxProvisioningPolicySchema.ProhibitSendReceiveQuota];
			}
			set
			{
				this[TeamMailboxProvisioningPolicySchema.ProhibitSendReceiveQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DefaultAliasPrefixEnabled
		{
			get
			{
				return (bool)this[TeamMailboxProvisioningPolicySchema.DefaultAliasPrefixEnabled];
			}
			set
			{
				this[TeamMailboxProvisioningPolicySchema.DefaultAliasPrefixEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AliasPrefix
		{
			get
			{
				return (string)this[TeamMailboxProvisioningPolicySchema.AliasPrefix];
			}
			set
			{
				this[TeamMailboxProvisioningPolicySchema.AliasPrefix] = value;
			}
		}

		private static TeamMailboxProvisioningPolicySchema schema = ObjectSchema.GetInstance<TeamMailboxProvisioningPolicySchema>();

		private static string mostDerivedClass = "msExchTeamMailboxProvisioningPolicy";

		private static ADObjectId parentPath = new ADObjectId("CN=Team Mailbox Provisioning Policies");
	}
}
