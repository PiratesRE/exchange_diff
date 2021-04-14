using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class RetrieveVoicemailManager
	{
		internal static void GetScope(ActivityManager manager, out RetrieveVoicemailManager scope)
		{
			for (scope = (manager as RetrieveVoicemailManager); scope == null; scope = (manager as RetrieveVoicemailManager))
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

		internal static TransitionBase DeleteVoiceMail(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase Disconnect(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase FindByName(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase FlagVoiceMail(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ForwardVoiceMail(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetEnvelopInfo(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetMessageReadProperty(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetNewMessages(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetNextMessage(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetPreviousMessage(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase GetSavedMessages(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase MarkUnreadVoiceMail(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase More(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase Pause(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase RemoveRecipient(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ReplyVoiceMail(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ResetPlayback(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SaveVoiceMail(ActivityManager manager, string methodName, BaseUMCallSession vo)
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

		internal static TransitionBase SlowDown(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase SpeedUp(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase UndeleteVoiceMail(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase ReplyAll(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return manager.GetTransition(retrieveVoicemailManager.ReplyAll(vo));
		}

		internal static TransitionBase ToggleImportance(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return manager.GetTransition(retrieveVoicemailManager.ToggleImportance(vo));
		}

		internal static TransitionBase TogglePrivacy(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return manager.GetTransition(retrieveVoicemailManager.TogglePrivacy(vo));
		}

		internal static TransitionBase ClearSelection(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return manager.GetTransition(retrieveVoicemailManager.ClearSelection(vo));
		}

		internal static TransitionBase Repeat(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return manager.GetTransition(retrieveVoicemailManager.Repeat(vo));
		}

		internal static TransitionBase SendMessagePrivate(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return manager.GetTransition(retrieveVoicemailManager.SendMessagePrivate(vo));
		}

		internal static TransitionBase SendMessagePrivateAndUrgent(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return manager.GetTransition(retrieveVoicemailManager.SendMessagePrivateAndUrgent(vo));
		}

		internal static object CanUndelete(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object DeclineIntro(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static int DurationMinutes(ActivityManager manager, string variableName)
		{
			return (int)(manager.ReadVariable(variableName) ?? 0);
		}

		internal static int DurationSeconds(ActivityManager manager, string variableName)
		{
			return (int)(manager.ReadVariable(variableName) ?? 0);
		}

		internal static object EmailSender(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object FindByName(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object FirstMessage(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object ForwardIntro(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object IsForward(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object IsHighPriority(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object IsProtected(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object IsReply(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object KnowSenderPhoneNumber(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object KnowVoicemailSender(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object LastActivity(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object LastRecoEvent(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static ExDateTime MessageReceivedTime(ActivityManager manager, string variableName)
		{
			object obj;
			if ((obj = manager.ReadVariable(variableName)) == null)
			{
				obj = default(ExDateTime);
			}
			return (ExDateTime)obj;
		}

		internal static object More(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object NamesGrammar(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static int NumMessagesFromName(ActivityManager manager, string variableName)
		{
			return (int)(manager.ReadVariable(variableName) ?? 0);
		}

		internal static object NumRecipients(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object OcFeature(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object PlayedUndelete(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object Read(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object ReceivedDayOfWeek(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object ReceivedOffset(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static ITempWavFile Recording(ActivityManager manager, string variableName)
		{
			return (ITempWavFile)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object RecordingFailureCount(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object RecordingTimedOut(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object Repeat(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object ReplyAllIntro(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object ReplyIntro(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static PhoneNumber SenderCallerID(ActivityManager manager, string variableName)
		{
			return (PhoneNumber)(manager.ReadVariable(variableName) ?? null);
		}

		internal static string SenderInfo(ActivityManager manager, string variableName)
		{
			return (string)(manager.ReadVariable(variableName) ?? null);
		}

		internal static object Urgent(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object Protected(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static object UserName(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}

		internal static NameOrNumberOfCaller SpecifiedCallerDetails(ActivityManager manager, string variableName)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return retrieveVoicemailManager.SpecifiedCallerDetails;
		}

		internal static PhoneNumber TargetPhoneNumber(ActivityManager manager, string variableName)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return retrieveVoicemailManager.TargetPhoneNumber;
		}

		internal static object MessageListIsNull(ActivityManager manager, string variableName)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return retrieveVoicemailManager.MessageListIsNull;
		}

		internal static object IsForwardEnabled(ActivityManager manager, string variableName)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return retrieveVoicemailManager.IsForwardEnabled;
		}

		internal static object DrmIsEnabled(ActivityManager manager, string variableName)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return retrieveVoicemailManager.DrmIsEnabled;
		}

		internal static object IsSentImportant(ActivityManager manager, string variableName)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return retrieveVoicemailManager.IsSentImportant;
		}

		internal static object MessageMarkedPrivate(ActivityManager manager, string variableName)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return retrieveVoicemailManager.MessageMarkedPrivate;
		}

		internal static object IsFindByContactEnabled(ActivityManager manager, string variableName)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return retrieveVoicemailManager.IsFindByContactEnabled;
		}

		internal static object IsForwardToContactEnabled(ActivityManager manager, string variableName)
		{
			RetrieveVoicemailManager retrieveVoicemailManager = manager as RetrieveVoicemailManager;
			if (retrieveVoicemailManager == null)
			{
				RetrieveVoicemailManager.GetScope(manager, out retrieveVoicemailManager);
			}
			return retrieveVoicemailManager.IsForwardToContactEnabled;
		}
	}
}
