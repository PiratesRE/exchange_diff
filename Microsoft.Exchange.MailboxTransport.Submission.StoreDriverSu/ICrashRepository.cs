using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal interface ICrashRepository
	{
		List<Guid> GetAllResourceIDs();

		bool GetQuarantineInfoContext(Guid resourceGuid, TimeSpan quarantineExpiryWindow, out QuarantineInfoContext quarantineInfoContext);

		bool GetResourceCrashInfoData(Guid resourceGuid, TimeSpan crashExpiryWindow, out Dictionary<long, ResourceEventCounterCrashInfo> resourceCrashData, out SortedSet<DateTime> allCrashTimes);

		void PersistCrashInfo(Guid resourceGuid, long eventCounter, ResourceEventCounterCrashInfo resourceEventCounterCrashInfo, int maxCrashEntries);

		bool PersistQuarantineInfo(Guid resourceGuid, QuarantineInfoContext quarantineInfoContext, bool overrideExisting = false);

		void PurgeResourceData(Guid resourceGuid);
	}
}
