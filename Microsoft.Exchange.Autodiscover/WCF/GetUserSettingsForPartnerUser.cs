using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.Security.PartnerToken;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class GetUserSettingsForPartnerUser : GetUserSettingsCommandBase
	{
		internal GetUserSettingsForPartnerUser(IOrganizationScopedIdentity callerIdentity, CallContext callContext) : base(callContext)
		{
			this.partnerUser = callerIdentity;
		}

		protected override IStandardBudget AcquireBudget()
		{
			return this.partnerUser.AcquireBudget();
		}

		protected override void AddToQueryList(UserResultMapping userResultMapping, IBudget budget)
		{
			OrganizationId organizationId;
			if (!base.TryGetOrganizationId(userResultMapping, out organizationId))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "Mailbox '{0}' org Id was not found, possibly in other forest.", userResultMapping.Mailbox);
				base.SetInvalidSmtpAddressResult(userResultMapping);
				return;
			}
			if (organizationId.Equals(this.partnerUser.OrganizationId))
			{
				base.AddToADQueryList(userResultMapping, this.partnerUser.OrganizationId, null, budget);
				return;
			}
			ExTraceGlobals.FrameworkTracer.TraceDebug<string, OrganizationId, string>((long)this.GetHashCode(), "Mailbox '{0}' has different org id {1}. The identity is {1}.", userResultMapping.Mailbox, organizationId, this.partnerUser.Name);
			base.SetInvalidSmtpAddressResult(userResultMapping);
		}

		protected override bool IsPostAdQueryAuthorized(UserResultMapping userResultMapping)
		{
			ADQueryResult adqueryResult = userResultMapping.Result as ADQueryResult;
			if (adqueryResult == null)
			{
				return false;
			}
			if (adqueryResult.Result.Data == null)
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "ADQuery result is null for '{0}'.", userResultMapping.Mailbox);
				return false;
			}
			OrganizationId organizationId = adqueryResult.Result.Data.OrganizationId;
			if (this.partnerUser.OrganizationId.Equals(organizationId))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "ADQuery result for '{0}' matches the organization id.", userResultMapping.Mailbox);
				return true;
			}
			ExTraceGlobals.FrameworkTracer.TraceDebug<string, OrganizationId, OrganizationId>((long)this.GetHashCode(), "ADQuery result for '{0}' is {1}, does not match the organization id {2}.", userResultMapping.Mailbox, organizationId, this.partnerUser.OrganizationId);
			return false;
		}

		private readonly IOrganizationScopedIdentity partnerUser;
	}
}
