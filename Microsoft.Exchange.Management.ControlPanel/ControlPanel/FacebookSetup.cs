using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class FacebookSetup : EcpContentPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			try
			{
				this.ctlUserConsentForm.Visible = false;
				FacebookAuthenticatorConfig config = this.ReadConfiguration();
				FacebookAuthenticator facebookAuthenticator = new FacebookAuthenticator(config);
				AppAuthorizationResponse response = FacebookAuthenticator.ParseAppAuthorizationResponse(base.Request.QueryString);
				if (!FacebookAuthenticator.IsRedirectFromFacebook(response))
				{
					string text = facebookAuthenticator.GetAppAuthorizationUri().ToString();
					if (this.IsReconnect())
					{
						base.Response.Redirect(text);
					}
					else
					{
						this.ctlUserConsentForm.Visible = true;
						this.ctlUserConsentForm.AuthorizationUrl = text;
					}
				}
				else if (facebookAuthenticator.IsAuthorizationGranted(response))
				{
					this.ProcessAuthorizationGranted(response);
				}
				else
				{
					this.ProcessAuthorizationDenied();
				}
			}
			catch (ExchangeConfigurationException ex)
			{
				EcpEventLogConstants.Tuple_BadFacebookConfiguration.LogPeriodicEvent(EcpEventLogExtensions.GetPeriodicKeyPerUser(), new object[]
				{
					EcpEventLogExtensions.GetUserNameToLog(),
					ex
				});
				ErrorHandlingUtil.TransferToErrorPage("badfacebookconfiguration");
			}
		}

		private FacebookAuthenticatorConfig ReadConfiguration()
		{
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadFacebook();
			return FacebookAuthenticatorConfig.CreateForAppAuthorization(peopleConnectApplicationConfig.AppId, this.GetRedirectUri(), peopleConnectApplicationConfig.AuthorizationEndpoint, Thread.CurrentThread.CurrentUICulture, peopleConnectApplicationConfig.ReadTimeUtc);
		}

		private string GetRedirectUri()
		{
			IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadFacebook();
			string text = string.IsNullOrWhiteSpace(peopleConnectApplicationConfig.ConsentRedirectEndpoint) ? EcpFeature.FacebookSetup.GetFeatureDescriptor().AbsoluteUrl.ToEscapedString() : peopleConnectApplicationConfig.ConsentRedirectEndpoint;
			if (this.IsReconnect())
			{
				UriBuilder uriBuilder = new UriBuilder(text)
				{
					Query = string.Format("{0}={1}", "Action", "Reconnect")
				};
				text = uriBuilder.Uri.ToEscapedString();
			}
			return text;
		}

		private bool IsReconnect()
		{
			return "Reconnect".Equals(base.Request.QueryString["Action"], StringComparison.OrdinalIgnoreCase);
		}

		private void ProcessAuthorizationGranted(AppAuthorizationResponse response)
		{
			if (this.IsReconnect())
			{
				this.ctlSetConnectSubscription.SetFacebook = true;
				this.ctlSetConnectSubscription.AppAuthorizationCode = response.AppAuthorizationCode;
				this.ctlSetConnectSubscription.RedirectUri = this.GetRedirectUri();
				return;
			}
			this.ctlNewConnectSubscription.CreateFacebook = true;
			this.ctlNewConnectSubscription.AppAuthorizationCode = response.AppAuthorizationCode;
			this.ctlNewConnectSubscription.RedirectUri = this.GetRedirectUri();
		}

		private void ProcessAuthorizationDenied()
		{
			if (this.IsReconnect())
			{
				this.ctlSetConnectSubscription.CloseWindowWithoutUpdatingSubscription = true;
				return;
			}
			this.ctlNewConnectSubscription.CloseWindowWithoutCreatingSubscription = true;
		}

		private const string ActionParameter = "Action";

		private const string ReconnectValue = "Reconnect";

		protected NewConnectSubscription ctlNewConnectSubscription;

		protected SetConnectSubscription ctlSetConnectSubscription;

		protected FacebookUserConsentForm ctlUserConsentForm;
	}
}
