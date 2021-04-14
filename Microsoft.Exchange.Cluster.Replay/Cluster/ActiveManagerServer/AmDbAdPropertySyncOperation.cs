using System;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbAdPropertySyncOperation : AmDbOperation
	{
		internal AmDbAdPropertySyncOperation(IADDatabase db, AmDbActionCode actionCode) : base(db)
		{
			this.ActionCode = actionCode;
		}

		internal AmDbActionCode ActionCode { get; set; }

		public override string ToString()
		{
			return string.Format("Synchronize AD property for database {0} (actionCode={1})", base.Database.Name, this.ActionCode);
		}

		protected override void RunInternal()
		{
			Exception lastException = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				this.CheckIfOperationIsAllowedOnCurrentRole();
				AmDbAction.SyncDatabaseOwningServerAndLegacyDn(base.Database, this.ActionCode);
			});
			base.LastException = lastException;
		}
	}
}
