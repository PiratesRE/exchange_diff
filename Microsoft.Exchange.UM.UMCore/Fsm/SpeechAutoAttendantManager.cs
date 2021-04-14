using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class SpeechAutoAttendantManager
	{
		internal static void GetScope(ActivityManager manager, out SpeechAutoAttendantManager scope)
		{
			for (scope = (manager as SpeechAutoAttendantManager); scope == null; scope = (manager as SpeechAutoAttendantManager))
			{
				if (manager.Manager == null)
				{
					throw new FsmConfigurationException(string.Empty);
				}
				manager = manager.Manager;
			}
		}

		internal static TransitionBase CanonicalizeNumber(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase CheckDialPermissions(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase CheckRestrictedUser(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase Disconnect(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase InitializeNamesGrammar(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase InitializeState(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForProtectedSubscriberOperatorTransfer(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForTransferToSendMessage(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForTransferToKeyMappingAutoAttendant(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForTransferToDtmfFallbackAutoAttendant(ActivityManager manager, string methodName, BaseUMCallSession vo)
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

		internal static TransitionBase ProcessResult(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase QuickMessage(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase RetryAsrSearch(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetCustomExtensionNumber(ActivityManager manager, string methodName, BaseUMCallSession vo)
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

		internal static TransitionBase SetCustomMenuAutoAttendant(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetFallbackAutoAttendant(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetOperatorNumber(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase PrepareForCallAnswering(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			SpeechAutoAttendantManager speechAutoAttendantManager = manager as SpeechAutoAttendantManager;
			if (speechAutoAttendantManager == null)
			{
				SpeechAutoAttendantManager.GetScope(manager, out speechAutoAttendantManager);
			}
			return manager.GetTransition(speechAutoAttendantManager.PrepareForCallAnswering(vo));
		}

		internal static TransitionBase EnableMainMenuRepetition(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			SpeechAutoAttendantManager speechAutoAttendantManager = manager as SpeechAutoAttendantManager;
			if (speechAutoAttendantManager == null)
			{
				SpeechAutoAttendantManager.GetScope(manager, out speechAutoAttendantManager);
			}
			return manager.GetTransition(speechAutoAttendantManager.EnableMainMenuRepetition(vo));
		}

		internal static TransitionBase DisableMainMenuRepetition(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			SpeechAutoAttendantManager speechAutoAttendantManager = manager as SpeechAutoAttendantManager;
			if (speechAutoAttendantManager == null)
			{
				SpeechAutoAttendantManager.GetScope(manager, out speechAutoAttendantManager);
			}
			return manager.GetTransition(speechAutoAttendantManager.DisableMainMenuRepetition(vo));
		}

		internal static object Aa_customizedMenuEnabled(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object Aa_goto_dtmf_autoattendant(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object Aa_goto_operator(ActivityManager manager, string variableName)
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

		internal static string CustomMenuOptionPrompt(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object CustomMenuOption(ActivityManager manager, string variableName)
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

		internal static object ResultTypeString(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object UserName(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static AutoAttendantContext AAContext(ActivityManager manager, string variableName)
		{
			SpeechAutoAttendantManager speechAutoAttendantManager = manager as SpeechAutoAttendantManager;
			if (speechAutoAttendantManager == null)
			{
				SpeechAutoAttendantManager.GetScope(manager, out speechAutoAttendantManager);
			}
			return speechAutoAttendantManager.AAContext;
		}

		internal static PhoneNumber TargetPhoneNumber(ActivityManager manager, string variableName)
		{
			SpeechAutoAttendantManager speechAutoAttendantManager = manager as SpeechAutoAttendantManager;
			if (speechAutoAttendantManager == null)
			{
				SpeechAutoAttendantManager.GetScope(manager, out speechAutoAttendantManager);
			}
			return speechAutoAttendantManager.TargetPhoneNumber;
		}

		internal static AutoAttendantLocationContext AALocationContext(ActivityManager manager, string variableName)
		{
			SpeechAutoAttendantManager speechAutoAttendantManager = manager as SpeechAutoAttendantManager;
			if (speechAutoAttendantManager == null)
			{
				SpeechAutoAttendantManager.GetScope(manager, out speechAutoAttendantManager);
			}
			return speechAutoAttendantManager.AALocationContext;
		}

		internal static UMAutoAttendant ThisAutoAttendant(ActivityManager manager, string variableName)
		{
			SpeechAutoAttendantManager speechAutoAttendantManager = manager as SpeechAutoAttendantManager;
			if (speechAutoAttendantManager == null)
			{
				SpeechAutoAttendantManager.GetScope(manager, out speechAutoAttendantManager);
			}
			return speechAutoAttendantManager.ThisAutoAttendant;
		}

		internal static object ForwardCallsToDefaultMailbox(ActivityManager manager, string variableName)
		{
			SpeechAutoAttendantManager speechAutoAttendantManager = manager as SpeechAutoAttendantManager;
			if (speechAutoAttendantManager == null)
			{
				SpeechAutoAttendantManager.GetScope(manager, out speechAutoAttendantManager);
			}
			return speechAutoAttendantManager.ForwardCallsToDefaultMailbox;
		}
	}
}
