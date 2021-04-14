using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class CreateEventAttachmentRequest : CreateAttachmentRequest
	{
		public CreateEventAttachmentRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		protected override string ParentItemNavigationName
		{
			get
			{
				return UserSchema.Events.Name;
			}
		}
	}
}
