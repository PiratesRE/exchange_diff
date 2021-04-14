using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseReplicationNotStalledValidator : DatabaseValidatorBase
	{
		public DatabaseReplicationNotStalledValidator(IADDatabase database, ICopyStatusClientLookup statusLookup, IMonitoringADConfig adConfig, PropertyUpdateTracker propertyUpdateTracker, bool shouldSkipEvents = true) : base(database, 0, 1, statusLookup, adConfig, propertyUpdateTracker, false, true, true, true, shouldSkipEvents)
		{
			if (propertyUpdateTracker == null)
			{
				throw new ArgumentNullException("propertyUpdateTracker shouldn't be null for DatabaseReplicationNotStalledValidator");
			}
		}

		protected override DatabaseValidationMultiChecks ActiveCopyChecks
		{
			get
			{
				return DatabaseValidationChecks.DatabaseConnectedActiveChecks;
			}
		}

		protected override DatabaseValidationMultiChecks PassiveCopyChecks
		{
			get
			{
				return DatabaseValidationChecks.DatabaseConnectedPassiveChecks;
			}
		}

		protected override string GetValidationRollupErrorMessage(int healthyCopiesCount, int expectedHealthyCopiesCount, int totalPassiveCopiesCount, int healthyPassiveCopiesCount, string rollupMessage)
		{
			return ReplayStrings.PotentialRedundancyValidationDBReplicationStalled(base.Database.Name, totalPassiveCopiesCount, rollupMessage);
		}

		protected override Exception UpdateReplayStateProperties(RegistryStateAccess regState, bool validationChecksPassed)
		{
			return null;
		}
	}
}
