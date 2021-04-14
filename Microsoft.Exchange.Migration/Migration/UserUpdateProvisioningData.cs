using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Migration
{
	internal class UserUpdateProvisioningData : ProvisioningData
	{
		internal UserUpdateProvisioningData()
		{
			base.Action = ProvisioningAction.UpdateExisting;
			base.ProvisioningType = ProvisioningType.UserUpdate;
		}

		public string Manager
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Manager];
			}
			set
			{
				base[ADOrgPersonSchema.Manager] = value;
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

		public static UserUpdateProvisioningData Create(string userId)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(userId, "userId");
			return new UserUpdateProvisioningData
			{
				Identity = userId
			};
		}

		public bool IsEmpty()
		{
			return string.IsNullOrEmpty(base.Identity) || (string.IsNullOrEmpty(this.Manager) && (this.GrantSendOnBehalfTo == null || this.GrantSendOnBehalfTo.Length <= 0));
		}
	}
}
