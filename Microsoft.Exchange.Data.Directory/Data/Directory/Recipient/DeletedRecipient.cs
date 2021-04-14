using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class DeletedRecipient : DeletedObject
	{
		public DeletedRecipient()
		{
		}

		internal DeletedRecipient(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new AndFilter(new QueryFilter[]
				{
					base.ImplicitFilter,
					new ExistsFilter(ADRecipientSchema.LegacyExchangeDN)
				});
			}
		}
	}
}
