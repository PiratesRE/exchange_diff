using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class LocalForestOtherServerOutboundAuthenticator : IPhotoRequestOutboundAuthenticator
	{
		public LocalForestOtherServerOutboundAuthenticator(string certificateValidationComponentId, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("certificateValidationComponentId", certificateValidationComponentId);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.certificateValidationComponentId = certificateValidationComponentId;
			this.tracer = upstreamTracer;
		}

		public HttpWebResponse AuthenticateAndGetResponse(HttpWebRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			NetworkServiceImpersonator.Initialize();
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "LOCAL FOREST OTHER SERVER OUTBOUND AUTHENTICATOR: stamping component id '{0}' onto request.", this.certificateValidationComponentId);
			CertificateValidationManager.SetComponentId(request, this.certificateValidationComponentId);
			request.PreAuthenticate = true;
			return HttpAuthenticator.NetworkService.AuthenticateAndExecute<HttpWebResponse>(request, () => (HttpWebResponse)request.GetResponse());
		}

		private readonly string certificateValidationComponentId;

		private readonly ITracer tracer;
	}
}
