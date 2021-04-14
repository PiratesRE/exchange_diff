using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabasePotentialOneCopyAlert : MonitoringAlert
	{
		public DatabasePotentialOneCopyAlert(string databaseName, Guid dbGuid, string targetServer) : base(databaseName, dbGuid)
		{
			this.TargetServer = targetServer;
		}

		public string TargetServer { get; private set; }

		protected override TimeSpan DatabaseHealthCheckRedTransitionSuppression
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckPotentialOneCopyRedTransitionSuppressionInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckGreenTransitionSuppression
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckPotentialOneCopyGreenTransitionSuppressionInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckRedPeriodicInterval
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckPotentialOneCopyRedPeriodicIntervalInSec);
			}
		}

		protected override void RaiseGreenEvent(IHealthValidationResultMinimal result)
		{
		}

		protected override void RaiseRedEvent(IHealthValidationResultMinimal result)
		{
		}
	}
}
