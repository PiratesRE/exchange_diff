using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckPassiveConnected : PassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckPassiveConnected() : base(DatabaseValidationCheck.ID.DatabaseCheckPassiveConnected)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			error = LocalizedString.Empty;
			DatabaseValidationCheck.Result result = DatabaseValidationCheck.Result.Passed;
			if (args.Status.CopyStatus != null)
			{
				string text = args.Status.CopyStatus.DBName + "\\" + args.Status.CopyStatus.MailboxServer;
				if (args.PropertyUpdateTracker.LastTimeCopierTimeUpdateTracker.UpdateCurrentValueOrReturnStaleness(text, args.Status.CopyStatus.LastLogInfoFromCopierTime).TotalSeconds >= (double)RegistryParameters.DatabaseHealthCheckCopyConnectedErrorThresholdInSec)
				{
					error = ReplayStrings.PassiveCopyDisconnected;
					result = DatabaseValidationCheck.Result.Failed;
				}
			}
			return result;
		}
	}
}
