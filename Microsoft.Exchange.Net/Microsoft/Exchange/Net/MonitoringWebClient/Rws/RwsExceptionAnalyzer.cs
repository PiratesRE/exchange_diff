using System;
using System.Collections.Generic;
using Microsoft.Exchange.Net.MonitoringWebClient.Rws.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Rws
{
	internal class RwsExceptionAnalyzer : BaseExceptionAnalyzer
	{
		static RwsExceptionAnalyzer()
		{
			Dictionary<FailureReason, FailingComponent> dictionary = new Dictionary<FailureReason, FailingComponent>();
			dictionary.Add(FailureReason.NameResolution, FailingComponent.Networking);
			dictionary.Add(FailureReason.RwsActiveDirectoryErrorResponse, FailingComponent.ActiveDirectory);
			dictionary.Add(FailureReason.NetworkConnection, FailingComponent.Rws);
			dictionary.Add(FailureReason.RequestTimeout, FailingComponent.Rws);
			dictionary.Add(FailureReason.RwsError, FailingComponent.Rws);
			dictionary.Add(FailureReason.UnexpectedHttpResponseCode, FailingComponent.Rws);
			dictionary.Add(FailureReason.RwsDataMartErrorResponse, FailingComponent.DataMart);
			RwsExceptionAnalyzer.FailureMatrix.Add(RequestTarget.Rws, dictionary);
		}

		public RwsExceptionAnalyzer(Dictionary<string, RequestTarget> hostNameSourceMapping) : base(hostNameSourceMapping)
		{
		}

		public override HttpWebResponseWrapperException VerifyResponse(HttpWebRequestWrapper request, HttpWebResponseWrapper response, CafeErrorPageValidationRules cafeErrorPageValidationRules)
		{
			RwsErrorResponse rwsErrorResponse;
			if (RwsErrorResponse.TryParse(response, out rwsErrorResponse))
			{
				return new RwsErrorResponseException("The response contained an RWS error", request, response, rwsErrorResponse);
			}
			return null;
		}

		protected override FailureReason GetFailureReason(Exception exception, HttpWebRequestWrapper request, HttpWebResponseWrapper response)
		{
			if (exception is UnexpectedStatusCodeException)
			{
				RwsErrorResponse rwsErrorResponse;
				if (RwsErrorResponse.TryParse(response, out rwsErrorResponse))
				{
					return rwsErrorResponse.FailureReason;
				}
			}
			else if (exception is RwsErrorResponseException)
			{
				return (exception as RwsErrorResponseException).RwsErrorResponse.FailureReason;
			}
			return base.GetFailureReason(exception, request, response);
		}

		protected override Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>> ComponentMatrix
		{
			get
			{
				return RwsExceptionAnalyzer.FailureMatrix;
			}
		}

		protected override FailingComponent DefaultComponent
		{
			get
			{
				return FailingComponent.Rws;
			}
		}

		private static readonly Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>> FailureMatrix = new Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>>();
	}
}
