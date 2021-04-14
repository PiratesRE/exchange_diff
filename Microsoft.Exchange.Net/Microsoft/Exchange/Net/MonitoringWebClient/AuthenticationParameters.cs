using System;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class AuthenticationParameters
	{
		internal CommonAccessToken CommonAccessToken { get; set; }

		internal bool ShouldDownloadStaticFileOnLogonPage
		{
			get
			{
				return this.shouldDownloadStaticFileOnLogonPage;
			}
			set
			{
				this.shouldDownloadStaticFileOnLogonPage = value;
			}
		}

		internal bool ShouldUseTenantHintOnLiveIdLogon
		{
			get
			{
				return this.shouldUseTenantHintOnLiveIdLogon;
			}
			set
			{
				this.shouldUseTenantHintOnLiveIdLogon = value;
			}
		}

		private bool shouldUseTenantHintOnLiveIdLogon = true;

		private bool shouldDownloadStaticFileOnLogonPage;
	}
}
