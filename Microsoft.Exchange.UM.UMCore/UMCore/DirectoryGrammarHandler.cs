using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class DirectoryGrammarHandler
	{
		public static DirectoryGrammarHandler CreateHandler(OrganizationId orgId)
		{
			if (!CommonConstants.UseDataCenterCallRouting)
			{
				return new StaticDirectoryGrammarHandler(orgId);
			}
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromOrganizationId(orgId, null);
			bool flag = !GrammarFileDistributionShare.SpeechGrammarsFolderExists(orgId) && !iadrecipientLookup.TenantSizeExceedsThreshold(GrammarRecipientHelper.GetUserFilter(), 10);
			if (flag)
			{
				return new DynamicDirectoryGrammarHandler(orgId);
			}
			return new StaticDirectoryGrammarHandler(orgId);
		}

		public abstract bool DeleteFileAfterUse { get; }

		public OrganizationId OrgId { get; private set; }

		protected DirectoryGrammarHandler(OrganizationId orgId)
		{
			this.OrgId = orgId;
		}

		public static OrganizationId GetOrganizationId(CallContext callContext)
		{
			ValidateArgument.NotNull(callContext, "callContext");
			OrganizationId organizationId = null;
			switch (callContext.CallType)
			{
			case 1:
				ExAssert.RetailAssert(callContext.DialPlan != null, "callContext.DialPlan is null");
				ExAssert.RetailAssert(callContext.DialPlan.OrganizationId != null, "callContext.DialPlan.OrganizationId is null");
				organizationId = callContext.DialPlan.OrganizationId;
				break;
			case 2:
				ExAssert.RetailAssert(callContext.AutoAttendantInfo != null, "callContext.AutoAttendantInfo is null");
				ExAssert.RetailAssert(callContext.AutoAttendantInfo.OrganizationId != null, "callContext.AutoAttendantInfo.OrganizationId is null");
				organizationId = callContext.AutoAttendantInfo.OrganizationId;
				break;
			case 3:
			{
				UMSubscriber callerInfo = callContext.CallerInfo;
				ADRecipient adrecipient = callerInfo.ADRecipient;
				ExAssert.RetailAssert(adrecipient != null, "subscriber.ADRecipient is null");
				ExAssert.RetailAssert(adrecipient.OrganizationId != null, "subscriber.ADRecipient.OrganizationId is null");
				organizationId = adrecipient.OrganizationId;
				break;
			}
			default:
				ExAssert.RetailAssert(false, "Unsupported call type '{0}' for directory grammar handler", new object[]
				{
					callContext.CallType
				});
				break;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "DirectoryGrammarHandler.GetOrganizationId() - orgId='{0}'", new object[]
			{
				organizationId
			});
			return organizationId;
		}

		public abstract void PrepareGrammarAsync(CallContext callContext, DirectoryGrammarHandler.GrammarType grammarType);

		public abstract void PrepareGrammarAsync(ADRecipient recipient, CultureInfo culture);

		public abstract SearchGrammarFile WaitForPrepareGrammarCompletion();

		public enum GrammarType
		{
			User,
			DL
		}
	}
}
