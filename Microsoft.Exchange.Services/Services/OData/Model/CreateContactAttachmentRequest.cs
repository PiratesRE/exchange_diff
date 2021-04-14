using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Contacts.Write")]
	internal class CreateContactAttachmentRequest : CreateAttachmentRequest
	{
		public CreateContactAttachmentRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		protected override string ParentItemNavigationName
		{
			get
			{
				return UserSchema.Contacts.Name;
			}
		}
	}
}
