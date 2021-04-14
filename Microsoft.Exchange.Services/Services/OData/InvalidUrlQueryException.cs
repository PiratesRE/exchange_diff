using System;
using System.Net;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal class InvalidUrlQueryException : ODataResponseException
	{
		public InvalidUrlQueryException(string parameter) : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidUrlQuery, CoreResources.ErrorInvalidUrlQuery(parameter), null)
		{
		}

		public InvalidUrlQueryException(string parameter, Exception internalException) : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidUrlQuery, CoreResources.ErrorInvalidUrlQuery(parameter), internalException)
		{
		}
	}
}
