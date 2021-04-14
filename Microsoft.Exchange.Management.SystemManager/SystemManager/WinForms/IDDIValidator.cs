using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IDDIValidator
	{
		List<string> Validate(object target, PageConfigurableProfile profile);
	}
}
