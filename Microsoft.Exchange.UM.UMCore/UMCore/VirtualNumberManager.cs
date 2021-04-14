using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class VirtualNumberManager : ActivityManager
	{
		internal VirtualNumberManager(ActivityManager manager, VirtualNumberManager.ConfigClass config) : base(manager, config)
		{
		}

		internal string PrepareForVoiceMail(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "VirtualNumberManager::PrepareForVoiceMail()", new object[0]);
			return null;
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			base.Start(vo, refInfo);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_VirtualNumberCall, null, new object[]
			{
				vo.CallId,
				vo.CurrentCallContext.CallerId.ToDial,
				vo.CurrentCallContext.CalleeInfo.ToString()
			});
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal string CheckIfCallFromBlockedNumber(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "VirtualNumberManager: CheckIfCallFromBlockedNumber() ", new object[0]);
			UMSubscriber umsubscriber = (UMSubscriber)vo.CurrentCallContext.CalleeInfo;
			if (umsubscriber.IsBlockedNumber(vo.CurrentCallContext.CallerId))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "VirtualNumberManager: CheckIfCallFromBlockedNumber() : Call Not Blocked", new object[0]);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_VirtualNumberCallBlocked, null, new object[]
				{
					vo.CallId,
					vo.CurrentCallContext.CallerId.ToDial,
					vo.CurrentCallContext.CalleeInfo.ToString()
				});
				return "blockedCall";
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "VirtualNumberManager: CheckIfCallFromBlockedNumber() : Call Blocked ", new object[0]);
			return null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<VirtualNumberManager>(this);
		}

		internal class ConfigClass : ActivityManagerConfig
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.FindMeTracer, this, "Constructing VirtualNumberManager.", new object[0]);
				return new VirtualNumberManager(manager, this);
			}

			internal override void Load(XmlNode rootNode)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.FindMeTracer, this, "Loading a new VirtualNumberManager.", new object[0]);
				base.Load(rootNode);
			}
		}
	}
}
