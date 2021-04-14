using System;

namespace Microsoft.Exchange.Services.OData
{
	internal class PropertyNotSupportFilterException : InvalidPropertyException
	{
		public PropertyNotSupportFilterException(string propertyName) : base(propertyName, CoreResources.ErrorPropertyNotSupportFilter(propertyName))
		{
		}
	}
}
