using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class OutCallingHandler
	{
		protected OutCallingHandler(BaseUMCallSession callSession, UMSipPeer outboundProxy)
		{
			if (callSession == null)
			{
				throw new InvalidArgumentException("callSession");
			}
			if (outboundProxy == null)
			{
				throw new InvalidArgumentException("outboundProxy");
			}
			this.callSession = callSession;
			this.outboundProxy = outboundProxy;
		}

		protected abstract string GetCallerName { get; }

		protected abstract UMDialPlan GetOriginatingDialplan { get; }

		protected abstract ExEventLog.EventTuple GetPOPEventTuple { get; }

		protected abstract ExEventLog.EventTuple GetPOPFailureEventTuple { get; }

		protected abstract DialingPermissionsCheck GetDialingPermissionsChecker { get; }

		internal virtual void MakeCall(string callerIdToUse, string numberToCall, IList<PlatformSignalingHeader> additionalHeaders)
		{
			if (string.IsNullOrEmpty(callerIdToUse))
			{
				throw new InvalidArgumentException("callerIdToUse");
			}
			if (string.IsNullOrEmpty(numberToCall))
			{
				throw new InvalidArgumentException("numberToCall");
			}
			PlatformSipUri platformSipUri = null;
			PlatformSipUri platformSipUri2 = null;
			IList<PlatformSignalingHeader> additionalItems = null;
			try
			{
				this.BuildSipUrisForOutboundCall(callerIdToUse, numberToCall, out platformSipUri, out platformSipUri2, out additionalItems);
			}
			catch (ArgumentException ex)
			{
				PIIMessage[] data = new PIIMessage[]
				{
					PIIMessage.Create(PIIType._PII, callerIdToUse),
					PIIMessage.Create(PIIType._PhoneNumber, numberToCall)
				};
				CallIdTracer.TraceError(ExTraceGlobals.AsrContactsTracer, null, data, "Failed to build Sip Uris for outbound call.  CallerIdtoUse _PII, NumberToCall _PhoneNumber. Error {0}", new object[]
				{
					ex
				});
				throw new InvalidSipUriException();
			}
			UmGlobals.ExEvent.LogEvent(this.GetPOPEventTuple, null, new object[]
			{
				this.GetCallerName,
				CommonUtil.ToEventLogString(platformSipUri2.ToString()),
				CommonUtil.ToEventLogString(platformSipUri.ToString()),
				CommonUtil.ToEventLogString(this.outboundProxy.Address)
			});
			IList<PlatformSignalingHeader> list = new List<PlatformSignalingHeader>();
			this.AddHeadersToList(list, additionalItems);
			this.AddHeadersToList(list, additionalHeaders);
			this.callSession.OpenAsync(platformSipUri, platformSipUri2, this.outboundProxy, list);
		}

		private PhoneNumber GetOutboundNumberToDial(string dialString)
		{
			PIIMessage data = PIIMessage.Create(PIIType._PII, dialString);
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, null, data, "GetOutboundNumberToDial(DialString=(_PII))", new object[0]);
			PhoneNumber result = null;
			OutdialingDiagnostics outdialingDiagnostics = new OutdialingDiagnostics();
			try
			{
				result = this.ValidateOutboundRules(dialString, outdialingDiagnostics);
			}
			catch (DialingRulesException)
			{
				UmGlobals.ExEvent.LogEvent(this.GetPOPFailureEventTuple, null, new object[]
				{
					CommonUtil.ToEventLogString(this.GetCallerName),
					CommonUtil.ToEventLogString(dialString),
					CommonUtil.ToEventLogString(outdialingDiagnostics.GetDetails())
				});
				throw;
			}
			return result;
		}

		private PhoneNumber ValidateOutboundRules(string dialString, OutdialingDiagnostics diagnostics)
		{
			PhoneNumber number = this.ValidateDialString(dialString, diagnostics);
			DialingPermissionsCheck getDialingPermissionsChecker = this.GetDialingPermissionsChecker;
			DialingPermissionsCheck.DialingPermissionsCheckResult dialingPermissionsCheckResult = getDialingPermissionsChecker.CheckPhoneNumber(number);
			if (!dialingPermissionsCheckResult.AllowCall)
			{
				diagnostics.AddAccessCheckFailed(dialString);
				throw new DialingRulesException();
			}
			return dialingPermissionsCheckResult.NumberToDial;
		}

		private void AddHeadersToList(IList<PlatformSignalingHeader> originalList, IList<PlatformSignalingHeader> additionalItems)
		{
			if (additionalItems != null)
			{
				foreach (PlatformSignalingHeader item in additionalItems)
				{
					originalList.Add(item);
				}
			}
		}

		private PhoneNumber ValidateDialString(string dialString, OutdialingDiagnostics diagnostics)
		{
			if (string.IsNullOrEmpty(dialString))
			{
				diagnostics.AddInvalidPlayOnPhoneNumber(string.Empty);
				throw new DialingRulesException();
			}
			UMDialPlan getOriginatingDialplan = this.GetOriginatingDialplan;
			PhoneNumber result = null;
			if (!PhoneNumber.TryParse(getOriginatingDialplan, dialString, out result))
			{
				diagnostics.AddInvalidPlayOnPhoneNumber(dialString);
				throw new DialingRulesException();
			}
			return result;
		}

		private void BuildSipUrisForOutboundCall(string callingParty, string calledParty, out PlatformSipUri toAddress, out PlatformSipUri fromAddress, out IList<PlatformSignalingHeader> headers)
		{
			toAddress = null;
			fromAddress = null;
			headers = new List<PlatformSignalingHeader>();
			UMDialPlan getOriginatingDialplan = this.GetOriginatingDialplan;
			string hostFqdn = Utils.GetHostFqdn();
			calledParty = calledParty.Trim();
			if (callingParty.StartsWith("SIP:", StringComparison.InvariantCultureIgnoreCase))
			{
				fromAddress = Platform.Builder.CreateSipUri(callingParty);
			}
			else
			{
				switch (Utils.DetermineNumberType(callingParty))
				{
				case UMUriType.TelExtn:
					fromAddress = Platform.Builder.CreateSipUri(SipUriScheme.Sip, callingParty, hostFqdn);
					fromAddress.UserParameter = UserParameter.Phone;
					break;
				case UMUriType.E164:
					fromAddress = Platform.Builder.CreateSipUri(SipUriScheme.Sip, callingParty, hostFqdn);
					fromAddress.UserParameter = UserParameter.Phone;
					break;
				case UMUriType.SipName:
					fromAddress = Platform.Builder.CreateSipUri("SIP:" + callingParty);
					break;
				}
			}
			switch (getOriginatingDialplan.URIType)
			{
			case UMUriType.TelExtn:
				switch (Utils.DetermineNumberType(calledParty))
				{
				case UMUriType.TelExtn:
				case UMUriType.E164:
				{
					PhoneNumber outboundNumberToDial = this.GetOutboundNumberToDial(calledParty);
					toAddress = Platform.Builder.CreateSipUri(SipUriScheme.Sip, outboundNumberToDial.ToDial, this.outboundProxy.Address.ToString());
					toAddress.UserParameter = UserParameter.Phone;
					break;
				}
				case UMUriType.SipName:
					throw new InvalidPhoneNumberException();
				}
				break;
			case UMUriType.E164:
				switch (Utils.DetermineNumberType(calledParty))
				{
				case UMUriType.TelExtn:
				{
					PhoneNumber outboundNumberToDial = this.GetOutboundNumberToDial(calledParty);
					toAddress = Platform.Builder.CreateSipUri(SipUriScheme.Sip, Util.FormatE164Number(outboundNumberToDial.ToDial), this.outboundProxy.Address.ToString());
					toAddress.UserParameter = UserParameter.Phone;
					break;
				}
				case UMUriType.E164:
				{
					calledParty = Utils.NormalizeNumber(calledParty, UMUriType.E164);
					if (!Utils.IsUriValid(calledParty, UMUriType.E164))
					{
						throw new InvalidPhoneNumberException();
					}
					PhoneNumber outboundNumberToDial = this.GetOutboundNumberToDial(calledParty);
					toAddress = Platform.Builder.CreateSipUri(SipUriScheme.Sip, outboundNumberToDial.ToDial, this.outboundProxy.Address.ToString());
					toAddress.UserParameter = UserParameter.Phone;
					break;
				}
				case UMUriType.SipName:
					throw new InvalidPhoneNumberException();
				}
				break;
			case UMUriType.SipName:
				headers.Add(Platform.Builder.CreateSignalingHeader("Ms-Sensitivity", "private-no-diversion"));
				headers.Add(Platform.Builder.CreateSignalingHeader("Ms-Target-Class", "secondary"));
				switch (Utils.DetermineNumberType(calledParty))
				{
				case UMUriType.TelExtn:
				{
					PhoneNumber outboundNumberToDial = this.GetOutboundNumberToDial(calledParty);
					string user = outboundNumberToDial.RenderUserPart(getOriginatingDialplan);
					toAddress = Platform.Builder.CreateSipUri(SipUriScheme.Sip, user, fromAddress.Host);
					toAddress.UserParameter = UserParameter.Phone;
					break;
				}
				case UMUriType.E164:
				{
					calledParty = Utils.NormalizeNumber(calledParty, UMUriType.E164);
					if (!Utils.IsUriValid(calledParty, UMUriType.E164))
					{
						throw new InvalidPhoneNumberException();
					}
					PhoneNumber outboundNumberToDial = this.GetOutboundNumberToDial(calledParty);
					toAddress = Platform.Builder.CreateSipUri(SipUriScheme.Sip, outboundNumberToDial.ToDial, fromAddress.Host);
					toAddress.UserParameter = UserParameter.Phone;
					break;
				}
				case UMUriType.SipName:
					calledParty = Utils.RemoveSIPPrefix(calledParty);
					if (!Utils.IsUriValid(calledParty, UMUriType.SipName))
					{
						throw new InvalidSipUriException();
					}
					toAddress = Platform.Builder.CreateSipUri("SIP:" + calledParty);
					break;
				}
				break;
			}
			if (getOriginatingDialplan.URIType == UMUriType.E164 && !string.IsNullOrEmpty(getOriginatingDialplan.DefaultOutboundCallingLineId))
			{
				headers.Add(Platform.Builder.CreateSignalingHeader("X-MSUM-Call-On-Behalf-Of", getOriginatingDialplan.DefaultOutboundCallingLineId));
			}
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._Caller, this.GetCallerName),
				PIIMessage.Create(PIIType._Callee, calledParty),
				PIIMessage.Create(PIIType._Uri, toAddress),
				PIIMessage.Create(PIIType._Uri, fromAddress)
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this.GetHashCode(), data, "BuildSipUrisForOutboundCall(_Caller, _Callee, {0}) return toAddress: _Uri, fromAddress: _Uri", new object[]
			{
				this.outboundProxy
			});
		}

		protected BaseUMCallSession callSession;

		private UMSipPeer outboundProxy;
	}
}
