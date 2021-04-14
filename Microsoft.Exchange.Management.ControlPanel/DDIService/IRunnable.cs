using System;
using System.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface IRunnable
	{
		bool IsRunnable(DataRow input, DataTable dataTable);
	}
}
