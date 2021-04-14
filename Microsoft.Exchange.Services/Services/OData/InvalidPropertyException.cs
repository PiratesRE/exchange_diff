using System;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData
{
	internal class InvalidPropertyException : ODataResponseException
	{
		public InvalidPropertyException(string propertyName) : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidProperty, CoreResources.ErrorInvalidProperty(propertyName), null)
		{
			this.PropertyName = propertyName;
		}

		public InvalidPropertyException(string propertyName, LocalizedString errorMessage) : base(HttpStatusCode.BadRequest, ResponseCodeType.ErrorInvalidProperty, errorMessage, null)
		{
			this.PropertyName = propertyName;
		}

		public string PropertyName { get; private set; }
	}
}
