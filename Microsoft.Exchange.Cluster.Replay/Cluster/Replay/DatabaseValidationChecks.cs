using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class DatabaseValidationChecks
	{
		public static readonly DatabaseValidationMultiChecks DatabaseAvailabilityActiveChecks = new DatabaseAvailabilityActiveChecks();

		public static readonly DatabaseValidationMultiChecks DatabaseAvailabilityPassiveChecks = new DatabaseAvailabilityPassiveChecks();

		public static readonly DatabaseValidationMultiChecks DatabaseRedundancyDatabaseLevelActiveChecks = new DatabaseRedundancyDatabaseLevelActiveChecks();

		public static readonly DatabaseValidationMultiChecks DatabaseRedundancyDatabaseLevelPassiveChecks = new DatabaseRedundancyDatabaseLevelPassiveChecks();

		public static readonly DatabaseValidationMultiChecks DatabaseConnectedActiveChecks = new DatabaseConnectedActiveChecks();

		public static readonly DatabaseValidationMultiChecks DatabaseConnectedPassiveChecks = new DatabaseConnectedPassiveChecks();
	}
}
