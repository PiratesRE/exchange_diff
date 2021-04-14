using System;
using System.Collections.Generic;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class GetDomainSettingsCallContext
	{
		internal GetDomainSettingsCallContext(string userAgent, ExchangeVersion? requestedVersion, DomainCollection domains, HashSet<DomainConfigurationSettingName> requestedSettings, DomainSettingErrorCollection settingErrors, GetDomainSettingsResponse response)
		{
			this.userAgent = userAgent;
			this.requestedVersion = requestedVersion;
			this.domains = domains;
			this.requestedSettings = requestedSettings;
			this.settingErrors = settingErrors;
			this.response = response;
		}

		internal string UserAgent
		{
			get
			{
				return this.userAgent;
			}
		}

		internal DomainCollection Domains
		{
			get
			{
				return this.domains;
			}
		}

		internal HashSet<DomainConfigurationSettingName> RequestedSettings
		{
			get
			{
				return this.requestedSettings;
			}
		}

		internal ExchangeVersion? RequestedVersion
		{
			get
			{
				return this.requestedVersion;
			}
		}

		internal DomainSettingErrorCollection SettingErrors
		{
			get
			{
				return this.settingErrors;
			}
		}

		internal GetDomainSettingsResponse Response
		{
			get
			{
				return this.response;
			}
		}

		private string userAgent;

		private DomainCollection domains;

		private HashSet<DomainConfigurationSettingName> requestedSettings;

		private DomainSettingErrorCollection settingErrors;

		private GetDomainSettingsResponse response;

		private ExchangeVersion? requestedVersion;
	}
}
