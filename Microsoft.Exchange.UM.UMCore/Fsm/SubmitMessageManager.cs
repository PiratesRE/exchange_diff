using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class SubmitMessageManager
	{
		internal static void GetScope(ActivityManager manager, out SubmitMessageManager scope)
		{
			for (scope = (manager as SubmitMessageManager); scope == null; scope = (manager as SubmitMessageManager))
			{
				if (manager.Manager == null)
				{
					throw new FsmConfigurationException(string.Empty);
				}
				manager = manager.Manager;
			}
		}

		internal static TransitionBase AddRecipientBySearch(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase AppendRecording(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase CancelMessage(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ClearRecording(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase Disconnect(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase RemoveRecipient(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SendMessage(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SendMessageUrgent(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ToggleImportance(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			SubmitMessageManager submitMessageManager = manager as SubmitMessageManager;
			if (submitMessageManager == null)
			{
				SubmitMessageManager.GetScope(manager, out submitMessageManager);
			}
			return manager.GetTransition(submitMessageManager.ToggleImportance(vo));
		}

		internal static TransitionBase TogglePrivacy(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			SubmitMessageManager submitMessageManager = manager as SubmitMessageManager;
			if (submitMessageManager == null)
			{
				SubmitMessageManager.GetScope(manager, out submitMessageManager);
			}
			return manager.GetTransition(submitMessageManager.TogglePrivacy(vo));
		}

		internal static TransitionBase ClearSelection(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			SubmitMessageManager submitMessageManager = manager as SubmitMessageManager;
			if (submitMessageManager == null)
			{
				SubmitMessageManager.GetScope(manager, out submitMessageManager);
			}
			return manager.GetTransition(submitMessageManager.ClearSelection(vo));
		}

		internal static TransitionBase SendMessagePrivate(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			SubmitMessageManager submitMessageManager = manager as SubmitMessageManager;
			if (submitMessageManager == null)
			{
				SubmitMessageManager.GetScope(manager, out submitMessageManager);
			}
			return manager.GetTransition(submitMessageManager.SendMessagePrivate(vo));
		}

		internal static TransitionBase SendMessagePrivateAndUrgent(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			SubmitMessageManager submitMessageManager = manager as SubmitMessageManager;
			if (submitMessageManager == null)
			{
				SubmitMessageManager.GetScope(manager, out submitMessageManager);
			}
			return manager.GetTransition(submitMessageManager.SendMessagePrivateAndUrgent(vo));
		}

		internal static object LastActivity(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object NumRecipients(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static ITempWavFile Recording(ActivityManager manager, string variableName)
		{
			return (ITempWavFile)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object RecordingTimedOut(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object UserName(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DrmIsEnabled(ActivityManager manager, string variableName)
		{
			SubmitMessageManager submitMessageManager = manager as SubmitMessageManager;
			if (submitMessageManager == null)
			{
				SubmitMessageManager.GetScope(manager, out submitMessageManager);
			}
			return submitMessageManager.DrmIsEnabled;
		}

		internal static object IsSentImportant(ActivityManager manager, string variableName)
		{
			SubmitMessageManager submitMessageManager = manager as SubmitMessageManager;
			if (submitMessageManager == null)
			{
				SubmitMessageManager.GetScope(manager, out submitMessageManager);
			}
			return submitMessageManager.IsSentImportant;
		}

		internal static object MessageMarkedPrivate(ActivityManager manager, string variableName)
		{
			SubmitMessageManager submitMessageManager = manager as SubmitMessageManager;
			if (submitMessageManager == null)
			{
				SubmitMessageManager.GetScope(manager, out submitMessageManager);
			}
			return submitMessageManager.MessageMarkedPrivate;
		}
	}
}
