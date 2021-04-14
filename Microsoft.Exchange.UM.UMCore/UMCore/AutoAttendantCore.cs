using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.PersonalAutoAttendant;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AutoAttendantCore
	{
		protected AutoAttendantCore(IAutoAttendantUI autoAttendantManager, BaseUMCallSession voiceObject)
		{
			this.VoiceObject = voiceObject;
			this.CallContext = voiceObject.CurrentCallContext;
			this.AutoAttendantManager = autoAttendantManager;
			this.startTime = ExDateTime.UtcNow;
			this.Config = voiceObject.CurrentCallContext.AutoAttendantInfo;
			this.settings = voiceObject.CurrentCallContext.CurrentAutoAttendantSettings;
			this.holidaySchedule = voiceObject.CurrentCallContext.AutoAttendantHolidaySettings;
			this.nameLookupConfigured = this.Config.NameLookupEnabled;
			this.CustomizedMenuConfigured = false;
			this.businessHoursCall = voiceObject.CurrentCallContext.AutoAttendantBusinessHourCall;
			this.transferToOperatorConfigured = Util.GetOperatorExtension(this.callContext, out this.operatorNumberConfigured);
			this.aaCoutersUtil = new AutoAttendantCountersUtil(this.VoiceObject);
			this.PerfCounters.IncrementCallTypeCounters(this.businessHoursCall);
		}

		public CustomMenuKeyMapping[] CustomMenu
		{
			get
			{
				return this.customMenu;
			}
			set
			{
				this.customMenu = value;
			}
		}

		public CustomMenuKeyMapping CustomMenuTimeoutOption
		{
			get
			{
				return this.customMenuTimeoutOption;
			}
			set
			{
				this.customMenuTimeoutOption = value;
			}
		}

		public int NumCustomizedMenuOptions
		{
			get
			{
				return this.numCustomizedMenuOptions;
			}
			set
			{
				this.numCustomizedMenuOptions = value;
			}
		}

		public bool NameLookupConfigured
		{
			get
			{
				return this.nameLookupConfigured;
			}
			set
			{
				this.nameLookupConfigured = value;
			}
		}

		public bool CustomizedMenuConfigured
		{
			get
			{
				return this.customizedMenuConfigured;
			}
			set
			{
				this.customizedMenuConfigured = value;
			}
		}

		public UMAutoAttendant Config
		{
			get
			{
				return this.config;
			}
			set
			{
				this.config = value;
			}
		}

		public bool BusinessHoursCall
		{
			get
			{
				return this.businessHoursCall;
			}
			set
			{
				this.businessHoursCall = value;
			}
		}

		public AutoAttendantSettings Settings
		{
			get
			{
				return this.settings;
			}
			set
			{
				this.settings = value;
			}
		}

		public bool CustomMenuTimeoutEnabled
		{
			get
			{
				return this.customMenuTimeoutEnabled;
			}
		}

		public CustomMenuKeyMapping SelectedMenu
		{
			get
			{
				return this.selectedMenu;
			}
			set
			{
				this.selectedMenu = value;
			}
		}

		public bool TimeoutPending
		{
			get
			{
				return this.timeoutPending;
			}
			set
			{
				this.timeoutPending = value;
			}
		}

		internal CallContext CallContext
		{
			get
			{
				return this.callContext;
			}
			set
			{
				this.callContext = value;
			}
		}

		internal IAutoAttendantUI AutoAttendantManager
		{
			get
			{
				return this.autoAttendantManager;
			}
			set
			{
				this.autoAttendantManager = value;
			}
		}

		internal BaseUMCallSession VoiceObject
		{
			get
			{
				return this.voiceObject;
			}
			set
			{
				this.voiceObject = value;
			}
		}

		internal bool StarOutToDialPlanEnabled
		{
			get
			{
				return this.Config.StarOutToDialPlanEnabled;
			}
		}

		internal bool ForwardCallsToDefaultMailbox
		{
			get
			{
				return this.Config.ForwardCallsToDefaultMailbox;
			}
		}

		internal string BusinessName
		{
			get
			{
				return Utils.TrimSpaces(this.Config.BusinessName);
			}
		}

		protected AutoAttendantCountersUtil PerfCounters
		{
			get
			{
				return this.aaCoutersUtil;
			}
		}

		protected PhoneNumber OperatorNumber
		{
			get
			{
				return this.operatorNumberConfigured;
			}
			set
			{
				this.operatorNumberConfigured = value;
			}
		}

		protected bool OperatorEnabled
		{
			get
			{
				return this.transferToOperatorConfigured;
			}
			set
			{
				this.transferToOperatorConfigured = value;
			}
		}

		internal static bool IsRunnableAutoAttendant(UMAutoAttendant aaconfig, out LocalizedString error)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, null, "IsRunnableAutoAttendant {0}.", new object[]
			{
				aaconfig.Name
			});
			bool flag = true;
			HolidaySchedule holidaySchedule = null;
			error = LocalizedString.Empty;
			if (aaconfig.Status == StatusEnum.Disabled)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, null, "IsRunnableAA: Autoattendant with Name={0} Is Disabled.", new object[]
				{
					aaconfig.Name
				});
				error = Strings.DisabledAA;
				return false;
			}
			if (!aaconfig.ForwardCallsToDefaultMailbox)
			{
				AutoAttendantSettings currentSettings = aaconfig.GetCurrentSettings(out holidaySchedule, ref flag);
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(aaconfig.OrganizationId);
				UMDialPlan dialPlanFromId = iadsystemConfigurationLookup.GetDialPlanFromId(aaconfig.UMDialPlan);
				bool flag2 = false;
				PhoneNumber phoneNumber = null;
				if (currentSettings.TransferToOperatorEnabled)
				{
					flag2 = CommonUtil.GetOperatorExtensionForAutoAttendant(aaconfig, currentSettings, dialPlanFromId, false, out phoneNumber);
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, null, "AA_Name={0} CallSomeone={1} SendVoiceMsg={2} NameLookup={3} KeyMapping={4} Status={5} OperatorEnabled={6}.", new object[]
				{
					aaconfig.Name,
					aaconfig.CallSomeoneEnabled,
					aaconfig.SendVoiceMsgEnabled,
					aaconfig.NameLookupEnabled,
					currentSettings.KeyMappingEnabled,
					aaconfig.Status,
					flag2
				});
				bool flag3;
				if (aaconfig.SpeechEnabled)
				{
					flag3 = ((aaconfig.NameLookupEnabled && (aaconfig.CallSomeoneEnabled || aaconfig.SendVoiceMsgEnabled)) || (currentSettings.KeyMappingEnabled && currentSettings.KeyMapping.Length > 0));
					if (!flag3)
					{
						error = Strings.NonFunctionalAsrAA;
					}
				}
				else
				{
					flag3 = (aaconfig.CallSomeoneEnabled || aaconfig.SendVoiceMsgEnabled || (currentSettings.KeyMappingEnabled && currentSettings.KeyMapping.Length > 0) || flag2);
					if (!flag3)
					{
						error = Strings.NonFunctionalDtmfAA;
					}
				}
				return flag3;
			}
			if (!string.IsNullOrEmpty(aaconfig.DefaultMailboxLegacyDN))
			{
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromOrganizationId(aaconfig.OrganizationId, null);
				aaconfig.DefaultMailbox = (iadrecipientLookup.LookupByLegacyExchangeDN(aaconfig.DefaultMailboxLegacyDN) as ADUser);
			}
			if (!Utils.IsUserUMEnabledInGivenDialplan(aaconfig.DefaultMailbox, aaconfig.UMDialPlan))
			{
				error = Strings.InvalidDefaultMailboxAA;
				return false;
			}
			return true;
		}

		internal static AutoAttendantCore Create(IAutoAttendantUI autoAttendantManager, BaseUMCallSession voiceObject)
		{
			if (autoAttendantManager is SpeechAutoAttendantManager)
			{
				return new SpeechAutoAttendant(autoAttendantManager, voiceObject);
			}
			if (autoAttendantManager is AutoAttendantManager)
			{
				return new DtmfAutoAttendant(autoAttendantManager, voiceObject);
			}
			return null;
		}

		internal static bool GetExtensionForKey(int key, CustomMenuKeyMapping[] km, out CustomMenuKeyMapping keyMapping)
		{
			keyMapping = null;
			for (int i = 0; i < km.Length; i++)
			{
				int mappedKey = (int)km[i].MappedKey;
				if (mappedKey == key)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, null, "DTMF Key [{0}] Mapped to Menu option [{1}:{2}].", new object[]
					{
						key,
						km[i].Description,
						km[i].Extension
					});
					keyMapping = km[i];
					return true;
				}
			}
			return false;
		}

		internal static bool CheckCustomMenuTimeoutOption(CustomMenuKeyMapping[] km, out CustomMenuKeyMapping timeoutOption)
		{
			timeoutOption = null;
			for (int i = 0; i < km.Length; i++)
			{
				CustomMenuKey mappedKey = km[i].MappedKey;
				if (mappedKey == CustomMenuKey.Timeout)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, null, "[{0}] Mapped to Menu option [{1}:{2}].", new object[]
					{
						mappedKey,
						km[i].Description,
						km[i].Extension
					});
					timeoutOption = km[i];
					return true;
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, null, "Did not get a timeout option for this AA.", new object[0]);
			return false;
		}

		internal static PhoneNumber GetCustomExtensionNumberToDial(CallContext cc, string number)
		{
			PhoneNumber phoneNumber = null;
			if (!PhoneNumber.TryParse(cc.DialPlan, number, out phoneNumber))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, null, "GetCustomExtensionNumberToDial: selectedMenu.Extension ={0} is not a valid PhoneNumber", new object[]
				{
					number
				});
			}
			else
			{
				PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber.ToString());
				CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, null, data, "GetCustomExtensionNumberToDial::Trying with Phone Number = _PhoneNumber", new object[0]);
				DialingPermissionsCheck dialingPermissionsCheck = new DialingPermissionsCheck(cc.AutoAttendantInfo, cc.CurrentAutoAttendantSettings, cc.DialPlan);
				DialingPermissionsCheck.DialingPermissionsCheckResult dialingPermissionsCheckResult = dialingPermissionsCheck.CheckPhoneNumber(phoneNumber);
				if (dialingPermissionsCheckResult.AllowCall)
				{
					phoneNumber = dialingPermissionsCheckResult.NumberToDial;
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, null, "GetCustomExtensionNumberToDial::DialPermissionsCheck failed", new object[0]);
					phoneNumber = null;
				}
			}
			return phoneNumber;
		}

		internal virtual void Initialize()
		{
		}

		internal virtual void Dispose()
		{
			this.PerfCounters.ComputeSuccessRate();
		}

		internal virtual bool ExecuteAction(string action, BaseUMCallSession voiceObject, ref string autoEvent)
		{
			bool result = false;
			if (string.Compare(action, "initializeState", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				this.Initialize();
				autoEvent = null;
			}
			else if (string.Compare(action, "setOperatorNumber", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_SetOperatorNumber();
			}
			else if (string.Compare(action, "processCustomMenuSelection", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_ProcessCustomMenuExtension();
			}
			else if (string.Compare(action, "prepareForTransferToPaa", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_PrepareForTransferToPaa();
			}
			else if (string.Compare(action, "setCustomMenuAutoAttendant", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_ProcessCustomMenuAutoAttendant();
			}
			else if (string.Compare(action, "setCustomMenuTargetPAA", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_ProcessCustomMenuTargetPAA();
			}
			else if (string.Compare(action, "setCustomMenuVoicemailTarget", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_ProcessCustomMenuVoicemailTarget();
			}
			else if (string.Compare(action, "transferToPAASiteFailed", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_TransferToPAASiteFailed();
			}
			else if (string.Compare(action, "setCustomExtensionNumber", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_ProcessCustomMenuTransferToNumber();
			}
			else if (string.Compare(action, "handleFaxTone", true, CultureInfo.InvariantCulture) == 0)
			{
				autoEvent = this.Action_OnFaxTone();
				result = true;
			}
			else if (string.Compare(action, "prepareForProtectedSubscriberOperatorTransfer", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_PrepareForProtectedSubscriberOperatorTransfer();
			}
			else if (string.Compare(action, "prepareForTransferToSendMessage", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_PrepareForTransferToSendMessage();
			}
			else if (string.Compare(action, "prepareForTransferToKeyMappingAutoAttendant", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_PrepareForTransferToKeyMappingAutoAttendant();
			}
			else if (string.Compare(action, "prepareForTransferToDtmfFallbackAutoAttendant", true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				autoEvent = this.Action_PrepareForTransferToDtmfFallbackAutoAttendant();
			}
			else if (string.Compare(action, "prepareForUserInitiatedOperatorTransfer", true, CultureInfo.InvariantCulture) == 0)
			{
				this.PerfCounters.IncrementUserInitiatedTransferToOperatorCounter();
				result = true;
				autoEvent = null;
			}
			else if (string.Compare(action, "prepareForUserInitiatedOperatorTransferFromOpeningMenu", true, CultureInfo.InvariantCulture) == 0)
			{
				this.PerfCounters.IncrementUserInitiatedTransferToOperatorFromMainMenuCounter();
				result = true;
				autoEvent = null;
			}
			return result;
		}

		internal string Action_PrepareForProtectedSubscriberOperatorTransfer()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Action_PrepareForProtectedSubscriberOperatorTransfer().", new object[0]);
			this.PerfCounters.IncrementDisallowedTransferCalls();
			return null;
		}

		internal string Action_PrepareForTransferToSendMessage()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Action_PrepareForTransferToSendMessage().", new object[0]);
			this.PerfCounters.IncrementTransfersToSendMessageCounter();
			return null;
		}

		internal string Action_PrepareForTransferToKeyMappingAutoAttendant()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Action_PrepareForTransferToKeyMappingAutoAttendant().", new object[0]);
			this.UpdateCountersOnTransferToAA(false);
			return null;
		}

		internal string Action_PrepareForTransferToDtmfFallbackAutoAttendant()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Action_PrepareForTransferToDtmfFallbackAutoAttendant().", new object[0]);
			this.UpdateCountersOnTransferToAA(true);
			return null;
		}

		internal string Action_OnFaxTone()
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMNumberNotConfiguredForFax, null, new object[]
			{
				this.VoiceObject.CurrentCallContext.Extension,
				this.VoiceObject.CurrentCallContext.CallerId
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "AutoAttendant got FaxTone. Dropping Call", new object[0]);
			((ActivityManager)this.AutoAttendantManager).DropCall(this.VoiceObject, DropCallReason.UserError);
			return "stopEvent";
		}

		internal string Action_SetOperatorNumber()
		{
			PhoneUtil.SetTransferTargetPhone((ActivityManager)this.AutoAttendantManager, TransferExtension.Operator, this.OperatorNumber);
			return null;
		}

		internal virtual string Action_ProcessCustomMenuExtension()
		{
			string dtmfDigits = ((ActivityManager)this.AutoAttendantManager).DtmfDigits;
			string result = null;
			bool flag = false;
			if (this.TimeoutPending)
			{
				flag = true;
				this.TimeoutPending = false;
				this.selectedMenu = this.customMenuTimeoutOption;
				this.selectedMenu = this.customMenuTimeoutOption;
			}
			else if (Constants.RegularExpressions.ValidDigitRegex.IsMatch(dtmfDigits))
			{
				int key = int.Parse(dtmfDigits, CultureInfo.InvariantCulture);
				CustomMenuKeyMapping[] keyMapping = this.settings.KeyMapping;
				CustomMenuKeyMapping customMenuKeyMapping;
				if (AutoAttendantCore.GetExtensionForKey(key, keyMapping, out customMenuKeyMapping))
				{
					this.selectedMenu = customMenuKeyMapping;
					flag = true;
				}
				else
				{
					result = "invalidOption";
					this.WriteVariable("invalidExtension", dtmfDigits);
				}
			}
			else
			{
				result = "invalidOption";
				this.WriteVariable("invalidExtension", dtmfDigits);
			}
			if (flag)
			{
				this.PerfCounters.Increment(DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccess);
				this.IncrementKeyMappingPerfCounters(this.selectedMenu.MappedKey);
				string promptFileName = this.selectedMenu.PromptFileName;
				bool flag2 = !string.IsNullOrEmpty(promptFileName);
				this.WriteVariable("haveCustomMenuOptionPrompt", flag2);
				if (flag2)
				{
					this.WriteVariable("customMenuOptionPrompt", this.CallContext.UMConfigCache.GetPrompt<UMAutoAttendant>(this.Config, promptFileName));
				}
				AutoAttendantCustomOptionType autoAttendantCustomOptionType = AutoAttendantCustomOptionType.None;
				if (!string.IsNullOrEmpty(this.selectedMenu.AutoAttendantName))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.TransferToAutoAttendant;
				}
				else if (!string.IsNullOrEmpty(this.selectedMenu.Extension))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.TransferToExtension;
				}
				else if (!string.IsNullOrEmpty(this.selectedMenu.LegacyDNToUseForLeaveVoicemailFor))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.TransferToVoicemailDirectly;
				}
				else if (!string.IsNullOrEmpty(this.selectedMenu.LegacyDNToUseForTransferToMailbox))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.TransferToVoicemailPAA;
				}
				else if (!string.IsNullOrEmpty(this.selectedMenu.AnnounceBusinessLocation))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.ReadBusinessLocation;
				}
				else if (!string.IsNullOrEmpty(this.selectedMenu.AnnounceBusinessHours))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.ReadBusinessHours;
				}
				this.WriteVariable("customMenuOption", autoAttendantCustomOptionType.ToString());
			}
			return result;
		}

		internal string Action_ProcessCustomMenuTargetPAA()
		{
			string customMenuTargetPAA = this.GetCustomMenuTargetPAA();
			return this.SwitchToTargetPAA(customMenuTargetPAA);
		}

		internal string Action_ProcessCustomMenuVoicemailTarget()
		{
			string customMenuVoicemailTarget = this.GetCustomMenuVoicemailTarget();
			return this.SwitchToVoicemailTarget(customMenuVoicemailTarget);
		}

		internal string Action_TransferToPAASiteFailed()
		{
			this.WriteGlobalVariable("directorySearchResult", ContactSearchItem.CreateFromRecipient(this.mailboxTransferTarget));
			return null;
		}

		internal string Action_ProcessCustomMenuAutoAttendant()
		{
			string customMenuAutoAttendant = this.GetCustomMenuAutoAttendant();
			return this.SwitchToAutoAttendant(customMenuAutoAttendant);
		}

		internal virtual string GetCustomMenuAutoAttendant()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "AutoAttendantCore::GetCustomMenuAutoAttendant().", new object[0]);
			return this.selectedMenu.AutoAttendantName;
		}

		internal virtual string GetCustomMenuVoicemailTarget()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "AutoAttendantCore::GetCustomMenuVoicemailTarget().", new object[0]);
			return this.selectedMenu.LegacyDNToUseForLeaveVoicemailFor;
		}

		internal virtual string GetCustomMenuTargetPAA()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "AutoAttendantCore::GetCustomMenuTargetPAA().", new object[0]);
			return this.selectedMenu.LegacyDNToUseForTransferToMailbox;
		}

		internal virtual void Configure()
		{
			this.ConfigureCommonVariables();
		}

		internal void ConfigureCommonVariables()
		{
			this.WriteVariable("aa_isBusinessHours", this.businessHoursCall);
			if (this.CallContext.AutoAttendantHolidaySettings != null)
			{
				this.WriteVariable("holidayHours", true);
				this.WriteVariable("holidayIntroductoryGreetingPrompt", this.CallContext.UMConfigCache.GetPrompt<UMAutoAttendant>(this.Config, this.holidaySchedule.Greeting));
			}
			else
			{
				this.WriteVariable("holidayHours", false);
				this.WriteVariable("holidayIntroductoryGreetingPrompt", null);
			}
			this.WriteVariable("infoAnnouncementEnabled", this.Config.InfoAnnouncementEnabled.ToString());
			if (this.Config.InfoAnnouncementEnabled != InfoAnnouncementEnabledEnum.False)
			{
				this.WriteVariable("infoAnnouncementFilename", this.CallContext.UMConfigCache.GetPrompt<UMAutoAttendant>(this.Config, this.Config.InfoAnnouncementFilename));
			}
			bool mainMenuCustomPromptEnabled = this.settings.MainMenuCustomPromptEnabled;
			this.WriteVariable("mainMenuCustomPromptEnabled", mainMenuCustomPromptEnabled);
			this.WriteVariable("mainMenuCustomPromptFilename", this.CallContext.UMConfigCache.GetPrompt<UMAutoAttendant>(this.Config, this.settings.MainMenuCustomPromptFilename));
			CustomMenuKeyMapping[] keyMapping = this.settings.KeyMapping;
			this.NumCustomizedMenuOptions = ((keyMapping != null) ? keyMapping.Length : 0);
			bool keyMappingEnabled = this.settings.KeyMappingEnabled;
			this.CustomizedMenuConfigured = false;
			if (keyMappingEnabled && this.NumCustomizedMenuOptions > 0)
			{
				this.CustomizedMenuConfigured = true;
				this.CustomMenu = keyMapping;
				this.customMenuTimeoutEnabled = AutoAttendantCore.CheckCustomMenuTimeoutOption(keyMapping, out this.customMenuTimeoutOption);
				this.ConfigureCustomMenuDtmfKeyInput(keyMapping);
			}
			else
			{
				this.CustomizedMenuConfigured = false;
				this.NumCustomizedMenuOptions = 0;
				this.CustomMenu = null;
			}
			this.WriteVariable("customizedMenuOptions", this.NumCustomizedMenuOptions);
			this.WriteVariable("aa_customizedMenuEnabled", this.CustomizedMenuConfigured);
			this.WriteVariable("aa_callSomeoneEnabled", this.Config.CallSomeoneEnabled);
			bool flag = this.Config.CallSomeoneEnabled || this.Config.SendVoiceMsgEnabled;
			this.WriteVariable("aa_contactSomeoneEnabled", flag);
			this.WriteVariable("aa_transferToOperatorEnabled", this.OperatorEnabled);
		}

		internal void ConfigureCustomMenuDtmfKeyInput(CustomMenuKeyMapping[] customExtensions)
		{
			int num = 0;
			foreach (CustomMenuKeyMapping customMenuKeyMapping in customExtensions)
			{
				num++;
				if (customMenuKeyMapping.MappedKey == CustomMenuKey.Timeout)
				{
					this.customMenuTimeoutEnabled = true;
					this.customMenuTimeoutOption = customMenuKeyMapping;
					this.WriteVariable("TimeoutOption", true);
				}
				else
				{
					num = (int)customMenuKeyMapping.MappedKey;
					this.WriteVariable(string.Format(CultureInfo.InvariantCulture, "DtmfKey{0}", new object[]
					{
						num
					}), true);
				}
			}
		}

		internal virtual void OnTimeout()
		{
			if (this.customMenuTimeoutEnabled)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "setting timeoutPending = true.", new object[0]);
				this.TimeoutPending = true;
			}
		}

		internal virtual void OnInput()
		{
			this.receivedInput = true;
		}

		internal virtual void OnHangup()
		{
			this.PerfCounters.UpdateAverageTimeCounters(this.startTime);
			if (!this.receivedInput)
			{
				this.VoiceObject.IncrementCounter(this.PerfCounters.GetInstance().DisconnectedWithoutInput);
			}
		}

		internal virtual void OnTransferComplete(TransferExtension ext)
		{
			this.PerfCounters.IncrementTransferCounters(ext);
			this.PerfCounters.UpdateAverageTimeCounters(this.startTime);
		}

		internal virtual void OnSpeech()
		{
			this.receivedInput = true;
		}

		internal virtual void OnNameSpoken()
		{
			this.PerfCounters.Increment(DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedBySpokenName);
			if (!this.nameSpokenAtleastOnce)
			{
				this.nameSpokenAtleastOnce = true;
				this.PerfCounters.IncrementNameSpokenCounter();
			}
		}

		internal string Action_PrepareForTransferToPaa()
		{
			if (this.targetPAA == null)
			{
				throw new InvalidOperationException("Got a NULL TargetPAA");
			}
			PIIMessage data = PIIMessage.Create(PIIType._PII, this.mailboxTransferTarget.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, data, "AutoAttendantCore::Action_PrepareForTransferToPaa: Target Mailbox = _PII, Target PAA = {0}", new object[]
			{
				this.targetPAA.Identity.ToString()
			});
			this.WriteGlobalVariable("TargetPAA", this.targetPAA);
			return null;
		}

		internal string Action_ProcessCustomMenuTransferToNumber()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "AutoAttendantCore::Action_ProcessCustomMenuTransferToNumber()", new object[0]);
			PhoneNumber customExtensionNumberToDial = AutoAttendantCore.GetCustomExtensionNumberToDial(this.CallContext, this.selectedMenu.Extension);
			if (customExtensionNumberToDial == null)
			{
				return "cannotTransferToCustomExtension";
			}
			PhoneUtil.SetTransferTargetPhone((ActivityManager)this.AutoAttendantManager, TransferExtension.CustomMenuExtension, customExtensionNumberToDial);
			return null;
		}

		internal string Action_PrepareForCallAnswering()
		{
			UMAutoAttendant autoAttendantInfo = this.CallContext.AutoAttendantInfo;
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Action_PrepareForCallAnswering: DefaultMailbox = {0}", new object[]
			{
				autoAttendantInfo.DefaultMailbox
			});
			if (!autoAttendantInfo.ForwardCallsToDefaultMailbox)
			{
				throw new InvalidOperationException("ForwardCallsToDefaultMailbox is false");
			}
			if (autoAttendantInfo.DefaultMailbox == null)
			{
				throw new InvalidOperationException("DefaultMailbox is null");
			}
			UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(autoAttendantInfo.DefaultMailbox);
			if (umsubscriber == null)
			{
				throw new InvalidOperationException("Could not create UMSubscriber from DefaultMailbox");
			}
			this.CallContext.SwitchToCallAnswering(umsubscriber);
			return null;
		}

		protected void WriteVariable(string name, object value)
		{
			this.AutoAttendantManager.WriteProperty(name, value);
		}

		protected object ReadVariable(string name)
		{
			return this.AutoAttendantManager.ReadProperty(name);
		}

		protected void WriteGlobalVariable(string name, object value)
		{
			this.AutoAttendantManager.WriteGlobalProperty(name, value);
		}

		protected object ReadGlobalVariable(string name)
		{
			return this.AutoAttendantManager.ReadGlobalProperty(name);
		}

		protected void IncrementKeyMappingPerfCounters(CustomMenuKey keyPress)
		{
			this.PerfCounters.IncrementCustomMenuCounters(keyPress);
		}

		protected string SwitchToAutoAttendant(string autoAttendantName)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "AutoAttendantCore::SwitchToAutoAttendant().", new object[0]);
			string result = null;
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(this.Config.OrganizationId);
			UMAutoAttendant autoAttendantFromName = iadsystemConfigurationLookup.GetAutoAttendantFromName(autoAttendantName);
			if (autoAttendantFromName != null && !this.CallContext.SwitchToAutoAttendant(autoAttendantFromName.Id, autoAttendantFromName.OrganizationId))
			{
				result = "fallbackAutoAttendantFailure";
			}
			return result;
		}

		private string SwitchToTargetPAA(string user)
		{
			ADRecipient adrecipient;
			string text = this.TryEvaluateUser(user, out adrecipient);
			if (text != null)
			{
				return text;
			}
			this.mailboxTransferTarget = adrecipient;
			bool flag = false;
			PersonalAutoAttendant personalAutoAttendant = null;
			BricksRoutingBasedServerChooser bricksRoutingBasedServerChooser = null;
			bool flag2 = PersonalAutoAttendantManager.TryGetTargetPAA(this.VoiceObject.CurrentCallContext, adrecipient, this.VoiceObject.CurrentCallContext.DialPlan, this.VoiceObject.CurrentCallContext.CallerId, out personalAutoAttendant, out flag, out bricksRoutingBasedServerChooser);
			PIIMessage data = PIIMessage.Create(PIIType._PII, this.mailboxTransferTarget.DisplayName);
			if (flag2)
			{
				this.targetPAA = personalAutoAttendant;
				CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, data, "AutoAttendantCore::SwitchToTargetPAA: Target Mailbox = _PII, Target PAA = {0}", new object[]
				{
					this.targetPAA.Identity.ToString()
				});
				this.VoiceObject.CurrentCallContext.CalleeInfo = UMRecipient.Factory.FromADRecipient<UMRecipient>(adrecipient);
				this.VoiceObject.CurrentCallContext.AsyncGetCallAnsweringData(false);
				return null;
			}
			if (flag)
			{
				ExAssert.RetailAssert(bricksRoutingBasedServerChooser != null, "ServerPicker cannot be null if transferToAnotherServer is needed");
				this.VoiceObject.CurrentCallContext.ServerPicker = bricksRoutingBasedServerChooser;
				UserTransferWithContext userTransferWithContext = new UserTransferWithContext(this.VoiceObject.CurrentCallContext.CallInfo.ApplicationAor);
				this.WriteGlobalVariable("ReferredByUri", userTransferWithContext.SerializeCACallTransferWithContextUri(adrecipient.UMExtension, this.VoiceObject.CurrentCallContext.DialPlan.PhoneContext));
				this.WriteGlobalVariable("transferExtension", TransferExtension.CustomMenuExtension);
				return "targetPAAInDifferentSite";
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, data, "AutoAttendantCore::SwitchToTargetPAA: NO PAA found for Target Mailbox = _PII", new object[0]);
			this.WriteGlobalVariable("directorySearchResult", ContactSearchItem.CreateFromRecipient(adrecipient));
			return "noPAAFound";
		}

		private string SwitchToVoicemailTarget(string user)
		{
			ADRecipient r;
			string text = this.TryEvaluateUser(user, out r);
			if (text == null)
			{
				this.WriteGlobalVariable("directorySearchResult", ContactSearchItem.CreateFromRecipient(r));
				return text;
			}
			return text;
		}

		private string TryEvaluateUser(string user, out ADRecipient recipient)
		{
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromOrganizationId(this.config.OrganizationId, null);
			recipient = iadrecipientLookup.LookupByLegacyExchangeDN(user);
			PIIMessage data = PIIMessage.Create(PIIType._User, user);
			if (recipient == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, data, "AutoAttendantCore::TryEvaluateUser: NO user found for = _User", new object[0]);
				return "noResultsMatched";
			}
			ADUser user2 = recipient as ADUser;
			if (!Utils.IsUserUMEnabledInGivenDialplan(user2, this.config.UMDialPlan))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, data, "AutoAttendantCore::TryEvaluateUser: user not umenabled or not in same dialplan = _User", new object[0]);
				return "invalidOption";
			}
			return null;
		}

		private void UpdateCountersOnTransferToAA(bool dtmfFallbackAA)
		{
			if (dtmfFallbackAA)
			{
				this.PerfCounters.IncrementTransfersToDtmfFallbackAutoAttendantCounter();
			}
			else
			{
				this.PerfCounters.IncrementTransferToKeyMappingAutoAttendantCounter();
			}
			this.PerfCounters.UpdateAverageTimeCounters(this.startTime);
		}

		private bool receivedInput;

		private CallContext callContext;

		private IAutoAttendantUI autoAttendantManager;

		private BaseUMCallSession voiceObject;

		private bool customMenuTimeoutEnabled;

		private CustomMenuKeyMapping customMenuTimeoutOption;

		private CustomMenuKeyMapping selectedMenu;

		private PersonalAutoAttendant targetPAA;

		private ADRecipient mailboxTransferTarget;

		private bool timeoutPending;

		private PhoneNumber operatorNumberConfigured;

		private bool transferToOperatorConfigured;

		private CustomMenuKeyMapping[] customMenu;

		private int numCustomizedMenuOptions;

		private bool nameLookupConfigured;

		private bool customizedMenuConfigured;

		private bool businessHoursCall;

		private UMAutoAttendant config;

		private AutoAttendantSettings settings;

		private HolidaySchedule holidaySchedule;

		private ExDateTime startTime;

		private bool nameSpokenAtleastOnce;

		private AutoAttendantCountersUtil aaCoutersUtil;
	}
}
