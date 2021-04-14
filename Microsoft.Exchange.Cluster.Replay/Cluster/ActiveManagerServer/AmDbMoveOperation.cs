using System;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbMoveOperation : AmDbOperation
	{
		internal AmDbMoveOperation(IADDatabase db, AmDbActionCode actionCode) : base(db)
		{
			this.Arguments = new AmDbMoveArguments(actionCode);
		}

		internal AmDbMoveArguments Arguments { get; set; }

		public override string ToString()
		{
			return string.Format("Move database {0} from {1} to {2} with action code {3}.  (Mount flags {4}, dismount flags {5}, MountDialOverride {6}, IncludeOtherServers {7}, SkipValidationChecks {8}, MoveComment '{9}', ComponentName '{10}')", new object[]
			{
				base.Database.Name,
				this.Arguments.SourceServer,
				(this.Arguments.TargetServer != null) ? this.Arguments.TargetServer.ToString() : "(no target)",
				this.Arguments.ActionCode,
				this.Arguments.MountFlags,
				this.Arguments.DismountFlags,
				this.Arguments.MountDialOverride,
				this.Arguments.TryOtherHealthyServers,
				this.Arguments.SkipValidationChecks,
				this.Arguments.MoveComment,
				this.Arguments.ComponentName
			});
		}

		protected override void RunInternal()
		{
			AmDbOperationDetailedStatus moveStatus = null;
			Exception lastException = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				AmDbAction amDbAction = this.PrepareDbAction(this.Arguments.ActionCode);
				amDbAction.Move(this.Arguments.MountFlags, this.Arguments.DismountFlags, this.Arguments.MountDialOverride, this.Arguments.SourceServer, this.Arguments.TargetServer, this.Arguments.TryOtherHealthyServers, this.Arguments.SkipValidationChecks, this.Arguments.MoveComment, this.Arguments.ComponentName, ref moveStatus);
			});
			base.DetailedStatus = moveStatus;
			base.LastException = lastException;
		}
	}
}
