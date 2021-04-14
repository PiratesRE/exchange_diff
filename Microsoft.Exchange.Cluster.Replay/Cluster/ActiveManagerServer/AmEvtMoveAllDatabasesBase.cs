using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtMoveAllDatabasesBase : AmEvtServerSwitchoverBase
	{
		internal AmEvtMoveAllDatabasesBase(AmServerName nodeName) : base(nodeName)
		{
		}

		internal AmDbMoveArguments MoveArgs { get; set; }

		internal List<AmDatabaseMoveResult> GetMoveResultsForOperationsRun()
		{
			List<AmDatabaseMoveResult> moveResults = new List<AmDatabaseMoveResult>(base.OperationList.Count);
			base.OperationList.ForEach(delegate(AmDbOperation op)
			{
				if (op.DetailedStatus != null)
				{
					moveResults.Add(op.ConvertDetailedStatusToRpcMoveResult(op.DetailedStatus));
				}
			});
			return moveResults;
		}
	}
}
