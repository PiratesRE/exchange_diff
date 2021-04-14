using System;

namespace Microsoft.Exchange.Management.SystemManager
{
	public interface IFilterableConverter
	{
		IConvertible ToFilterable(object item);
	}
}
