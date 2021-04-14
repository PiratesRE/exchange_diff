using System;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UmTroubleshootingToolManager : ActivityManager
	{
		internal UmTroubleshootingToolManager(ActivityManager manager, UmTroubleshootingToolManager.ConfigClass config) : base(manager, config)
		{
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal string ConfirmAcceptedCallType(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, this, "ConfirmAcceptedCallType", new object[0]);
			InfoMessage infoMessage = new InfoMessage();
			infoMessage.ContentType = CommonConstants.ContentTypeTextPlain;
			infoMessage.Headers["msexum-diagtool-info"] = 0.ToString();
			UMToolCallAccepted umtoolCallAccepted = new UMToolCallAccepted(UmTroubleshootingToolManager.GetToolCallType(vo.CurrentCallContext.CallType), (vo.CurrentCallContext.CalleeInfo == null) ? string.Empty : vo.CurrentCallContext.CalleeInfo.DisplayName, vo.CurrentCallContext.DialPlan.Name, (vo.CurrentCallContext.AutoAttendantInfo == null) ? string.Empty : vo.CurrentCallContext.AutoAttendantInfo.Name, string.Empty, (vo.CurrentCallContext.DialPlan.URIType == UMUriType.TelExtn && vo.CurrentCallContext.CalleeInfo != null) ? vo.CurrentCallContext.CalleeInfo.ADRecipient.UMExtension : string.Empty, (vo.CurrentCallContext.DialPlan.URIType != UMUriType.TelExtn && vo.CurrentCallContext.CalleeInfo != null) ? vo.CurrentCallContext.CalleeInfo.ADRecipient.UMExtension : string.Empty, (vo.CurrentCallContext.CalleeInfo == null) ? string.Empty : vo.CurrentCallContext.CalleeInfo.MailAddress);
			infoMessage.Body = Encoding.UTF8.GetBytes(UMTypeSerializer.SerializeToString<UMToolCallAccepted>(umtoolCallAccepted));
			vo.SendMessage(infoMessage);
			return "stopEvent";
		}

		internal override void OnMessageSent(BaseUMCallSession vo, EventArgs e)
		{
			base.OnMessageSent(vo, e);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "UmTroubleshootingToolManager in OnMessageSent.", new object[0]);
			TransitionBase transition = this.GetTransition("toolInfoSent");
			if (transition != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Menu Activity making toolinfosent transition: {0}.", new object[]
				{
					transition
				});
				transition.Execute(this, vo);
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Menu Activity has no toolinfosent transition", new object[0]);
			this.DropCall(vo, DropCallReason.UserError);
		}

		internal string EchoBackDtmf(BaseUMCallSession vo)
		{
			PIIMessage data = PIIMessage.Create(PIIType._PII, base.DtmfDigits);
			CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, this, data, "EchoBackDtmf. Dtmfs received so far _PII", new object[0]);
			if (!string.IsNullOrEmpty(base.DtmfDigits))
			{
				vo.SendDtmf(base.DtmfDigits, TimeSpan.Zero);
				return "stopEvent";
			}
			this.DropCall(vo, DropCallReason.GracefulHangup);
			return null;
		}

		internal string SendStopRecordingDtmf(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DiagnosticTracer, this, "SendStopRecordingDtmf. sending #", new object[0]);
			vo.SendDtmf("#", TimeSpan.Zero);
			return "stopEvent";
		}

		private static string GetToolCallType(CallType type)
		{
			switch (type)
			{
			case 2:
				return type.ToString();
			case 3:
				return type.ToString();
			case 4:
				return type.ToString();
			case 5:
				return type.ToString();
			case 9:
				return type.ToString();
			}
			return 0.ToString();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UmTroubleshootingToolManager>(this);
		}

		internal class ConfigClass : ActivityManagerConfig
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing UmTroubleshootingTool activity manager.", new object[0]);
				return new UmTroubleshootingToolManager(manager, this);
			}
		}
	}
}
