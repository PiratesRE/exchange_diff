using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public interface IExportMetadata
	{
		string ExportName { get; }

		string ExportId { get; }

		DateTime ExportStartTime { get; }

		bool RemoveRms { get; }

		bool IncludeDuplicates { get; }

		bool IncludeUnsearchableItems { get; }

		bool IncludeSearchableItems { get; }

		int EstimateItems { get; }

		ulong EstimateBytes { get; }

		string Language { get; }
	}
}
