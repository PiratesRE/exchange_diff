using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DialingPermissionsCheckFactory
	{
		internal static DialingPermissionsCheck Create(BaseUMCallSession vo)
		{
			UMDialPlan dialPlan = vo.CurrentCallContext.DialPlan;
			DialingPermissionsCheck result;
			switch (vo.CurrentCallContext.CallType)
			{
			case 1:
				result = new DialingPermissionsCheck(dialPlan, false);
				break;
			case 2:
				result = new DialingPermissionsCheck(vo.CurrentCallContext.AutoAttendantInfo, vo.CurrentCallContext.CurrentAutoAttendantSettings, dialPlan);
				break;
			case 3:
				result = new DialingPermissionsCheck((ADUser)vo.CurrentCallContext.CallerInfo.ADRecipient, dialPlan);
				break;
			default:
				throw new ArgumentException("Invalid calltype: " + vo.CurrentCallContext.CallType.ToString());
			}
			return result;
		}
	}
}
