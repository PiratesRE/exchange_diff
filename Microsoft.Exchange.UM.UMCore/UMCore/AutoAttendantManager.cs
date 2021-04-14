using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AutoAttendantManager : ActivityManager, IAutoAttendantUI
	{
		internal AutoAttendantManager(ActivityManager manager, AutoAttendantManager.ConfigClass config) : base(manager, config)
		{
		}

		internal bool StarOutToDialPlanEnabled
		{
			get
			{
				return this.commonAABehavior.StarOutToDialPlanEnabled;
			}
		}

		internal bool ForwardCallsToDefaultMailbox
		{
			get
			{
				return this.commonAABehavior.ForwardCallsToDefaultMailbox;
			}
		}

		internal string BusinessName
		{
			get
			{
				return this.commonAABehavior.BusinessName;
			}
		}

		internal string BusinessLocation
		{
			get
			{
				return this.commonAABehavior.Config.BusinessLocation;
			}
		}

		internal bool BusinessLocationIsSet
		{
			get
			{
				return !string.IsNullOrEmpty(this.BusinessLocation);
			}
		}

		internal UMAutoAttendant ThisAutoAttendant
		{
			get
			{
				return this.commonAABehavior.Config;
			}
		}

		internal string SelectedMenu
		{
			get
			{
				if (this.commonAABehavior.SelectedMenu == null)
				{
					return string.Empty;
				}
				return this.commonAABehavior.SelectedMenu.Description;
			}
		}

		internal AutoAttendantContext AAContext
		{
			get
			{
				bool isBusinessHours = false;
				HolidaySchedule holidaySchedule = null;
				this.ThisAutoAttendant.GetCurrentSettings(out holidaySchedule, ref isBusinessHours);
				return new AutoAttendantContext(this.ThisAutoAttendant, isBusinessHours);
			}
		}

		internal AutoAttendantLocationContext AALocationContext
		{
			get
			{
				return new AutoAttendantLocationContext(this.ThisAutoAttendant, this.SelectedMenu);
			}
		}

		public object ReadProperty(string name)
		{
			return this.ReadVariable(name);
		}

		public object ReadGlobalProperty(string name)
		{
			return this.GlobalManager.ReadVariable(name);
		}

		public void WriteProperty(string name, object value)
		{
			base.WriteVariable(name, value);
		}

		public void WriteGlobalProperty(string name, object value)
		{
			this.GlobalManager.WriteVariable(name, value);
		}

		public void SetTextPrompt(string name, string promptText)
		{
			base.SetTextPartVariable(name, promptText);
		}

		public void SetWavePrompt(string name, ITempWavFile promptFile)
		{
			base.SetWavePartVariable(name, promptFile);
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			this.commonAABehavior = AutoAttendantCore.Create(this, vo);
			this.Configure(vo);
			base.WriteVariable("tuiPromptEditingEnabled", this.GlobalManager.ReadVariable("tuiPromptEditingEnabled"));
			base.Start(vo, refInfo);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "AutoAttendant Manager asked to do action: {0}.", new object[]
			{
				action
			});
			string input = null;
			if (string.Equals(action, "setPromptProvContext", StringComparison.OrdinalIgnoreCase))
			{
				this.GlobalManager.WriteVariable("promptProvContext", "AutoAttendant");
			}
			else if (!this.commonAABehavior.ExecuteAction(action, vo, ref input))
			{
				return base.ExecuteAction(action, vo);
			}
			return base.CurrentActivity.GetTransition(input);
		}

		internal string PrepareForCallAnswering(BaseUMCallSession session)
		{
			return this.commonAABehavior.Action_PrepareForCallAnswering();
		}

		internal string CheckNonUmExtension(BaseUMCallSession vo)
		{
			string result = null;
			DialPermissionWrapper dialPermissionWrapper = DialPermissionWrapperFactory.Create(vo);
			if (dialPermissionWrapper.CallingNonUmExtensionsAllowed)
			{
				result = "denyCallNonUmExtension";
				PhoneNumber phone;
				if (DirectorySearchManager.DialableNonUmExtension(base.DtmfDigits, vo.CurrentCallContext.DialPlan, out phone))
				{
					PhoneUtil.SetTransferTargetPhone(this, TransferExtension.UserExtension, phone);
					result = "allowCallNonUmExtension";
				}
			}
			return result;
		}

		internal override void OnTimeout(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Timeout.", new object[0]);
			this.commonAABehavior.OnTimeout();
			base.CurrentActivity.OnTimeout(vo, callSessionEventArgs);
		}

		internal override void OnTransferComplete(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			TransferExtension transferExtension = (TransferExtension)this.ReadVariable("transferExtension");
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "OnTransferComplete Type = {0}.", new object[]
			{
				transferExtension
			});
			this.commonAABehavior.OnTransferComplete(transferExtension);
			base.OnTransferComplete(vo, callSessionEventArgs);
		}

		internal override void OnInput(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			this.commonAABehavior.OnInput();
			base.OnInput(vo, callSessionEventArgs);
		}

		internal override void OnUserHangup(BaseUMCallSession vo, UMCallSessionEventArgs callSessionEventArgs)
		{
			this.commonAABehavior.OnHangup();
			base.OnUserHangup(vo, callSessionEventArgs);
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.commonAABehavior.Dispose();
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AutoAttendantManager>(this);
		}

		private void Configure(BaseUMCallSession vo)
		{
			this.commonAABehavior.Configure();
		}

		private AutoAttendantCore commonAABehavior;

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing AutoAttendant activity manager.", new object[0]);
				return new AutoAttendantManager(manager, this);
			}
		}
	}
}
