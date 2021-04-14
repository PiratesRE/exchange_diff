using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbMountOperation : AmDbOperation
	{
		internal AmDbMountOperation(IADDatabase db, AmDbActionCode actionCode) : base(db)
		{
			this.ActionCode = actionCode;
			this.StoreMountFlags = MountFlags.None;
			this.AmMountFlags = AmMountFlags.None;
			this.MountDialOverride = DatabaseMountDialOverride.None;
		}

		internal AmDbActionCode ActionCode { get; set; }

		internal MountFlags StoreMountFlags { get; set; }

		internal AmMountFlags AmMountFlags { get; set; }

		internal DatabaseMountDialOverride MountDialOverride { get; set; }

		public override string ToString()
		{
			return string.Format("Mount database {0} with action code {1} (StoreMountFlags {2}, AmMountFlags {3}, MountDialOverride {4})", new object[]
			{
				base.Database.Name,
				this.ActionCode,
				this.StoreMountFlags,
				this.AmMountFlags,
				this.MountDialOverride
			});
		}

		protected override void RunInternal()
		{
			AmDbOperationDetailedStatus mountStatus = null;
			Exception lastException = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				AmDbAction amDbAction = this.PrepareDbAction(this.ActionCode);
				amDbAction.Mount(this.StoreMountFlags, this.AmMountFlags, this.MountDialOverride, ref mountStatus);
			});
			base.DetailedStatus = mountStatus;
			base.LastException = lastException;
		}
	}
}
