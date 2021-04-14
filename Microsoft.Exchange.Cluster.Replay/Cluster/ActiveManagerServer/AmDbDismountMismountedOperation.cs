using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbDismountMismountedOperation : AmDbOperation
	{
		internal AmDbDismountMismountedOperation(IADDatabase db, AmDbActionCode actionCode, List<AmServerName> mismountedNodes) : base(db)
		{
			this.ActionCode = actionCode;
			this.MismountedNodes = mismountedNodes;
		}

		internal AmDbActionCode ActionCode { get; set; }

		internal List<AmServerName> MismountedNodes { get; set; }

		public override string ToString()
		{
			return string.Format("Force dismount database {0} (actionCode={1})", base.Database.Name, this.ActionCode);
		}

		protected override void RunInternal()
		{
			Exception lastException = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				AmDbAction.DismountIfMismounted(base.Database, this.ActionCode, this.MismountedNodes);
			});
			base.LastException = lastException;
		}
	}
}
