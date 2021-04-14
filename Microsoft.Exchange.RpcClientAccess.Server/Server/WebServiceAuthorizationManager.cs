using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Net.XropService;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class WebServiceAuthorizationManager : IAuthorizationManager
	{
		public bool CheckAccess(TokenValidationResults validationResults)
		{
			string emailAddress = validationResults.EmailAddress;
			string text = null;
			string organization = null;
			bool result;
			try
			{
				if (!SmtpAddress.IsValidSmtpAddress(emailAddress))
				{
					WebServiceEndPoint.LogFailure(string.Format("Authorization failed. Address in authentication token is not a valid SMTP address: {0}", emailAddress), null, emailAddress, "<unknown>", null, ExTraceGlobals.ConnectXropTracer);
					result = false;
				}
				else
				{
					SmtpAddress smtpAddress = new SmtpAddress(emailAddress);
					text = smtpAddress.Domain;
					OrganizationId organizationId = DomainToOrganizationIdCache.Singleton.Get(new SmtpDomain(text));
					if (organizationId == null)
					{
						WebServiceEndPoint.LogFailure(string.Format("Authorization failed. No AcceptedDomain matches domain from EmailAddress in authentication token: {0}", text), null, emailAddress, text, null, ExTraceGlobals.ConnectXropTracer);
						result = false;
					}
					else
					{
						organization = ((organizationId.OrganizationalUnit != null) ? organizationId.OrganizationalUnit.ToCanonicalName() : string.Empty);
						if (Configuration.ServiceConfiguration.EnableWebServicesOrganizationRelationshipCheck)
						{
							OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
							OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(text);
							if (organizationRelationship == null)
							{
								WebServiceEndPoint.LogFailure(string.Format("Authorization failed. No organization relationship configured for domain: {0}", text), null, emailAddress, text, organization, ExTraceGlobals.ConnectXropTracer);
								return false;
							}
							if (!organizationRelationship.Enabled)
							{
								WebServiceEndPoint.LogFailure(string.Format("Authorization failed. Organization relationship is not enabled for domain: {0}", text), null, emailAddress, text, organization, ExTraceGlobals.ConnectXropTracer);
								return false;
							}
						}
						result = true;
					}
				}
			}
			catch (Exception exception)
			{
				WebServiceEndPoint.LogFailure("Authorization failed. Unhandled exception thrown.", exception, emailAddress, text ?? "<unknown>", organization, ExTraceGlobals.ConnectXropTracer);
				throw;
			}
			return result;
		}
	}
}
