using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.LinkedIn;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class LinkedInSetup : EcpContentPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			try
			{
				LinkedInConfig linkedInConfig = this.ReadConfiguration();
				LinkedInAppConfig config = this.ReadAppConfiguration();
				LinkedInAppAuthorizationResponse response = new LinkedInAuthenticator(linkedInConfig, new LinkedInWebClient(config, LinkedInSetup.Tracer), LinkedInSetup.Tracer).AuthorizeApplication(base.Request.QueryString, base.Request.Cookies, base.Response.Cookies, this.GetAuthorizationCallbackUrl());
				this.ProcessAuthorizationResponse(response);
			}
			catch (ExchangeConfigurationException ex)
			{
				EcpEventLogConstants.Tuple_BadLinkedInConfiguration.LogPeriodicEvent(EcpEventLogExtensions.GetPeriodicKeyPerUser(), new object[]
				{
					EcpEventLogExtensions.GetUserNameToLog(),
					ex
				});
				ErrorHandlingUtil.TransferToErrorPage("badlinkedinconfiguration");
			}
			catch (LinkedInAuthenticationException ex2)
			{
				EcpEventLogConstants.Tuple_LinkedInAuthorizationError.LogEvent(new object[]
				{
					EcpEventLogExtensions.GetUserNameToLog(),
					ex2
				});
				ErrorHandlingUtil.TransferToErrorPage("linkedinauthorizationerror");
			}
		}

		private void ProcessAuthorizationResponse(LinkedInAppAuthorizationResponse response)
		{
			if (!string.IsNullOrEmpty(response.AppAuthorizationRedirectUri))
			{
				this.RedirectToAuthorizationEndpoint(response.AppAuthorizationRedirectUri);
				return;
			}
			if (!string.IsNullOrEmpty(response.OAuthProblem))
			{
				this.ProcessAuthorizationDenied();
				return;
			}
			this.ProcessAuthorizationGranted(response.RequestToken, response.RequestSecret, response.OAuthVerifier);
		}

		private void RedirectToAuthorizationEndpoint(string authorizationEndpoint)
		{
			base.Response.Redirect(authorizationEndpoint);
		}

		private void ProcessAuthorizationGranted(string requestToken, string requestSecret, string verifier)
		{
			this.ctlNewConnectSubscription.CreateLinkedIn = true;
			this.ctlNewConnectSubscription.RequestToken = requestToken;
			this.ctlNewConnectSubscription.RequestSecret = requestSecret;
			this.ctlNewConnectSubscription.Verifier = verifier;
		}

		private void ProcessAuthorizationDenied()
		{
			this.ctlNewConnectSubscription.CloseWindowWithoutCreatingSubscription = true;
		}

		private Uri GetAuthorizationCallbackUrl()
		{
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadLinkedIn();
			if (!string.IsNullOrWhiteSpace(peopleConnectApplicationConfig.ConsentRedirectEndpoint))
			{
				return new Uri(peopleConnectApplicationConfig.ConsentRedirectEndpoint);
			}
			return EcpFeature.LinkedInSetup.GetFeatureDescriptor().AbsoluteUrl;
		}

		private LinkedInConfig ReadConfiguration()
		{
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadLinkedIn();
			return LinkedInConfig.CreateForAppAuth(peopleConnectApplicationConfig.AppId, peopleConnectApplicationConfig.AppSecretClearText, peopleConnectApplicationConfig.RequestTokenEndpoint, peopleConnectApplicationConfig.AccessTokenEndpoint, peopleConnectApplicationConfig.WebRequestTimeout, peopleConnectApplicationConfig.WebProxyUri, peopleConnectApplicationConfig.ReadTimeUtc);
		}

		private LinkedInAppConfig ReadAppConfiguration()
		{
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadLinkedIn();
			return new LinkedInAppConfig(peopleConnectApplicationConfig.AppId, peopleConnectApplicationConfig.AppSecretClearText, peopleConnectApplicationConfig.ProfileEndpoint, peopleConnectApplicationConfig.ConnectionsEndpoint, peopleConnectApplicationConfig.RemoveAppEndpoint, peopleConnectApplicationConfig.WebRequestTimeout, peopleConnectApplicationConfig.WebProxyUri);
		}

		private static readonly Trace Tracer = ExTraceGlobals.LinkedInTracer;

		protected NewConnectSubscription ctlNewConnectSubscription;
	}
}
