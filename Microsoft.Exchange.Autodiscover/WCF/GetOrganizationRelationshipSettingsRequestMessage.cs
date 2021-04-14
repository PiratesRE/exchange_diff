using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[MessageContract]
	public class GetOrganizationRelationshipSettingsRequestMessage : AutodiscoverRequestMessage
	{
		[MessageBodyMember(Name = "Request", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
		public GetOrganizationRelationshipSettingsRequest Request { get; set; }

		internal override AutodiscoverResponseMessage Execute()
		{
			GetOrganizationRelationshipSettingsResponseMessage getOrganizationRelationshipSettingsResponseMessage = new GetOrganizationRelationshipSettingsResponseMessage();
			GetOrganizationRelationshipSettingsResponse response = getOrganizationRelationshipSettingsResponseMessage.Response;
			if (this.Request == null || this.Request.Domains == null)
			{
				response.ErrorCode = ErrorCode.InvalidRequest;
				response.ErrorMessage = Strings.InvalidRequest;
			}
			else
			{
				OrganizationIdCacheValue organizationIdCachedValueFromDomains = this.GetOrganizationIdCachedValueFromDomains();
				if (organizationIdCachedValueFromDomains == null)
				{
					response.ErrorCode = ErrorCode.InvalidRequest;
					response.ErrorMessage = Strings.InvalidRequest;
				}
				else
				{
					List<SmtpAddress> emailAddressesInClaimSets = this.GetEmailAddressesInClaimSets();
					if (emailAddressesInClaimSets == null)
					{
						response.ErrorCode = ErrorCode.InvalidRequest;
						response.ErrorMessage = Strings.InvalidRequest;
					}
					else
					{
						ICollection<OrganizationRelationshipSettings> organizationRelationships = this.GetOrganizationRelationships(organizationIdCachedValueFromDomains, emailAddressesInClaimSets);
						if (organizationRelationships == null)
						{
							response.ErrorCode = ErrorCode.InvalidRequest;
							response.ErrorMessage = Strings.InvalidRequest;
						}
						else
						{
							response.ErrorCode = ErrorCode.NoError;
							response.OrganizationRelationships = new OrganizationRelationshipSettingsCollection(organizationRelationships);
						}
					}
				}
			}
			if (getOrganizationRelationshipSettingsResponseMessage.Response.ErrorCode == ErrorCode.InvalidRequest)
			{
				this.Set401Status();
				getOrganizationRelationshipSettingsResponseMessage = null;
			}
			return getOrganizationRelationshipSettingsResponseMessage;
		}

		private void Set401Status()
		{
			OperationContext operationContext = OperationContext.Current;
			HttpResponseMessageProperty httpResponseMessageProperty = null;
			object obj = null;
			if (operationContext.OutgoingMessageProperties.TryGetValue(HttpResponseMessageProperty.Name, out obj))
			{
				httpResponseMessageProperty = (HttpResponseMessageProperty)obj;
			}
			if (obj == null)
			{
				httpResponseMessageProperty = new HttpResponseMessageProperty();
				operationContext.OutgoingMessageProperties.Add(HttpResponseMessageProperty.Name, httpResponseMessageProperty);
			}
			httpResponseMessageProperty.StatusCode = HttpStatusCode.Unauthorized;
			httpResponseMessageProperty.SuppressEntityBody = true;
		}

		private OrganizationIdCacheValue GetOrganizationIdCachedValueFromDomains()
		{
			OrganizationId organizationId = null;
			OrganizationIdCacheValue result = null;
			bool flag = false;
			foreach (string text in this.Request.Domains)
			{
				if (!SmtpAddress.IsValidDomain(text))
				{
					ExTraceGlobals.FrameworkTracer.TraceDebug<string>(0L, "GetOrganizationRelationshipSettingsRequestMessage.GetOrganizationIdCachedValueFromDomains() returning null because of an invalid smtp domain in the request: {0}.", text);
					flag = true;
					break;
				}
				OrganizationId organizationId2 = DomainToOrganizationIdCache.Singleton.Get(new SmtpDomain(text));
				if (!(organizationId2 == null))
				{
					if (organizationId == null)
					{
						organizationId = organizationId2;
					}
					else
					{
						string x = string.Empty;
						if (organizationId.OrganizationalUnit != null)
						{
							x = (organizationId.OrganizationalUnit.DistinguishedName ?? string.Empty);
						}
						string y = string.Empty;
						if (organizationId2.OrganizationalUnit != null)
						{
							y = (organizationId2.OrganizationalUnit.DistinguishedName ?? string.Empty);
						}
						if (!StringComparer.OrdinalIgnoreCase.Equals(x, y))
						{
							ExTraceGlobals.FrameworkTracer.TraceDebug<string>(0L, "GetOrganizationRelationshipSettingsRequestMessage.GetOrganizationIdCachedValueFromDomains() returning null because domain: {0} resolves to multiple organizations.", text);
							flag = true;
							break;
						}
					}
				}
			}
			if (organizationId == null)
			{
				return null;
			}
			if (!flag)
			{
				result = OrganizationIdCache.Singleton.Get(organizationId);
			}
			return result;
		}

		private List<SmtpAddress> GetEmailAddressesInClaimSets()
		{
			ReadOnlyCollection<ClaimSet> claimSets = ServiceSecurityContext.Current.AuthorizationContext.ClaimSets;
			List<SmtpAddress> list = new List<SmtpAddress>();
			foreach (ClaimSet claimSet in claimSets)
			{
				foreach (Claim claim in claimSet.FindClaims("http://schemas.xmlsoap.org/claims/EmailAddress", Rights.PossessProperty))
				{
					string text = claim.Resource as string;
					if (string.IsNullOrEmpty(text) || !SmtpAddress.IsValidSmtpAddress(text))
					{
						list = null;
						ExTraceGlobals.FrameworkTracer.TraceDebug<string>(0L, "GetOrganizationRelationshipSettingsRequestMessage.GetEmailAddressesInClaimSets() The claim processing was stopped because of one invalid Smtp Address in the claim set: {0}.", text ?? string.Empty);
						break;
					}
					list.Add(new SmtpAddress(text));
				}
			}
			return list;
		}

		private ICollection<OrganizationRelationshipSettings> GetOrganizationRelationships(OrganizationIdCacheValue organizationIdCacheValue, List<SmtpAddress> addressList)
		{
			Dictionary<string, OrganizationRelationshipSettings> dictionary = new Dictionary<string, OrganizationRelationshipSettings>();
			foreach (SmtpAddress smtpAddress in addressList)
			{
				OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(smtpAddress.Domain);
				if (organizationRelationship == null)
				{
					ExTraceGlobals.FrameworkTracer.TraceDebug<string>(0L, "GetOrganizationRelationshipSettingsRequestMessage.GetOrganizationRelationships() domain: {0} does not match any organization relationship.", smtpAddress.Domain);
				}
				else if (!organizationRelationship.Enabled)
				{
					ExTraceGlobals.FrameworkTracer.TraceDebug<string, OrganizationId>(0L, "GetOrganizationRelationshipSettingsRequestMessage.GetOrganizationRelationships() organization relationship for domain: {0} with id {1} is disabled.", smtpAddress.Domain, organizationIdCacheValue.OrganizationId);
				}
				else if (!dictionary.ContainsKey(organizationRelationship.DistinguishedName))
				{
					dictionary.Add(organizationRelationship.DistinguishedName, new OrganizationRelationshipSettings(organizationRelationship));
				}
			}
			if (dictionary.Count == 0)
			{
				return null;
			}
			return dictionary.Values;
		}
	}
}
