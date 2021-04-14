using System;
using System.Configuration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes
{
	public class FilteredEventSection : ConfigurationSection
	{
		[ConfigurationProperty("controlPanelRedEvent4")]
		public KeyValueConfigurationCollection ControlPanelRedEvent4
		{
			get
			{
				return (KeyValueConfigurationCollection)base["controlPanelRedEvent4"];
			}
		}
	}
}
