using System;
using System.Net;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData
{
	internal class UrlResolutionException : ODataResponseException
	{
		public UrlResolutionException(ODataException odataException) : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidRequest, CoreResources.ErrorCannotResolveODataUrl, odataException)
		{
		}
	}
}
