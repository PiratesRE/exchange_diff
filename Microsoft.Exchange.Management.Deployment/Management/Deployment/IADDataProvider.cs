using System;
using System.Collections.Generic;
using System.DirectoryServices;

namespace Microsoft.Exchange.Management.Deployment
{
	internal interface IADDataProvider
	{
		int SizeLimit { get; set; }

		int PageSize { get; set; }

		TimeSpan ServerTimeLimit { get; }

		SearchResultCollection Run(bool useGC, string directoryEntry, string[] listOfPropertiesToCollect, string filter, SearchScope searchScope);

		List<string> Run(bool useGC, string directoryEntry);
	}
}
