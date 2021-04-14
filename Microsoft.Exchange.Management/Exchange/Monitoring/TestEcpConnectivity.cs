using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Net.Mime;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.WebApplicationClient;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "EcpConnectivity", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class TestEcpConnectivity : TestWebApplicationConnectivity
	{
		public TestEcpConnectivity() : base(Strings.CasHealthEcpLongName, Strings.CasHealthEcpShortName, TransientErrorCache.EcpInternalTransientCache, "MSExchange Monitoring ECPConnectivity Internal", "MSExchange Monitoring ECPConnectivity External")
		{
		}

		internal override TransientErrorCache GetTransientErrorCache()
		{
			if (!base.MonitoringContext)
			{
				return null;
			}
			if (base.TestType != OwaConnectivityTestType.Internal)
			{
				return TransientErrorCache.EcpExternalTransientCache;
			}
			return TransientErrorCache.EcpInternalTransientCache;
		}

		protected override uint GetDefaultTimeOut()
		{
			if (base.TestType != OwaConnectivityTestType.Internal)
			{
				return 120U;
			}
			return 90U;
		}

		protected override IEnumerable<ExchangeVirtualDirectory> GetVirtualDirectories(ADObjectId serverId, QueryFilter filter)
		{
			return base.GetVirtualDirectories<ADEcpVirtualDirectory>(serverId, filter);
		}

		protected override WebApplication CreateWebApplication(string virtualDirectory, WebSession webSession)
		{
			return new ExchangeControlPanelApplication(virtualDirectory, webSession);
		}

		protected override void ExecuteWebApplicationTests(TestCasConnectivity.TestCasConnectivityRunInstance instance, WebApplication webApplication)
		{
			this.ExecuteWebServiceTest(instance, webApplication);
			base.ExecuteWebApplicationTests(instance, webApplication);
		}

		private void ExecuteWebServiceTest(TestCasConnectivity.TestCasConnectivityRunInstance instance, WebApplication webApplication)
		{
			StringBody stringBody = new StringBody("{\"filter\":{\"SearchText\":\"\"},\"sort\":{\"Direction\":0,\"PropertyName\":\"Name\"}}");
			stringBody.ContentType = new System.Net.Mime.ContentType("application/json");
			string text = "RulesEditor/InboxRules.svc/GetList";
			string relativeUrl = "default.aspx";
			CasTransactionOutcome casTransactionOutcome = this.BuildOutcome(Strings.CasHealthEcpScenarioTestWebService, Strings.CasHealthEcpTestWebService(new Uri(webApplication.BaseUri, text).ToString()), "ECP Web Sevice Logon Latency", instance);
			bool flag = false;
			string additionalInformation = "";
			try
			{
				TextResponse textResponse = webApplication.Get<TextResponse>(relativeUrl);
				flag = (textResponse.StatusCode == HttpStatusCode.OK);
				additionalInformation = (flag ? "" : Strings.CasHealthEcpServiceRequestResult(textResponse.StatusCode.ToString()));
				base.WriteVerbose(Strings.CasHealthEcpServiceResponse(textResponse.Text));
				if (flag)
				{
					textResponse = webApplication.Post<TextResponse>(text, stringBody);
					flag = (textResponse.StatusCode == HttpStatusCode.OK);
					additionalInformation = (flag ? "" : Strings.CasHealthEcpServiceRequestResult(textResponse.StatusCode.ToString()));
					base.WriteVerbose(Strings.CasHealthEcpServiceResponse(textResponse.Text));
				}
			}
			catch (WebException ex)
			{
				string casServer = string.Empty;
				string fullResponse = string.Empty;
				if (ex.Response != null)
				{
					casServer = ex.Response.Headers["X-DiagInfo"];
					try
					{
						fullResponse = this.GetResponseHtml(ex.Response);
					}
					catch (Exception ex2)
					{
						if (!(ex2 is ProtocolViolationException) && !(ex2 is IOException) && !(ex2 is NotSupportedException))
						{
							throw;
						}
					}
				}
				additionalInformation = Strings.CasHealthEcpServiceRequestException(ex.Message, casServer, fullResponse);
			}
			casTransactionOutcome.Update(flag ? CasTransactionResultEnum.Success : CasTransactionResultEnum.Failure, additionalInformation);
			instance.Outcomes.Enqueue(casTransactionOutcome);
			instance.Result.Outcomes.Add(casTransactionOutcome);
		}

		protected override WebSession CreateWebSession(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			ExchangeWebAppVirtualDirectory exchangeWebAppVirtualDirectory = (ExchangeWebAppVirtualDirectory)instance.VirtualDirectory;
			if (exchangeWebAppVirtualDirectory.LiveIdAuthentication && instance.UrlType == VirtualDirectoryUriScope.Internal)
			{
				return new WindowsLiveIdWebSession(TestCasConnectivity.GetUrlWithTrailingSlash(exchangeWebAppVirtualDirectory.ExternalUrl), instance.baseUri, instance.credentials, instance.LiveIdAuthenticationConfiguration);
			}
			return base.CreateWebSession(instance);
		}

		private string GetResponseHtml(WebResponse response)
		{
			string result;
			using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
			{
				result = streamReader.ReadToEnd();
			}
			return result;
		}

		private const string monitoringEventSourceInternal = "MSExchange Monitoring ECPConnectivity Internal";

		private const string monitoringEventSourceExternal = "MSExchange Monitoring ECPConnectivity External";

		private const string monitoringServicePerfCounter = "ECP Web Sevice Logon Latency";

		private const uint ExternalTimeOut = 120U;

		private const uint InternalTimeOut = 90U;
	}
}
