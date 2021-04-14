using System;

namespace Microsoft.Exchange.Entities.TypeConversion.Converters
{
	public delegate TDestination ConvertValue<in TSource, out TDestination>(TSource value);
}
