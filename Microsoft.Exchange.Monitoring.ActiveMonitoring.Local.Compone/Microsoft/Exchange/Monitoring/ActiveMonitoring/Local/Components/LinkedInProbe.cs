using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.LinkedIn;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components
{
	internal sealed class LinkedInProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!PeopleConnectMaintenance.ShouldRun(base.TraceContext))
			{
				base.Result.StateAttribute1 = "Probe not run, since this server is not primary active manager of the DAG";
				WTFDiagnostics.TraceInformation(ExTraceGlobals.PeopleConnectTracer, base.TraceContext, "LinkedInProbe.DoWork(): Not run because local server is not PAM of the DAG.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\peopleconnect\\linkedinprobe.cs", 46);
				return;
			}
			try
			{
				IPeopleConnectApplicationConfig peopleConnectApplicationConfig = CachedPeopleConnectApplicationConfig.Instance.ReadLinkedIn();
				PeopleConnectMaintenance.LogApplicationConfig(base.Result, peopleConnectApplicationConfig);
				base.Result.StateAttribute2 = "Redirect Url = " + base.Definition.Endpoint;
				LinkedInConfig linkedInConfig = LinkedInConfig.CreateForAppAuth(peopleConnectApplicationConfig.AppId, peopleConnectApplicationConfig.AppSecretClearText, peopleConnectApplicationConfig.RequestTokenEndpoint, peopleConnectApplicationConfig.AccessTokenEndpoint, peopleConnectApplicationConfig.WebRequestTimeout, peopleConnectApplicationConfig.WebProxyUri, peopleConnectApplicationConfig.ReadTimeUtc);
				LinkedInAppConfig config = new LinkedInAppConfig(peopleConnectApplicationConfig.AppId, peopleConnectApplicationConfig.AppSecretClearText, peopleConnectApplicationConfig.ProfileEndpoint, peopleConnectApplicationConfig.ConnectionsEndpoint, peopleConnectApplicationConfig.RemoveAppEndpoint, peopleConnectApplicationConfig.WebRequestTimeout, peopleConnectApplicationConfig.WebProxyUri);
				LinkedInAppAuthorizationResponse linkedInAppAuthorizationResponse = new LinkedInAuthenticator(linkedInConfig, new LinkedInWebClient(config, NullTracer.Instance), NullTracer.Instance).AuthorizeApplication(new NameValueCollection(), new HttpCookieCollection(), new HttpCookieCollection(), new Uri(base.Definition.Endpoint));
				base.Result.StateAttribute3 = "Application Redirection Uri = " + linkedInAppAuthorizationResponse.AppAuthorizationRedirectUri;
			}
			catch (ExchangeConfigurationException ex)
			{
				this.LogExceptionInResult(ex);
				throw;
			}
			catch (LinkedInAuthenticationException ex2)
			{
				Exception ex3 = ex2.InnerException ?? ex2;
				this.LogExceptionInResult(ex3);
				throw;
			}
		}

		private void LogExceptionInResult(Exception ex)
		{
			Exception ex2 = ex.InnerException ?? ex;
			base.Result.StateAttribute1 = ex2.GetType().Name;
			base.Result.Exception = ex2.GetType().Name;
			base.Result.FailureContext = ex2.StackTrace;
			base.Result.Error = ex2.Message;
			if (ex2 is WebException)
			{
				base.Result.StateAttribute4 = (ex2 as WebException).Status.ToString();
			}
		}
	}
}
