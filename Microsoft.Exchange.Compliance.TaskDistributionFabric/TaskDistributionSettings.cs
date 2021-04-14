using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric
{
	internal static class TaskDistributionSettings
	{
		static TaskDistributionSettings()
		{
			TaskDistributionSettings.MaxQueuePerBlock = 100;
			TaskDistributionSettings.ApplicationExecutionTime = TimeSpan.FromMinutes(5.0);
			TaskDistributionSettings.DataLookupTime = TimeSpan.FromMinutes(1.0);
			TaskDistributionSettings.DispatchQueueTime = TimeSpan.FromMilliseconds(500.0);
			TaskDistributionSettings.GeneralOperationTime = TimeSpan.FromMinutes(1.0);
			TaskDistributionSettings.IncomingEntryExpiryTime = TimeSpan.FromMinutes(5.0);
			TaskDistributionSettings.OutgoingEntryExpiryTime = TimeSpan.FromMinutes(5.0);
			TaskDistributionSettings.RemoteExecutionTime = TimeSpan.FromMinutes(3.0);
			TaskDistributionSettings.IncomingEntryRetriesToAbandon = 5;
			TaskDistributionSettings.OutgoingEntryRetriesToAbandon = 5;
			TaskDistributionSettings.IncomingEntryRetriesToFailure = 3;
			TaskDistributionSettings.OutgoingEntryRetriesToFailure = 3;
		}

		public static int MaxQueuePerBlock { get; set; }

		public static bool EnableDispatchQueue { get; set; } = false;

		public static TimeSpan DispatchQueueTime { get; set; }

		public static TimeSpan DataLookupTime { get; set; }

		public static TimeSpan ApplicationExecutionTime { get; set; }

		public static TimeSpan GeneralOperationTime { get; set; }

		public static TimeSpan RemoteExecutionTime { get; set; }

		public static TimeSpan IncomingEntryExpiryTime { get; set; }

		public static TimeSpan OutgoingEntryExpiryTime { get; set; }

		public static int OutgoingEntryRetriesToFailure { get; set; }

		public static int OutgoingEntryRetriesToAbandon { get; set; }

		public static int IncomingEntryRetriesToFailure { get; set; }

		public static int IncomingEntryRetriesToAbandon { get; set; }
	}
}
