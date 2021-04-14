using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class InstantMessagePayloadUtilities
	{
		internal static void GenerateUnavailablePayload(InstantMessagePayload payload, Exception exception, string errorLocation, InstantMessageFailure errorCode, bool recurseThroughException)
		{
			InstantMessagePayloadUtilities.GenerateUnavailablePayload(payload, exception, errorLocation, errorCode, 0, recurseThroughException);
		}

		internal static void GenerateUnavailablePayload(InstantMessagePayload payload, Exception exception, string errorLocation, InstantMessageFailure errorCode, int reconnectInterval, bool recurseThroughException)
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
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateUnavailablePayload. Payload is null.");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("UN(");
				payload.Append((int)errorCode);
				payload.Append(",");
				if (reconnectInterval >= 0)
				{
					payload.Append(reconnectInterval);
					payload.Append(",");
				}
				payload.Append("'");
				payload.Append(Utilities.JavascriptEncode(string.IsNullOrEmpty(text) ? (errorLocation ?? string.Empty) : text));
				payload.Append("');");
			}
			payload.PickupData(length);
		}

		internal static void GenerateMessageNotDeliveredPayload(InstantMessagePayload payload, string chatId)
		{
			InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(payload, chatId, false);
		}

		internal static void GenerateInstantMessageUnavailablePayload(InstantMessagePayload payload, string methodName, InstantMessageFailure errorCode, Exception exception)
		{
			string arg = string.Empty;
			if (exception != null && exception.Message != null)
			{
				arg = exception.Message;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceError<string, int, string>(0L, "{0} failed with error code {1}. {2}", methodName, (int)errorCode, arg);
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload. Payload is null.");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("UN(");
				payload.Append((int)errorCode);
				payload.Append(");");
			}
			payload.PickupData(length);
		}

		internal static void GenerateMessageNotDeliveredPayload(InstantMessagePayload payload, string methodName, int conversationId, Exception exception)
		{
			string arg = string.Empty;
			if (exception != null && exception.Message != null)
			{
				arg = exception.Message;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceError<string, string>(0L, "{0} failed. {1}", methodName, arg);
			InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(payload, conversationId.ToString(CultureInfo.InvariantCulture));
		}

		internal static void GenerateMessageNotDeliveredPayload(InstantMessagePayload payload, string chatId, bool serverPolicy)
		{
			if (serverPolicy)
			{
				InstantMessagePayloadUtilities.GenerateInstantMessageAlertPayload(payload, chatId, InstantMessageAlert.FailedDeliveryDueToServerPolicy);
				return;
			}
			InstantMessagePayloadUtilities.GenerateInstantMessageAlertPayload(payload, chatId, InstantMessageAlert.FailedDelivery);
		}

		internal static void GenerateInstantMessageAlertPayload(InstantMessagePayload payload, string chatId, InstantMessageAlert alertType)
		{
			InstantMessagePayloadUtilities.GenerateInstantMessageAlertPayload(payload, chatId, alertType, null);
		}

		internal static void GenerateInstantMessageAlertPayload(InstantMessagePayload payload, string chatId, InstantMessageAlert alertType, string imAddress)
		{
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateInstantMessageAlertPayload. Payload is null.");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("IMA(\"");
				payload.Append(Utilities.JavascriptEncode(chatId));
				payload.Append("\", ");
				payload.Append((int)alertType);
				if (imAddress != null)
				{
					payload.Append(", \"");
					payload.Append(Utilities.JavascriptEncode(imAddress));
					payload.Append("\"");
				}
				payload.Append(");");
			}
			payload.PickupData(length);
		}

		internal static void GenerateUpdatePresencePayload(InstantMessagePayload payload, int presence)
		{
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateUpdatePresencePayload. Payload is null.");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("IMUP(");
				payload.Append(presence);
				payload.Append(");");
			}
			payload.PickupData(length);
		}

		internal static void GenerateParticipantJoinedPayload(InstantMessagePayload payload, string chatId, string imAddress, string displayName)
		{
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateParticipantJoinedPayload. Payload is null.");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("ACC(\"");
				payload.Append(chatId);
				payload.Append("\", \"");
				payload.Append(Utilities.JavascriptEncode(imAddress));
				payload.Append("\", \"");
				payload.Append(Utilities.JavascriptEncode(displayName));
				payload.Append("\");");
			}
			payload.PickupData(length);
		}

		internal static void GenerateParticipantLeftPayload(InstantMessagePayload payload, string chatId, string imAddress)
		{
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateParticipantLeftPayload. Payload is null.");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("RCC(\"");
				payload.Append(chatId);
				payload.Append("\", \"");
				payload.Append(Utilities.JavascriptEncode(imAddress));
				payload.Append("\");");
			}
			payload.PickupData(length);
		}

		internal static void GenerateInstantMessageConversationClosePayload(InstantMessagePayload payload, string chatId)
		{
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateInstantMessageConversationClosePayload. Payload is null.");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("CLSC(\"");
				payload.Append(chatId);
				payload.Append("\");");
			}
			payload.PickupData(length);
		}

		internal static void GenerateMaxChatSessionCountPayload(InstantMessagePayload payload)
		{
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateMaxChatSessionCountPayload. Payload is null.");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("MCht();");
			}
			payload.PickupData(length);
		}

		internal static void GenerateInstantMessageSignInPayload(InstantMessagePayload payload, bool isStarted)
		{
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateInstantMessageSignInPayload. Payload is null.");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("SI(");
				payload.Append(isStarted ? "1" : "0");
				payload.Append(");");
			}
			payload.PickupData(length);
		}

		internal static void GenerateProxiesPayload(InstantMessagePayload payload, Dictionary<string, string[]> proxyAddresses)
		{
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateInstantMessageSignInPayload. Payload is null.");
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("PXY([");
				foreach (string text in proxyAddresses.Keys)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append("['");
					stringBuilder.Append(Utilities.JavascriptEncode(text));
					stringBuilder.Append("',[");
					string[] array = null;
					if (proxyAddresses.TryGetValue(text, out array) && array != null && array.Length > 0)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (i != 0)
							{
								stringBuilder.Append(",");
							}
							stringBuilder.Append("'");
							stringBuilder.Append(Utilities.JavascriptEncode(array[i]));
							stringBuilder.Append("'");
						}
					}
					stringBuilder.Append("]]");
				}
				stringBuilder.Append("]);");
				payload.Append(stringBuilder);
				stringBuilder = null;
			}
			payload.PickupData(length);
		}

		internal static void GenerateBuddyListPayload(InstantMessagePayload payload, IEnumerable<InstantMessageGroup> groups, IEnumerable<InstantMessageBuddy> buddies)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateBuddyListPayload.");
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateBuddyListPayload. Payload is null.");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("APS([[");
				if (groups != null)
				{
					payload.Append(string.Join(",", (from g in groups
					where g.VisibleOnClient
					select g.SerializeToJavascript()).ToArray<string>()));
				}
				payload.Append("],[");
				if (buddies != null)
				{
					payload.Append(string.Join(",", (from b in buddies
					select b.SerializeToJavascript()).ToArray<string>()));
				}
				payload.Append("]]);");
			}
			payload.PickupData(length);
		}

		internal static void GenerateDeletedGroupsPayload(InstantMessagePayload payload, List<InstantMessageGroup> groups)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateDeletedGroupsPayload.");
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateDeletedGroupsPayload. Payload is null.");
				return;
			}
			if (groups == null || groups.Count == 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateDeletedGroupsPayload. Empty group list, ignoring");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("RG([");
				payload.Append(string.Join(",", (from g in groups
				select g.SerializeIdToJavascript()).ToArray<string>()));
				payload.Append("]);");
			}
			payload.PickupData(length);
		}

		internal static void GenerateDeletedFromGroupsPayload(InstantMessagePayload payload, List<InstantMessageBuddy> buddies)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateDeletedFromGroupsPayload.");
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateDeletedFromGroupsPayload. Payload is null.");
				return;
			}
			if (buddies == null || buddies.Count == 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateDeletedFromGroupsPayload. Empty buddy list, ignoring");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("RCG([");
				payload.Append(string.Join(",", (from b in buddies
				select string.Join(",", (from gId in b.GroupIds
				select string.Concat(new string[]
				{
					"['",
					Utilities.JavascriptEncode(InstantMessageUtilities.ToGroupFormat(gId)),
					"',",
					b.SerializeSipToJavascript(),
					"]"
				})).ToArray<string>())).ToArray<string>()));
				payload.Append("]);");
			}
			payload.PickupData(length);
		}

		internal static void GenerateDeletedBuddiesPayload(InstantMessagePayload payload, List<InstantMessageBuddy> buddies)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateDeletedBuddiesPayload.");
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateDeletedBuddiesPayload. Payload is null.");
				return;
			}
			if (buddies == null || buddies.Count == 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateDeletedBuddiesPayload. Empty buddies list, ignoring");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("RC([");
				payload.Append(string.Join(",", (from b in buddies
				select b.SerializeSipToJavascript()).ToArray<string>()));
				payload.Append("]);");
			}
			payload.PickupData(length);
		}

		internal static void GenerateGroupRenamePayload(InstantMessagePayload payload, InstantMessageGroup group)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateGroupRenamePayload.");
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessagePayloadUtilities.GenerateGroupRenamePayload. Payload is null.");
				return;
			}
			if (group == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessagePayloadUtilities.GenerateGroupRenamePayload. Null group, ignoring");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append("GPC(");
				payload.Append(group.SerializeIdAndNameToJavascript());
				payload.Append(");");
			}
			payload.PickupData(length);
		}
	}
}
