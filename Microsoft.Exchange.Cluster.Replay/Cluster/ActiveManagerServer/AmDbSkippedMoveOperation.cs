using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbSkippedMoveOperation : AmDbOperation
	{
		internal AmDbSkippedMoveOperation(IADDatabase db, LocalizedString reason) : base(db)
		{
			this.MoveSkipReason = reason;
		}

		internal LocalizedString MoveSkipReason { get; private set; }

		public override string ToString()
		{
			return string.Format("Skipping move of database '{0}'. Reason: {1}", base.Database.Name, this.MoveSkipReason);
		}

		protected override void RunInternal()
		{
			AmDbOperationDetailedStatus detailedStatus = new AmDbOperationDetailedStatus(base.Database);
			base.DetailedStatus = detailedStatus;
			base.LastException = new AmDbMoveOperationSkippedException(base.Database.Name, this.MoveSkipReason);
		}
	}
}
