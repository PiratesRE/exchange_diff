using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class FindMeSubscriberManager : ActivityManager, IPAAChild, IPAACommonInterface
	{
		internal FindMeSubscriberManager(ActivityManager manager, FindMeSubscriberManager.ConfigClass config) : base(manager, config)
		{
		}

		internal object CallerRecordedName
		{
			get
			{
				return this.paaManager.GetCallerRecordedName();
			}
		}

		internal object CalleeRecordName
		{
			get
			{
				return this.paaManager.GetCalleeRecordedName();
			}
		}

		public void TerminateCall()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.FindMeTracer, this, "FindMeSubscriberManager : TerminateCall() called ", new object[0]);
			if (!this.isAlreadyDisposed)
			{
				base.DropCall(base.CallSession, DropCallReason.GracefulHangup);
			}
		}

		public void TerminateCallToTryNextNumberTransfer()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.FindMeTracer, this, "FindMeSubscriberManager : TerminateCallToTryNextNumberTransfer() called ", new object[0]);
			if (this.isAlreadyDisposed)
			{
				this.paaManager.ContinueFindMe();
				return;
			}
			this.paaManager.DisconnectChildCall();
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			this.paaManager = (IPAAParent)vo.CurrentCallContext.LinkedManagerPointer;
			this.paaManager.SetPointerToChild(this);
			base.Start(vo, refInfo);
		}

		internal string TerminateFindMe(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.FindMeTracer, this, "FindMeSubscriberManager : TerminateFindMe() decided by user ", new object[0]);
			this.paaManager.TerminateFindMe();
			return null;
		}

		internal string SendDtmf(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.FindMeTracer, this, "FindMeSubscriberManager : SendDtmf() = sending dummy DTMF ", new object[0]);
			vo.SendDtmf("D", TimeSpan.Zero);
			return "stopEvent";
		}

		internal override void DropCall(BaseUMCallSession vo, DropCallReason reason)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.FindMeTracer, this, "FindMeSubscriberManager : DropCall() ", new object[0]);
			this.paaManager.DisconnectChildCall();
		}

		internal string AcceptFindMe(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.FindMeTracer, this, "FindMeSubscriberManager : AcceptFindMe() decided by user ", new object[0]);
			this.paaManager.AcceptCall();
			return null;
		}

		internal override void OnUserHangup(BaseUMCallSession vo, UMCallSessionEventArgs voiceEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.FindMeTracer, this, "FindMeSubscriberManager : OnUserHangup() : the call was disconnected by user ", new object[0]);
			IPAAParent ipaaparent = this.paaManager;
			this.isAlreadyDisposed = true;
			base.OnUserHangup(vo, voiceEventArgs);
			ipaaparent.ContinueFindMe();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FindMeSubscriberManager>(this);
		}

		private bool isAlreadyDisposed;

		private IPAAParent paaManager;

		internal class ConfigClass : ActivityManagerConfig
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.FindMeTracer, this, "Constructing FindMeSubscriberManager.", new object[0]);
				return new FindMeSubscriberManager(manager, this);
			}

			internal override void Load(XmlNode rootNode)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.FindMeTracer, this, "Loading a new FindMeSubscriberManager.", new object[0]);
				base.Load(rootNode);
			}
		}
	}
}
