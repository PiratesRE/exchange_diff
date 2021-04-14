using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class GetMailUserBase<TIdentity> : GetRecipientWithAddressListBase<TIdentity, ADUser> where TIdentity : MailUserIdParameterBase, new()
	{
		public GetMailUserBase()
		{
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MailUserSchema>();
			}
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetMailUserBase<MailUserIdParameter>.SortPropertiesArray;
			}
		}

		protected override string SystemAddressListRdn
		{
			get
			{
				return "All Mail Users(VLV)";
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return MailUser.FromDataObject((ADUser)dataObject);
		}

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			MailEnabledRecipientSchema.DisplayName,
			MailEnabledRecipientSchema.Alias
		};
	}
}
