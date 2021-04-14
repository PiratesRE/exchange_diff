using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Migration
{
	internal class MailEnabledUserUpdateProvisioningData : ProvisioningData
	{
		internal MailEnabledUserUpdateProvisioningData()
		{
			base.Action = ProvisioningAction.UpdateExisting;
			base.ProvisioningType = ProvisioningType.MailEnabledUserUpdate;
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

		public static MailEnabledUserUpdateProvisioningData Create(string meuId)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(meuId, "meuId");
			return new MailEnabledUserUpdateProvisioningData
			{
				Identity = meuId
			};
		}

		public bool IsEmpty()
		{
			return string.IsNullOrEmpty(base.Identity) || (string.IsNullOrEmpty(this.Manager) && (this.GrantSendOnBehalfTo == null || this.GrantSendOnBehalfTo.Length <= 0));
		}
	}
}
