using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class CafeErrorPage
	{
		public FailureReason FailureReason { get; private set; }

		public RequestFailureContext RequestFailureContext { get; private set; }

		private CafeErrorPage()
		{
		}

		public static bool TryParse(HttpWebResponseWrapper response, CafeErrorPageValidationRules cafeErrorPageValidationRules, out CafeErrorPage errorPage)
		{
			RequestFailureContext requestFailureContext;
			if (!RequestFailureContext.TryCreateFromResponseHeaders(response.Headers, out requestFailureContext))
			{
				errorPage = null;
				return false;
			}
			if ((cafeErrorPageValidationRules & CafeErrorPageValidationRules.Accept401Response) == CafeErrorPageValidationRules.Accept401Response && response.StatusCode == HttpStatusCode.Unauthorized)
			{
				errorPage = null;
				return false;
			}
			OwaErrorPage owaErrorPage;
			if (OwaErrorPage.TryParse(response, out owaErrorPage))
			{
				errorPage = null;
				return false;
			}
			FailureReason failureReason = FailureReason.CafeFailure;
			if (requestFailureContext.HttpProxySubErrorCode != null)
			{
				HttpProxySubErrorCode value = requestFailureContext.HttpProxySubErrorCode.Value;
				if (value <= HttpProxySubErrorCode.BackEndRequestTimedOut)
				{
					switch (value)
					{
					case HttpProxySubErrorCode.DirectoryOperationError:
					case HttpProxySubErrorCode.MServOperationError:
					case HttpProxySubErrorCode.ServerDiscoveryError:
						break;
					case HttpProxySubErrorCode.ServerLocatorError:
						goto IL_B7;
					default:
						if (value != HttpProxySubErrorCode.BackEndRequestTimedOut)
						{
							goto IL_10C;
						}
						failureReason = FailureReason.CafeTimeoutContactingBackend;
						goto IL_10C;
					}
				}
				else
				{
					switch (value)
					{
					case HttpProxySubErrorCode.DatabaseNameNotFound:
					case HttpProxySubErrorCode.DatabaseGuidNotFound:
					case HttpProxySubErrorCode.OrganizationMailboxNotFound:
						goto IL_B7;
					default:
						if (value != HttpProxySubErrorCode.BadSamlToken)
						{
							goto IL_10C;
						}
						break;
					}
				}
				failureReason = FailureReason.CafeActiveDirectoryFailure;
				goto IL_10C;
				IL_B7:
				failureReason = FailureReason.CafeHighAvailabilityFailure;
			}
			else if (requestFailureContext.WebExceptionStatus != null)
			{
				if (BaseExceptionAnalyzer.IsNetworkRelatedError(requestFailureContext.WebExceptionStatus.Value))
				{
					failureReason = FailureReason.CafeToMailboxNetworkingFailure;
				}
			}
			else if (requestFailureContext.Error != null && requestFailureContext.Error.IndexOf("NegotiateSecurityContext", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				failureReason = FailureReason.CafeActiveDirectoryFailure;
			}
			IL_10C:
			errorPage = new CafeErrorPage();
			errorPage.FailureReason = failureReason;
			errorPage.RequestFailureContext = requestFailureContext;
			return true;
		}

		public override string ToString()
		{
			return string.Format("ErrorPageFailureReason: {0}, RequestFailureContext: {1}", this.FailureReason, this.RequestFailureContext);
		}
	}
}
