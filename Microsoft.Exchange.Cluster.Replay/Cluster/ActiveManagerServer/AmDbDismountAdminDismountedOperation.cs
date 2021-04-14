using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbDismountAdminDismountedOperation : AmDbOperation
	{
		internal AmDbDismountAdminDismountedOperation(IADDatabase db, AmDbActionCode actionCode) : base(db)
		{
			this.ActionCode = actionCode;
		}

		internal AmDbActionCode ActionCode { get; set; }

		public override string ToString()
		{
			return string.Format("Dismount admin dismounted database {0} (actionCode={1})", base.Database.Name, this.ActionCode);
		}

		protected override void RunInternal()
		{
			Exception lastException = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				this.CheckIfOperationIsAllowedOnCurrentRole();
				AmDbStateInfo amDbStateInfo = AmSystemManager.Instance.Config.DbState.Read(base.Database.Guid);
				if (!amDbStateInfo.IsAdminDismounted)
				{
					AmTrace.Debug("AmDbDismountAdminDismountedOperation skipped since database is not marked as admin dismounted anymore", new object[0]);
					return;
				}
				MountStatus storeDatabaseMountStatus = AmStoreHelper.GetStoreDatabaseMountStatus(null, base.Database.Guid);
				if (storeDatabaseMountStatus == MountStatus.Mounted)
				{
					AmDbAction amDbAction = base.PrepareDbAction(this.ActionCode);
					ReplayCrimsonEvents.DismountingAdminDismountRequestedDatabase.Log<string, Guid, AmServerName>(base.Database.Name, base.Database.Guid, amDbStateInfo.ActiveServer);
					amDbAction.Dismount(UnmountFlags.None);
					return;
				}
				AmTrace.Debug("AmDbDismountAdminDismountedOperation skipped since database is not mounted (db={0}, mountStatus={1})", new object[]
				{
					base.Database.Name,
					storeDatabaseMountStatus
				});
			});
			base.LastException = lastException;
		}
	}
}
