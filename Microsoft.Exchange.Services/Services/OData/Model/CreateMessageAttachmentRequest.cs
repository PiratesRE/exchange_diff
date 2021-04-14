using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class CreateMessageAttachmentRequest : CreateAttachmentRequest
	{
		public CreateMessageAttachmentRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		protected override string ParentItemNavigationName
		{
			get
			{
				return UserSchema.Messages.Name;
			}
		}
	}
}
