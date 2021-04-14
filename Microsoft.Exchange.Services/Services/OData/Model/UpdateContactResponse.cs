using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateContactResponse : UpdateEntityResponse<Contact>
	{
		public UpdateContactResponse(UpdateContactRequest request) : base(request)
		{
		}
	}
}
