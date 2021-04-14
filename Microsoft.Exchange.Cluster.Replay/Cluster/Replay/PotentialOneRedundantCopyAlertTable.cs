using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PotentialOneRedundantCopyAlertTable : DatabaseAlertInfoTable<DatabasePotentialOneCopyAlert>
	{
		public PotentialOneRedundantCopyAlertTable() : base(new Func<IHealthValidationResultMinimal, DatabasePotentialOneCopyAlert>(PotentialOneRedundantCopyAlertTable.CreateDatabaseOneRedundantCopyAlert))
		{
		}

		protected override MonitoringAlert GetExistingOrNewAlertInfo(IHealthValidationResultMinimal result)
		{
			DatabasePotentialOneCopyAlert databasePotentialOneCopyAlert = (DatabasePotentialOneCopyAlert)base.GetExistingOrNewAlertInfo(result);
			string activeServerName = PotentialOneRedundantCopyAlertTable.GetActiveServerName(result);
			if (databasePotentialOneCopyAlert.TargetServer != activeServerName)
			{
				DatabaseAlertInfoTable<DatabasePotentialOneCopyAlert>.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "PotentialOneRedundantCopyAlertTable::GetExistingOrNewAlertInfo: TargetServer has been changed, create one new alert for {0}, new TargetServer is {1}", result.Identity, activeServerName);
				base.RemoveDatabaseAlert(result.IdentityGuid);
				databasePotentialOneCopyAlert = (DatabasePotentialOneCopyAlert)base.GetExistingOrNewAlertInfo(result);
			}
			return databasePotentialOneCopyAlert;
		}

		private static DatabasePotentialOneCopyAlert CreateDatabaseOneRedundantCopyAlert(IHealthValidationResultMinimal validationResult)
		{
			string activeServerName = PotentialOneRedundantCopyAlertTable.GetActiveServerName(validationResult);
			return new DatabasePotentialOneCopyAlert(validationResult.Identity, validationResult.IdentityGuid, activeServerName);
		}

		private static string GetActiveServerName(IHealthValidationResultMinimal validationResult)
		{
			IHealthValidationResult healthValidationResult = (IHealthValidationResult)validationResult;
			string result = null;
			if (healthValidationResult.ActiveCopyStatus != null)
			{
				result = healthValidationResult.ActiveCopyStatus.ServerContacted.NetbiosName;
			}
			else if (healthValidationResult.TargetCopyStatus != null && healthValidationResult.TargetCopyStatus.ActiveServer != null)
			{
				result = healthValidationResult.TargetCopyStatus.ActiveServer.NetbiosName;
			}
			return result;
		}
	}
}
