using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DatabaseRedundancyCheck : ReplicationCheck
	{
		public DatabaseRedundancyCheck(string serverName, IEventManager eventManager, string momEventSource, DatabaseHealthValidationRunner validationRunner, uint ignoreTransientErrorsThreshold) : base("DatabaseRedundancy", CheckId.DatabasesRedundancy, Strings.DatabaseRedundancyCheckDesc, CheckCategory.SystemHighPriority, eventManager, momEventSource, serverName, new uint?(ignoreTransientErrorsThreshold))
		{
			this.m_validationRunner = validationRunner;
		}

		protected override void InternalRun()
		{
			if (!ReplicationCheckGlobals.DatabaseRedundancyCheckHasRun)
			{
				ReplicationCheckGlobals.DatabaseRedundancyCheckHasRun = true;
			}
			else
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "DatabaseRedundancyCheck skipping because it has already been run once.");
				base.Skip();
			}
			Exception ex = DatabaseRedundancyCheck.InitializeValidationRunner(this.m_validationRunner);
			if (ex != null)
			{
				base.Fail(Strings.DatabasesRedundancyCheckFailed(base.ServerName, ex.Message));
			}
			foreach (IHealthValidationResultMinimal healthValidationResultMinimal in this.m_validationRunner.RunDatabaseRedundancyChecks(null, true))
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

		internal static Exception InitializeValidationRunner(DatabaseHealthValidationRunner runner)
		{
			Exception result = null;
			try
			{
				runner.Initialize();
			}
			catch (MonitoringADConfigException ex)
			{
				result = ex;
			}
			catch (AmServerException ex2)
			{
				result = ex2;
			}
			catch (AmServerTransientException ex3)
			{
				result = ex3;
			}
			return result;
		}

		private DatabaseHealthValidationRunner m_validationRunner;
	}
}
