using System;

namespace Microsoft.Exchange.Services.OData
{
	internal class EmptyResultResponse : ODataResponse<EmptyResult>
	{
		public EmptyResultResponse(ODataRequest request) : base(request)
		{
		}
	}
}
