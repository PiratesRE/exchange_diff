using System;
using System.Net;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal class InvalidValueForCountSystemQueryOptionException : ODataResponseException
	{
		public InvalidValueForCountSystemQueryOptionException() : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidValueForCountSystemQueryOption, CoreResources.ErrorInvalidValueForCountSystemQueryOption, null)
		{
		}
	}
}
