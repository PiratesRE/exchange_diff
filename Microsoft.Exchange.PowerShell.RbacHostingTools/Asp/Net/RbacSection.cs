using System;
using System.Configuration;
using System.Web.Configuration;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.PowerShell.RbacHostingTools.Asp.Net
{
	internal class RbacSection : ConfigurationSection
	{
		public static RbacSection Instance
		{
			get
			{
				return RbacSection.instance;
			}
		}

		[ConfigurationProperty("rbacPrincipalMaximumAge", IsRequired = true, DefaultValue = "00:15:00")]
		public TimeSpan RbacPrincipalMaximumAge
		{
			get
			{
				TimeSpan maximumAgeLimit = ExchangeExpiringRunspaceConfiguration.GetMaximumAgeLimit(ExpirationLimit.RunspaceRefresh);
				TimeSpan timeSpan = (TimeSpan)base["rbacPrincipalMaximumAge"];
				if (!(timeSpan < maximumAgeLimit))
				{
					return maximumAgeLimit;
				}
				return timeSpan;
			}
			set
			{
				base["rbacPrincipalMaximumAge"] = value;
			}
		}

		[ConfigurationProperty("rbacRunspaceSlidingExpiration", IsRequired = true, DefaultValue = "00:05:00")]
		public TimeSpan RbacRunspaceSlidingExpiration
		{
			get
			{
				return (TimeSpan)base["rbacRunspaceSlidingExpiration"];
			}
			set
			{
				base["rbacRunspaceSlidingExpiration"] = value;
			}
		}

		private static RbacSection instance = ((RbacSection)WebConfigurationManager.GetSection("rbacConfig")) ?? new RbacSection();
	}
}
