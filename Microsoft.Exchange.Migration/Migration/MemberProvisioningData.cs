using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Migration
{
	internal class MemberProvisioningData : ProvisioningData
	{
		internal MemberProvisioningData()
		{
			base.Action = ProvisioningAction.UpdateExisting;
			base.ProvisioningType = ProvisioningType.GroupMember;
		}

		public string[] Members
		{
			get
			{
				return (string[])base[ADGroupSchema.Members];
			}
			set
			{
				base[ADGroupSchema.Members] = value;
			}
		}

		public string[] GrantSendOnBehalfTo
		{
			get
			{
				return (string[])base[ADRecipientSchema.GrantSendOnBehalfTo];
			}
			set
			{
				base[ADRecipientSchema.GrantSendOnBehalfTo] = value;
			}
		}

		public string ManagedBy
		{
			get
			{
				return (string)base[ADGroupSchema.ManagedBy];
			}
			set
			{
				base[ADGroupSchema.ManagedBy] = value;
			}
		}

		public static MemberProvisioningData Create(string groupId)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(groupId, "groupName");
			return new MemberProvisioningData
			{
				Identity = groupId
			};
		}

		public bool IsEmpty()
		{
			return string.IsNullOrEmpty(base.Identity) || ((this.Members == null || this.Members.Length <= 0) && (this.GrantSendOnBehalfTo == null || this.GrantSendOnBehalfTo.Length <= 0) && (this.ManagedBy == null || this.ManagedBy.Length <= 0));
		}
	}
}
