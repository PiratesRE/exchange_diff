using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.InstantMessaging;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class InstantMessagePayloadUtilities
	{
		internal static void GenerateUnavailablePayload(InstantMessageNotifier notifier, Exception exception, string errorLocation, InstantMessageServiceError errorCode, bool recurseThroughException)
		{
			InstantMessagePayloadUtilities.GenerateUnavailablePayload(notifier, exception, errorLocation, errorCode, 0, recurseThroughException);
		}

		internal static void GenerateUnavailablePayload(InstantMessageNotifier notifier, Exception exception, string errorLocation, InstantMessageServiceError errorCode, int reconnectInterval, bool recurseThroughException)
		{
			if (exception != null && recurseThroughException)
			{
				while (exception.InnerException != null)
				{
					exception = exception.InnerException;
				}
			}
			string text = string.Empty;
			if (exception != null)
			{
				if (exception is SoapException)
				{
					SoapException ex = exception as SoapException;
					text = string.Format(CultureInfo.InvariantCulture, "Exception Message: {0}, Node: {1}, Code: {2}", new object[]
					{
						exception.Message,
						ex.Node,
						ex.Code.ToString()
					});
				}
				else
				{
					text = "Exception Message: " + exception.Message;
				}
			}
			ExTraceGlobals.InstantMessagingTracer.TraceError<string>(0L, errorLocation, text);
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateUnavailablePayload. Notifier is null.");
				return;
			}
			lock (notifier)
			{
				text = (string.IsNullOrEmpty(text) ? (errorLocation ?? string.Empty) : text);
				notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.ServiceUnavailable)
				{
					ServiceError = errorCode,
					ErrorMessage = text,
					ReconnectInterval = new int?(reconnectInterval)
				});
			}
			notifier.PickupData();
		}

		internal static void GenerateInstantMessageUnavailablePayload(InstantMessageNotifier notifier, string methodName, InstantMessageServiceError errorCode, Exception exception)
		{
			string arg = string.Empty;
			if (exception != null && exception.Message != null)
			{
				arg = exception.Message;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceError<string, int, string>(0L, "{0} failed with error code {1}. {2}", methodName, (int)errorCode, arg);
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload. Notifier is null.");
				return;
			}
			lock (notifier)
			{
				notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.ServiceUnavailable)
				{
					ServiceError = errorCode
				});
			}
			notifier.PickupData();
		}

		internal static void GenerateMessageNotDeliveredPayload(InstantMessageNotifier notifier, string methodName, int conversationId, Exception exception)
		{
			string arg = string.Empty;
			if (exception != null && exception.Message != null)
			{
				arg = exception.Message;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceError<string, string>(0L, "{0} failed. {1}", methodName, arg);
			UserActivityType alertType = UserActivityType.FailedDelivery;
			InstantMessagingException ex = exception as InstantMessagingException;
			if (ex != null && ex.SubCode == 504)
			{
				alertType = UserActivityType.DeliveryTimeout;
			}
			InstantMessagePayloadUtilities.GenerateInstantMessageAlertPayload(notifier, conversationId, alertType);
		}

		internal static void GenerateMessageNotDeliveredPayload(InstantMessageNotifier notifier, string methodName, int chatId, UserActivityType alertType)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceError<string, UserActivityType>(0L, "{0} failed. {1}", methodName, alertType);
			InstantMessagePayloadUtilities.GenerateInstantMessageAlertPayload(notifier, chatId, alertType);
		}

		internal static void GenerateInstantMessageAlertPayload(InstantMessageNotifier notifier, int chatId, UserActivityType alertType)
		{
			InstantMessagePayloadUtilities.GenerateInstantMessageAlertPayload(notifier, chatId, alertType, null);
		}

		internal static void GenerateInstantMessageAlertPayload(InstantMessageNotifier notifier, int chatId, UserActivityType alertType, string imAddress)
		{
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateInstantMessageAlertPayload. Notifier is null.");
				return;
			}
			lock (notifier)
			{
				notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.UserActivity)
				{
					ChatSessionId = new int?(chatId),
					UserActivity = alertType,
					SipUri = imAddress
				});
			}
			notifier.PickupData();
		}

		internal static void GenerateUpdatePresencePayload(InstantMessageNotifier notifier, int presence)
		{
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateUpdatePresencePayload. Notifier is null.");
				return;
			}
			lock (notifier)
			{
				notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.UpdateUserPresence)
				{
					NewUserPresence = (InstantMessagePresenceType)presence
				});
			}
			notifier.PickupData();
		}

		internal static void GenerateParticipantJoinedPayload(InstantMessageNotifier notifier, int chatId, string imAddress, string displayName)
		{
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateParticipantJoinedPayload. Notifier is null.");
				return;
			}
			lock (notifier)
			{
				notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.ParticipantJoined)
				{
					ChatSessionId = new int?(chatId),
					DisplayName = displayName,
					SipUri = imAddress
				});
			}
			notifier.PickupData();
		}

		internal static void GenerateParticipantLeftPayload(InstantMessageNotifier notifier, int chatId, string imAddress)
		{
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateParticipantLeftPayload. Notifier is null.");
				return;
			}
			lock (notifier)
			{
				notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.ParticipantLeft)
				{
					ChatSessionId = new int?(chatId),
					SipUri = imAddress
				});
			}
			notifier.PickupData();
		}

		internal static void GenerateInstantMessageSignInPayload(InstantMessageNotifier notifier, bool isStarted, bool isCurrentUserInUcsMode)
		{
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateInstantMessageSignInPayload. Notifier is null.");
				return;
			}
			lock (notifier)
			{
				notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.SignOn)
				{
					SignOnStarted = isStarted,
					IsUserInUcsMode = isCurrentUserInUcsMode
				});
			}
			notifier.PickupData();
		}

		internal static void GenerateBuddyListPayload(InstantMessageNotifier notifier, IEnumerable<InstantMessageGroup> groups, IEnumerable<InstantMessageBuddy> buddies)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateBuddyListPayload.");
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateBuddyListPayload. Notifier is null.");
				return;
			}
			lock (notifier)
			{
				InstantMessagePayload instantMessagePayload = new InstantMessagePayload(InstantMessagePayloadType.AddToBuddyList);
				instantMessagePayload.AddedGroups = (from g in groups
				where g.VisibleOnClient
				select g).ToArray<InstantMessageGroup>();
				instantMessagePayload.AddedContacts = buddies.ToArray<InstantMessageBuddy>();
				notifier.Add(instantMessagePayload);
			}
			notifier.PickupData();
		}

		internal static void GenerateDeletedGroupsPayload(InstantMessageNotifier notifier, List<InstantMessageGroup> groups)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateDeletedGroupsPayload.");
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateDeletedGroupsPayload. Notifier is null.");
				return;
			}
			if (groups == null || groups.Count == 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateDeletedGroupsPayload. Empty group list, ignoring");
				return;
			}
			lock (notifier)
			{
				InstantMessagePayload instantMessagePayload = new InstantMessagePayload(InstantMessagePayloadType.DeleteGroup);
				instantMessagePayload.DeletedGroupIds = groups.ConvertAll<string>((InstantMessageGroup g) => g.Id).ToArray();
				notifier.Add(instantMessagePayload);
			}
			notifier.PickupData();
		}

		internal static void GenerateDeletedBuddiesPayload(InstantMessageNotifier notifier, List<InstantMessageBuddy> buddies)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateDeletedBuddiesPayload.");
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateDeletedBuddiesPayload. Notifier is null.");
				return;
			}
			if (buddies == null || buddies.Count == 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateDeletedBuddiesPayload. Empty buddies list, ignoring");
				return;
			}
			lock (notifier)
			{
				InstantMessagePayload instantMessagePayload = new InstantMessagePayload(InstantMessagePayloadType.DeleteBuddy);
				instantMessagePayload.DeletedBuddySipUrls = buddies.ConvertAll<string>((InstantMessageBuddy b) => b.SipUri).ToArray();
				notifier.Add(instantMessagePayload);
			}
			notifier.PickupData();
		}

		internal static void GenerateGroupRenamePayload(InstantMessageNotifier notifier, List<InstantMessageGroup> groups)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateGroupRenamePayload.");
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateGroupRenamePayload. Notifier is null.");
				return;
			}
			if (groups == null || groups.Count == 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateGroupRenamePayload. No groups renamed, ignoring");
				return;
			}
			lock (notifier)
			{
				notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.RenameGroup)
				{
					RenamedGroups = groups.ToArray()
				});
			}
			notifier.PickupData();
		}
	}
}
