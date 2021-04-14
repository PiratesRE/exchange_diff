using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class GlobalActivityManager
	{
		internal static void GetScope(ActivityManager manager, out GlobalActivityManager scope)
		{
			for (scope = (manager as GlobalActivityManager); scope == null; scope = (manager as GlobalActivityManager))
			{
				if (manager.Manager == null)
				{
					throw new FsmConfigurationException(string.Empty);
				}
				manager = manager.Manager;
			}
		}

		internal static TransitionBase ClearCaller(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase CreateCallee(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase Disconnect(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase DoLogon(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetExtension(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetName(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetSummaryInfo(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase HandleCallSomeone(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase OofShortcut(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase QuickMessage(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetInitialSearchTargetContacts(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetInitialSearchTargetGAL(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetPromptProvContext(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase StopASR(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ValidateCaller(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ValidateMailbox(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForTransferToServer(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return manager.GetTransition(globalActivityManager.PrepareForTransferToServer(vo));
		}

		internal static TransitionBase FillCallerInfo(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return manager.GetTransition(globalActivityManager.FillCallerInfo(vo));
		}

		internal static TransitionBase HandleIsPinRequired(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return manager.GetTransition(globalActivityManager.HandleIsPinRequired(vo));
		}

		internal static AutoAttendantContext AAContext(ActivityManager manager, string variableName)
		{
			return (AutoAttendantContext)(manager.ReadVariable(variableName) ?? null);
		}

		internal static AutoAttendantLocationContext AALocationContext(ActivityManager manager, string variableName)
		{
			return (AutoAttendantLocationContext)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string BusinessAddress(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object BusinessName(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static UMAutoAttendant BusinessSchedule(ActivityManager manager, string variableName)
		{
			return (UMAutoAttendant)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object CalendarAccessEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object ContactSomeoneEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object CurrentActivity(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static AutoAttendantContext CustomMenu(ActivityManager manager, string variableName)
		{
			return (AutoAttendantContext)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string CustomPrompt(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string DepartmentName(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object EmailAccessEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static DayOfWeekTimeContext EndDay(ActivityManager manager, string variableName)
		{
			return (DayOfWeekTimeContext)(manager.ReadVariable(variableName) ?? null);
		}

		internal static DayOfWeekTimeContext EndDayTime(ActivityManager manager, string variableName)
		{
			return (DayOfWeekTimeContext)(manager.ReadVariable(variableName) ?? null);
		}

		internal static ExDateTime EndTime(ActivityManager manager, string variableName)
		{
			object obj;
			if ((obj = manager.ReadVariable(variableName)) == null)
			{
				obj = default(ExDateTime);
			}
			return (ExDateTime)obj;
		}

		internal static ITempWavFile CustomGreeting(ActivityManager manager, string variableName)
		{
			return (ITempWavFile)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object HaveSummary(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object IsInProgress(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object IsMaxEmail(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object LastActivity(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static string LastInput(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object LastRecoEvent(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static string Location(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object MainMenuQA(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static int NumEmail(ActivityManager manager, string variableName)
		{
			return (int)(manager.ReadVariable(variableName) ?? 0);
		}

		internal static int NumEmailMax(ActivityManager manager, string variableName)
		{
			return (int)(manager.ReadVariable(variableName) ?? 0);
		}

		internal static int NumMeetings(ActivityManager manager, string variableName)
		{
			return (int)(manager.ReadVariable(variableName) ?? 0);
		}

		internal static int NumVoicemail(ActivityManager manager, string variableName)
		{
			return (int)(manager.ReadVariable(variableName) ?? 0);
		}

		internal static object OcFeature(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object Oof(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object PilotNumberInfoAnnouncementEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static string PilotNumberInfoAnnouncementFilename(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object PilotNumberInfoAnnouncementInterruptible(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object PilotNumberWelcomeGreetingEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static string PilotNumberWelcomeGreetingFilename(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object Repeat(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static List<string> SelectableDepartments(ActivityManager manager, string variableName)
		{
			return (List<string>)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string SelectedMenu(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static DayOfWeekTimeContext StartDay(ActivityManager manager, string variableName)
		{
			return (DayOfWeekTimeContext)(manager.ReadVariable(variableName) ?? null);
		}

		internal static DayOfWeekTimeContext StartDayTime(ActivityManager manager, string variableName)
		{
			return (DayOfWeekTimeContext)(manager.ReadVariable(variableName) ?? null);
		}

		internal static ExDateTime StartTime(ActivityManager manager, string variableName)
		{
			object obj;
			if ((obj = manager.ReadVariable(variableName)) == null)
			{
				obj = default(ExDateTime);
			}
			return (ExDateTime)obj;
		}

		internal static object SkipPinCheck(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static UMAutoAttendant ThisAutoAttendant(ActivityManager manager, string variableName)
		{
			return (UMAutoAttendant)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object TuiPromptEditingEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object UseAsr(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object UserName(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object WaitForSourcePartyInfo(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static List<ScheduleGroup> VarScheduleGroupList(ActivityManager manager, string variableName)
		{
			return (List<ScheduleGroup>)(manager.ReadVariable(variableName) ?? null);
		}

		internal static List<TimeRange> VarScheduleIntervalList(ActivityManager manager, string variableName)
		{
			return (List<TimeRange>)(manager.ReadVariable(variableName) ?? null);
		}

		internal static ExTimeZone VarTimeZone(ActivityManager manager, string variableName)
		{
			return (ExTimeZone)(manager.ReadVariable(variableName) ?? null);
		}

		internal static PhoneNumber TargetPhoneNumber(ActivityManager manager, string variableName)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return globalActivityManager.TargetPhoneNumber;
		}

		internal static object SkipInitialGreetings(ActivityManager manager, string variableName)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return globalActivityManager.SkipInitialGreetings;
		}

		internal static object ConsumerDialPlan(ActivityManager manager, string variableName)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return globalActivityManager.ConsumerDialPlan;
		}

		internal static object DialPlanType(ActivityManager manager, string variableName)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return globalActivityManager.DialPlanType;
		}

		internal static object ContactsAccessEnabled(ActivityManager manager, string variableName)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return globalActivityManager.ContactsAccessEnabled;
		}

		internal static object DirectoryAccessEnabled(ActivityManager manager, string variableName)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return globalActivityManager.DirectoryAccessEnabled;
		}

		internal static object AddressBookEnabled(ActivityManager manager, string variableName)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return globalActivityManager.AddressBookEnabled;
		}

		internal static object VoiceResponseToOtherMessageTypesEnabled(ActivityManager manager, string variableName)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return globalActivityManager.VoiceResponseToOtherMessageTypesEnabled;
		}

		internal static object IsPromptProvisioningCall(ActivityManager manager, string variableName)
		{
			GlobalActivityManager globalActivityManager = manager as GlobalActivityManager;
			if (globalActivityManager == null)
			{
				GlobalActivityManager.GetScope(manager, out globalActivityManager);
			}
			return globalActivityManager.IsPromptProvisioningCall;
		}
	}
}
