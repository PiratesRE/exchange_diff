using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess
{
	internal interface IUMCallDataRecordStorage : IDisposeTrackable, IDisposable
	{
		void CreateUMCallDataRecord(CDRData cdrData);

		CDRData[] GetUMCallDataRecordsForUser(string userLegacyExchangeDN);

		CDRData[] GetUMCallDataRecords(ExDateTime startDateTime, ExDateTime endDateTime, int offset, int numberOfRecordsToRead);

		UMReportRawCounters[] GetUMCallSummary(Guid dialPlanGuid, Guid gatewayGuid, GroupBy groupby);
	}
}
