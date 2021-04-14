using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Oab.Probes
{
	public abstract class OabBaseLocalProbe : ProbeWorkItem
	{
		public HttpWebRequestUtility WebRequestUtil { get; set; }

		protected HttpWebRequest GetRequest()
		{
			this.WebRequestUtil = new HttpWebRequestUtility(base.TraceContext);
			HttpWebRequest httpWebRequest = this.WebRequestUtil.CreateBasicHttpWebRequest(base.Definition.Endpoint, false);
			httpWebRequest.ContentType = "text/xml";
			httpWebRequest.Method = "GET";
			httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
			bool flag = false;
			if (base.Definition.Attributes.ContainsKey("TrustAnySslCertificate") && bool.TryParse(base.Definition.Attributes["TrustAnySslCertificate"], out flag) && flag)
			{
				string componentId = "OAB_AM_Probe";
				RemoteCertificateValidationCallback callback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
				CertificateValidationManager.SetComponentId(httpWebRequest, componentId);
				CertificateValidationManager.RegisterCallback(componentId, callback);
			}
			return httpWebRequest;
		}

		public const string TrustAnySslCertificateParameterName = "TrustAnySslCertificate";
	}
}
