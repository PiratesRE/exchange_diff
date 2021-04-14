using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckCopyStatusNotStale : ActiveOrPassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckCopyStatusNotStale() : base(DatabaseValidationCheck.ID.DatabaseCheckCopyStatusNotStale)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			CopyStatusClientCachedEntry status = args.Status;
			TimeSpan timeSpan = DateTime.UtcNow.Subtract(status.CreateTimeUtc);
			if (timeSpan > DatabaseCheckCopyStatusNotStale.s_maximumStatusAgeError)
			{
				DatabaseValidationCheck.Tracer.TraceDebug((long)this.GetHashCode(), "{0}: Database Copy '{1}' has copy status cached entry that is older than maximum *error* age of {2}. Actual age: {3}.", new object[]
				{
					base.CheckName,
					args.DatabaseCopyName,
					DatabaseCheckCopyStatusNotStale.s_maximumStatusAgeError,
					timeSpan
				});
				error = ReplayStrings.DbValidationCopyStatusTooOld(args.DatabaseCopyName, timeSpan, DatabaseCheckCopyStatusNotStale.s_maximumStatusAgeError);
				return DatabaseValidationCheck.Result.Failed;
			}
			if (timeSpan > DatabaseCheckCopyStatusNotStale.s_maximumStatusAgeWarning)
			{
				DatabaseValidationCheck.Tracer.TraceDebug((long)this.GetHashCode(), "{0}: Database Copy '{1}' has copy status cached entry that is older than maximum *warning* age of {2}. Actual age: {3}.", new object[]
				{
					base.CheckName,
					args.DatabaseCopyName,
					DatabaseCheckCopyStatusNotStale.s_maximumStatusAgeWarning,
					timeSpan
				});
				error = ReplayStrings.DbValidationCopyStatusTooOld(args.DatabaseCopyName, timeSpan, DatabaseCheckCopyStatusNotStale.s_maximumStatusAgeWarning);
				return DatabaseValidationCheck.Result.Warning;
			}
			return DatabaseValidationCheck.Result.Passed;
		}

		private static TimeSpan s_maximumStatusAgeWarning = TimeSpan.FromMilliseconds((double)(RegistryParameters.CopyStatusPollerIntervalInMsec * 4));

		private static TimeSpan s_maximumStatusAgeError = TimeSpan.FromMilliseconds((double)(RegistryParameters.CopyStatusPollerIntervalInMsec * 10));
	}
}
