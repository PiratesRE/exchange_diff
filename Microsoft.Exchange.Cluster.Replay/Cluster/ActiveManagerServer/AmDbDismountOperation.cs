using System;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbDismountOperation : AmDbOperation
	{
		internal AmDbDismountOperation(IADDatabase db, AmDbActionCode actionCode) : base(db)
		{
			this.ActionCode = actionCode;
			this.Flags = UnmountFlags.SkipCacheFlush;
		}

		internal AmDbActionCode ActionCode { get; set; }

		internal UnmountFlags Flags { get; set; }

		public override string ToString()
		{
			return string.Format("Dismount database {0} with action code {1} (Dismount flags {2})", base.Database.Name, this.ActionCode, this.Flags);
		}

		protected override void RunInternal()
		{
			Exception lastException = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				AmDbAction amDbAction = base.PrepareDbAction(this.ActionCode);
				amDbAction.Dismount(this.Flags);
			});
			base.LastException = lastException;
		}
	}
}
