using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface IDDIValidator
	{
		List<string> Validate(object target, Service profile);
	}
}
