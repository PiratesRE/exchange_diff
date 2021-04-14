using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DatabaseAvailabilityCheck : ReplicationCheck
	{
		public DatabaseAvailabilityCheck(string serverName, IEventManager eventManager, string momEventSource, DatabaseHealthValidationRunner validationRunner, uint ignoreTransientErrorsThreshold) : base("DatabaseAvailability", CheckId.DatabasesAvailability, Strings.DatabaseAvailabilityCheckDesc, CheckCategory.SystemHighPriority, eventManager, momEventSource, serverName, new uint?(ignoreTransientErrorsThreshold))
		{
			this.m_validationRunner = validationRunner;
		}

		protected override void InternalRun()
		{
			if (!ReplicationCheckGlobals.DatabaseAvailabilityCheckHasRun)
			{
				ReplicationCheckGlobals.DatabaseAvailabilityCheckHasRun = true;
			}
			else
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "DatabaseAvailabilityCheck skipping because it has already been run once.");
				base.Skip();
			}
			Exception ex = DatabaseRedundancyCheck.InitializeValidationRunner(this.m_validationRunner);
			if (ex != null)
			{
				base.Fail(Strings.DatabasesAvailabilityCheckFailed(base.ServerName, ex.Message));
			}
			foreach (IHealthValidationResultMinimal healthValidationResultMinimal in this.m_validationRunner.RunDatabaseAvailabilityChecks())
			{
				base.InstanceIdentity = healthValidationResultMinimal.IdentityGuid.ToString().ToLowerInvariant();
				if (healthValidationResultMinimal.IsValidationSuccessful)
				{
					base.ReportPassedInstance();
				}
				else
				{
					base.FailContinue(new LocalizedString(healthValidationResultMinimal.ErrorMessageWithoutFullStatus));
				}
				base.InstanceIdentity = null;
			}
		}

		private DatabaseHealthValidationRunner m_validationRunner;
	}
}
