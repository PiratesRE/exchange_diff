using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseRedundancyValidator : DatabaseValidatorBase
	{
		public DatabaseRedundancyValidator(IADDatabase database, int numHealthyCopiesMinimum, ICopyStatusClientLookup statusLookup, IMonitoringADConfig adConfig, PropertyUpdateTracker propertyUpdateTracker = null, bool shouldSkipEvents = true) : base(database, numHealthyCopiesMinimum, statusLookup, adConfig, propertyUpdateTracker, false, true, true, true, shouldSkipEvents)
		{
		}

		public DatabaseRedundancyValidator(IADDatabase database, int numHealthyCopiesMinimum, ICopyStatusClientLookup statusLookup, IMonitoringADConfig adConfig, PropertyUpdateTracker propertyUpdateTracker, bool isCopyRemoval, bool ignoreActivationDisfavored, bool ignoreMaintenanceChecks, bool ignoreTooManyActivesCheck, bool shouldSkipEvents = true) : base(database, numHealthyCopiesMinimum, statusLookup, adConfig, propertyUpdateTracker, isCopyRemoval, ignoreActivationDisfavored, ignoreMaintenanceChecks, ignoreTooManyActivesCheck, shouldSkipEvents)
		{
		}

		protected override DatabaseValidationMultiChecks ActiveCopyChecks
		{
			get
			{
				return DatabaseValidationChecks.DatabaseRedundancyDatabaseLevelActiveChecks;
			}
		}

		protected override DatabaseValidationMultiChecks PassiveCopyChecks
		{
			get
			{
				return DatabaseValidationChecks.DatabaseRedundancyDatabaseLevelPassiveChecks;
			}
		}

		protected override string GetValidationRollupErrorMessage(int healthyCopiesCount, int expectedHealthyCopiesCount, int totalPassiveCopiesCount, int healthyPassiveCopiesCount, string rollupMessage)
		{
			return ReplayStrings.DbRedundancyValidationErrorsOccurred(base.Database.Name, healthyCopiesCount, expectedHealthyCopiesCount, rollupMessage);
		}

		protected override Exception UpdateReplayStateProperties(RegistryStateAccess regState, bool validationChecksPassed)
		{
			return base.UpdateReplayStatePropertiesCommon(regState, validationChecksPassed, "LastCopyRedundancyChecksPassedTime", "IsLastCopyRedundancyChecksPassed");
		}
	}
}
