using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class OutCallingHandlerForAA : OutCallingHandler
	{
		internal OutCallingHandlerForAA(UMAutoAttendant aa, BaseUMCallSession callSession, UMSipPeer outboundProxy) : base(callSession, outboundProxy)
		{
			if (aa == null)
			{
				throw new InvalidArgumentException("UMAutoAttendant");
			}
			this.aa = aa;
		}

		protected override string GetCallerName
		{
			get
			{
				return this.aa.Name;
			}
		}

		protected override UMDialPlan GetOriginatingDialplan
		{
			get
			{
				return this.callSession.CurrentCallContext.DialPlan;
			}
		}

		protected override ExEventLog.EventTuple GetPOPEventTuple
		{
			get
			{
				return UMEventLogConstants.Tuple_AAPlayOnPhoneRequest;
			}
		}

		protected override ExEventLog.EventTuple GetPOPFailureEventTuple
		{
			get
			{
				return UMEventLogConstants.Tuple_AAOutDialingRulesFailure;
			}
		}

		protected override DialingPermissionsCheck GetDialingPermissionsChecker
		{
			get
			{
				return new DialingPermissionsCheck(this.callSession.CurrentCallContext.AutoAttendantInfo, this.callSession.CurrentCallContext.CurrentAutoAttendantSettings, this.GetOriginatingDialplan);
			}
		}

		internal void MakeCall(string numberToCall, IList<PlatformSignalingHeader> additionalHeaders)
		{
			string defaultOutboundCallingLineId = this.GetOriginatingDialplan.DefaultOutboundCallingLineId;
			string callerIdToUse;
			if (!string.IsNullOrEmpty(defaultOutboundCallingLineId))
			{
				callerIdToUse = defaultOutboundCallingLineId;
			}
			else
			{
				if (this.aa.PilotIdentifierList.Count <= 0)
				{
					throw new NoCallerIdToUseException();
				}
				callerIdToUse = this.aa.PilotIdentifierList[0];
			}
			base.MakeCall(callerIdToUse, numberToCall, additionalHeaders);
		}

		private UMAutoAttendant aa;
	}
}
