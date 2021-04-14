using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public interface IExportContext
	{
		bool IsResume { get; }

		IExportMetadata ExportMetadata { get; }

		IList<ISource> Sources { get; }

		ITargetLocation TargetLocation { get; }

		void WriteResultManifest(IEnumerable<ExportRecord> records);

		void WriteErrorLog(IEnumerable<ErrorRecord> errorRecords);
	}
}
