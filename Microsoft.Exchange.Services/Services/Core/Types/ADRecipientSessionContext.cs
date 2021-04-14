using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ADRecipientSessionContext
	{
		public OrganizationId OrganizationId { get; private set; }

		public string OrganizationPrefix
		{
			get
			{
				if (this.OrganizationId.OrganizationalUnit != null)
				{
					return this.OrganizationId.OrganizationalUnit.Name;
				}
				return string.Empty;
			}
		}

		public bool IsRootOrganization
		{
			get
			{
				return this.OrganizationId.Equals(OrganizationId.ForestWideOrgId);
			}
		}

		private ADRecipientSessionContext(OrganizationId organizationId, ADRecipientSessionContext.GetADRecipientSessionCallback getADRecipientSession, ADRecipientSessionContext.GetGALScopedADRecipientSessionCallback getGALScopedADRecipientSession)
		{
			this.getADRecipientSession = getADRecipientSession;
			this.getGALScopedADRecipientSession = getGALScopedADRecipientSession;
			this.OrganizationId = organizationId;
		}

		public static ADRecipientSessionContext CreateForRootOrganization()
		{
			return new ADRecipientSessionContext(OrganizationId.ForestWideOrgId, new ADRecipientSessionContext.GetADRecipientSessionCallback(Directory.CreateRootADRecipientSession), (ClientSecurityContext c) => Directory.CreateGALScopedADRecipientSessionForOrganization(null, 0, OrganizationId.ForestWideOrgId, c));
		}

		public static ADRecipientSessionContext CreateForMachine()
		{
			return ADRecipientSessionContext.CreateForRootOrganization();
		}

		public static ADRecipientSessionContext CreateForOrganization(OrganizationId orgId)
		{
			return new ADRecipientSessionContext(orgId, () => Directory.CreateADRecipientSessionForOrganization(null, orgId), (ClientSecurityContext c) => Directory.CreateGALScopedADRecipientSessionForOrganization(null, 0, orgId, c));
		}

		public static ADRecipientSessionContext CreateForPartner(OrganizationId targetPartnerOrg)
		{
			return ADRecipientSessionContext.CreateForOrganization(targetPartnerOrg);
		}

		public static ADRecipientSessionContext CreateFromSidInRootOrg(SecurityIdentifier sid)
		{
			ADIdentityInformation adidentityInformation = null;
			ADRecipientSessionContext adrecipientSessionContext = ADRecipientSessionContext.CreateForRootOrganization();
			if (!ADIdentityInformationCache.Singleton.TryGet(sid, adrecipientSessionContext, out adidentityInformation))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<SecurityIdentifier>(0L, "ADRecipientSessionContext.CreateFromSid. Sid {0} was not found.", sid);
				return adrecipientSessionContext;
			}
			if (adidentityInformation is ContactIdentity && EWSSettings.IsMultiTenancyEnabled)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<SecurityIdentifier>(0L, "ADRecipientSessionContext.CreateFromSid. CreateFromSid {0} is an ADContact, which isn't supported for datacenter topologies.", sid);
				throw new ADConfigurationException();
			}
			return new ADRecipientSessionContext(adidentityInformation.OrganizationId, new ADRecipientSessionContext.GetADRecipientSessionCallback(adidentityInformation.GetADRecipientSession), new ADRecipientSessionContext.GetGALScopedADRecipientSessionCallback(adidentityInformation.GetGALScopedADRecipientSession));
		}

		public static ADRecipientSessionContext CreateFromSmtpAddress(string smtpAddress)
		{
			ADRecipientSessionContext adRecipientSessionContext;
			if (EWSSettings.IsMultiTenancyEnabled)
			{
				ADSessionSettings adsessionSettings = Directory.SessionSettingsFromAddress(smtpAddress);
				adRecipientSessionContext = ADRecipientSessionContext.CreateForOrganization(adsessionSettings.CurrentOrganizationId);
			}
			else
			{
				adRecipientSessionContext = ADRecipientSessionContext.CreateForRootOrganization();
			}
			RecipientIdentity adIdentity = null;
			if (!ADIdentityInformationCache.Singleton.TryGetRecipientIdentity(smtpAddress, adRecipientSessionContext, out adIdentity))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>(0L, "ADRecipientSessionContext.CreateFromSmtpAddress. Smtp address {0} was not found.", smtpAddress);
				throw new ADConfigurationException();
			}
			return ADRecipientSessionContext.CreateFromADIdentityInformation(adIdentity);
		}

		public static ADRecipientSessionContext CreateFromADIdentityInformation(ADIdentityInformation adIdentity)
		{
			return new ADRecipientSessionContext(adIdentity.OrganizationId, new ADRecipientSessionContext.GetADRecipientSessionCallback(adIdentity.GetADRecipientSession), new ADRecipientSessionContext.GetGALScopedADRecipientSessionCallback(adIdentity.GetGALScopedADRecipientSession));
		}

		public IRecipientSession GetADRecipientSession()
		{
			if (this.adRecipientSession == null)
			{
				ADSessionSettingsFactory.RunWithInactiveMailboxVisibilityEnablerForDatacenter(delegate
				{
					this.adRecipientSession = this.getADRecipientSession();
				});
			}
			return this.adRecipientSession;
		}

		public void ExcludeInactiveMailboxInADRecipientSession()
		{
			if (this.adRecipientSession == null)
			{
				this.adRecipientSession = this.getADRecipientSession();
				return;
			}
			this.adRecipientSession.SessionSettings.IncludeSoftDeletedObjects = false;
			this.adRecipientSession.SessionSettings.IncludeInactiveMailbox = false;
		}

		public IRecipientSession GetGALScopedADRecipientSession(ClientSecurityContext clientSecurityContext)
		{
			if (this.adRecipientSessionGALScoped == null)
			{
				this.adRecipientSessionGALScoped = this.getGALScopedADRecipientSession(clientSecurityContext);
			}
			return this.adRecipientSessionGALScoped;
		}

		private IRecipientSession adRecipientSession;

		private IRecipientSession adRecipientSessionGALScoped;

		private ADRecipientSessionContext.GetADRecipientSessionCallback getADRecipientSession;

		private ADRecipientSessionContext.GetGALScopedADRecipientSessionCallback getGALScopedADRecipientSession;

		internal delegate IRecipientSession GetADRecipientSessionCallback();

		internal delegate IRecipientSession GetGALScopedADRecipientSessionCallback(ClientSecurityContext clientSecurityContext);
	}
}
