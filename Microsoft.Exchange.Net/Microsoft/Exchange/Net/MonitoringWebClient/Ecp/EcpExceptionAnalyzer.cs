using System;
using System.Collections.Generic;
using Microsoft.Exchange.Net.MonitoringWebClient.Ecp.Parsers;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Ecp
{
	internal class EcpExceptionAnalyzer : BaseExceptionAnalyzer
	{
		protected override bool AffinityCheck
		{
			get
			{
				return false;
			}
		}

		static EcpExceptionAnalyzer()
		{
			Dictionary<FailureReason, FailingComponent> dictionary = new Dictionary<FailureReason, FailingComponent>();
			dictionary.Add(FailureReason.MissingKeyword, FailingComponent.Ecp);
			dictionary.Add(FailureReason.NameResolution, FailingComponent.Networking);
			dictionary.Add(FailureReason.NetworkConnection, FailingComponent.Networking);
			dictionary.Add(FailureReason.EcpActiveDirectoryErrorResponse, FailingComponent.ActiveDirectory);
			dictionary.Add(FailureReason.EcpErrorPage, FailingComponent.Ecp);
			dictionary.Add(FailureReason.EcpMailboxErrorResponse, FailingComponent.Mailbox);
			dictionary.Add(FailureReason.OwaActiveDirectoryErrorPage, FailingComponent.ActiveDirectory);
			dictionary.Add(FailureReason.OwaErrorPage, FailingComponent.Owa);
			dictionary.Add(FailureReason.OwaMailboxErrorPage, FailingComponent.Mailbox);
			dictionary.Add(FailureReason.RequestTimeout, FailingComponent.EcpDependency);
			dictionary.Add(FailureReason.ConnectionTimeout, FailingComponent.Ecp);
			dictionary.Add(FailureReason.UnexpectedHttpResponseCode, FailingComponent.Ecp);
			dictionary.Add(FailureReason.SslNegotiation, FailingComponent.Networking);
			dictionary.Add(FailureReason.BrokenAffinity, FailingComponent.Networking);
			dictionary.Add(FailureReason.CafeFailure, FailingComponent.Ecp);
			dictionary.Add(FailureReason.CafeActiveDirectoryFailure, FailingComponent.ActiveDirectory);
			dictionary.Add(FailureReason.CafeHighAvailabilityFailure, FailingComponent.Mailbox);
			dictionary.Add(FailureReason.CafeToMailboxNetworkingFailure, FailingComponent.Networking);
			dictionary.Add(FailureReason.CafeTimeoutContactingBackend, FailingComponent.EcpDependency);
			EcpExceptionAnalyzer.FailureMatrix.Add(RequestTarget.Ecp, dictionary);
			dictionary = new Dictionary<FailureReason, FailingComponent>();
			dictionary.Add(FailureReason.MissingKeyword, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.NameResolution, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.NetworkConnection, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.RequestTimeout, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.ConnectionTimeout, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.UnexpectedHttpResponseCode, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.SslNegotiation, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.BadUserNameOrPassword, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.AccountLocked, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.LogonError, FailingComponent.LiveIdConsumer);
			EcpExceptionAnalyzer.FailureMatrix.Add(RequestTarget.LiveIdConsumer, dictionary);
			dictionary = new Dictionary<FailureReason, FailingComponent>();
			dictionary.Add(FailureReason.MissingKeyword, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.NameResolution, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.NetworkConnection, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.RequestTimeout, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.ConnectionTimeout, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.UnexpectedHttpResponseCode, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.SslNegotiation, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.BadUserNameOrPassword, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.AccountLocked, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.LogonError, FailingComponent.LiveIdBusiness);
			EcpExceptionAnalyzer.FailureMatrix.Add(RequestTarget.LiveIdBusiness, dictionary);
		}

		public EcpExceptionAnalyzer(Dictionary<string, RequestTarget> hostNameSourceMapping) : base(hostNameSourceMapping)
		{
		}

		public override HttpWebResponseWrapperException VerifyResponse(HttpWebRequestWrapper request, HttpWebResponseWrapper response, CafeErrorPageValidationRules cafeErrorPageValidationRules)
		{
			EcpErrorResponse ecpErrorResponse;
			if (EcpErrorResponse.TryParse(response, out ecpErrorResponse))
			{
				return new EcpErrorResponseException(MonitoringWebClientStrings.EcpErrorPage, request, response, ecpErrorResponse);
			}
			OwaErrorPage owaErrorPage;
			if (OwaErrorPage.TryParse(response, out owaErrorPage))
			{
				return new OwaErrorPageException(MonitoringWebClientStrings.OwaErrorPage, request, response, owaErrorPage);
			}
			HttpWebResponseWrapperException ex = base.VerifyResponse(request, response, cafeErrorPageValidationRules);
			if (ex != null)
			{
				return ex;
			}
			return null;
		}

		protected override FailureReason GetFailureReason(Exception exception, HttpWebRequestWrapper request, HttpWebResponseWrapper response)
		{
			if (exception is UnexpectedStatusCodeException)
			{
				EcpErrorResponse ecpErrorResponse;
				if (EcpErrorResponse.TryParse(response, out ecpErrorResponse))
				{
					return ecpErrorResponse.FailureReason;
				}
			}
			else
			{
				if (exception is EcpErrorResponseException)
				{
					return (exception as EcpErrorResponseException).EcpErrorResponse.FailureReason;
				}
				if (exception is OwaErrorPageException)
				{
					return (exception as OwaErrorPageException).OwaErrorPage.FailureReason;
				}
			}
			return base.GetFailureReason(exception, request, response);
		}

		protected override Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>> ComponentMatrix
		{
			get
			{
				return EcpExceptionAnalyzer.FailureMatrix;
			}
		}

		protected override FailingComponent DefaultComponent
		{
			get
			{
				return FailingComponent.Ecp;
			}
		}

		private static readonly Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>> FailureMatrix = new Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>>();
	}
}
