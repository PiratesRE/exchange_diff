using System;
using System.Net;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal class RequestUnsupportedException : ODataResponseException
	{
		public RequestUnsupportedException() : base(HttpStatusCode.MethodNotAllowed, ResponseCodeType.ErrorInvalidRequest, CoreResources.ErrorUnsupportedODataRequest, null)
		{
		}
	}
}
