using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DialPermissionWrapperFactory
	{
		internal static DialPermissionWrapper Create(BaseUMCallSession vo)
		{
			DialPermissionWrapper dialPermissionWrapper;
			switch (vo.CurrentCallContext.CallType)
			{
			case 1:
				dialPermissionWrapper = DialPermissionWrapper.CreateFromDialPlan(vo.CurrentCallContext.DialPlan);
				break;
			case 2:
				dialPermissionWrapper = DialPermissionWrapper.CreateFromAutoAttendant(vo.CurrentCallContext.AutoAttendantInfo);
				break;
			case 3:
			{
				UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
				ADUser user = callerInfo.ADRecipient as ADUser;
				dialPermissionWrapper = DialPermissionWrapper.CreateFromRecipientPolicy(user);
				break;
			}
			default:
				CallIdTracer.TraceError(ExTraceGlobals.OutdialingTracer, null, "Unexpected vo.CurrentCallContext.CallType value: {0}.", new object[]
				{
					vo.CurrentCallContext.CallType
				});
				throw new UnexpectedSwitchValueException(vo.CurrentCallContext.CallType.ToString());
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "DialPermissionWrapper::Create() Originating Object = '{0}', DialPlanSubscribersAllowed = {1} Extensions = {2}", new object[]
			{
				dialPermissionWrapper.Identity,
				dialPermissionWrapper.DialPlanSubscribersAllowed,
				dialPermissionWrapper.ExtensionLengthNumbersAllowed
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "DialPermissionWrapper::Create() Originating Object = '{0}', CallSomeone = {1} SendMessage = {2}", new object[]
			{
				dialPermissionWrapper.Identity,
				dialPermissionWrapper.CallSomeoneEnabled,
				dialPermissionWrapper.SendVoiceMessageEnabled
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "DialPermissionWrapper::Create() Originating Object = '{0}', ContactScope = {1} SearchRoot = {2}", new object[]
			{
				dialPermissionWrapper.Identity,
				dialPermissionWrapper.ContactScope,
				dialPermissionWrapper.SearchRoot
			});
			DialPermissionWrapperFactory.LogAllowedGroups("AllowedInCountryGroups", dialPermissionWrapper.Identity, dialPermissionWrapper.AllowedInCountryGroups);
			DialPermissionWrapperFactory.LogAllowedGroups("AllowedInternationalGroups", dialPermissionWrapper.Identity, dialPermissionWrapper.AllowedInternationalGroups);
			return dialPermissionWrapper;
		}

		internal static DialPermissionWrapper CreateFromDialPlan(UMDialPlan dialPlan)
		{
			return DialPermissionWrapper.CreateFromDialPlan(dialPlan);
		}

		internal static DialPermissionWrapper CreateFromAutoAttendant(UMAutoAttendant autoAttendant)
		{
			return DialPermissionWrapper.CreateFromAutoAttendant(autoAttendant);
		}

		internal static DialPermissionWrapper CreateFromRecipientPolicy(ADUser user)
		{
			return DialPermissionWrapper.CreateFromRecipientPolicy(user);
		}

		private static void LogAllowedGroups(string description, string originatingObjectIdentity, MultiValuedProperty<string> groups)
		{
			if (!ExTraceGlobals.OutdialingTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			if (groups.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder(16 * groups.Count);
				for (int i = 0; i < groups.Count; i++)
				{
					stringBuilder.Append(groups[i]);
					stringBuilder.Append(",");
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "DialPermissionWrapper::Create() Originating Object = '{0}', {1} = '{2}'", new object[]
				{
					originatingObjectIdentity,
					description,
					stringBuilder.ToString()
				});
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "DialPermissionWrapper::Create() Originating Object = '{0}', {1} = <null>", new object[]
			{
				originatingObjectIdentity,
				description
			});
		}
	}
}
