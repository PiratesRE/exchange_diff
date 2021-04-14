using System;

namespace Microsoft.Exchange.Services.OData
{
	internal class PropertyNotSupportCreateException : InvalidPropertyException
	{
		public PropertyNotSupportCreateException(string propertyName) : base(propertyName, CoreResources.ErrorPropertyNotSupportCreate(propertyName))
		{
		}
	}
}
