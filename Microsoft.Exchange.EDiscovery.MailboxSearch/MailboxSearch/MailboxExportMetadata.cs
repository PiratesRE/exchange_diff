using System;
using Microsoft.Exchange.EDiscovery.Export;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class MailboxExportMetadata : IExportMetadata
	{
		public MailboxExportMetadata(string exportId, string exportName, bool includeDuplicates, bool includeUnsearchableItems, DateTime startTime) : this(exportId, exportName, includeDuplicates, includeUnsearchableItems, startTime, null)
		{
		}

		public MailboxExportMetadata(string exportId, string exportName, bool includeDuplicates, bool includeUnsearchableItems, DateTime startTime, string language)
		{
			this.ExportId = exportId;
			this.ExportName = exportName;
			this.IncludeDuplicates = includeDuplicates;
			this.IncludeUnsearchableItems = includeUnsearchableItems;
			this.ExportStartTime = startTime;
			this.IncludeSearchableItems = true;
			this.Language = language;
		}

		public ulong EstimateBytes { get; private set; }

		public int EstimateItems { get; private set; }

		public string ExportId { get; private set; }

		public string ExportName { get; private set; }

		public DateTime ExportStartTime { get; private set; }

		public bool IncludeDuplicates { get; private set; }

		public bool RemoveRms { get; private set; }

		public bool IncludeUnsearchableItems { get; private set; }

		public bool IncludeSearchableItems { get; private set; }

		public string Language { get; private set; }
	}
}
