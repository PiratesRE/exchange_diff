using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UmDiagnosticManager : ActivityManager
	{
		internal UmDiagnosticManager(ActivityManager manager, UmDiagnosticManager.ConfigClass config) : base(manager, config)
		{
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal override void Start(BaseUMCallSession callSession, string refInfo)
		{
			callSession.CurrentCallContext.CallType = 7;
			callSession.CurrentCallContext.IsDiagnosticCall = true;
			base.Start(callSession, refInfo);
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "UmDiagnosticManager executing action={0}.", new object[]
			{
				action
			});
			string input = null;
			if (string.Equals(action, "isLocal", StringComparison.OrdinalIgnoreCase))
			{
				if (vo.CurrentCallContext.IsLocalDiagnosticCall)
				{
					input = "local";
				}
				return base.CurrentActivity.GetTransition(input);
			}
			if (string.Equals(action, "sendDtmf", StringComparison.OrdinalIgnoreCase))
			{
				this.SendDiagnosticDtmf(vo);
				return new StopTransition();
			}
			return base.ExecuteAction(action, vo);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UmDiagnosticManager>(this);
		}

		private void SendDiagnosticDtmf(BaseUMCallSession callSession)
		{
			string text = this.GlobalManager.ReadVariable("diagnosticDtmfDigits") as string;
			string str;
			try
			{
				IPAddress localIPv4Address = Utils.GetLocalIPv4Address();
				str = localIPv4Address.ToString().Replace('.', '*');
			}
			catch (ProtocolViolationException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "UmDiagnosticManager has an issue = {0}.", new object[]
				{
					ex.Message
				});
				str = string.Empty;
			}
			string str2 = "#" + str + "#";
			string str3;
			if (UmServiceGlobals.ArePerfCountersEnabled)
			{
				long rawValue = GeneralCounters.CurrentCalls.RawValue;
				if (rawValue < 0L)
				{
					str3 = "0";
				}
				else
				{
					str3 = rawValue.ToString(CultureInfo.InvariantCulture);
				}
			}
			else
			{
				str3 = "0";
			}
			text = text + str2 + str3;
			string text2 = string.Concat(new object[]
			{
				"#",
				text.Length,
				"#",
				text
			});
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DiagnosticResponseSequence, null, new object[]
			{
				text2,
				callSession.CallId
			});
			callSession.SendDtmf(text2, new TimeSpan(0, 0, 0, 0, 1000));
		}

		internal class ConfigClass : ActivityManagerConfig
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing UMDiagnostic activity manager.", new object[0]);
				return new UmDiagnosticManager(manager, this);
			}
		}
	}
}
