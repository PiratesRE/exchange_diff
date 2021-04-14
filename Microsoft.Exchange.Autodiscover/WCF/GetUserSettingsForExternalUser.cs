using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class GetUserSettingsForExternalUser : GetUserSettingsCommandBase
	{
		internal GetUserSettingsForExternalUser(ExternalIdentity callerExternalIdentity, CallContext callContext) : base(callContext)
		{
			this.callerExternalIdentity = callerExternalIdentity;
		}

		protected override IStandardBudget AcquireBudget()
		{
			return StandardBudget.AcquireFallback(this.callerExternalIdentity.EmailAddress.ToString(), BudgetType.Ews);
		}

		protected override void AddToQueryList(UserResultMapping userResultMapping, IBudget budget)
		{
			FaultInjection.GenerateFault((FaultInjection.LIDs)2745576765U);
			OrganizationId organizationId;
			if (base.TryGetOrganizationId(userResultMapping, out organizationId))
			{
				base.AddToADQueryList(userResultMapping, organizationId, null, budget);
				return;
			}
			this.AddToMServeQueryList(userResultMapping);
		}

		private void AddToMServeQueryList(UserResultMapping userResultMapping)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "AddToMServeQueryList() called for '{0}'.", userResultMapping.Mailbox);
			if (this.mServeDomainQueryList == null)
			{
				this.mServeDomainQueryList = new MServeDomainQueryList();
				this.queryLists.Add(this.mServeDomainQueryList);
			}
			this.mServeDomainQueryList.Add(userResultMapping);
		}

		protected override bool IsPostAdQueryAuthorized(UserResultMapping userResultMapping)
		{
			MServeQueryResult mserveQueryResult = userResultMapping.Result as MServeQueryResult;
			if (mserveQueryResult != null)
			{
				if (mserveQueryResult.RedirectServer == null)
				{
					ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "MServe provided NO redirect for '{0}'", userResultMapping.Mailbox);
					return false;
				}
				ExTraceGlobals.FrameworkTracer.TraceDebug<string, string>((long)this.GetHashCode(), "MServe provided redirect for '{0}' to {1}.", userResultMapping.Mailbox, mserveQueryResult.RedirectServer);
				return true;
			}
			else
			{
				ADQueryResult adqueryResult = userResultMapping.Result as ADQueryResult;
				if (adqueryResult == null)
				{
					return false;
				}
				if (adqueryResult.Result.Data == null)
				{
					return false;
				}
				if (this.HasOrganizationRelationship(adqueryResult.Result.Data.OrganizationId))
				{
					ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "Organization relationship for '{0}'.", userResultMapping.Mailbox);
					return true;
				}
				if (this.HasPersonalRelationship(adqueryResult.Result.Data as ADUser))
				{
					ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "Personal relationship for '{0}'.", userResultMapping.Mailbox);
					return true;
				}
				ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "No organization relationship for '{0}'.", userResultMapping.Mailbox);
				return false;
			}
		}

		private bool HasOrganizationRelationship(OrganizationId organizationId)
		{
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(this.callerExternalIdentity.EmailAddress.Domain);
			return organizationRelationship != null && organizationRelationship.Enabled && organizationRelationship.DomainNames.Contains(new SmtpDomain(this.callerExternalIdentity.EmailAddress.Domain));
		}

		private bool HasPersonalRelationship(ADUser adUser)
		{
			if (adUser == null)
			{
				return false;
			}
			SharingPartnerIdentityCollection sharingPartnerIdentities = adUser.SharingPartnerIdentities;
			return sharingPartnerIdentities != null && sharingPartnerIdentities.Contains(this.callerExternalIdentity.ExternalId.ToString());
		}

		private ExternalIdentity callerExternalIdentity;

		private MServeDomainQueryList mServeDomainQueryList;
	}
}
