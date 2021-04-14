using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbRemountOperation : AmDbOperation
	{
		internal AmDbRemountOperation(IADDatabase db, AmDbActionCode actionCode) : base(db)
		{
			this.ActionCode = actionCode;
			this.Flags = MountFlags.None;
			this.MountDialOverride = DatabaseMountDialOverride.None;
			this.FromServer = null;
		}

		internal AmDbActionCode ActionCode { get; set; }

		internal MountFlags Flags { get; set; }

		internal DatabaseMountDialOverride MountDialOverride { get; set; }

		internal AmServerName FromServer { get; set; }

		public override string ToString()
		{
			return string.Format("Remount database {0} on server {1} (Mount flags {2}, MountDialOverride {3}, ActionCode {4})", new object[]
			{
				base.Database.Name,
				this.FromServer,
				this.Flags,
				this.MountDialOverride,
				this.ActionCode
			});
		}

		protected override void RunInternal()
		{
			Exception lastException = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				AmDbAction amDbAction = base.PrepareDbAction(this.ActionCode);
				amDbAction.Remount(this.Flags, this.MountDialOverride, this.FromServer);
			});
			base.LastException = lastException;
		}
	}
}
