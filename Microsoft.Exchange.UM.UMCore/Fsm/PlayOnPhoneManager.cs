using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class PlayOnPhoneManager
	{
		internal static void GetScope(ActivityManager manager, out PlayOnPhoneManager scope)
		{
			for (scope = (manager as PlayOnPhoneManager); scope == null; scope = (manager as PlayOnPhoneManager))
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

		internal static TransitionBase DeleteExternal(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase DeleteOof(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase Disconnect(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetExternal(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetOof(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetPlayOnPhoneType(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ResetCallType(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SaveExternal(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SaveOof(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SetOperationResultFailed(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			PlayOnPhoneManager playOnPhoneManager = manager as PlayOnPhoneManager;
			if (playOnPhoneManager == null)
			{
				PlayOnPhoneManager.GetScope(manager, out playOnPhoneManager);
			}
			return manager.GetTransition(playOnPhoneManager.SetOperationResultFailed(vo));
		}

		internal static ITempWavFile Greeting(ActivityManager manager, string variableName)
		{
			return (ITempWavFile)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object GreetingType(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object NormalCustom(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object OofCustom(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static ITempWavFile Recording(ActivityManager manager, string variableName)
		{
			return (ITempWavFile)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object ProtectedMessage(ActivityManager manager, string variableName)
		{
			PlayOnPhoneManager playOnPhoneManager = manager as PlayOnPhoneManager;
			if (playOnPhoneManager == null)
			{
				PlayOnPhoneManager.GetScope(manager, out playOnPhoneManager);
			}
			return playOnPhoneManager.ProtectedMessage;
		}
	}
}
