using System;

namespace Microsoft.Exchange.Services.OData
{
	internal class PropertyNotSupportUpdateException : InvalidPropertyException
	{
		public PropertyNotSupportUpdateException(string propertyName) : base(propertyName, CoreResources.ErrorPropertyNotSupportUpdate(propertyName))
		{
		}
	}
}
