using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IMultivaluedProperty<T> : IProperty, IEnumerable<T>, IEnumerable
	{
		int Count { get; }
	}
}
