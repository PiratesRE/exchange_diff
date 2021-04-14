using System;
using System.Net;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateMessageResponseDraftResponse : ODataResponse<Message>
	{
		public CreateMessageResponseDraftResponse(CreateMessageResponseDraftRequest request) : base(request)
		{
		}

		protected override HttpStatusCode HttpResponseCodeOnSuccess
		{
			get
			{
				return HttpStatusCode.Created;
			}
		}
	}
}
