using System;
using System.Collections;
using System.Linq;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class PrimitiveArrayValueConverter<T> : IODataPropertyValueConverter
	{
		public object FromODataPropertyValue(object odataPropertyValue)
		{
			ODataCollectionValue odataCollectionValue = odataPropertyValue as ODataCollectionValue;
			return (odataCollectionValue != null) ? odataCollectionValue.Items.Cast<T>().ToArray<T>() : null;
		}

		public object ToODataPropertyValue(object rawValue)
		{
			T[] array = (T[])rawValue;
			return new ODataCollectionValue
			{
				TypeName = typeof(T).MakeODataCollectionTypeName(),
				Items = (IEnumerable)rawValue
			};
		}
	}
}
