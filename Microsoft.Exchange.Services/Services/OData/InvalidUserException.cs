using System;
using System.Net;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal class InvalidUserException : ODataResponseException
	{
		public InvalidUserException(string userAddress) : base(HttpStatusCode.NotFound, ResponseCodeType.ErrorInvalidUser, CoreResources.ErrorInvalidRequestedUser(userAddress), null)
		{
		}
	}
}
