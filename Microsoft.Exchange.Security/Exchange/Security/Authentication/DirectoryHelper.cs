using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class DirectoryHelper
	{
		internal static ITenantRecipientSession GetTenantRecipientSessionFromSmtpOrLiveId(string smtpOrLiveId)
		{
			if (string.IsNullOrEmpty(smtpOrLiveId))
			{
				throw new ArgumentNullException("smtpOrLiveId");
			}
			if (!SmtpAddress.IsValidSmtpAddress(smtpOrLiveId))
			{
				throw new ArgumentException(string.Format("{0} is not a valid SmtpAddress.", smtpOrLiveId));
			}
			string domain = SmtpAddress.Parse(smtpOrLiveId).Domain;
			if (string.IsNullOrEmpty(domain))
			{
				throw new ArgumentException(string.Format("Given SmtpAddress {0} does not contain a valid domain", smtpOrLiveId));
			}
			return DirectoryHelper.GetTenantRecipientSessionByDomainName(null, domain, false);
		}

		internal static ADRawEntry GetADRawEntry(string puid, string organizationContext, string smtpOrLiveId, PropertyDefinition[] properties)
		{
			ITenantRecipientSession tenantRecipientSession;
			return DirectoryHelper.GetADRawEntry(puid, organizationContext, smtpOrLiveId, properties, false, out tenantRecipientSession);
		}

		internal static ADRawEntry GetADRawEntry(string puid, string organizationContext, string smtpOrLiveId, PropertyDefinition[] properties, out ITenantRecipientSession recipientSession)
		{
			recipientSession = null;
			return DirectoryHelper.GetADRawEntry(puid, organizationContext, smtpOrLiveId, properties, false, out recipientSession);
		}

		internal static ADRawEntry GetADRawEntry(string puid, string organizationContext, string smtpOrLiveId, PropertyDefinition[] properties, bool requestForestWideDomainControllerAffinityByUserId, out ITenantRecipientSession tenantRecipient)
		{
			tenantRecipient = DirectoryHelper.GetTenantRecipientSession(puid, organizationContext, smtpOrLiveId, requestForestWideDomainControllerAffinityByUserId);
			ADRawEntry result;
			try
			{
				result = tenantRecipient.FindUniqueEntryByNetID(puid, organizationContext, properties);
			}
			catch (ADTransientException innerException)
			{
				throw new BackendRehydrationException(SecurityStrings.CannotLookupUserEx(puid, smtpOrLiveId), innerException);
			}
			catch (DataValidationException innerException2)
			{
				throw new BackendRehydrationException(SecurityStrings.CannotLookupUserEx(puid, smtpOrLiveId), innerException2);
			}
			catch (DataSourceOperationException innerException3)
			{
				throw new BackendRehydrationException(SecurityStrings.CannotLookupUserEx(puid, smtpOrLiveId), innerException3);
			}
			return result;
		}

		internal static List<string> GetTokenSids(ADRawEntry id, string puid, string organizationContext, string smtpOrLiveId, bool requestForestWideDomainControllerAffinityByUserId)
		{
			ITenantRecipientSession tenantRecipientSession = DirectoryHelper.GetTenantRecipientSession(puid, organizationContext, smtpOrLiveId, requestForestWideDomainControllerAffinityByUserId);
			List<string> tokenSids;
			try
			{
				tokenSids = tenantRecipientSession.GetTokenSids(id, AssignmentMethod.S4U);
			}
			catch (ADTransientException innerException)
			{
				throw new BackendRehydrationException(SecurityStrings.CannotLookupUserEx(puid, smtpOrLiveId), innerException);
			}
			catch (DataValidationException innerException2)
			{
				throw new BackendRehydrationException(SecurityStrings.CannotLookupUserEx(puid, smtpOrLiveId), innerException2);
			}
			catch (DataSourceOperationException innerException3)
			{
				throw new BackendRehydrationException(SecurityStrings.CannotLookupUserEx(puid, smtpOrLiveId), innerException3);
			}
			return tokenSids;
		}

		internal static string GetDomainControllerWithForestWideAffinityByUserId(string userNetId, OrganizationId orgId)
		{
			ADRunspaceServerSettingsProvider instance = ADRunspaceServerSettingsProvider.GetInstance();
			try
			{
				bool flag;
				ADServerInfo gcFromToken = instance.GetGcFromToken(orgId.PartitionId.ForestFQDN, RunspaceServerSettings.GetTokenForUser(userNetId, orgId), out flag, true);
				if (gcFromToken != null)
				{
					return gcFromToken.Fqdn;
				}
			}
			catch (ADTransientException arg)
			{
				ExTraceGlobals.BackendRehydrationTracer.TraceDebug<string, ADTransientException>(0L, "[DirectoryHelper::GetDomainControllerWithForestWideAffinityByUserId] Caught ADTransientException when trying to get ADServerInfo by user NetID for {0}. Exception details: {1}.", userNetId, arg);
			}
			return null;
		}

		private static ITenantRecipientSession GetTenantRecipientSessionByDomainName(string puid, string domain, bool requestForestWideDomainControllerAffinityByUserId)
		{
			ADSessionSettings adsessionSettings;
			try
			{
				adsessionSettings = ADSessionSettings.FromTenantAcceptedDomain(domain);
			}
			catch (CannotResolveTenantNameException ex)
			{
				throw new BackendRehydrationException(SecurityStrings.CannotResolveOrganization(domain), new UnauthorizedAccessException(ex.Message));
			}
			string domainController = null;
			if (requestForestWideDomainControllerAffinityByUserId)
			{
				domainController = DirectoryHelper.GetDomainControllerWithForestWideAffinityByUserId(puid, adsessionSettings.CurrentOrganizationId);
			}
			return DirectorySessionFactory.Default.CreateTenantRecipientSession(domainController, null, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, adsessionSettings, 215, "GetTenantRecipientSessionByDomainName", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\BackendAuthenticator\\DirectoryHelper.cs");
		}

		private static ITenantRecipientSession GetTenantRecipientSession(string puid, string organizationContext, string smtpOrLiveId, bool requestForestWideDomainControllerAffinityByUserId)
		{
			ITenantRecipientSession result;
			if (!string.IsNullOrEmpty(organizationContext))
			{
				result = DirectoryHelper.GetTenantRecipientSessionByDomainName(puid, organizationContext, requestForestWideDomainControllerAffinityByUserId);
			}
			else
			{
				result = DirectoryHelper.GetTenantRecipientSessionFromSmtpOrLiveId(smtpOrLiveId);
			}
			return result;
		}
	}
}
