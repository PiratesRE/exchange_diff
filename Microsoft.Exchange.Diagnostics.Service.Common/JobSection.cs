using System;
using System.Configuration;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public class JobSection : ConfigurationSection
	{
		static JobSection()
		{
			JobSection.properties.Add(JobSection.jobs);
		}

		public JobConfigurationCollection Jobs
		{
			get
			{
				return (JobConfigurationCollection)base[JobSection.jobs];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return JobSection.properties;
			}
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty jobs = new ConfigurationProperty("Jobs", typeof(JobConfigurationCollection), null, ConfigurationPropertyOptions.IsRequired);
	}
}
