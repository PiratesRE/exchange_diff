using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateContactResponse : CreateEntityResponse<Contact>
	{
		public CreateContactResponse(CreateContactRequest request) : base(request)
		{
		}
	}
}
