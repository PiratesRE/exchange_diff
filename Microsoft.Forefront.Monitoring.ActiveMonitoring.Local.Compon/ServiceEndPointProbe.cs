using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal class ServiceEndPointProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = string.Format("ServiceEndPointProbe started at {0}.{1}", DateTime.UtcNow, Environment.NewLine);
			string regionTag = ServiceEndPointProbe.GetRegionTag();
			if (regionTag == null)
			{
				string message = "Unable to retrieve region tag from registry";
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ServiceEndPointProbe.cs", 58);
				throw new FfoLAMProbeException(message);
			}
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.serviceEndPointName = attributeHelper.GetString("ServiceEndPointName", false, null);
			if (this.serviceEndPointName == null)
			{
				base.Result.StateAttribute1 = string.Format("Region: {0} / ServiceEndPointName not defined", regionTag);
				throw new ArgumentNullException(string.Format("The extension attribute: '{0}' was not specified in the probe definition. Check the probe definition to ensure it is defined correctly.", "ServiceEndPointName"));
			}
			base.Result.StateAttribute1 = string.Format("Region: {0} / ServiceEndPointName: {1}", regionTag, this.serviceEndPointName);
			ServiceEndpoint serviceEndpoint = null;
			try
			{
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "In ServiceEndPointProbe, fetching configuration session", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ServiceEndPointProbe.cs", 87);
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 88, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ServiceEndPointProbe.cs");
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Retrieving service end point container", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ServiceEndPointProbe.cs", 92);
				ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Retrieving service end point within the container", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ServiceEndPointProbe.cs", 95);
				serviceEndpoint = endpointContainer.GetEndpoint(this.serviceEndPointName);
			}
			catch (ServiceEndpointNotFoundException ex)
			{
				base.Result.StateAttribute6 = 0.0;
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("ServiceEndPointProbe was not found: {0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ServiceEndPointProbe.cs", 101);
				throw;
			}
			if (serviceEndpoint == null)
			{
				string text = "The service end point template retrieved was null";
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ServiceEndPointProbe.cs", 108);
				throw new ServiceEndpointNotFoundException(text);
			}
			base.Result.StateAttribute6 = 1.0;
			StringBuilder stringBuilder = new StringBuilder();
			if (serviceEndpoint.Uri != null)
			{
				string text2 = string.Format("Uri was NOT null when it should have been: {0}", serviceEndpoint.Uri);
				base.Result.StateAttribute11 = text2;
				stringBuilder.AppendLine(text2);
			}
			else
			{
				base.Result.StateAttribute11 = "ServiceEndpoint Uri was null as expected";
			}
			if (string.IsNullOrEmpty(serviceEndpoint.UriTemplate))
			{
				string text3 = "UriTemplate was null or empty.";
				base.Result.StateAttribute12 = text3;
				stringBuilder.AppendLine(text3);
			}
			else
			{
				base.Result.StateAttribute12 = string.Format("UriTemplate: {0}", serviceEndpoint.UriTemplate);
			}
			if (string.IsNullOrEmpty(serviceEndpoint.CertificateSubject))
			{
				string text4 = "CertificateSubject was null or empty";
				base.Result.StateAttribute13 = text4;
				stringBuilder.AppendLine(text4);
			}
			if (string.IsNullOrEmpty(serviceEndpoint.Token))
			{
				string text5 = "Token was null or empty";
				base.Result.StateAttribute14 = text5;
				stringBuilder.AppendLine(text5);
			}
			else
			{
				base.Result.StateAttribute14 = string.Format("Token: {0}", serviceEndpoint.Token);
			}
			if (stringBuilder.Length > 0)
			{
				string arg = stringBuilder.ToString();
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("ServiceEndPointProbe failures: {0}", arg), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ServiceEndPointProbe.cs", 167);
				throw new FfoLAMProbeException(stringBuilder.ToString());
			}
			try
			{
				ServiceEndpoint serviceEndpoint2 = serviceEndpoint.ApplyTemplate(new object[]
				{
					regionTag
				});
				string text6 = ServiceEndPointProbe.CreateReleaseMessageURL(serviceEndpoint2.Uri.ToString());
				string text7 = ServiceEndPointProbe.CreateReportMessageURL(serviceEndpoint2.Uri.ToString());
				base.Result.StateAttribute15 = text6;
				base.Result.StateAttribute21 = text7;
				HttpStatusCode httpWebRequestStatusCode = ServiceEndPointProbe.GetHttpWebRequestStatusCode(text6, serviceEndpoint2.CertificateSubject);
				HttpStatusCode httpWebRequestStatusCode2 = ServiceEndPointProbe.GetHttpWebRequestStatusCode(text7, serviceEndpoint2.CertificateSubject);
				base.Result.StateAttribute22 = httpWebRequestStatusCode.ToString();
				base.Result.StateAttribute23 = httpWebRequestStatusCode2.ToString();
				StringBuilder stringBuilder2 = new StringBuilder();
				if (httpWebRequestStatusCode != HttpStatusCode.OK)
				{
					stringBuilder2.AppendLine(string.Format("Release message with URL: {0} returned HTTP status code: {1}", text6, httpWebRequestStatusCode));
				}
				if (httpWebRequestStatusCode2 != HttpStatusCode.OK)
				{
					stringBuilder2.AppendLine(string.Format("Report message with URL: {0} returned HTTP status code: {1}", text7, httpWebRequestStatusCode2));
				}
				if (stringBuilder2.Length > 0)
				{
					string message2 = stringBuilder2.ToString();
					WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, message2, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ServiceEndPointProbe.cs", 202);
					throw new WebException(message2);
				}
			}
			catch (WebException ex2)
			{
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("WebException when making web request: {0}", ex2.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\ServiceEndPointProbe.cs", 208);
				base.Result.StateAttribute24 = ex2.Message;
				throw;
			}
		}

		private static string GetRegionTag()
		{
			string result;
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default).OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs"))
			{
				string text = registryKey.GetValue("RegionTag", null) as string;
				if (text != null)
				{
					result = text;
				}
				else
				{
					string str = registryKey.GetValue("Region", null) as string;
					string str2 = registryKey.GetValue("RegionServiceInstance", null) as string;
					result = str + str2;
				}
			}
			return result;
		}

		private static string CreateReleaseMessageURL(string uri)
		{
			if (string.IsNullOrEmpty(uri))
			{
				throw new ArgumentNullException("The URI parameter value was null or empty");
			}
			return string.Format("{0}/releasespam/orgs/{1}/users/0/mail/0?token=0", uri, ServiceEndPointProbe.SafeTenantId);
		}

		private static string CreateReportMessageURL(string uri)
		{
			if (string.IsNullOrEmpty(uri))
			{
				throw new ArgumentNullException("The URI parameter value was null or empty");
			}
			return string.Format("{0}/reportfalsepositive/orgs/{1}/users/0/mail/0?token=0", uri, ServiceEndPointProbe.SafeTenantId);
		}

		private static HttpWebResponse MakeHttpWebRequest(string requestUri, string clientCertificateSubject)
		{
			X509Certificate2 value = TlsCertificateInfo.FindFirstCertWithSubjectDistinguishedName(clientCertificateSubject);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			httpWebRequest.Method = "GET";
			httpWebRequest.AuthenticationLevel = AuthenticationLevel.MutualAuthRequired;
			httpWebRequest.ClientCertificates.Add(value);
			httpWebRequest.ServerCertificateValidationCallback = ((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => sslPolicyErrors == SslPolicyErrors.None);
			httpWebRequest.ClientCertificates.Add(value);
			return (HttpWebResponse)httpWebRequest.GetResponse();
		}

		private static HttpStatusCode GetHttpWebRequestStatusCode(string requestUri, string clientCertificateSubject)
		{
			HttpStatusCode statusCode;
			using (HttpWebResponse httpWebResponse = ServiceEndPointProbe.MakeHttpWebRequest(requestUri, clientCertificateSubject))
			{
				statusCode = httpWebResponse.StatusCode;
			}
			return statusCode;
		}

		private static readonly Guid SafeTenantId = new Guid("5afe0b00-7697-4969-b663-5eab37d5f47e");

		private string serviceEndPointName;

		internal static class AttributeNames
		{
			internal const string ServiceEndPointName = "ServiceEndPointName";
		}
	}
}
