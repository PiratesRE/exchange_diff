using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CafeLoggingHelper
	{
		internal static void LogCallStatistics(CafeRoutingContext cafeRoutingContext)
		{
			CafeCallStatisticsLogger.CafeCallStatisticsLogRow cafeCallStatisticsLogRow = new CafeCallStatisticsLogger.CafeCallStatisticsLogRow();
			cafeCallStatisticsLogRow.CallStartTime = cafeRoutingContext.CallReceivedTime.UniversalTime;
			cafeCallStatisticsLogRow.CallLatency = cafeRoutingContext.CallLatency;
			cafeCallStatisticsLogRow.CallType = string.Empty;
			cafeCallStatisticsLogRow.CallIdentity = Utils.CheckString(cafeRoutingContext.CallId);
			cafeCallStatisticsLogRow.CafeServerName = Utils.GetLocalHostName();
			cafeCallStatisticsLogRow.DialPlanGuid = CafeLoggingHelper.GetDialPlanId(cafeRoutingContext);
			cafeCallStatisticsLogRow.DialPlanType = CafeLoggingHelper.GetDialPlanType(cafeRoutingContext);
			cafeCallStatisticsLogRow.CalledPhoneNumber = CafeLoggingHelper.GetCalledPhoneNumber(cafeRoutingContext);
			cafeCallStatisticsLogRow.CallerPhoneNumber = CafeLoggingHelper.GetCallerPhoneNumber(cafeRoutingContext);
			cafeCallStatisticsLogRow.OfferResult = CafeLoggingHelper.GetOfferResultDescription(cafeRoutingContext);
			cafeCallStatisticsLogRow.OrganizationId = Util.GetTenantName(cafeRoutingContext.DialPlan);
			CafeCallStatisticsLogger.Instance.Append(cafeCallStatisticsLogRow);
		}

		private static string GetCalledPhoneNumber(CafeRoutingContext cafeRoutingContext)
		{
			if (cafeRoutingContext.CalledParty != null)
			{
				return cafeRoutingContext.CalledParty.ToString();
			}
			return string.Empty;
		}

		private static string GetCallerPhoneNumber(CafeRoutingContext cafeRoutingContext)
		{
			if (cafeRoutingContext.CallingParty != null)
			{
				return cafeRoutingContext.CallingParty.ToString();
			}
			return string.Empty;
		}

		private static Guid GetDialPlanId(CafeRoutingContext cafeRoutingContext)
		{
			if (cafeRoutingContext.DialPlan != null)
			{
				return cafeRoutingContext.DialPlan.Guid;
			}
			return Guid.Empty;
		}

		private static string GetDialPlanType(CafeRoutingContext cafeRoutingContext)
		{
			if (cafeRoutingContext.DialPlan != null)
			{
				return cafeRoutingContext.DialPlan.URIType.ToString();
			}
			return string.Empty;
		}

		private static string GetOfferResultDescription(CafeRoutingContext cafeRoutingContext)
		{
			if (cafeRoutingContext.RedirectUri != null)
			{
				return "Redirect";
			}
			return "Reject";
		}
	}
}
