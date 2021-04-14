using System;
using Microsoft.Exchange.Collections.TimeoutCache;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class MDBPerfCounterHelperCollection
	{
		public static MDBPerfCounterHelper GetMDBHelper(Guid mdbGuid, bool createIfNotPresent)
		{
			MDBPerfCounterHelper mdbperfCounterHelper = null;
			MDBPerfCounterHelper result;
			lock (MDBPerfCounterHelperCollection.locker)
			{
				if (!MDBPerfCounterHelperCollection.data.TryGetValue(mdbGuid, out mdbperfCounterHelper) && createIfNotPresent)
				{
					DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(mdbGuid, null, null, FindServerFlags.AllowMissing);
					mdbperfCounterHelper = new MDBPerfCounterHelper(databaseInformation.DatabaseName ?? MrsStrings.MissingDatabaseName2(mdbGuid, databaseInformation.ForestFqdn));
					MDBPerfCounterHelperCollection.data.TryInsertSliding(mdbGuid, mdbperfCounterHelper, MDBPerfCounterHelperCollection.RefreshInterval);
				}
				result = mdbperfCounterHelper;
			}
			return result;
		}

		private static bool ShouldRemovePerfCounter(Guid mdbGuid, MDBPerfCounterHelper perfCounter)
		{
			bool result = false;
			CommonUtils.CatchKnownExceptions(delegate
			{
				DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(mdbGuid, null, null, FindServerFlags.AllowMissing);
				result = (databaseInformation.IsMissing || !databaseInformation.IsOnThisServer);
			}, null);
			return result;
		}

		private static void RemovePerfCounter(Guid mdbGuid, MDBPerfCounterHelper perfCounter, RemoveReason reason)
		{
			perfCounter.RemovePerfCounter();
		}

		private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(15.0);

		private static readonly ExactTimeoutCache<Guid, MDBPerfCounterHelper> data = new ExactTimeoutCache<Guid, MDBPerfCounterHelper>(new RemoveItemDelegate<Guid, MDBPerfCounterHelper>(MDBPerfCounterHelperCollection.RemovePerfCounter), new ShouldRemoveDelegate<Guid, MDBPerfCounterHelper>(MDBPerfCounterHelperCollection.ShouldRemovePerfCounter), null, 10000, false);

		private static readonly object locker = new object();
	}
}
