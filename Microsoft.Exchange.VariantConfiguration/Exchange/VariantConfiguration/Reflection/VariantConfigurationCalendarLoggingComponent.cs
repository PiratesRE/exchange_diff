using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationCalendarLoggingComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationCalendarLoggingComponent() : base("CalendarLogging")
		{
			base.Add(new VariantConfigurationSection("CalendarLogging.settings.ini", "FixMissingMeetingBody", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CalendarLogging.settings.ini", "CalendarLoggingIncludeSeriesMeetingMessagesInCVS", typeof(IFeature), false));
		}

		public VariantConfigurationSection FixMissingMeetingBody
		{
			get
			{
				return base["FixMissingMeetingBody"];
			}
		}

		public VariantConfigurationSection CalendarLoggingIncludeSeriesMeetingMessagesInCVS
		{
			get
			{
				return base["CalendarLoggingIncludeSeriesMeetingMessagesInCVS"];
			}
		}
	}
}
