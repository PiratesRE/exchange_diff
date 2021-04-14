using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetContactResponse : GetEntityResponse<Contact>
	{
		public GetContactResponse(GetContactRequest request) : base(request)
		{
		}
	}
}
