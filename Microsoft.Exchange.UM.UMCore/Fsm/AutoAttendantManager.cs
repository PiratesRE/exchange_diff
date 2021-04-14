using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class AutoAttendantManager
	{
		internal static void GetScope(ActivityManager manager, out AutoAttendantManager scope)
		{
			for (scope = (manager as AutoAttendantManager); scope == null; scope = (manager as AutoAttendantManager))
			{
				if (manager.Manager == null)
				{
					throw new FsmConfigurationException(string.Empty);
				}
				manager = manager.Manager;
			}
		}

		internal static TransitionBase Disconnect(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase HandleFaxTone(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForTransferToSendMessage(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForProtectedSubscriberOperatorTransfer(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForTransferToPaa(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase TransferToPAASiteFailed(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForTransferToKeyMappingAutoAttendant(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForUserInitiatedOperatorTransferFromOpeningMenu(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForUserInitiatedOperatorTransfer(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ProcessCustomMenuSelection(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ProcessResult(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase QuickMessage(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetCustomExtensionNumber(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetCustomMenuAutoAttendant(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetCustomMenuVoicemailTarget(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetCustomMenuTargetPAA(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetExtensionNumber(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetOperatorNumber(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetPromptProvContext(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForCallAnswering(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			AutoAttendantManager autoAttendantManager = manager as AutoAttendantManager;
			if (autoAttendantManager == null)
			{
				AutoAttendantManager.GetScope(manager, out autoAttendantManager);
			}
			return manager.GetTransition(autoAttendantManager.PrepareForCallAnswering(vo));
		}

		internal static TransitionBase CheckNonUmExtension(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			AutoAttendantManager autoAttendantManager = manager as AutoAttendantManager;
			if (autoAttendantManager == null)
			{
				AutoAttendantManager.GetScope(manager, out autoAttendantManager);
			}
			return manager.GetTransition(autoAttendantManager.CheckNonUmExtension(vo));
		}

		internal static object Aa_contactSomeoneEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object Aa_customizedMenuEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object Aa_isBusinessHours(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object Aa_transferToOperatorEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object AllowCall(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object AllowMessage(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static string NameOfDepartment1(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string NameOfDepartment2(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string NameOfDepartment3(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string NameOfDepartment4(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string NameOfDepartment5(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string NameOfDepartment6(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string NameOfDepartment7(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string NameOfDepartment8(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string NameOfDepartment9(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string NameOfDepartmentTimeOut(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string CustomMenuOptionPrompt(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object CustomMenuOption(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DirectorySearchEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DtmfKey1(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DtmfKey2(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DtmfKey3(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DtmfKey4(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DtmfKey5(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DtmfKey6(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DtmfKey7(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DtmfKey8(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DtmfKey9(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object HaveCustomMenuOptionPrompt(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object HolidayHours(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static string HolidayIntroductoryGreetingPrompt(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object InfoAnnouncementEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static string InfoAnnouncementFilename(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string InvalidExtension(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object MainMenuCustomPromptEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static string MainMenuCustomPromptFilename(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object TextPart(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object TimeoutOption(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object TuiPromptEditingEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object UserName(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static AutoAttendantContext AAContext(ActivityManager manager, string variableName)
		{
			AutoAttendantManager autoAttendantManager = manager as AutoAttendantManager;
			if (autoAttendantManager == null)
			{
				AutoAttendantManager.GetScope(manager, out autoAttendantManager);
			}
			return autoAttendantManager.AAContext;
		}

		internal static PhoneNumber TargetPhoneNumber(ActivityManager manager, string variableName)
		{
			AutoAttendantManager autoAttendantManager = manager as AutoAttendantManager;
			if (autoAttendantManager == null)
			{
				AutoAttendantManager.GetScope(manager, out autoAttendantManager);
			}
			return autoAttendantManager.TargetPhoneNumber;
		}

		internal static AutoAttendantLocationContext AALocationContext(ActivityManager manager, string variableName)
		{
			AutoAttendantManager autoAttendantManager = manager as AutoAttendantManager;
			if (autoAttendantManager == null)
			{
				AutoAttendantManager.GetScope(manager, out autoAttendantManager);
			}
			return autoAttendantManager.AALocationContext;
		}

		internal static UMAutoAttendant ThisAutoAttendant(ActivityManager manager, string variableName)
		{
			AutoAttendantManager autoAttendantManager = manager as AutoAttendantManager;
			if (autoAttendantManager == null)
			{
				AutoAttendantManager.GetScope(manager, out autoAttendantManager);
			}
			return autoAttendantManager.ThisAutoAttendant;
		}

		internal static object ForwardCallsToDefaultMailbox(ActivityManager manager, string variableName)
		{
			AutoAttendantManager autoAttendantManager = manager as AutoAttendantManager;
			if (autoAttendantManager == null)
			{
				AutoAttendantManager.GetScope(manager, out autoAttendantManager);
			}
			return autoAttendantManager.ForwardCallsToDefaultMailbox;
		}

		internal static object StarOutToDialPlanEnabled(ActivityManager manager, string variableName)
		{
			AutoAttendantManager autoAttendantManager = manager as AutoAttendantManager;
			if (autoAttendantManager == null)
			{
				AutoAttendantManager.GetScope(manager, out autoAttendantManager);
			}
			return autoAttendantManager.StarOutToDialPlanEnabled;
		}
	}
}
