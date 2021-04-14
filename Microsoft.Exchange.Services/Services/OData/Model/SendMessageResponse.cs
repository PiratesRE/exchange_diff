using System;
using System.Net;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class SendMessageResponse : EmptyResultResponse
	{
		public SendMessageResponse(SendMessageRequest request) : base(request)
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
