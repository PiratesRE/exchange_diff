using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteContactResponse : DeleteEntityResponse<Contact>
	{
		public DeleteContactResponse(DeleteContactRequest request) : base(request)
		{
		}
	}
}
