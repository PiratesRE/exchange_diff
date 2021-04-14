using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.Publisher
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class WorkingSetPublisherConfiguration
	{
		public static bool PublishModernGroupsSignals { get; private set; } = AppConfigLoader.GetConfigBoolValue("PublishModernGroupsSignals", true);

		public static TimeSpan ModernGroupsDataExpiryTime { get; private set; } = AppConfigLoader.GetConfigTimeSpanValue("ModernGroupsDataExpiryTime", WorkingSetPublisherConfiguration.ModernGroupsDataExpiryTimeMinValue, WorkingSetPublisherConfiguration.ModernGroupsDataExpiryTimeMaxValue, WorkingSetPublisherConfiguration.ModernGroupsDataExpiryTimeDefaultValue);

		public static int ModernGroupsItemAddWeight { get; private set; } = AppConfigLoader.GetConfigIntValue("ModernGroupsItemAddWeight", 1, int.MaxValue, 10);

		public static int MaxTargetUsersToCachePerModernGroup { get; private set; } = AppConfigLoader.GetConfigIntValue("MaxTargetUsersToCachePerModernGroup", 1, int.MaxValue, 1000);

		public const string PublishModernGroupsSignalsSetting = "PublishModernGroupsSignals";

		public const string ModernGroupsDataExpiryTimeSetting = "ModernGroupsDataExpiryTime";

		public const string ModernGroupsItemAddWeightSetting = "ModernGroupsItemAddWeight";

		public const string MaxTargetUsersToCachePerModernGroupSetting = "MaxTargetUsersToCachePerModernGroup";

		public const bool PublishModernGroupsSignalsDefaultValue = true;

		public const int ModernGroupsItemAddWeightDefaultValue = 10;

		public const int ModernGroupsItemAddWeightMinValue = 1;

		public const int ModernGroupsItemAddWeightMaxValue = 2147483647;

		public const int MaxTargetUsersToCachePerModernGroupDefaultValue = 1000;

		public const int MaxTargetUsersToCachePerModernGroupMinValue = 1;

		public const int MaxTargetUsersToCachePerModernGroupMaxValue = 2147483647;

		public static readonly TimeSpan ModernGroupsDataExpiryTimeDefaultValue = TimeSpan.FromMinutes(60.0);

		public static readonly TimeSpan ModernGroupsDataExpiryTimeMinValue = TimeSpan.FromMinutes(0.0);

		public static readonly TimeSpan ModernGroupsDataExpiryTimeMaxValue = TimeSpan.FromDays(365.0);
	}
}
