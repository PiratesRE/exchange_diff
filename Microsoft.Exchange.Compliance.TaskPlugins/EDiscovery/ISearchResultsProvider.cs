using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol.EDiscovery;

namespace Microsoft.Exchange.Compliance.TaskPlugins.EDiscovery
{
	internal interface ISearchResultsProvider
	{
		SearchResult PerformSearch(ComplianceMessage target, SearchWorkDefinition definition);

		SearchWorkDefinition ParseSearch(ComplianceMessage target, SearchWorkDefinition definition);
	}
}
