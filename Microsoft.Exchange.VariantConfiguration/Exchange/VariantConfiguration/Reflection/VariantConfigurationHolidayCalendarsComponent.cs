using System;
using Microsoft.Exchange.HolidayCalendars;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationHolidayCalendarsComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationHolidayCalendarsComponent() : base("HolidayCalendars")
		{
			base.Add(new VariantConfigurationSection("HolidayCalendars.settings.ini", "HostConfiguration", typeof(IHostSettings), true));
		}

		public VariantConfigurationSection HostConfiguration
		{
			get
			{
				return base["HostConfiguration"];
			}
		}
	}
}
