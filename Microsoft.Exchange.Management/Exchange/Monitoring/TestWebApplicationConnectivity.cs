using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.WebApplicationClient;

namespace Microsoft.Exchange.Monitoring
{
	public abstract class TestWebApplicationConnectivity : TestVirtualDirectoryConnectivity
	{
		internal TestWebApplicationConnectivity(LocalizedString applicationName, LocalizedString applicationShortName, TransientErrorCache transientErrorCache, string monitoringEventSourceInternal, string monitoringEventSourceExternal) : base(applicationName, applicationShortName, transientErrorCache, monitoringEventSourceInternal, monitoringEventSourceExternal)
		{
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public string RSTEndpoint
		{
			get
			{
				if (!(this.liveRSTEndpoint != null))
				{
					return string.Empty;
				}
				return this.liveRSTEndpoint.AbsoluteUri;
			}
			set
			{
				this.liveRSTEndpoint = new Uri(value);
			}
		}

		protected override List<CasTransactionOutcome> ExecuteTests(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			TaskLogger.LogEnter();
			instance.liveRSTEndpointUri = this.liveRSTEndpoint;
			try
			{
				base.WriteVerbose(Strings.CasHealthWebAppStartTest(instance.baseUri));
				WebApplication webApplication = this.GetWebApplication(instance);
				if (webApplication != null)
				{
					this.ExecuteWebApplicationTests(instance, webApplication);
				}
				else
				{
					instance.Result.Complete();
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return null;
		}

		private WebApplication GetWebApplication(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			CasTransactionOutcome casTransactionOutcome = base.CreateLogonOutcome(instance);
			WebApplication result = null;
			try
			{
				WebApplication webApplication = this.CreateWebApplication(instance);
				if (webApplication.ValidateLogin())
				{
					casTransactionOutcome.Update(CasTransactionResultEnum.Success, LocalizedString.Empty);
					result = webApplication;
				}
				else
				{
					casTransactionOutcome.Update(CasTransactionResultEnum.Failure, Strings.CasHealthOwaNoLogonCookieReturned);
				}
			}
			catch (AuthenticationException ex)
			{
				casTransactionOutcome.Update(CasTransactionResultEnum.Failure, Strings.CasHealthWebAppNoSession(instance.CasFqdn, ex.LocalizedString, (ex.InnerException != null) ? ex.GetBaseException().Message : ""));
			}
			catch (WebException ex2)
			{
				casTransactionOutcome.Update(CasTransactionResultEnum.Failure, Strings.CasHealthWebAppNoSession(instance.CasFqdn, ex2.Message, (ex2.InnerException != null) ? ex2.GetBaseException().Message : ""));
			}
			instance.Outcomes.Enqueue(casTransactionOutcome);
			instance.Result.Outcomes.Add(casTransactionOutcome);
			return result;
		}

		protected virtual WebApplication CreateWebApplication(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			WebSession webSession = this.GetWebSession(instance);
			return this.CreateWebApplication(instance.baseUri.AbsolutePath, webSession);
		}

		private WebSession GetWebSession(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			WebSession webSession = this.CreateWebSession(instance);
			if (instance.UrlType == VirtualDirectoryUriScope.Internal)
			{
				webSession.TrustAnySSLCertificate = true;
				base.WriteVerbose(Strings.CasHealthOwaInternalTrustCertificate);
			}
			else if (instance.trustAllCertificates)
			{
				webSession.TrustAnySSLCertificate = true;
				base.WriteVerbose(Strings.CasHealthOwaTrustAnyCertificate);
			}
			webSession.SendingRequest += delegate(object sender, HttpWebRequestEventArgs e)
			{
				LocalizedString localizedString = Strings.CasHealthWebAppSendingRequest(e.Request.RequestUri);
				instance.Outcomes.Enqueue(localizedString);
			};
			webSession.ResponseReceived += delegate(object sender, HttpWebResponseEventArgs e)
			{
				if (e.Response != null)
				{
					string responseHeader = e.Response.GetResponseHeader("X-DiagInfo");
					LocalizedString localizedString = Strings.CasHealthWebAppResponseReceived(e.Response.ResponseUri, e.Response.StatusCode, responseHeader ?? string.Empty, TestWebApplicationConnectivity.GetResponseAdditionalInformation(e.Response));
					instance.Outcomes.Enqueue(localizedString);
				}
			};
			webSession.RequestException += delegate(object sender, WebExceptionEventArgs e)
			{
				if (e.Response != null)
				{
					string responseHeader = e.Response.GetResponseHeader("X-DiagInfo");
					LocalizedString localizedString = Strings.CasHealthWebAppRequestException(e.Request.RequestUri, e.Exception.Status, responseHeader ?? string.Empty, e.Exception.Message);
					instance.Outcomes.Enqueue(localizedString);
				}
			};
			webSession.Initialize();
			return webSession;
		}

		private static string GetResponseAdditionalInformation(HttpWebResponse response)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (response.Cookies != null && response.Cookies.Count > 0)
			{
				stringBuilder.Append("Cookie=");
				foreach (object obj in response.Cookies)
				{
					Cookie cookie = (Cookie)obj;
					stringBuilder.Append(cookie.ToString());
					stringBuilder.Append("; ");
				}
			}
			if (response.StatusCode == HttpStatusCode.Found)
			{
				stringBuilder.Append("Location=");
				stringBuilder.Append(response.Headers[HttpResponseHeader.Location]);
			}
			return stringBuilder.ToString();
		}

		protected virtual WebSession CreateWebSession(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			return ExchangeWebApplication.GetWebSession(instance);
		}

		protected abstract WebApplication CreateWebApplication(string virtualDirectory, WebSession webSession);

		protected virtual void ExecuteWebApplicationTests(TestCasConnectivity.TestCasConnectivityRunInstance instance, WebApplication webApplication)
		{
			instance.Result.Complete();
		}

		protected const string DiagInfo = "X-DiagInfo";

		protected Uri liveRSTEndpoint = new Uri("https://login.live.com/RST.srf");
	}
}
