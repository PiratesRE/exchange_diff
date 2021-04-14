using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Psws.Probes
{
	public class PswsBackEndProbe : PswsProbeBase
	{
		protected override string ComponentId
		{
			get
			{
				return "PswsBackEndProbe";
			}
		}

		public PswsBackEndProbe()
		{
			this.tokenInitialized = false;
		}

		protected override void DoInitialize()
		{
			base.DoInitialize();
			if (!this.tokenInitialized)
			{
				CertificateValidationManager.RegisterCallback(this.ComponentId, (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true);
				this.CaculateCommonAccessToken();
				this.tokenInitialized = true;
			}
		}

		protected override HttpWebRequest GetRequest()
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Entering GetRequest in PswsBackEndProbe", null, "GetRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsBackEndProbe.cs", 98);
			HttpWebRequest request = base.GetRequest();
			request.ContentType = "application/soap+xml;charset=UTF-8";
			request.KeepAlive = true;
			request.ServicePoint.Expect100Continue = false;
			request.UnsafeAuthenticatedConnectionSharing = true;
			CertificateValidationManager.SetComponentId(request, this.ComponentId);
			request.Headers.Add("X-CommonAccessToken", this.token.Serialize());
			if (this.tokenType == AccessTokenType.LiveIdBasic)
			{
				request.Headers.Add("X-WLID-MemberName", this.token.ExtensionData["MemberName"]);
			}
			request.Credentials = CredentialCache.DefaultNetworkCredentials.GetCredential(request.RequestUri, "Kerberos");
			WTFDiagnostics.TraceFunction(ExTraceGlobals.PswsTracer, base.TraceContext, "Leaving GetRequest in PswsBackEndProbe", null, "GetRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PSWS\\PswsBackEndProbe.cs", 121);
			return request;
		}

		private void CaculateCommonAccessToken()
		{
			if (!Enum.TryParse<AccessTokenType>(base.Definition.Attributes["AccessTokenType"], out this.tokenType))
			{
				throw new ApplicationException(this.probeInfo + "Create PswsBackEndProbe without correct 'AccessTokenType'!");
			}
			AccessTokenType accessTokenType = this.tokenType;
			if (accessTokenType == AccessTokenType.LiveIdBasic)
			{
				this.token = CommonAccessTokenHelper.CreateLiveIdBasic(base.Definition.Account);
				return;
			}
			if (accessTokenType != AccessTokenType.CertificateSid)
			{
				throw new ApplicationException(this.probeInfo + "Unhandled AccessTokenType for PswsBackEndProbe : " + this.tokenType.ToString());
			}
			this.token = CommonAccessTokenHelper.CreateCertificateSid(base.Definition.Account);
		}

		public const string AccessTokenTypeParameterName = "AccessTokenType";

		private AccessTokenType tokenType;

		private CommonAccessToken token;

		private bool tokenInitialized;
	}
}
