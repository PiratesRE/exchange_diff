using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.HttpProxy
{
	internal abstract class RwsPswsProxyRequestHandlerBase<ServiceType> : BEServerCookieProxyRequestHandler<ServiceType> where ServiceType : HttpService
	{
		protected abstract string ServiceName { get; }

		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.Internal;
			}
		}

		protected override Uri GetTargetBackEndServerUrl()
		{
			Uri targetBackEndServerUrl = base.GetTargetBackEndServerUrl();
			if (base.AnchoredRoutingTarget.BackEndServer.Version < Server.E15MinVersion)
			{
				string arg = Utilities.FormatServerVersion(base.AnchoredRoutingTarget.BackEndServer.Version);
				ExTraceGlobals.VerboseTracer.TraceError<string, AnchoredRoutingTarget, string>((long)this.GetHashCode(), "[RwsPswsProxyRequestHandlerBase::GetTargetBackEndServerUrl]: Backend server doesn't support {0}. Backend server version: {1}; AnchoredRoutingTarget: {2}", arg, base.AnchoredRoutingTarget, this.ServiceName);
				string message = string.Format("The target site (version {0}) doesn't support {1}.", arg, this.ServiceName);
				throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.EndpointNotFound, message);
			}
			return targetBackEndServerUrl;
		}

		protected bool TryGetTenantDomain(string parameterName, out string tenantDomain)
		{
			tenantDomain = base.HttpContext.Request.QueryString[parameterName];
			if (string.IsNullOrEmpty(tenantDomain))
			{
				return false;
			}
			if (!SmtpAddress.IsValidDomain(tenantDomain))
			{
				ExTraceGlobals.VerboseTracer.TraceError<int, string, string>((long)this.GetHashCode(), "[RwsPswsProxyRequestHandlerBase::TryGetTenantDomain]: Context {0}; TenantDomain parameter is invalid. ParameterName: {1}; Value: {2}.", base.TraceContext, parameterName, tenantDomain);
				string message = string.Format("{0} parameter is invalid.", parameterName);
				throw new HttpException(400, message);
			}
			return true;
		}
	}
}
