using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class CallContext
	{
		internal CallContext(HttpContext httpContext, UserCollection users, HashSet<UserConfigurationSettingName> requestedSettings, ExchangeServerVersion? requestedVersion, UserSettingErrorCollection settingErrors, GetUserSettingsResponse response)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (httpContext.Request == null)
			{
				throw new ArgumentException("The specified HTTP context has the Request property null", "httpContext");
			}
			this.Users = users;
			this.RequestedSettings = requestedSettings;
			this.RequestedVersion = requestedVersion;
			this.SettingErrors = settingErrors;
			this.Response = response;
			this.UseClientCertificateAuthentication = Common.CheckClientCertificate(httpContext.Request);
			this.UserAgent = httpContext.Request.UserAgent;
			this.UserAuthType = httpContext.Request.ServerVariables["AUTH_TYPE"];
			this.CallerCapabilities = CallerRequestedCapabilities.GetInstance(httpContext);
		}

		internal string UserAgent { get; private set; }

		internal string UserAuthType { get; private set; }

		internal CallerRequestedCapabilities CallerCapabilities { get; private set; }

		internal UserCollection Users { get; private set; }

		internal HashSet<UserConfigurationSettingName> RequestedSettings { get; private set; }

		internal ExchangeServerVersion? RequestedVersion { get; private set; }

		internal UserSettingErrorCollection SettingErrors { get; private set; }

		internal GetUserSettingsResponse Response { get; private set; }

		internal bool UseClientCertificateAuthentication { get; private set; }
	}
}
