using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IReportHelper
	{
		void Load(ReportData report, MapiStore storeToUse);

		void Flush(ReportData report, MapiStore storeToUse);

		void Delete(ReportData report, MapiStore storeToUse);
	}
}
