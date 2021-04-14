using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class PlayOnPhoneAAManager
	{
		internal static void GetScope(ActivityManager manager, out PlayOnPhoneAAManager scope)
		{
			for (scope = (manager as PlayOnPhoneAAManager); scope == null; scope = (manager as PlayOnPhoneAAManager))
			{
				if (manager.Manager == null)
				{
					throw new FsmConfigurationException(string.Empty);
				}
				manager = manager.Manager;
			}
		}

		internal static TransitionBase ClearRecording(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase Disconnect(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetOperationResultFailed(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PlayOnPhoneAAManager playOnPhoneAAManager = manager as PlayOnPhoneAAManager;
			if (playOnPhoneAAManager == null)
			{
				PlayOnPhoneAAManager.GetScope(manager, out playOnPhoneAAManager);
			}
			return manager.GetTransition(playOnPhoneAAManager.SetOperationResultFailed(vo));
		}

		internal static TransitionBase ExistingGreetingAlreadyPlayed(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PlayOnPhoneAAManager playOnPhoneAAManager = manager as PlayOnPhoneAAManager;
			if (playOnPhoneAAManager == null)
			{
				PlayOnPhoneAAManager.GetScope(manager, out playOnPhoneAAManager);
			}
			return manager.GetTransition(playOnPhoneAAManager.ExistingGreetingAlreadyPlayed(vo));
		}

		internal static TransitionBase SaveGreeting(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PlayOnPhoneAAManager playOnPhoneAAManager = manager as PlayOnPhoneAAManager;
			if (playOnPhoneAAManager == null)
			{
				PlayOnPhoneAAManager.GetScope(manager, out playOnPhoneAAManager);
			}
			return manager.GetTransition(playOnPhoneAAManager.SaveGreeting(vo));
		}

		internal static ITempWavFile Recording(ActivityManager manager, string variableName)
		{
			return (ITempWavFile)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string ExistingFilePath(ActivityManager manager, string variableName)
		{
			PlayOnPhoneAAManager playOnPhoneAAManager = manager as PlayOnPhoneAAManager;
			if (playOnPhoneAAManager == null)
			{
				PlayOnPhoneAAManager.GetScope(manager, out playOnPhoneAAManager);
			}
			return playOnPhoneAAManager.ExistingFilePath;
		}

		internal static object FileExists(ActivityManager manager, string variableName)
		{
			PlayOnPhoneAAManager playOnPhoneAAManager = manager as PlayOnPhoneAAManager;
			if (playOnPhoneAAManager == null)
			{
				PlayOnPhoneAAManager.GetScope(manager, out playOnPhoneAAManager);
			}
			return playOnPhoneAAManager.FileExists;
		}

		internal static object PlayingExistingGreetingFirstTime(ActivityManager manager, string variableName)
		{
			PlayOnPhoneAAManager playOnPhoneAAManager = manager as PlayOnPhoneAAManager;
			if (playOnPhoneAAManager == null)
			{
				PlayOnPhoneAAManager.GetScope(manager, out playOnPhoneAAManager);
			}
			return playOnPhoneAAManager.PlayingExistingGreetingFirstTime;
		}
	}
}
