using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPropertyBag
	{
		AnnotatedPropertyValue GetAnnotatedProperty(PropertyTag propertyTag);

		IEnumerable<AnnotatedPropertyValue> GetAnnotatedProperties();

		void SetProperty(PropertyValue propertyValue);

		void Delete(PropertyTag property);

		Stream GetPropertyStream(PropertyTag property);

		Stream SetPropertyStream(PropertyTag property, long dataSizeEstimate);

		ISession Session { get; }
	}
}
