using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal static class Util
	{
		internal static bool IncludeUnsearchableItems(IExportContext exportContext)
		{
			if (exportContext == null || exportContext.ExportMetadata == null)
			{
				throw new ArgumentException("exportContext.ExportMetadata should not be null.");
			}
			if (exportContext.ExportMetadata.IncludeUnsearchableItems)
			{
				if (exportContext.Sources == null || exportContext.Sources.Count == 0)
				{
					return exportContext.ExportMetadata.IncludeUnsearchableItems;
				}
				string pattern = string.Format("(^$|^{0}$|^received[><]=\\\".+\\\"$|^\\(received>=\\\".+\\\" AND received<=\\\".+\\\"\\)$)", Util.EmptyQueryReplacement);
				Regex regex = new Regex(pattern);
				string sourceFilter = exportContext.Sources[0].SourceFilter;
				if (regex.IsMatch(sourceFilter))
				{
					return false;
				}
			}
			return exportContext.ExportMetadata.IncludeUnsearchableItems;
		}

		internal static readonly string EmptyQueryReplacement = "size>=0";
	}
}
