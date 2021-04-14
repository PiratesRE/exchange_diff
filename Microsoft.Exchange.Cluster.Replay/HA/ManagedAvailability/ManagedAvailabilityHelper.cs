using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.HA.ManagedAvailability
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ManagedAvailabilityHelper
	{
		internal static List<AmDatabaseMoveResult> PerformSystemFailover(string componentName, string comment)
		{
			return ManagedAvailabilityHelper.PerformSystemFailover(componentName, comment, AmServerName.LocalComputerName.Fqdn);
		}

		internal static List<AmDatabaseMoveResult> PerformSystemFailover(string componentName, string comment, string fromServerFqdn)
		{
			AmDbActionCode amDbActionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, AmDbActionReason.ManagedAvailability, AmDbActionCategory.Move);
			string text;
			return AmRpcClientHelper.ServerMoveAllDatabases(fromServerFqdn, null, 0, 16, -1, true, 0, amDbActionCode.IntValue, comment, componentName, out text);
		}

		internal static AmDatabaseMoveResult PerformDatabaseFailover(string componentName, string comment, Database database)
		{
			AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, AmDbActionReason.FailureItem, AmDbActionCategory.Move);
			AmDatabaseMoveResult result = null;
			string text;
			AmRpcClientHelper.MoveDatabaseEx(ADObjectWrapperFactory.CreateWrapper(database), 0, 16, -1, AmServerName.LocalComputerName.Fqdn, null, true, 0, actionCode, comment, out text, ref result);
			return result;
		}
	}
}
