using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class OutCallingHandlerForUser : OutCallingHandler
	{
		internal OutCallingHandlerForUser(UMSubscriber caller, BaseUMCallSession callSession, UMSipPeer outboundProxy, TypeOfOutboundCall type) : base(callSession, outboundProxy)
		{
			if (caller == null)
			{
				throw new InvalidArgumentException("caller");
			}
			this.callType = type;
			this.caller = caller;
		}

		protected override string GetCallerName
		{
			get
			{
				return this.caller.ADRecipient.Name;
			}
		}

		protected override UMDialPlan GetOriginatingDialplan
		{
			get
			{
				return this.caller.DialPlan;
			}
		}

		protected override ExEventLog.EventTuple GetPOPEventTuple
		{
			get
			{
				TypeOfOutboundCall typeOfOutboundCall = this.callType;
				if (typeOfOutboundCall == TypeOfOutboundCall.PlayOnPhone)
				{
					return UMEventLogConstants.Tuple_PlayOnPhoneRequest;
				}
				return UMEventLogConstants.Tuple_OutDialingRequest;
			}
		}

		protected override ExEventLog.EventTuple GetPOPFailureEventTuple
		{
			get
			{
				TypeOfOutboundCall typeOfOutboundCall = this.callType;
				if (typeOfOutboundCall == TypeOfOutboundCall.PlayOnPhone)
				{
					return UMEventLogConstants.Tuple_OutDialingRulesFailure;
				}
				return UMEventLogConstants.Tuple_FindMeOutDialingRulesFailure;
			}
		}

		protected override DialingPermissionsCheck GetDialingPermissionsChecker
		{
			get
			{
				return new DialingPermissionsCheck(this.caller.ADUser, this.GetOriginatingDialplan);
			}
		}

		internal override void MakeCall(string callerIdToUse, string numberToCall, IList<PlatformSignalingHeader> additionalHeaders)
		{
			if (this.callType == TypeOfOutboundCall.PlayOnPhone)
			{
				this.callSession.PlayOnPhoneSMTPAddress = this.caller.MailAddress;
			}
			base.MakeCall(callerIdToUse, numberToCall, additionalHeaders);
		}

		private TypeOfOutboundCall callType;

		private UMSubscriber caller;
	}
}
