using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseAvailabilityValidator : DatabaseValidatorBase
	{
		public DatabaseAvailabilityValidator(IADDatabase database, int numAvailableCopiesMinimum, ICopyStatusClientLookup statusLookup, IMonitoringADConfig adConfig, PropertyUpdateTracker propertyUpdateTracker = null, bool shouldSkipEvents = true) : base(database, numAvailableCopiesMinimum, statusLookup, adConfig, propertyUpdateTracker, false, false, false, false, shouldSkipEvents)
		{
		}

		protected override DatabaseValidationMultiChecks ActiveCopyChecks
		{
			get
			{
				return DatabaseValidationChecks.DatabaseAvailabilityActiveChecks;
			}
		}

		protected override DatabaseValidationMultiChecks PassiveCopyChecks
		{
			get
			{
				return DatabaseValidationChecks.DatabaseAvailabilityPassiveChecks;
			}
		}

		protected override string GetValidationRollupErrorMessage(int healthyCopiesCount, int expectedHealthyCopiesCount, int totalPassiveCopiesCount, int healthyPassiveCopiesCount, string rollupMessage)
		{
			return ReplayStrings.DbAvailabilityValidationErrorsOccurred(base.Database.Name, healthyCopiesCount, expectedHealthyCopiesCount, rollupMessage);
		}

		protected override Exception UpdateReplayStateProperties(RegistryStateAccess regState, bool validationChecksPassed)
		{
			return base.UpdateReplayStatePropertiesCommon(regState, validationChecksPassed, "LastCopyAvailabilityChecksPassedTime", "IsLastCopyAvailabilityChecksPassed");
		}
	}
}
