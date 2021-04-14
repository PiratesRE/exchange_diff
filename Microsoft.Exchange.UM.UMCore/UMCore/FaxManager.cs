using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class FaxManager : CAMessageSubmissionManager
	{
		internal FaxManager(ActivityManager manager, FaxManager.ConfigClass config) : base(manager, config)
		{
		}

		internal override string HandleFailedTransfer(BaseUMCallSession callSession)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "FaxManager handleFailedTransfer", new object[0]);
			CallTransfer callTransfer = (CallTransfer)base.CurrentActivity;
			callTransfer.HandleFailedFaxTransfer(callSession);
			return null;
		}

		internal void OnFaxRequestReceive(BaseUMCallSession callSession, UMCallSessionEventArgs callSessionEventArgs)
		{
			FaxRequest faxRequest = (FaxRequest)base.CurrentActivity;
			faxRequest.OnFaxRequestReceive(callSession);
		}

		internal override void Start(BaseUMCallSession callSession, string refInfo)
		{
			base.Start(callSession, refInfo);
		}

		internal override void OnUserHangup(BaseUMCallSession callSession, UMCallSessionEventArgs callSessionEventArgs)
		{
			base.OnUserHangup(callSession, callSessionEventArgs);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing fax activity manager.", new object[0]);
				return new FaxManager(manager, this);
			}
		}
	}
}
