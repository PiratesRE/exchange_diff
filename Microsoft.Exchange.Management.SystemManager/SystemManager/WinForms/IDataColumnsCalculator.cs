using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IDataColumnsCalculator
	{
		void Calculate(ResultsLoaderProfile profile, DataTable dataTable, DataRow dataRow);
	}
}
