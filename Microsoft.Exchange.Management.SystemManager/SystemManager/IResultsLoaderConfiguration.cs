using System;

namespace Microsoft.Exchange.Management.SystemManager
{
	public interface IResultsLoaderConfiguration
	{
		ResultsLoaderProfile BuildResultsLoaderProfile();
	}
}
