using System;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckServerAllowedForActivation : ActiveOrPassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckServerAllowedForActivation() : base(DatabaseValidationCheck.ID.DatabaseCheckServerAllowedForActivation)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			error = LocalizedString.Empty;
			AmBcsServerChecks checks = AmBcsServerChecks.DatacenterActivationModeStarted | AmBcsServerChecks.AutoActivationAllowed;
			AmBcsServerValidation amBcsServerValidation = new AmBcsServerValidation(args.TargetServer, args.ActiveServer, args.Database, null, null, args.ADConfig);
			if (!amBcsServerValidation.RunChecks(checks, ref error))
			{
				return DatabaseValidationCheck.Result.Failed;
			}
			return DatabaseValidationCheck.Result.Passed;
		}
	}
}
