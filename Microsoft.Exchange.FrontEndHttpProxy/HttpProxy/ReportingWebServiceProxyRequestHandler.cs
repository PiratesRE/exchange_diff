using System;
using System.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.HttpProxy
{
	internal class ReportingWebServiceProxyRequestHandler : RwsPswsProxyRequestHandlerBase<EcpService>
	{
		protected override string ServiceName
		{
			get
			{
				return "Reporting Web Service";
			}
		}

		public static bool IsReportingWebServicePartnerRequest(HttpRequest request)
		{
			return !string.IsNullOrEmpty(request.Url.LocalPath) && request.Url.LocalPath.IndexOf("ReportingWebService/partner/", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		protected override void DoProtocolSpecificBeginProcess()
		{
			base.DoProtocolSpecificBeginProcess();
			if (!ReportingWebServiceProxyRequestHandler.IsReportingWebServicePartnerRequest(base.HttpContext.Request))
			{
				string domain;
				if (base.TryGetTenantDomain("DelegatedOrg", out domain))
				{
					base.IsDomainBasedRequest = true;
					base.Domain = domain;
				}
				return;
			}
			string domain2;
			if (base.TryGetTenantDomain("tenantDomain", out domain2))
			{
				base.IsDomainBasedRequest = true;
				base.Domain = domain2;
				return;
			}
			ExTraceGlobals.VerboseTracer.TraceError<int>((long)this.GetHashCode(), "[ReportingWebServiceProxyRequestHandler::DoProtocolSpecificBeginProcess]: Context {0}; TenantDomain parameter isn't specified in the request URL.", base.TraceContext);
			throw new HttpException(400, "TenantDomain parameter isn't specified in the request URL.");
		}

		private const string ReportingWebServicePartnerPathName = "ReportingWebService/partner/";

		private const string TenantParameterName = "tenantDomain";
	}
}
