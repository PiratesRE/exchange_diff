using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class CommonActivity : ActivityBase
	{
		internal CommonActivity(ActivityManager manager, ActivityConfig config) : base(manager, config)
		{
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			if (vo.CurrentCallContext.IsTestCall || vo.CurrentCallContext.IsTUIDiagnosticCall)
			{
				this.refInfo = refInfo;
				vo.SendStateInfo(vo.CallId, base.UniqueId);
				return;
			}
			this.StartActivity(vo, refInfo);
		}

		internal override void OnStateInfoSent(BaseUMCallSession vo, UMCallSessionEventArgs voiceObjectEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ActivityBase OnStateInfoSent called.", new object[0]);
			this.StartActivity(vo, this.refInfo);
		}

		internal abstract void StartActivity(BaseUMCallSession vo, string refInfo);

		private string refInfo;
	}
}
