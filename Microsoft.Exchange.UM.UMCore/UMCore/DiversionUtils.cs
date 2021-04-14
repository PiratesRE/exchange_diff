using System;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DiversionUtils
	{
		public static bool TryAnalyzeDiversionForRoutingPurposes(CafeRoutingContext context, out UMAutoAttendant aa, out UMRecipient user)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "DiversionUtils: TryAnalyzeDiversionForRoutingPurposes", new object[0]);
			int num = 0;
			aa = null;
			user = null;
			StringBuilder stringBuilder = new StringBuilder(1);
			foreach (PlatformDiversionInfo platformDiversionInfo in context.CallInfo.DiversionInfo)
			{
				if (platformDiversionInfo.DiversionSource != DiversionSource.SipInfo)
				{
					stringBuilder.Append(platformDiversionInfo.OriginalCalledParty).Append(" ");
					SetDiversionInfoResult diversionInfo = DiversionUtils.GetDiversionInfo(platformDiversionInfo, context.DialPlan, context.CallInfo.CallId, context.CallingParty, context.ScopedADConfigurationSession, out aa, out user);
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "AnalyzeDiversionForRoutingPurposes returned:{0} - Iteration:{1}", new object[]
					{
						diversionInfo,
						num
					});
					if (diversionInfo == SetDiversionInfoResult.ObjectFound || diversionInfo == SetDiversionInfoResult.UserCallingItself)
					{
						break;
					}
					if (SetDiversionInfoResult.ObjectNotFound == diversionInfo)
					{
						if (++num == 6)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "AnalyzeDiversionForRoutingPurposes: MaxNumOfDiversionLookups reached.", new object[0]);
							break;
						}
					}
					else
					{
						if (diversionInfo == SetDiversionInfoResult.Invalid)
						{
							throw CallRejectedException.Create(Strings.InvalidDiversionReceived(platformDiversionInfo.DiversionHeader), CallEndingReason.InvalidDiversionFormat, "Diversion number: {0}.", new object[]
							{
								platformDiversionInfo.OriginalCalledParty
							});
						}
						throw new NotSupportedException(diversionInfo.ToString());
					}
				}
			}
			return aa != null || (user != null && user.ADRecipient.RecipientType == RecipientType.UserMailbox);
		}

		public static SetDiversionInfoResult GetInitialDiversionInfo(PlatformDiversionInfo diversionInfo, UMDialPlan dp, string callId, PhoneNumber callerId, out string rawExtnNumber, out string numberToCompare)
		{
			rawExtnNumber = diversionInfo.OriginalCalledParty;
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, rawExtnNumber);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, data, "DiversionUtils: GetInitialDiversionInfo: Processing diversion information:_PhoneNumber - DiversionHeader:{0}", new object[]
			{
				diversionInfo.DiversionHeader
			});
			SetDiversionInfoResult setDiversionInfoResult = SetDiversionInfoResult.Invalid;
			numberToCompare = null;
			switch (dp.URIType)
			{
			case UMUriType.TelExtn:
				if (Constants.RegularExpressions.ValidNumberRegex.IsMatch(rawExtnNumber) && rawExtnNumber.Length >= dp.NumberOfDigitsInExtension)
				{
					numberToCompare = rawExtnNumber;
					rawExtnNumber = rawExtnNumber.Substring(rawExtnNumber.Length - dp.NumberOfDigitsInExtension);
					setDiversionInfoResult = SetDiversionInfoResult.ObjectNotFound;
				}
				break;
			case UMUriType.E164:
				rawExtnNumber = Utils.NormalizeNumber(rawExtnNumber, UMUriType.E164);
				if (Utils.IsUriValid(rawExtnNumber, UMUriType.E164))
				{
					setDiversionInfoResult = SetDiversionInfoResult.ObjectNotFound;
					numberToCompare = rawExtnNumber;
				}
				break;
			case UMUriType.SipName:
				rawExtnNumber = diversionInfo.UserAtHost;
				if (Utils.IsUriValid(rawExtnNumber, UMUriType.SipName))
				{
					setDiversionInfoResult = SetDiversionInfoResult.ObjectNotFound;
					numberToCompare = rawExtnNumber;
				}
				break;
			}
			if (setDiversionInfoResult == SetDiversionInfoResult.Invalid)
			{
				CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, null, "Invalid Extension received in call {0}.", new object[]
				{
					diversionInfo.DiversionHeader
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_InvalidExtensionInCall, null, new object[]
				{
					CommonUtil.ToEventLogString(diversionInfo.DiversionHeader),
					dp.Name,
					callId
				});
			}
			return setDiversionInfoResult;
		}

		private static SetDiversionInfoResult GetDiversionInfo(PlatformDiversionInfo diversion, UMDialPlan dp, string callId, PhoneNumber callerId, IADSystemConfigurationLookup adSession, out UMAutoAttendant aa, out UMRecipient recipient)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "DiversionUtils: GetDiversionInfo", new object[0]);
			aa = null;
			recipient = null;
			string b = null;
			string text = null;
			SetDiversionInfoResult setDiversionInfoResult = DiversionUtils.GetInitialDiversionInfo(diversion, dp, callId, callerId, out text, out b);
			if (setDiversionInfoResult != SetDiversionInfoResult.Invalid)
			{
				if (!string.Equals(callerId.ToDial, b, StringComparison.OrdinalIgnoreCase))
				{
					UMAutoAttendant umautoAttendant = AutoAttendantUtils.LookupAutoAttendantInDialPlan(text, true, dp.Id, adSession);
					if (AutoAttendantUtils.IsAutoAttendantUsable(umautoAttendant, text))
					{
						aa = umautoAttendant;
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "Found valid AA.", new object[0]);
						return SetDiversionInfoResult.ObjectFound;
					}
					try
					{
						PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, text);
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, data, "AA not found, try to find a user with _PhoneNumber", new object[0]);
						recipient = UMRecipient.Factory.FromExtension<UMRecipient>(text, dp, null);
						return (recipient != null) ? SetDiversionInfoResult.ObjectFound : SetDiversionInfoResult.ObjectNotFound;
					}
					catch (LocalizedException ex)
					{
						CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, null, "Failed to find the callee : {0}.", new object[]
						{
							ex
						});
						return setDiversionInfoResult;
					}
				}
				setDiversionInfoResult = SetDiversionInfoResult.UserCallingItself;
				PIIMessage piimessage = PIIMessage.Create(PIIType._Caller, callerId.ToDial);
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, piimessage, "Bypassing diversion because callerId and diversion are same. _Caller. .", new object[0]);
				if (dp.URIType == UMUriType.TelExtn && text.Length != callerId.ToDial.Length)
				{
					PIIMessage[] data2 = new PIIMessage[]
					{
						piimessage,
						PIIMessage.Create(PIIType._PhoneNumber, text)
					};
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, data2, "Repopulating the callerId _Caller with _PhoneNumber for telex dial plan.", new object[0]);
					PhoneNumber.TryParse(text, out callerId);
				}
			}
			return setDiversionInfoResult;
		}
	}
}
