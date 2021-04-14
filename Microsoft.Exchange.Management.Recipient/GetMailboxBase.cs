using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class GetMailboxBase<TIdentity> : GetRecipientWithAddressListBase<TIdentity, ADUser> where TIdentity : RecipientIdParameter, new()
	{
		public GetMailboxBase()
		{
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetMailboxBase<TIdentity>.SortPropertiesArray;
			}
		}

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			MailEnabledRecipientSchema.Alias,
			MailEnabledRecipientSchema.DisplayName,
			ADObjectSchema.Name,
			MailboxSchema.Office,
			MailboxSchema.ServerLegacyDN
		};
	}
}
