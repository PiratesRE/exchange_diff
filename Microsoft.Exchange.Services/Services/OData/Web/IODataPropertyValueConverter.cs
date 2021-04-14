using System;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal interface IODataPropertyValueConverter
	{
		object FromODataPropertyValue(object odataPropertyValue);

		object ToODataPropertyValue(object rawValue);
	}
}
