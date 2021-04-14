using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;

namespace Microsoft.Exchange.SoapWebClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class GetFederationInformationClient
	{
		public static IEnumerable<GetFederationInformationResult> Discover(AutodiscoverClient client, string domain)
		{
			GetFederationInformationRequest request = new GetFederationInformationRequest
			{
				Domain = domain
			};
			InvokeDelegate invokeDelegate = (DefaultBinding_Autodiscover binding) => binding.GetFederationInformation(request);
			IEnumerable<AutodiscoverResultData> enumerable = client.InvokeWithDiscovery(invokeDelegate, domain);
			List<GetFederationInformationResult> list = new List<GetFederationInformationResult>(4);
			foreach (AutodiscoverResultData result in enumerable)
			{
				list.Add(GetFederationInformationClient.GetResult(domain, result));
			}
			return list;
		}

		public static GetFederationInformationResult Endpoint(AutodiscoverClient client, Uri endpoint, string domain)
		{
			GetFederationInformationRequest request = new GetFederationInformationRequest
			{
				Domain = domain
			};
			InvokeDelegate invokeDelegate = (DefaultBinding_Autodiscover binding) => binding.GetFederationInformation(request);
			AutodiscoverResultData result = client.InvokeWithEndpoint(invokeDelegate, endpoint);
			return GetFederationInformationClient.GetResult(domain, result);
		}

		private static GetFederationInformationResult GetResult(string domain, AutodiscoverResultData result)
		{
			if (result.Type == AutodiscoverResult.UnsecuredRedirect || result.Type == AutodiscoverResult.InvalidSslHostname)
			{
				if (result.Alternate == null)
				{
					return new GetFederationInformationResult
					{
						Type = AutodiscoverResult.Failure,
						Url = result.Url,
						Details = result.ToString(),
						Exception = new GetFederationInformationException(NetException.DiscoveryFailed(domain), result.Exception)
					};
				}
				if (result.Alternate.Type != AutodiscoverResult.Success)
				{
					return new GetFederationInformationResult
					{
						Type = AutodiscoverResult.Failure,
						Url = result.Url,
						Details = result.ToString(),
						Exception = new GetFederationInformationException(NetException.DiscoveryFailed(domain), result.Exception)
					};
				}
				return new GetFederationInformationResult
				{
					Type = result.Type,
					Url = result.Url,
					Details = result.ToString(),
					SslCertificateHostnames = result.SslCertificateHostnames,
					RedirectUrl = result.RedirectUrl,
					Alternate = GetFederationInformationClient.GetResult(domain, result.Alternate)
				};
			}
			else
			{
				if (result.Type != AutodiscoverResult.Success)
				{
					return new GetFederationInformationResult
					{
						Type = AutodiscoverResult.Failure,
						Url = result.Url,
						Details = result.ToString(),
						Exception = new GetFederationInformationException(NetException.DiscoveryFailed(domain), result.Exception)
					};
				}
				if (!result.Response.ErrorCodeSpecified)
				{
					GetFederationInformationClient.Tracer.TraceError(0L, "Response not valid because it contains no ErrorCode element");
					return new GetFederationInformationResult
					{
						Type = AutodiscoverResult.Failure,
						Url = result.Url,
						Details = result.ToString(),
						Exception = new GetFederationInformationException(NetException.ResponseMissingErrorCode)
					};
				}
				if (result.Response.ErrorCode == ErrorCode.InvalidDomain || result.Response.ErrorCode == ErrorCode.NotFederated)
				{
					GetFederationInformationClient.Tracer.TraceError<ErrorCode>(0L, "Response indicates the domain is not federated: {0}", result.Response.ErrorCode);
					return new GetFederationInformationResult
					{
						Type = AutodiscoverResult.Failure,
						Url = result.Url,
						Details = result.ToString(),
						Exception = new GetFederationInformationException(NetException.NotFederated)
					};
				}
				if (result.Response.ErrorCode != ErrorCode.NoError)
				{
					GetFederationInformationClient.Tracer.TraceError<ErrorCode>(0L, "Response has error {0}.", result.Response.ErrorCode);
					return new GetFederationInformationResult
					{
						Type = AutodiscoverResult.Failure,
						Details = result.ToString(),
						Url = result.Url,
						Exception = new GetFederationInformationException(NetException.ErrorInResponse(result.Response.ErrorCode.ToString()))
					};
				}
				GetFederationInformationResponse getFederationInformationResponse = result.Response as GetFederationInformationResponse;
				if (getFederationInformationResponse == null)
				{
					GetFederationInformationClient.Tracer.TraceError<string>(0L, "Received response is not of expected type: {0}", result.Response.GetType().Name);
					return new GetFederationInformationResult
					{
						Type = AutodiscoverResult.Failure,
						Url = result.Url,
						Details = result.ToString(),
						Exception = new GetFederationInformationException(NetException.UnexpectedResponseType(result.Response.GetType().Name))
					};
				}
				if (getFederationInformationResponse.ApplicationUri == null)
				{
					GetFederationInformationClient.Tracer.TraceError(0L, "Response has no ApplicationUri");
					return new GetFederationInformationResult
					{
						Type = AutodiscoverResult.Failure,
						Url = result.Url,
						Details = result.ToString(),
						Exception = new GetFederationInformationException(NetException.ApplicationUriMissing)
					};
				}
				if (!Uri.IsWellFormedUriString(getFederationInformationResponse.ApplicationUri, UriKind.RelativeOrAbsolute))
				{
					GetFederationInformationClient.Tracer.TraceError<string>(0L, "ApplicationUri value is malformed: {0}", getFederationInformationResponse.ApplicationUri);
					return new GetFederationInformationResult
					{
						Type = AutodiscoverResult.Failure,
						Url = result.Url,
						Details = result.ToString(),
						Exception = new GetFederationInformationException(NetException.ApplicationUriMalformed(getFederationInformationResponse.ApplicationUri))
					};
				}
				if (getFederationInformationResponse.Domains == null || getFederationInformationResponse.Domains.Length == 0)
				{
					GetFederationInformationClient.Tracer.TraceError(0L, "Response has no domains");
					return new GetFederationInformationResult
					{
						Type = AutodiscoverResult.Failure,
						Url = result.Url,
						Details = result.ToString(),
						Exception = new GetFederationInformationException(NetException.DomainsMissing)
					};
				}
				bool flag = Array.Exists<string>(getFederationInformationResponse.Domains, (string responseDomain) => StringComparer.OrdinalIgnoreCase.Equals(responseDomain, domain));
				ArrayTracer<string> arrayTracer = new ArrayTracer<string>(getFederationInformationResponse.Domains);
				if (!flag)
				{
					GetFederationInformationClient.Tracer.TraceError<ArrayTracer<string>>(0L, "Requested domain is not present in Domains element: {0}", arrayTracer);
					return new GetFederationInformationResult
					{
						Type = AutodiscoverResult.Failure,
						Url = result.Url,
						Details = result.ToString(),
						Exception = new GetFederationInformationException(NetException.DomainNotPresentInResponse(domain, arrayTracer.ToString()))
					};
				}
				GetFederationInformationClient.Tracer.TraceDebug<string, ArrayTracer<string>>(0L, "Response has ApplicationUri={0} and Domains={1}", getFederationInformationResponse.ApplicationUri, arrayTracer);
				string[] tokenIssuerUris = null;
				if (getFederationInformationResponse.TokenIssuers != null && getFederationInformationResponse.TokenIssuers.Length > 0)
				{
					List<string> list = new List<string>(getFederationInformationResponse.TokenIssuers.Length);
					foreach (TokenIssuer tokenIssuer in getFederationInformationResponse.TokenIssuers)
					{
						if (!string.IsNullOrEmpty(tokenIssuer.Uri))
						{
							list.Add(tokenIssuer.Uri);
						}
					}
					if (list.Count > 0)
					{
						tokenIssuerUris = list.ToArray();
					}
				}
				return new GetFederationInformationResult
				{
					Type = AutodiscoverResult.Success,
					Url = result.Url,
					Details = result.ToString(),
					ApplicationUri = getFederationInformationResponse.ApplicationUri,
					TokenIssuerUris = tokenIssuerUris,
					Domains = new StringList(getFederationInformationResponse.Domains)
				};
			}
		}

		private static Trace Tracer = ExTraceGlobals.EwsClientTracer;
	}
}
