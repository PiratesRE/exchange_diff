using System;
using System.Collections.Specialized;
using System.Management.Automation;
using System.Web;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.LinkedIn;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Test", "LinkedInConnect")]
	internal sealed class TestLinkedInConnect : Task
	{
		public TestLinkedInConnect()
		{
			this.tracer = new CmdletWriteVerboseTracer(this);
		}

		[Parameter(Mandatory = true)]
		public string AuthorizationCallbackUrl { get; set; }

		protected override void InternalProcessRecord()
		{
			base.WriteObject(this.AuthorizeApplication());
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || typeof(ExchangeConfigurationException).IsInstanceOfType(e) || typeof(LinkedInAuthenticationException).IsInstanceOfType(e);
		}

		private LinkedInAppAuthorizationResponse AuthorizeApplication()
		{
			return this.CreateAuthenticator().AuthorizeApplication(new NameValueCollection(), new HttpCookieCollection(), new HttpCookieCollection(), new Uri(this.AuthorizationCallbackUrl));
		}

		private LinkedInAuthenticator CreateAuthenticator()
		{
			return new LinkedInAuthenticator(this.ReadConfiguration(), this.CreateWebClient(), this.tracer);
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

		private LinkedInWebClient CreateWebClient()
		{
			return new LinkedInWebClient(this.ReadAppConfiguration(), this.tracer);
		}

		private readonly ITracer tracer;
	}
}
