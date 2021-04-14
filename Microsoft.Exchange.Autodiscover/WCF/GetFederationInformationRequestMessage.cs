using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[MessageContract]
	public class GetFederationInformationRequestMessage : AutodiscoverRequestMessage
	{
		[MessageBodyMember(Name = "Request", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
		public GetFederationInformationRequest Request { get; set; }

		internal override AutodiscoverResponseMessage Execute()
		{
			GetFederationInformationResponseMessage getFederationInformationResponseMessage = new GetFederationInformationResponseMessage();
			GetFederationInformationResponse response = getFederationInformationResponseMessage.Response;
			if (this.Request == null || this.Request.Domain == null || !SmtpAddress.IsValidDomain(this.Request.Domain))
			{
				response.ErrorCode = ErrorCode.InvalidRequest;
				response.ErrorMessage = Strings.InvalidRequest;
			}
			else
			{
				ExternalAuthentication current = ExternalAuthentication.GetCurrent();
				if (!current.Enabled)
				{
					response.ErrorCode = ErrorCode.NotFederated;
					response.ErrorMessage = Strings.NotFederated;
				}
				else
				{
					IEnumerable<string> enumerable = null;
					OrganizationId organizationId = DomainToOrganizationIdCache.Singleton.Get(new SmtpDomain(this.Request.Domain));
					if (organizationId != null)
					{
						OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
						enumerable = organizationIdCacheValue.FederatedDomains;
					}
					else
					{
						try
						{
							string text = MserveDomainCache.Singleton.Get(this.Request.Domain);
							if (!string.IsNullOrEmpty(text))
							{
								AutodiscoverAuthorizationManager.BuildRedirectUrlAndRedirectCaller(OperationContext.Current, text);
								return null;
							}
						}
						catch (OverBudgetException arg)
						{
							ExTraceGlobals.FrameworkTracer.TraceError<OverBudgetException>(0L, "GetFederationInformationRequestMessage.Execute() returning ServerBusy for exception: {0}.", arg);
							response.ErrorCode = ErrorCode.ServerBusy;
							response.ErrorMessage = Strings.ServerBusy;
							return getFederationInformationResponseMessage;
						}
					}
					if (enumerable == null)
					{
						response.ErrorCode = ErrorCode.InvalidDomain;
						response.ErrorMessage = Strings.InvalidDomain;
					}
					else
					{
						List<TokenIssuer> list = new List<TokenIssuer>(2);
						SecurityTokenService securityTokenService = current.GetSecurityTokenService(organizationId);
						if (securityTokenService != null)
						{
							list.Add(new TokenIssuer(securityTokenService.TokenIssuerUri, securityTokenService.TokenIssuerEndpoint));
						}
						response.ErrorCode = ErrorCode.NoError;
						response.ApplicationUri = current.ApplicationUri;
						response.Domains = new DomainCollection(enumerable);
						response.TokenIssuers = new TokenIssuerCollection(list);
					}
				}
			}
			return getFederationInformationResponseMessage;
		}
	}
}
