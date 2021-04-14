using System;

namespace Microsoft.Exchange.Entities.TypeConversion.Converters
{
	public interface IConverter<in TSource, out TDestination>
	{
		TDestination Convert(TSource value);
	}
}
