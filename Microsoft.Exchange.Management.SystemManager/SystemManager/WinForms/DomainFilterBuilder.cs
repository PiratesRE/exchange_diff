using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class DomainFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			filter = " | Filter-PropertyEqualTo -Property 'Type' -Value 'Domain'";
			preArgs = null;
			parameterList = null;
		}
	}
}
