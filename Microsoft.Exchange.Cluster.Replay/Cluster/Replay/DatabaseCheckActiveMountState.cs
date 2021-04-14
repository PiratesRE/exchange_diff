using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckActiveMountState : ActiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckActiveMountState() : base(DatabaseValidationCheck.ID.DatabaseCheckActiveMountState)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			AmServerName targetServer = args.TargetServer;
			RpcDatabaseCopyStatus2 copyStatus = args.Status.CopyStatus;
			if (copyStatus.CopyStatus == CopyStatusEnum.Dismounted || copyStatus.CopyStatus == CopyStatusEnum.Dismounting)
			{
				if (!string.IsNullOrEmpty(copyStatus.ErrorMessage))
				{
					error = ReplayStrings.DbAvailabilityActiveCopyDismountedError(args.DatabaseName, targetServer.NetbiosName, copyStatus.ErrorMessage);
					return DatabaseValidationCheck.Result.Failed;
				}
				DatabaseValidationCheck.Tracer.TraceDebug<string, string, CopyStatusEnum>((long)this.GetHashCode(), "{0}: Active database copy '{1}' is dismounted/dismounting. CopyStatus={2}. Returning Warning.", base.CheckName, args.DatabaseCopyName, copyStatus.CopyStatus);
				error = ReplayStrings.DbAvailabilityActiveCopyMountState(args.DatabaseName, targetServer.NetbiosName, copyStatus.CopyStatus.ToString());
				return DatabaseValidationCheck.Result.Warning;
			}
			else
			{
				if (copyStatus.CopyStatus == CopyStatusEnum.Mounted)
				{
					DatabaseValidationCheck.Tracer.TraceDebug<string, string, CopyStatusEnum>((long)this.GetHashCode(), "{0}: Active database copy '{1}' is mounted. CopyStatus={2}. Returning Passed.", base.CheckName, args.DatabaseCopyName, copyStatus.CopyStatus);
					return DatabaseValidationCheck.Result.Passed;
				}
				if (copyStatus.CopyStatus == CopyStatusEnum.Mounting)
				{
					DatabaseValidationCheck.Tracer.TraceDebug<string, string, CopyStatusEnum>((long)this.GetHashCode(), "{0}: Active database copy '{1}' is mounting. CopyStatus={2}. Returning Warning.", base.CheckName, args.DatabaseCopyName, copyStatus.CopyStatus);
					error = ReplayStrings.DbAvailabilityActiveCopyMountState(args.DatabaseName, targetServer.NetbiosName, copyStatus.CopyStatus.ToString());
					return DatabaseValidationCheck.Result.Warning;
				}
				error = ReplayStrings.DbAvailabilityActiveCopyUnknownState(args.DatabaseName, targetServer.NetbiosName, copyStatus.CopyStatus.ToString());
				return DatabaseValidationCheck.Result.Failed;
			}
		}
	}
}
