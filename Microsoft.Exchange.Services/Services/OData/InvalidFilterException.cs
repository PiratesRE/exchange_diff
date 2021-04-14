using System;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal class InvalidFilterException : ODataResponseException
	{
		public InvalidFilterException(LocalizedString errorMessage) : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidUrlQueryFilter, errorMessage, null)
		{
		}
	}
}
