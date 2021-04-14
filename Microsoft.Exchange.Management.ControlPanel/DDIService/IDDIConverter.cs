using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface IDDIConverter
	{
		bool CanConvert(object sourceValue);

		object Convert(object sourceValue);
	}
}
