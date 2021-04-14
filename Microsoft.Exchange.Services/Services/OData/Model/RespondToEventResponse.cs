using System;
using System.Net;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class RespondToEventResponse : EmptyResultResponse
	{
		public RespondToEventResponse(RespondToEventRequest request) : base(request)
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
