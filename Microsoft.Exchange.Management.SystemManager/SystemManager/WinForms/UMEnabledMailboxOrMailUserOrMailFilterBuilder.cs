using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class UMEnabledMailboxOrMailUserOrMailFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			filter = "| Filter-PropertyEqualTo -Property UMEnabled -Value true";
			parameterList = null;
			preArgs = null;
		}
	}
}
