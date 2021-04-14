using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol.EDiscovery;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskPlugins.EDiscovery
{
	internal class SearchApplicationPlugin : IApplicationPlugin
	{
		public WorkPayload DoWork(ComplianceMessage target, WorkPayload payload)
		{
			SearchWorkDefinition searchWorkDefinition;
			FaultDefinition faultDefinition;
			ISearchResultsProvider searchResultsProvider;
			if (ComplianceSerializer.TryDeserialize<SearchWorkDefinition>(SearchWorkDefinition.Description, payload.WorkDefinition, out searchWorkDefinition, out faultDefinition, "DoWork", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\ApplicationPlugins\\EDiscovery\\SearchApplicationPlugin.cs", 40) && Registry.Instance.TryGetInstance<ISearchResultsProvider>(RegistryComponent.EDiscovery, EDiscoveryComponent.SearchResultProvider, out searchResultsProvider, out faultDefinition, "DoWork", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\ApplicationPlugins\\EDiscovery\\SearchApplicationPlugin.cs", 42))
			{
				SearchResult searchResult = searchResultsProvider.PerformSearch(target, searchWorkDefinition);
				searchResult.PageSize = searchWorkDefinition.DetailCount;
				return new WorkPayload
				{
					PayloadId = payload.PayloadId,
					WorkDefinitionType = WorkDefinitionType.EDiscovery,
					WorkDefinition = ComplianceSerializer.Serialize<SearchResult>(SearchResult.Description, searchResult)
				};
			}
			if (faultDefinition != null)
			{
				ExceptionHandler.FaultMessage(target, faultDefinition, true);
			}
			return null;
		}

		public ResultBase RecordResult(ResultBase existing, WorkPayload addition)
		{
			SearchResult searchResult = new SearchResult();
			SearchResult searchResult2 = existing as SearchResult;
			if (addition != null)
			{
				FaultDefinition faultDefinition;
				if (addition.WorkDefinitionType == WorkDefinitionType.Fault)
				{
					FaultDefinition faultDefinition2;
					if (!ComplianceSerializer.TryDeserialize<FaultDefinition>(FaultDefinition.Description, addition.WorkDefinition, out faultDefinition, out faultDefinition2, "RecordResult", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\ApplicationPlugins\\EDiscovery\\SearchApplicationPlugin.cs", 82))
					{
						faultDefinition = faultDefinition2;
					}
				}
				else if (ComplianceSerializer.TryDeserialize<SearchResult>(SearchResult.Description, addition.WorkDefinition, out searchResult, out faultDefinition, "RecordResult", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\ApplicationPlugins\\EDiscovery\\SearchApplicationPlugin.cs", 87))
				{
					if (searchResult2 == null)
					{
						searchResult2 = searchResult;
					}
					else
					{
						searchResult2.UpdateTotalSize(searchResult.TotalSize);
						searchResult2.UpdateTotalCount(searchResult.TotalCount);
						searchResult2.MergeFaults(searchResult);
					}
				}
				if (faultDefinition != null)
				{
					if (searchResult2 == null)
					{
						searchResult2 = new SearchResult();
					}
					searchResult2.MergeFaults(faultDefinition);
				}
			}
			return searchResult2;
		}

		public WorkPayload Preprocess(ComplianceMessage target, WorkPayload payload)
		{
			SearchWorkDefinition searchWorkDefinition;
			FaultDefinition faultDefinition;
			ISearchResultsProvider searchResultsProvider;
			if (ComplianceSerializer.TryDeserialize<SearchWorkDefinition>(SearchWorkDefinition.Description, payload.WorkDefinition, out searchWorkDefinition, out faultDefinition, "Preprocess", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\ApplicationPlugins\\EDiscovery\\SearchApplicationPlugin.cs", 128) && Registry.Instance.TryGetInstance<ISearchResultsProvider>(RegistryComponent.EDiscovery, EDiscoveryComponent.SearchResultProvider, out searchResultsProvider, out faultDefinition, "Preprocess", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\ApplicationPlugins\\EDiscovery\\SearchApplicationPlugin.cs", 130))
			{
				searchWorkDefinition = searchResultsProvider.ParseSearch(target, searchWorkDefinition);
				return new WorkPayload
				{
					PayloadId = payload.PayloadId,
					WorkDefinitionType = WorkDefinitionType.EDiscovery,
					WorkDefinition = ComplianceSerializer.Serialize<SearchWorkDefinition>(SearchWorkDefinition.Description, searchWorkDefinition)
				};
			}
			if (faultDefinition != null)
			{
				ExceptionHandler.FaultMessage(target, faultDefinition, true);
			}
			return null;
		}
	}
}
