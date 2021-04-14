using System;
using System.Collections.Generic;
using Microsoft.Exchange.EDiscovery.Export;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class MailboxExportContext : IExportContext
	{
		public MailboxExportContext(IExportMetadata exportMetadata, IList<ISource> sourceMailboxes, ITargetLocation targetLocation, bool isResume, Action<IEnumerable<ErrorRecord>> onWriteErrorRecord, Action<IEnumerable<ExportRecord>> onWriteResultManifest)
		{
			this.ExportMetadata = exportMetadata;
			this.Sources = sourceMailboxes;
			this.TargetLocation = targetLocation;
			this.IsResume = isResume;
			this.OnWriteErrorRecord = onWriteErrorRecord;
			this.OnWriteResultManifest = onWriteResultManifest;
		}

		public Action<IEnumerable<ErrorRecord>> OnWriteErrorRecord { get; internal set; }

		public Action<IEnumerable<ExportRecord>> OnWriteResultManifest { get; internal set; }

		public bool IsResume { get; private set; }

		public IExportMetadata ExportMetadata { get; private set; }

		public IList<ISource> Sources { get; private set; }

		public ITargetLocation TargetLocation { get; private set; }

		public void WriteErrorLog(IEnumerable<ErrorRecord> errorRecords)
		{
			if (this.OnWriteErrorRecord != null)
			{
				this.OnWriteErrorRecord(errorRecords);
			}
		}

		public void WriteResultManifest(IEnumerable<ExportRecord> records)
		{
			if (this.OnWriteResultManifest != null)
			{
				this.OnWriteResultManifest(records);
			}
		}
	}
}
