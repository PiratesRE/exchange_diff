using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SpeechAutoAttendantManager : AsrContactsManager, IAutoAttendantUI
	{
		internal SpeechAutoAttendantManager(ActivityManager manager, SpeechAutoAttendantManager.ConfigClass config) : base(manager, config)
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

		internal bool RepeatMainMenu
		{
			get
			{
				return this.repeatMainMenu;
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
			base.WriteVariable("tuiPromptEditingEnabled", this.GlobalManager.ReadVariable("tuiPromptEditingEnabled"));
			this.commonAABehavior = AutoAttendantCore.Create(this, vo);
			base.SetInitialSearchTargetGal();
			base.Start(vo, refInfo);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			string text = null;
			if (string.Equals(action, "checkRestrictedUser", StringComparison.OrdinalIgnoreCase))
			{
				text = "unrestrictedUser";
				if (base.SelectedSearchItem != null && (base.SelectedSearchItem.Recipient.AllowUMCallsFromNonUsers & AllowUMCallsFromNonUsersFlags.SearchEnabled) != AllowUMCallsFromNonUsersFlags.SearchEnabled)
				{
					text = "restrictedUser";
					PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, base.SelectedSearchItem.Recipient.DisplayName);
					CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, data, "Recipient _UserDisplayName is a protected user. returning autoEvent: {0}.", new object[]
					{
						text
					});
				}
			}
			else if (string.Equals(action, "setPromptProvContext", StringComparison.OrdinalIgnoreCase))
			{
				this.GlobalManager.WriteVariable("promptProvContext", "AutoAttendant");
			}
			else if (!this.commonAABehavior.ExecuteAction(action, vo, ref text))
			{
				return base.ExecuteAction(action, vo);
			}
			return base.CurrentActivity.GetTransition(text);
		}

		internal string PrepareForCallAnswering(BaseUMCallSession session)
		{
			return this.commonAABehavior.Action_PrepareForCallAnswering();
		}

		internal string DisableMainMenuRepetition(BaseUMCallSession session)
		{
			this.repeatMainMenu = false;
			return null;
		}

		internal string EnableMainMenuRepetition(BaseUMCallSession session)
		{
			this.repeatMainMenu = true;
			return null;
		}

		internal override void OnTransferComplete(BaseUMCallSession vo, UMCallSessionEventArgs voiceEventArgs)
		{
			TransferExtension transferExtension = (TransferExtension)this.ReadVariable("transferExtension");
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "OnTransferComplete Type = {0}.", new object[]
			{
				transferExtension
			});
			this.commonAABehavior.OnTransferComplete(transferExtension);
			base.OnTransferComplete(vo, voiceEventArgs);
		}

		internal override void OnInput(BaseUMCallSession vo, UMCallSessionEventArgs voiceEventArgs)
		{
			this.commonAABehavior.OnInput();
			base.OnInput(vo, voiceEventArgs);
		}

		internal override void OnUserHangup(BaseUMCallSession vo, UMCallSessionEventArgs voiceEventArgs)
		{
			this.commonAABehavior.OnHangup();
			base.OnUserHangup(vo, voiceEventArgs);
		}

		internal override void OnTimeout(BaseUMCallSession vo, UMCallSessionEventArgs voiceEventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Timeout.", new object[0]);
			this.commonAABehavior.OnTimeout();
			base.CurrentActivity.OnTimeout(vo, voiceEventArgs);
		}

		internal override void OnNameSpoken()
		{
			this.commonAABehavior.OnNameSpoken();
		}

		internal override void PrepareForNBestPhase2()
		{
			this.commonAABehavior.Initialize();
		}

		internal override void OnSpeech(object sender, UMSpeechEventArgs args)
		{
			this.commonAABehavior.OnSpeech();
			base.OnSpeech(sender, args);
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
			return DisposeTracker.Get<SpeechAutoAttendantManager>(this);
		}

		protected override void LookupRecipientAndDialPlan(BaseUMCallSession vo, PhoneNumber numberToDial)
		{
			if (base.SelectedResultType == ResultType.Department)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Result is a department, assuming same dialplan as originating dialplan.", new object[0]);
				base.TargetDialPlan = base.OriginatingDialPlan;
				return;
			}
			base.LookupRecipientAndDialPlan(vo, numberToDial);
		}

		protected override void ConfigureForCall(BaseUMCallSession vo)
		{
			this.commonAABehavior.Configure();
		}

		protected override SearchGrammarFile CreateNamesGrammar(BaseUMCallSession vo)
		{
			ExAssert.RetailAssert(this.DirectoryGrammarHandler != null, "DirectoryGrammarHandler was not pre-initialized for Speech AA Call");
			SearchGrammarFile searchGrammarFile = this.DirectoryGrammarHandler.WaitForPrepareGrammarCompletion();
			if (searchGrammarFile == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "SpeechAAManager: CreateNamesGrammar : Gal Grammar Fetch Error for grammar='{0}'", new object[]
				{
					this.DirectoryGrammarHandler
				});
				GalGrammarFile.LogErrorEvent(vo.CurrentCallContext);
			}
			return searchGrammarFile;
		}

		protected override bool CheckDialPermissions(BaseUMCallSession vo, out PhoneNumber numberToDial)
		{
			return DialPermissions.Check(base.CanonicalizedNumber, vo.CurrentCallContext.AutoAttendantInfo, vo.CurrentCallContext.DialPlan, base.TargetDialPlan, out numberToDial);
		}

		private AutoAttendantCore commonAABehavior;

		private bool repeatMainMenu = true;

		internal new class ConfigClass : AsrContactsManager.ConfigClass
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing AutoAttendant activity manager.", new object[0]);
				return new SpeechAutoAttendantManager(manager, this);
			}
		}
	}
}
