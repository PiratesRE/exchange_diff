using System;
using System.Net;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData
{
	internal class InvalidContentTypeException : ODataResponseException
	{
		public InvalidContentTypeException(ODataContentTypeException innerException) : base(HttpStatusCode.NotAcceptable, ResponseCodeType.ErrorNotAcceptable, CoreResources.ErrorNotAcceptable, innerException)
		{
		}
	}
}
