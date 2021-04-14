using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckDatabaseIsReplicated : ActiveOrPassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckDatabaseIsReplicated() : base(DatabaseValidationCheck.ID.DatabaseCheckDatabaseIsReplicated)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			if (args.Database.ReplicationType != ReplicationType.Remote)
			{
				DatabaseValidationCheck.Tracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "{0}: Database '{1}' is *NOT* replicated! It has only {2} copy(ies) configured in the AD.", base.CheckName, args.DatabaseName, args.Database.DatabaseCopies.Length);
				error = ReplayStrings.DbValidationDbNotReplicated(args.DatabaseName);
				return DatabaseValidationCheck.Result.Warning;
			}
			return DatabaseValidationCheck.Result.Passed;
		}
	}
}
