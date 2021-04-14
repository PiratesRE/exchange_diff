using System;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal class InvalidIdException : ODataResponseException
	{
		public InvalidIdException() : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidId, CoreResources.ErrorInvalidId, null)
		{
		}

		public InvalidIdException(LocalizedString errorMessage) : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidId, errorMessage, null)
		{
		}

		public InvalidIdException(ResponseCodeType errorCode, LocalizedString errorMessage) : base(HttpStatusCode.BadRequest, errorCode.ToString(), errorMessage, null)
		{
		}
	}
}
