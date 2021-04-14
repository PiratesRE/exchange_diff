using System;

namespace Microsoft.Exchange.Services.OData
{
	internal class MandatoryPropertyMissingException : InvalidPropertyException
	{
		public MandatoryPropertyMissingException(string propertyName) : base(propertyName, CoreResources.ErrorMandatoryPropertyMissing(propertyName))
		{
		}
	}
}
