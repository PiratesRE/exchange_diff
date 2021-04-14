using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmPeriodicSplitbrainChecker : AmStartupAutoMounter
	{
		internal AmPeriodicSplitbrainChecker()
		{
		}

		protected override void LogStartupInternal()
		{
			AmTrace.Debug("Starting {0}", new object[]
			{
				base.GetType().Name
			});
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3446025533U);
		}

		protected override void LogCompletionInternal()
		{
			AmTrace.Debug("Finished {0}", new object[]
			{
				base.GetType().Name
			});
		}

		protected override void PopulateWithDatabaseOperations(Dictionary<Guid, DatabaseInfo> dbMap)
		{
			new Dictionary<Guid, DatabaseInfo>();
			foreach (DatabaseInfo databaseInfo in dbMap.Values)
			{
				new List<AmDbOperation>();
				databaseInfo.Analyze();
				IADDatabase database = databaseInfo.Database;
				if (databaseInfo.IsMismounted)
				{
					AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, AmDbActionReason.PeriodicAction, AmDbActionCategory.ForceDismount);
					AmDbDismountMismountedOperation item = new AmDbDismountMismountedOperation(database, actionCode, databaseInfo.MisMountedServerList);
					databaseInfo.OperationsQueued = new List<AmDbOperation>
					{
						item
					};
				}
			}
		}

		protected override List<AmServerName> GetServers()
		{
			return new List<AmServerName>
			{
				AmServerName.LocalComputerName
			};
		}
	}
}
