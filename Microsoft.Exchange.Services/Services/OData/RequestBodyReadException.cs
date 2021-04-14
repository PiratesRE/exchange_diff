using System;
using System.Net;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData
{
	internal class RequestBodyReadException : ODataResponseException
	{
		public RequestBodyReadException(ODataException oDataException) : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidRequest, CoreResources.ErrorCannotReadRequestBody, oDataException)
		{
		}

		public RequestBodyReadException(HttpRequestTransportException readException) : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidRequest, CoreResources.ErrorCannotReadRequestBody, readException)
		{
		}
	}
}
