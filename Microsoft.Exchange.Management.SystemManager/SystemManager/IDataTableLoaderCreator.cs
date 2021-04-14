using System;

namespace Microsoft.Exchange.Management.SystemManager
{
	public interface IDataTableLoaderCreator
	{
		DataTableLoader CreateDataTableLoader(string name);
	}
}
