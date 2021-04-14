using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class MoveBackToServer
	{
		public static List<AmDatabaseMoveResult> Move(AmDbMoveArguments moveArgs)
		{
			List<AmDatabaseMoveResult> result;
			using (BatchDatabaseOperation batchDatabaseOperation = new BatchDatabaseOperation())
			{
				IADConfig adconfig = Dependencies.ADConfig;
				IEnumerable<IADDatabase> databasesOnServer = adconfig.GetDatabasesOnServer(moveArgs.TargetServer);
				AmConfig config = AmSystemManager.Instance.Config;
				foreach (IADDatabase iaddatabase in databasesOnServer)
				{
					IADDatabaseCopy databaseCopy = iaddatabase.GetDatabaseCopy(moveArgs.TargetServer.NetbiosName);
					if (databaseCopy != null && iaddatabase.ReplicationType == ReplicationType.Remote && databaseCopy.ActivationPreference == 1 && !MoveBackToServer.IsAlreadyOnTarget(iaddatabase, moveArgs.TargetServer, config))
					{
						batchDatabaseOperation.AddOperation(new AmDbMoveOperation(iaddatabase, moveArgs.ActionCode)
						{
							Arguments = moveArgs
						});
					}
				}
				batchDatabaseOperation.DispatchOperations();
				batchDatabaseOperation.WaitForComplete();
				List<AmDbOperation> completedOperationList = batchDatabaseOperation.GetCompletedOperationList();
				List<AmDatabaseMoveResult> list = new List<AmDatabaseMoveResult>(completedOperationList.Count);
				foreach (AmDbOperation amDbOperation in completedOperationList)
				{
					if (amDbOperation.DetailedStatus != null)
					{
						list.Add(amDbOperation.ConvertDetailedStatusToRpcMoveResult(amDbOperation.DetailedStatus));
					}
				}
				result = list;
			}
			return result;
		}

		private static bool IsAlreadyOnTarget(IADDatabase db, AmServerName targetServer, AmConfig amConfig)
		{
			AmDbStateInfo amDbStateInfo = amConfig.DbState.Read(db.Guid);
			return amDbStateInfo != null && AmServerName.IsEqual(amDbStateInfo.ActiveServer, targetServer);
		}
	}
}
