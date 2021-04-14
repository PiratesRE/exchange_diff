using System;
using System.Net;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class RespondToMessageResponse : EmptyResultResponse
	{
		public RespondToMessageResponse(RespondToMessageRequest request) : base(request)
		{
		}

		protected override HttpStatusCode HttpResponseCodeOnSuccess
		{
			get
			{
				return HttpStatusCode.Accepted;
			}
		}
	}
}
