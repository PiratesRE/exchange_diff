using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IDataExport
	{
		DataExportBatch ExportData();

		void CancelExport();
	}
}
