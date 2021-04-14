using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface IDDIHasArgumentValidator
	{
		List<string> ValidateWithArg(object target, Service profile, Dictionary<string, string> arguments);
	}
}
