using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class GetUserSettingsForPartner : GetUserSettingsCommandBase
	{
		internal GetUserSettingsForPartner(SecurityIdentifier callerSid, CallContext callContext) : base(callContext)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<SecurityIdentifier>((long)this.GetHashCode(), "GetUserSettingsForPartner constructor called for '{0}'.", callerSid);
			this.callerSid = callerSid;
		}

		protected override IStandardBudget AcquireBudget()
		{
			return StandardBudget.Acquire(this.callerSid, BudgetType.Ews, Common.GetSessionSettingsForCallerScope());
		}

		protected override void AddToQueryList(UserResultMapping userResultMapping, IBudget budget)
		{
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
			if (this.mServeQueryList == null)
			{
				this.mServeQueryList = new MServeQueryList();
				this.queryLists.Add(this.mServeQueryList);
			}
			this.mServeQueryList.Add(userResultMapping);
		}

		private SecurityIdentifier callerSid;

		private MServeQueryList mServeQueryList;
	}
}
