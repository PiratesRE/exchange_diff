using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Migration
{
	internal class GroupProvisioningData : RecipientProvisioningData
	{
		internal GroupProvisioningData()
		{
			base.Action = ProvisioningAction.CreateNew;
			base.ProvisioningType = ProvisioningType.Group;
		}

		public string[] ManagedBy
		{
			get
			{
				return (string[])base[ADGroupSchema.ManagedBy];
			}
			set
			{
				base[ADGroupSchema.ManagedBy] = value;
			}
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

		public static GroupProvisioningData Create(string name)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(name, "name");
			return new GroupProvisioningData
			{
				Name = name
			};
		}
	}
}
