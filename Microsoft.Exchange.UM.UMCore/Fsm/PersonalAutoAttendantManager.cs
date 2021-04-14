using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class PersonalAutoAttendantManager
	{
		internal static void GetScope(ActivityManager manager, out PersonalAutoAttendantManager scope)
		{
			for (scope = (manager as PersonalAutoAttendantManager); scope == null; scope = (manager as PersonalAutoAttendantManager))
			{
				if (manager.Manager == null)
				{
					throw new FsmConfigurationException(string.Empty);
				}
				manager = manager.Manager;
			}
		}

		internal static TransitionBase QuickMessage(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase Disconnect(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetAutoAttendant(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.GetAutoAttendant(vo));
		}

		internal static TransitionBase PrepareToExecutePAA(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.PrepareToExecutePAA(vo));
		}

		internal static TransitionBase PrepareForVoiceMail(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.PrepareForVoiceMail(vo));
		}

		internal static TransitionBase SelectNextAction(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.SelectNextAction(vo));
		}

		internal static TransitionBase GetGreeting(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.GetGreeting(vo));
		}

		internal static TransitionBase ProcessSelection(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.ProcessSelection(vo));
		}

		internal static TransitionBase HandleTimeout(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.HandleTimeout(vo));
		}

		internal static TransitionBase PrepareForTransfer(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.PrepareForTransfer(vo));
		}

		internal static TransitionBase PrepareForTransferToVoicemail(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.PrepareForTransferToVoicemail(vo));
		}

		internal static TransitionBase PrepareForFindMe(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.PrepareForFindMe(vo));
		}

		internal static TransitionBase PrepareForTransferToMailbox(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.PrepareForTransferToMailbox(vo));
		}

		internal static TransitionBase Reset(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.Reset(vo));
		}

		internal static TransitionBase PrepareForTransferToPaa(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.PrepareForTransferToPaa(vo));
		}

		internal static TransitionBase TransferToPAASiteFailed(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.TransferToPAASiteFailed(vo));
		}

		internal static TransitionBase ContinueFindMe(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.ContinueFindMe(vo));
		}

		internal static TransitionBase CleanupFindMe(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.CleanupFindMe(vo));
		}

		internal static TransitionBase StartFindMe(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.StartFindMe(vo));
		}

		internal static TransitionBase SetOperatorNumber(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.SetOperatorNumber(vo));
		}

		internal static TransitionBase TerminateFindMe(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return manager.GetTransition(personalAutoAttendantManager.TerminateFindMe(vo));
		}

		internal static ITempWavFile PersonalGreeting(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.PersonalGreeting;
		}

		internal static object RecordedName(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.RecordedName;
		}

		internal static string Context1(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Context1;
		}

		internal static object TargetName1(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetName1;
		}

		internal static PhoneNumber TargetPhone1(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetPhone1;
		}

		internal static string Context2(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Context2;
		}

		internal static object TargetName2(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetName2;
		}

		internal static PhoneNumber TargetPhone2(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetPhone2;
		}

		internal static string Context3(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Context3;
		}

		internal static object TargetName3(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetName3;
		}

		internal static PhoneNumber TargetPhone3(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetPhone3;
		}

		internal static string Context4(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Context4;
		}

		internal static object TargetName4(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetName4;
		}

		internal static PhoneNumber TargetPhone4(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetPhone4;
		}

		internal static string Context5(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Context5;
		}

		internal static object TargetName5(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetName5;
		}

		internal static PhoneNumber TargetPhone5(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetPhone5;
		}

		internal static string Context6(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Context6;
		}

		internal static object TargetName6(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetName6;
		}

		internal static PhoneNumber TargetPhone6(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetPhone6;
		}

		internal static string Context7(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Context7;
		}

		internal static object TargetName7(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetName7;
		}

		internal static PhoneNumber TargetPhone7(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetPhone7;
		}

		internal static string Context8(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Context8;
		}

		internal static object TargetName8(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetName8;
		}

		internal static PhoneNumber TargetPhone8(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetPhone8;
		}

		internal static string Context9(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Context9;
		}

		internal static object TargetName9(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetName9;
		}

		internal static PhoneNumber TargetPhone9(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetPhone9;
		}

		internal static PhoneNumber TargetPhoneNumber(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetPhoneNumber;
		}

		internal static object EvaluationStatus(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.EvaluationStatus;
		}

		internal static object HaveAutoActions(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.HaveAutoActions;
		}

		internal static object MainMenuUninterruptible(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.MainMenuUninterruptible;
		}

		internal static object HaveActions(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.HaveActions;
		}

		internal static object LastActivity(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.LastActivity;
		}

		internal static object HaveGreeting(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.HaveGreeting;
		}

		internal static object Key1Enabled(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Key1Enabled;
		}

		internal static object MenuType1(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.MenuType1;
		}

		internal static object Key2Enabled(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Key2Enabled;
		}

		internal static object MenuType2(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.MenuType2;
		}

		internal static object Key3Enabled(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Key3Enabled;
		}

		internal static object MenuType3(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.MenuType3;
		}

		internal static object Key4Enabled(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Key4Enabled;
		}

		internal static object MenuType4(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.MenuType4;
		}

		internal static object Key5Enabled(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Key5Enabled;
		}

		internal static object MenuType5(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.MenuType5;
		}

		internal static object Key6Enabled(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Key6Enabled;
		}

		internal static object MenuType6(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.MenuType6;
		}

		internal static object Key7Enabled(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Key7Enabled;
		}

		internal static object MenuType7(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.MenuType7;
		}

		internal static object Key8Enabled(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Key8Enabled;
		}

		internal static object MenuType8(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.MenuType8;
		}

		internal static object Key9Enabled(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.Key9Enabled;
		}

		internal static object MenuType9(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.MenuType9;
		}

		internal static object TransferToVoiceMessageEnabled(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TransferToVoiceMessageEnabled;
		}

		internal static object TimeOut(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TimeOut;
		}

		internal static object HavePersonalOperator(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.HavePersonalOperator;
		}

		internal static object ExecuteBlindTransfer(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.ExecuteBlindTransfer;
		}

		internal static object PermissionCheckFailure(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.PermissionCheckFailure;
		}

		internal static object ExecuteTransferToVoiceMessage(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.ExecuteTransferToVoiceMessage;
		}

		internal static object FindMeEnabled(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.FindMeEnabled;
		}

		internal static object ExecuteTransferToMailbox(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.ExecuteTransferToMailbox;
		}

		internal static object InvalidADContact(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.InvalidADContact;
		}

		internal static object TargetHasValidPAA(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetHasValidPAA;
		}

		internal static object TargetPAAInDifferentSite(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.TargetPAAInDifferentSite;
		}

		internal static object CallerIsResolvedToADContact(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.CallerIsResolvedToADContact;
		}

		internal static object RecordedNameOfCaller(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.RecordedNameOfCaller;
		}

		internal static object IsFirstFindMeTry(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.IsFirstFindMeTry;
		}

		internal static object FindMeSuccessful(ActivityManager manager, string variableName)
		{
			PersonalAutoAttendantManager personalAutoAttendantManager = manager as PersonalAutoAttendantManager;
			if (personalAutoAttendantManager == null)
			{
				PersonalAutoAttendantManager.GetScope(manager, out personalAutoAttendantManager);
			}
			return personalAutoAttendantManager.FindMeSuccessful;
		}
	}
}
