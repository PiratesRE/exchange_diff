using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IExchangeCommandFilterBuilder
	{
		void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row);
	}
}
