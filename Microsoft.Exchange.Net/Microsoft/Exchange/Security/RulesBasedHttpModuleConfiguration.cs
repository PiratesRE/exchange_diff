using System;
using System.Configuration;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	public class RulesBasedHttpModuleConfiguration : ConfigurationSection
	{
		public static RulesBasedHttpModuleConfiguration Instance
		{
			get
			{
				if (RulesBasedHttpModuleConfiguration.instance == null)
				{
					try
					{
						RulesBasedHttpModuleConfiguration.instance = (RulesBasedHttpModuleConfiguration)ConfigurationManager.GetSection("RulesBasedHttpModuleConfigurationSection");
					}
					catch (ConfigurationErrorsException ex)
					{
						ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string>(0L, "Failed to load RulesBasedHttpModuleConfiguration: {0}", ex.ToString());
					}
					if (RulesBasedHttpModuleConfiguration.instance == null)
					{
						RulesBasedHttpModuleConfiguration.instance = new RulesBasedHttpModuleConfiguration();
					}
				}
				return RulesBasedHttpModuleConfiguration.instance;
			}
		}

		[ConfigurationProperty("DenyRules", IsRequired = false)]
		public HttpModuleAuthenticationDenyRulesCollection AuthenticationDenyRules
		{
			get
			{
				return base["DenyRules"] as HttpModuleAuthenticationDenyRulesCollection;
			}
		}

		internal bool TryLoad()
		{
			return this.AuthenticationDenyRules.TryLoad();
		}

		private static RulesBasedHttpModuleConfiguration instance;

		private static class RulesBasedHttpModuleConfigSchema
		{
			internal const string SectionName = "RulesBasedHttpModuleConfigurationSection";

			internal const string DenyRules = "DenyRules";
		}
	}
}
