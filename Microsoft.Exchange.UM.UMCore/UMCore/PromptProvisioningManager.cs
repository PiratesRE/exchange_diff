using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PromptProvisioningManager : ActivityManager
	{
		internal PromptProvisioningManager(ActivityManager manager, PromptProvisioningManager.ConfigClass config) : base(manager, config)
		{
		}

		internal bool IsNullHolidayEndDate
		{
			get
			{
				return !this.hasHolidayEndDate;
			}
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			this.callSession = vo;
			this.InitGlobalVariable("promptProvContext");
			this.InitGlobalVariable("pilotNumberWelcomeGreetingEnabled");
			this.InitGlobalVariable("pilotNumberInfoAnnouncementEnabled");
			base.WriteVariable("selectedPrompt", null);
			base.WriteVariable("selectedPromptGroup", this.selectedPromptGroup.ToString());
			base.WriteVariable("holidayCount", 0);
			base.WriteVariable("playbackIndex", 0);
			string objA = (string)base.Manager.ReadVariable("promptProvContext");
			if (object.ReferenceEquals(objA, "DialPlan"))
			{
				this.SetDialPlanContext();
			}
			else
			{
				this.SetAutoAttendantContext();
			}
			this.configCache = vo.CurrentCallContext.UMConfigCache;
			base.Start(vo, refInfo);
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "PromptProvisioningManager.ExecuteAction({0}).", new object[]
			{
				action
			});
			this.callSession = vo;
			UMSubscriber callerInfo = this.callSession.CurrentCallContext.CallerInfo;
			string input;
			if (string.Equals(action, "setDialPlanContext", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SetDialPlanContext();
			}
			else if (string.Equals(action, "setAutoAttendantContext", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SetAutoAttendantContext();
			}
			else if (string.Equals(action, "canUpdatePrompts", StringComparison.OrdinalIgnoreCase))
			{
				input = this.CanUpdatePrompts(callerInfo);
			}
			else if (string.Equals(action, "publishPrompt", StringComparison.OrdinalIgnoreCase))
			{
				input = this.PublishPrompt();
			}
			else if (string.Equals(action, "prepareForPlayback", StringComparison.OrdinalIgnoreCase))
			{
				input = this.PrepareForPlayback();
			}
			else if (string.Equals(action, "selectAfterHoursGroup", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SelectGroupAfterHours();
			}
			else if (string.Equals(action, "selectBusinessHoursGroup", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SelectGroupBusinessHours();
			}
			else if (string.Equals(action, "selectPromptIndex", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SelectPromptIndex();
			}
			else if (string.Equals(action, "nextPlaybackIndex", StringComparison.OrdinalIgnoreCase))
			{
				input = this.NextPlaybackIndex();
			}
			else if (string.Equals(action, "resetPlaybackIndex", StringComparison.OrdinalIgnoreCase))
			{
				input = this.ResetPlaybackIndex();
			}
			else if (string.Equals(action, "selectNextHolidayPage", StringComparison.OrdinalIgnoreCase))
			{
				input = this.SelectNextHolidayPage();
			}
			else if (string.Equals(action, "exitPromptProvisioning", StringComparison.OrdinalIgnoreCase))
			{
				input = this.ExitPromptProvisioning();
			}
			else
			{
				if (!action.StartsWith("select", StringComparison.InvariantCulture))
				{
					return base.ExecuteAction(action, vo);
				}
				input = this.SelectPrompt(action);
			}
			return base.CurrentActivity.GetTransition(input);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PromptProvisioningManager>(this);
		}

		protected override string GetExceptionCategory(UMCallSessionEventArgs voEventArgs)
		{
			if (voEventArgs.Error is PublishingException)
			{
				return "publishingError";
			}
			return base.GetExceptionCategory(voEventArgs);
		}

		private static bool HasKeyMappings(MultiValuedProperty<CustomMenuKeyMapping> keyMapList)
		{
			if (keyMapList != null)
			{
				foreach (CustomMenuKeyMapping customMenuKeyMapping in keyMapList)
				{
					if (!string.IsNullOrEmpty(customMenuKeyMapping.PromptFileName))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private void InitGlobalVariable(string variableName)
		{
			base.WriteVariable(variableName, this.GlobalManager.ReadVariable(variableName));
		}

		private string ExitPromptProvisioning()
		{
			string objA = (string)this.ReadVariable("promptProvContext");
			if (object.ReferenceEquals(objA, "DialPlan"))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ExitPromptProvisioning: Restoring CallType to UnauthenticatedPilotNumber.", new object[0]);
				this.callSession.CurrentCallContext.CallType = 1;
			}
			else
			{
				if (!object.ReferenceEquals(objA, "AutoAttendant"))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ExitPromptProvisioning: Unexpected value in promptProvContext.", new object[0]);
					throw new ArgumentException("promptProvContext");
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ExitPromptProvisioning: Restoring CallType to AutoAttendant.", new object[0]);
				this.callSession.CurrentCallContext.CallType = 2;
			}
			return null;
		}

		private string SetDialPlanContext()
		{
			bool flag;
			bool flag2;
			this.GetDialPlanPromptStats(out flag, out flag2);
			base.WriteVariable("haveWelcomeGreeting", flag);
			base.WriteVariable("haveInfoAnnouncement", flag2);
			base.WriteVariable("haveDialPlanPrompts", flag || flag2);
			this.GlobalManager.WriteVariable("promptProvContext", "DialPlan");
			return null;
		}

		private string SetAutoAttendantContext()
		{
			UMAutoAttendant autoAttendantInfo = this.callSession.CurrentCallContext.AutoAttendantInfo;
			this.PrepareHolidayScheduleList();
			bool flag;
			bool flag2;
			bool flag3;
			this.GetBusinessHoursPromptStats(out flag, out flag2, out flag3);
			base.WriteVariable("haveBusinessHoursPrompts", flag || flag2 || flag3);
			this.GetAfterHoursPromptStats(out flag, out flag2, out flag3);
			base.WriteVariable("haveAfterHoursPrompts", flag || flag2 || flag3);
			base.WriteVariable("haveInfoAnnouncement", autoAttendantInfo.InfoAnnouncementEnabled != InfoAnnouncementEnabledEnum.False && !string.IsNullOrEmpty(autoAttendantInfo.InfoAnnouncementFilename));
			base.WriteVariable("haveHolidayPrompts", this.holidayList.Count > 0);
			bool flag4;
			this.GetDialPlanPromptStats(out flag, out flag4);
			base.WriteVariable("haveDialPlanPrompts", flag || flag4);
			base.WriteVariable("haveAutoAttendantPrompts", (bool)this.ReadVariable("haveBusinessHoursPrompts") || (bool)this.ReadVariable("haveAfterHoursPrompts") || (bool)this.ReadVariable("haveDialPlanPrompts") || (bool)this.ReadVariable("haveInfoAnnouncement") || (bool)this.ReadVariable("haveHolidayPrompts"));
			this.GlobalManager.WriteVariable("promptProvContext", "AutoAttendant");
			return null;
		}

		private string SelectGroupBusinessHours()
		{
			this.selectedPromptGroup = PromptProvisioningManager.PromptGroup.BusinessHours;
			base.WriteVariable("selectedPromptGroup", this.selectedPromptGroup.ToString());
			bool flag;
			bool flag2;
			bool flag3;
			this.GetBusinessHoursPromptStats(out flag, out flag2, out flag3);
			base.WriteVariable("haveWelcomeGreeting", flag);
			base.WriteVariable("haveKeyMapping", flag3);
			base.WriteVariable("haveMainMenu", flag2);
			return null;
		}

		private string SelectGroupAfterHours()
		{
			this.selectedPromptGroup = PromptProvisioningManager.PromptGroup.AfterHours;
			base.WriteVariable("selectedPromptGroup", this.selectedPromptGroup.ToString());
			bool flag;
			bool flag2;
			bool flag3;
			this.GetAfterHoursPromptStats(out flag, out flag2, out flag3);
			base.WriteVariable("haveWelcomeGreeting", flag);
			base.WriteVariable("haveKeyMapping", flag3);
			base.WriteVariable("haveMainMenu", flag2);
			return null;
		}

		private string SelectNextHolidayPage()
		{
			this.holidayPageStart += 9;
			base.WriteVariable("moreHolidaysAvailable", this.holidayPageStart + 9 < this.holidayList.Count);
			this.ResetPlaybackIndex();
			this.NextPlaybackIndex();
			return null;
		}

		private string ResetPlaybackIndex()
		{
			base.WriteVariable("playbackIndex", -1);
			return null;
		}

		private string NextPlaybackIndex()
		{
			string result = null;
			int num = (int)this.ReadVariable("playbackIndex");
			if (++num < this.holidayList.Count - this.holidayPageStart)
			{
				base.WriteVariable("playbackIndex", num);
				int index = this.holidayPageStart + num;
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "HolidayName:{0}, StartDate:{1}, EndDate: {2}.", new object[]
				{
					this.holidayList[index].Name,
					this.holidayList[index].StartDate,
					this.holidayList[index].EndDate
				});
				base.WriteVariable("holidayName", this.holidayList[index].Name);
				base.WriteVariable("holidayStartDate", (ExDateTime)this.holidayList[index].StartDate);
				if (this.holidayList[index].EndDate > this.holidayList[index].StartDate)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Holiday.StartDate>Holiday.HolidayEndDate -> Set HolidayEndDate.", new object[0]);
					base.WriteVariable("holidayEndDate", (ExDateTime)this.holidayList[index].EndDate);
					this.hasHolidayEndDate = true;
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Holiday.StartDate<=Holiday.HolidayEndDate -> Set HolidayEndDate = null.", new object[0]);
					this.hasHolidayEndDate = false;
				}
			}
			else
			{
				this.ResetPlaybackIndex();
				result = "endOfHolidayPage";
			}
			return result;
		}

		private string CanUpdatePrompts(UMSubscriber user)
		{
			ADUser aduser = (ADUser)user.ADRecipient;
			Exception ex = null;
			bool flag = false;
			try
			{
				PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, user.DisplayName);
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "CanUpdatePrompts: Creating a GenericIdentity for {0}.", new object[]
				{
					aduser
				});
				GenericIdentity identity = new GenericIdentity(aduser.Sid.ToString());
				ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = new ExchangeRunspaceConfiguration(identity);
				ADConfigurationObject currentConfig = this.GetCurrentConfig();
				string text = (currentConfig is UMAutoAttendant) ? "UMAutoAttendant" : "UMDialPlan";
				flag = exchangeRunspaceConfiguration.IsCmdletAllowedInScope("import-umprompt", new string[]
				{
					"PromptFileName",
					"PromptFileData",
					text
				}, aduser, ScopeLocation.RecipientWrite);
			}
			catch (UnauthorizedAccessException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (SystemException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				PIIMessage data2 = PIIMessage.Create(PIIType._UserDisplayName, user.DisplayName);
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data2, "PromptProvisioningManager.CanUpdatePrompts(_UserDisplayName): {0}.", new object[]
				{
					ex
				});
			}
			if (ex != null || !flag)
			{
				return null;
			}
			return "updatePromptsAllowed";
		}

		private string PrepareForPlayback()
		{
			ADConfigurationObject currentConfig = this.GetCurrentConfig();
			base.WriteVariable("selectedPrompt", this.configCache.GetPrompt<ADConfigurationObject>(currentConfig, this.selectedFileName));
			return null;
		}

		private string PublishPrompt()
		{
			ADConfigurationObject currentConfig = this.GetCurrentConfig();
			string filePath = base.RecordContext.Recording.FilePath;
			string displayName = this.callSession.CurrentCallContext.CallerInfo.ExchangePrincipal.MailboxInfo.DisplayName;
			using (IPublishingSession publishingSession = PublishingPoint.GetPublishingSession(displayName, currentConfig))
			{
				publishingSession.Upload(filePath, this.selectedFileName);
			}
			this.configCache.SetPrompt<ADConfigurationObject>(currentConfig, filePath, this.selectedFileName);
			return null;
		}

		private string SelectPrompt(string action)
		{
			ADConfigurationObject currentConfig = this.GetCurrentConfig();
			this.selectedPromptType = (PromptProvisioningManager.PromptType)Enum.Parse(typeof(PromptProvisioningManager.PromptType), action.Substring("select".Length));
			base.WriteVariable("selectedPromptType", this.selectedPromptType.ToString());
			if (currentConfig is UMDialPlan)
			{
				this.SelectDialPlanPrompt(this.selectedPromptGroup, this.selectedPromptType);
			}
			else if (this.selectedPromptType != PromptProvisioningManager.PromptType.KeyMapping && this.selectedPromptType != PromptProvisioningManager.PromptType.HolidaySchedule)
			{
				this.SelectAutoAttendantPrompt(this.selectedPromptGroup, this.selectedPromptType, null);
			}
			else if (this.selectedPromptType == PromptProvisioningManager.PromptType.HolidaySchedule)
			{
				this.holidayPageStart = 0;
				this.ResetPlaybackIndex();
				this.PrepareHolidayScheduleList();
				base.WriteVariable("holidayCount", this.holidayList.Count);
				base.WriteVariable("moreHolidaysAvailable", this.holidayPageStart + 9 < this.holidayList.Count);
			}
			return null;
		}

		private ADConfigurationObject GetCurrentConfig()
		{
			string objA = (string)base.Manager.ReadVariable("promptProvContext");
			if (object.ReferenceEquals(objA, "DialPlan"))
			{
				return this.callSession.CurrentCallContext.DialPlan;
			}
			return this.callSession.CurrentCallContext.AutoAttendantInfo;
		}

		private void PrepareHolidayScheduleList()
		{
			if (this.holidayList.Count > 0)
			{
				return;
			}
			ExTimeZone timeZone = null;
			UMAutoAttendant autoAttendantInfo = this.callSession.CurrentCallContext.AutoAttendantInfo;
			if (string.IsNullOrEmpty(autoAttendantInfo.TimeZone) || !ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(autoAttendantInfo.TimeZone, out timeZone))
			{
				timeZone = ExTimeZone.CurrentTimeZone;
			}
			ExDateTime now = ExDateTime.GetNow(timeZone);
			foreach (HolidaySchedule holidaySchedule in autoAttendantInfo.HolidaySchedule)
			{
				if ((ExDateTime)holidaySchedule.StartDate.Date >= now.Date && !string.IsNullOrEmpty(holidaySchedule.Name) && !string.IsNullOrEmpty(holidaySchedule.Greeting))
				{
					this.holidayList.Add(holidaySchedule);
				}
			}
			this.holidayList.Sort(new Comparison<HolidaySchedule>(this.CompareHolidaySchedules));
		}

		private int CompareHolidaySchedules(HolidaySchedule sched1, HolidaySchedule sched2)
		{
			if (sched1.StartDate < sched2.StartDate)
			{
				return -1;
			}
			if (sched1.StartDate > sched2.StartDate)
			{
				return 1;
			}
			return 0;
		}

		private string GetKeyMappingFileName(PromptProvisioningManager.PromptGroup promptGroup, string key)
		{
			UMAutoAttendant autoAttendantInfo = this.callSession.CurrentCallContext.AutoAttendantInfo;
			MultiValuedProperty<CustomMenuKeyMapping> multiValuedProperty;
			if (promptGroup == PromptProvisioningManager.PromptGroup.AfterHours)
			{
				multiValuedProperty = autoAttendantInfo.AfterHoursKeyMapping;
			}
			else
			{
				multiValuedProperty = autoAttendantInfo.BusinessHoursKeyMapping;
			}
			foreach (CustomMenuKeyMapping customMenuKeyMapping in multiValuedProperty)
			{
				if (customMenuKeyMapping.MappedKey != CustomMenuKey.Timeout)
				{
					if (customMenuKeyMapping.Key == key)
					{
						return customMenuKeyMapping.PromptFileName;
					}
				}
				else if (key == "#")
				{
					return customMenuKeyMapping.PromptFileName;
				}
			}
			return null;
		}

		private string SelectPromptIndex()
		{
			string text = (string)this.ReadVariable("lastInput");
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentException("lastInput");
			}
			string objA = (string)base.Manager.ReadVariable("promptProvContext");
			if (!object.ReferenceEquals(objA, "AutoAttendant"))
			{
				throw new ArgumentException("promptProvContext");
			}
			if (this.selectedPromptType != PromptProvisioningManager.PromptType.KeyMapping && this.selectedPromptType != PromptProvisioningManager.PromptType.HolidaySchedule)
			{
				throw new ArgumentException("selectedPromptType");
			}
			return this.SelectAutoAttendantPrompt(this.selectedPromptGroup, this.selectedPromptType, text);
		}

		private string SelectAutoAttendantPrompt(PromptProvisioningManager.PromptGroup promptGroup, PromptProvisioningManager.PromptType promptType, string promptKey)
		{
			string result = null;
			UMAutoAttendant autoAttendantInfo = this.callSession.CurrentCallContext.AutoAttendantInfo;
			switch (promptType)
			{
			case PromptProvisioningManager.PromptType.MainMenuCustomPrompt:
				if (this.selectedPromptGroup == PromptProvisioningManager.PromptGroup.AfterHours)
				{
					this.selectedFileName = autoAttendantInfo.AfterHoursMainMenuCustomPromptFilename;
				}
				else if (this.selectedPromptGroup == PromptProvisioningManager.PromptGroup.BusinessHours)
				{
					this.selectedFileName = autoAttendantInfo.BusinessHoursMainMenuCustomPromptFilename;
				}
				else
				{
					result = "invalidSelectedPrompt";
				}
				break;
			case PromptProvisioningManager.PromptType.InfoAnnouncement:
				this.selectedFileName = autoAttendantInfo.InfoAnnouncementFilename;
				break;
			case PromptProvisioningManager.PromptType.WelcomeGreeting:
				if (this.selectedPromptGroup == PromptProvisioningManager.PromptGroup.AfterHours)
				{
					this.selectedFileName = autoAttendantInfo.AfterHoursWelcomeGreetingFilename;
				}
				else if (this.selectedPromptGroup == PromptProvisioningManager.PromptGroup.BusinessHours)
				{
					this.selectedFileName = autoAttendantInfo.BusinessHoursWelcomeGreetingFilename;
				}
				else
				{
					result = "invalidSelectedPrompt";
				}
				break;
			case PromptProvisioningManager.PromptType.HolidaySchedule:
			{
				int num = this.holidayPageStart + int.Parse(promptKey, CultureInfo.InvariantCulture) - 1;
				if (num < 0 || num >= this.holidayList.Count)
				{
					return "invalidSelectedPrompt";
				}
				this.selectedFileName = this.holidayList[num].Greeting;
				break;
			}
			case PromptProvisioningManager.PromptType.KeyMapping:
				this.selectedFileName = this.GetKeyMappingFileName(promptGroup, promptKey);
				if (this.selectedFileName == null)
				{
					result = "invalidSelectedPrompt";
				}
				break;
			}
			return result;
		}

		private string SelectDialPlanPrompt(PromptProvisioningManager.PromptGroup promptGroup, PromptProvisioningManager.PromptType promptType)
		{
			string result = null;
			UMDialPlan dialPlan = this.callSession.CurrentCallContext.DialPlan;
			switch (promptType)
			{
			case PromptProvisioningManager.PromptType.InfoAnnouncement:
				this.selectedFileName = dialPlan.InfoAnnouncementFilename;
				break;
			case PromptProvisioningManager.PromptType.WelcomeGreeting:
				this.selectedFileName = dialPlan.WelcomeGreetingFilename;
				break;
			default:
				result = "invalidSelectedPrompt";
				break;
			}
			return result;
		}

		private void GetDialPlanPromptStats(out bool hasWelcomeGreeting, out bool hasInfoAnnouncement)
		{
			hasWelcomeGreeting = (hasInfoAnnouncement = false);
			UMDialPlan dialPlan = this.callSession.CurrentCallContext.DialPlan;
			if (dialPlan.WelcomeGreetingEnabled && !string.IsNullOrEmpty(dialPlan.WelcomeGreetingFilename))
			{
				hasWelcomeGreeting = true;
			}
			if (dialPlan.InfoAnnouncementEnabled != InfoAnnouncementEnabledEnum.False && !string.IsNullOrEmpty(dialPlan.InfoAnnouncementFilename))
			{
				hasInfoAnnouncement = true;
			}
		}

		private void GetBusinessHoursPromptStats(out bool hasWelcomeGreeting, out bool hasMainMenu, out bool hasKeyMappings)
		{
			hasWelcomeGreeting = (hasMainMenu = (hasKeyMappings = false));
			UMAutoAttendant autoAttendantInfo = this.callSession.CurrentCallContext.AutoAttendantInfo;
			if (autoAttendantInfo.BusinessHoursWelcomeGreetingEnabled && !string.IsNullOrEmpty(autoAttendantInfo.BusinessHoursWelcomeGreetingFilename))
			{
				hasWelcomeGreeting = true;
			}
			if (autoAttendantInfo.BusinessHoursMainMenuCustomPromptEnabled && !string.IsNullOrEmpty(autoAttendantInfo.BusinessHoursMainMenuCustomPromptFilename))
			{
				hasMainMenu = true;
			}
			if (autoAttendantInfo.BusinessHoursKeyMappingEnabled)
			{
				hasKeyMappings = PromptProvisioningManager.HasKeyMappings(autoAttendantInfo.BusinessHoursKeyMapping);
			}
		}

		private void GetAfterHoursPromptStats(out bool hasWelcomeGreeting, out bool hasMainMenu, out bool hasKeyMappings)
		{
			hasWelcomeGreeting = (hasMainMenu = (hasKeyMappings = false));
			UMAutoAttendant autoAttendantInfo = this.callSession.CurrentCallContext.AutoAttendantInfo;
			if (autoAttendantInfo.AfterHoursWelcomeGreetingEnabled && !string.IsNullOrEmpty(autoAttendantInfo.AfterHoursWelcomeGreetingFilename))
			{
				hasWelcomeGreeting = true;
			}
			if (autoAttendantInfo.AfterHoursMainMenuCustomPromptEnabled && !string.IsNullOrEmpty(autoAttendantInfo.AfterHoursMainMenuCustomPromptFilename))
			{
				hasMainMenu = true;
			}
			if (autoAttendantInfo.AfterHoursKeyMappingEnabled)
			{
				hasKeyMappings = PromptProvisioningManager.HasKeyMappings(autoAttendantInfo.AfterHoursKeyMapping);
			}
		}

		internal const string DialPlanContext = "DialPlan";

		internal const string AutoAttendantContext = "AutoAttendant";

		private const string SelectActionBase = "select";

		private const int HolidayPageSize = 9;

		private BaseUMCallSession callSession;

		private string selectedFileName;

		private PromptProvisioningManager.PromptType selectedPromptType;

		private PromptProvisioningManager.PromptGroup selectedPromptGroup;

		private int holidayPageStart;

		private List<HolidaySchedule> holidayList = new List<HolidaySchedule>();

		private bool hasHolidayEndDate;

		private UMConfigCache configCache;

		private enum PromptGroup
		{
			None,
			BusinessHours,
			AfterHours
		}

		private enum PromptType
		{
			MainMenuCustomPrompt,
			InfoAnnouncement,
			WelcomeGreeting,
			HolidaySchedule,
			KeyMapping
		}

		internal class ConfigClass : ActivityManagerConfig
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing PromptProvisioningManager.", new object[0]);
				return new PromptProvisioningManager(manager, this);
			}
		}
	}
}
