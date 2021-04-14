using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class RecordVoicemailManager
	{
		internal static void GetScope(ActivityManager manager, out RecordVoicemailManager scope)
		{
			for (scope = (manager as RecordVoicemailManager); scope == null; scope = (manager as RecordVoicemailManager))
			{
				if (manager.Manager == null)
				{
					throw new FsmConfigurationException(string.Empty);
				}
				manager = manager.Manager;
			}
		}

		internal static TransitionBase AppendRecording(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase CanAnnonLeaveMessage(ActivityManager manager, string methodName, BaseUMCallSession vo)
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

		internal static TransitionBase GetGreeting(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetName(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase IsQuotaExceeded(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase IsPipelineHealthy(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SubmitVoiceMail(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SubmitVoiceMailUrgent(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ToggleImportance(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			RecordVoicemailManager recordVoicemailManager = manager as RecordVoicemailManager;
			if (recordVoicemailManager == null)
			{
				RecordVoicemailManager.GetScope(manager, out recordVoicemailManager);
			}
			return manager.GetTransition(recordVoicemailManager.ToggleImportance(vo));
		}

		internal static TransitionBase TogglePrivacy(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			RecordVoicemailManager recordVoicemailManager = manager as RecordVoicemailManager;
			if (recordVoicemailManager == null)
			{
				RecordVoicemailManager.GetScope(manager, out recordVoicemailManager);
			}
			return manager.GetTransition(recordVoicemailManager.TogglePrivacy(vo));
		}

		internal static TransitionBase ClearSelection(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			RecordVoicemailManager recordVoicemailManager = manager as RecordVoicemailManager;
			if (recordVoicemailManager == null)
			{
				RecordVoicemailManager.GetScope(manager, out recordVoicemailManager);
			}
			return manager.GetTransition(recordVoicemailManager.ClearSelection(vo));
		}

		internal static ITempWavFile Greeting(ActivityManager manager, string variableName)
		{
			return (ITempWavFile)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object Mode(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object OcFeature(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static PhoneNumber OperatorNumber(ActivityManager manager, string variableName)
		{
			return (PhoneNumber)(manager.ReadVariable(variableName) ?? null);
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

		internal static object VoiceMailAnalysisWarningRequired(ActivityManager manager, string variableName)
		{
			RecordVoicemailManager recordVoicemailManager = manager as RecordVoicemailManager;
			if (recordVoicemailManager == null)
			{
				RecordVoicemailManager.GetScope(manager, out recordVoicemailManager);
			}
			return recordVoicemailManager.VoiceMailAnalysisWarningRequired;
		}

		internal static object AllowMarkAsPrivate(ActivityManager manager, string variableName)
		{
			RecordVoicemailManager recordVoicemailManager = manager as RecordVoicemailManager;
			if (recordVoicemailManager == null)
			{
				RecordVoicemailManager.GetScope(manager, out recordVoicemailManager);
			}
			return recordVoicemailManager.AllowMarkAsPrivate;
		}

		internal static object IsSentImportant(ActivityManager manager, string variableName)
		{
			RecordVoicemailManager recordVoicemailManager = manager as RecordVoicemailManager;
			if (recordVoicemailManager == null)
			{
				RecordVoicemailManager.GetScope(manager, out recordVoicemailManager);
			}
			return recordVoicemailManager.IsSentImportant;
		}

		internal static object MessageMarkedPrivate(ActivityManager manager, string variableName)
		{
			RecordVoicemailManager recordVoicemailManager = manager as RecordVoicemailManager;
			if (recordVoicemailManager == null)
			{
				RecordVoicemailManager.GetScope(manager, out recordVoicemailManager);
			}
			return recordVoicemailManager.MessageMarkedPrivate;
		}
	}
}
