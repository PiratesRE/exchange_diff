using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface IDDIValidationObjectConverter
	{
		object ConvertTo(object obj);
	}
}
