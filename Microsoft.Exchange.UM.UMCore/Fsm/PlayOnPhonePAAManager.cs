using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class PlayOnPhonePAAManager
	{
		internal static void GetScope(ActivityManager manager, out PlayOnPhonePAAManager scope)
		{
			for (scope = (manager as PlayOnPhonePAAManager); scope == null; scope = (manager as PlayOnPhonePAAManager))
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

		internal static TransitionBase GetAutoAttendant(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return manager.GetTransition(playOnPhonePAAManager.GetAutoAttendant(vo));
		}

		internal static TransitionBase PrepareToExecutePAA(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return manager.GetTransition(playOnPhonePAAManager.PrepareToExecutePAA(vo));
		}

		internal static TransitionBase GetGreeting(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return manager.GetTransition(playOnPhonePAAManager.GetGreeting(vo));
		}

		internal static TransitionBase DeleteGreeting(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return manager.GetTransition(playOnPhonePAAManager.DeleteGreeting(vo));
		}

		internal static TransitionBase ClearRecording(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return manager.GetTransition(playOnPhonePAAManager.ClearRecording(vo));
		}

		internal static TransitionBase SaveGreeting(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return manager.GetTransition(playOnPhonePAAManager.SaveGreeting(vo));
		}

		internal static object RecordedName(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.RecordedName;
		}

		internal static string Context1(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Context1;
		}

		internal static object TargetName1(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetName1;
		}

		internal static PhoneNumber TargetPhone1(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetPhone1;
		}

		internal static string Context2(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Context2;
		}

		internal static object TargetName2(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetName2;
		}

		internal static PhoneNumber TargetPhone2(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetPhone2;
		}

		internal static string Context3(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Context3;
		}

		internal static object TargetName3(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetName3;
		}

		internal static PhoneNumber TargetPhone3(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetPhone3;
		}

		internal static string Context4(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Context4;
		}

		internal static object TargetName4(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetName4;
		}

		internal static PhoneNumber TargetPhone4(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetPhone4;
		}

		internal static string Context5(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Context5;
		}

		internal static object TargetName5(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetName5;
		}

		internal static PhoneNumber TargetPhone5(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetPhone5;
		}

		internal static string Context6(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Context6;
		}

		internal static object TargetName6(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetName6;
		}

		internal static PhoneNumber TargetPhone6(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetPhone6;
		}

		internal static string Context7(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Context7;
		}

		internal static object TargetName7(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetName7;
		}

		internal static PhoneNumber TargetPhone7(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetPhone7;
		}

		internal static string Context8(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Context8;
		}

		internal static object TargetName8(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetName8;
		}

		internal static PhoneNumber TargetPhone8(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetPhone8;
		}

		internal static string Context9(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Context9;
		}

		internal static object TargetName9(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetName9;
		}

		internal static PhoneNumber TargetPhone9(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TargetPhone9;
		}

		internal static ITempWavFile PersonalGreeting(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.PersonalGreeting;
		}

		internal static ITempWavFile Recording(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Recording;
		}

		internal static object ValidPAA(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.ValidPAA;
		}

		internal static object HaveGreeting(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.HaveGreeting;
		}

		internal static object HaveActions(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.HaveActions;
		}

		internal static object Key1Enabled(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Key1Enabled;
		}

		internal static object MenuType1(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.MenuType1;
		}

		internal static object Key2Enabled(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Key2Enabled;
		}

		internal static object MenuType2(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.MenuType2;
		}

		internal static object Key3Enabled(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Key3Enabled;
		}

		internal static object MenuType3(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.MenuType3;
		}

		internal static object Key4Enabled(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Key4Enabled;
		}

		internal static object MenuType4(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.MenuType4;
		}

		internal static object Key5Enabled(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Key5Enabled;
		}

		internal static object MenuType5(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.MenuType5;
		}

		internal static object Key6Enabled(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Key6Enabled;
		}

		internal static object MenuType6(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.MenuType6;
		}

		internal static object Key7Enabled(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Key7Enabled;
		}

		internal static object MenuType7(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.MenuType7;
		}

		internal static object Key8Enabled(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Key8Enabled;
		}

		internal static object MenuType8(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.MenuType8;
		}

		internal static object Key9Enabled(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.Key9Enabled;
		}

		internal static object MenuType9(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.MenuType9;
		}

		internal static object TransferToVoiceMessageEnabled(ActivityManager manager, string variableName)
		{
			PlayOnPhonePAAManager playOnPhonePAAManager = manager as PlayOnPhonePAAManager;
			if (playOnPhonePAAManager == null)
			{
				PlayOnPhonePAAManager.GetScope(manager, out playOnPhonePAAManager);
			}
			return playOnPhonePAAManager.TransferToVoiceMessageEnabled;
		}
	}
}
