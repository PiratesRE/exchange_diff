using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AutodiscoverProxyService : IAutodiscoverService
	{
		public AutodiscoverProxyService(ExchangeVersion exchangeVersion, NetworkCredential credentials)
		{
			MigrationUtil.ThrowOnNullArgument(credentials, "credentials");
			this.service = new AutodiscoverService(exchangeVersion);
			this.service.IsExternal = new bool?(true);
			this.service.Credentials = credentials;
			this.service.EnableScpLookup = false;
			this.service.RedirectionUrlValidationCallback = new AutodiscoverRedirectionUrlValidationCallback(this.MigrationAutodiscoverRedirectionUrlValidationCallback);
		}

		public Uri Url
		{
			get
			{
				return this.service.Url;
			}
			set
			{
				this.service.Url = value;
			}
		}

		public GetUserSettingsResponse GetUserSettings(string userSmtpAddress, params UserSettingName[] userSettingNames)
		{
			return this.service.GetUserSettings(userSmtpAddress, userSettingNames);
		}

		public bool MigrationAutodiscoverRedirectionUrlValidationCallback(string redirectionUrl)
		{
			return true;
		}

		private readonly AutodiscoverService service;
	}
}
