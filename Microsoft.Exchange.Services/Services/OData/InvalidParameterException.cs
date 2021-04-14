using System;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal class InvalidParameterException : ODataResponseException
	{
		public InvalidParameterException(string parameterName, LocalizedString errorMessage) : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidParameter, errorMessage, null)
		{
			this.ParameterName = parameterName;
		}

		public InvalidParameterException(string parameterName) : this(parameterName, CoreResources.ErrorInvalidParameter(parameterName))
		{
		}

		public string ParameterName { get; private set; }
	}
}
