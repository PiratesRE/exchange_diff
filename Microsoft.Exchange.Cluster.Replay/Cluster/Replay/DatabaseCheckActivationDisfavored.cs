using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckActivationDisfavored : ActiveOrPassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckActivationDisfavored() : base(DatabaseValidationCheck.ID.DatabaseCheckActivationDisfavored)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			if (!args.IgnoreActivationDisfavored)
			{
				AmServerName targetServer = args.TargetServer;
				IADServer iadserver = args.ADConfig.LookupMiniServerByName(targetServer);
				if (iadserver == null)
				{
					error = ReplayStrings.AmBcsTargetServerADError(args.TargetServer.Fqdn, ReplayStrings.AmBcsNoneSpecified);
					return DatabaseValidationCheck.Result.Failed;
				}
				if (iadserver.DatabaseCopyActivationDisabledAndMoveNow)
				{
					error = ReplayStrings.AmBcsTargetServerActivationDisabled(args.ActiveServer.Fqdn);
					return DatabaseValidationCheck.Result.Failed;
				}
			}
			return DatabaseValidationCheck.Result.Passed;
		}
	}
}
