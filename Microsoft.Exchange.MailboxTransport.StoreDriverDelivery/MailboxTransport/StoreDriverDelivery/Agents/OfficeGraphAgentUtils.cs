using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.SharePointSignalStore;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal static class OfficeGraphAgentUtils
	{
		internal static string CreateAttachmentsSignal(MessageItem messageItem, List<Dictionary<string, string>> attachmentsProperties, string receiverEmail, Guid tenantId)
		{
			List<AnalyticsSignal> list = new List<AnalyticsSignal>();
			Dictionary<string, string> properties = new Dictionary<string, string>();
			Dictionary<string, string> properties2 = new Dictionary<string, string>
			{
				{
					"Context",
					"ReceivedRegularAttachment"
				}
			};
			AnalyticsSignal.AnalyticsActor actor = new AnalyticsSignal.AnalyticsActor
			{
				Id = receiverEmail,
				Properties = SharePointSignalRestDataProvider.CreateSignalProperties(properties),
				TenantId = tenantId
			};
			AnalyticsSignal.AnalyticsAction action = new AnalyticsSignal.AnalyticsAction
			{
				ActionType = "Received",
				Properties = SharePointSignalRestDataProvider.CreateSignalProperties(properties2)
			};
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < messageItem.Recipients.Count; i++)
			{
				stringBuilder.AppendFormat("{0}|{1}", messageItem.Recipients[i].Participant.DisplayName, messageItem.Recipients[i].Participant.SmtpEmailAddress);
				if (i < messageItem.Recipients.Count - 1)
				{
					stringBuilder.Append(",");
				}
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"ConversationId",
					OfficeGraphAgentUtils.GetCoversationIdBase64(messageItem)
				},
				{
					"ConversationTopic",
					messageItem.ConversationTopic
				},
				{
					"CreationTime",
					messageItem.CreationTime.ToISOString()
				},
				{
					"FromDisplayName",
					messageItem.From.DisplayName
				},
				{
					"FromEmail",
					messageItem.From.SmtpEmailAddress
				},
				{
					"Subject",
					messageItem.Subject
				},
				{
					"Importance",
					messageItem.Importance.ToString()
				},
				{
					"MailPreview",
					messageItem.Preview
				},
				{
					"Recipients",
					stringBuilder.ToString()
				},
				{
					"StoreObjectId",
					messageItem.StoreObjectId.ToBase64String()
				}
			};
			foreach (Dictionary<string, string> dictionary2 in attachmentsProperties)
			{
				Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
				foreach (KeyValuePair<string, string> keyValuePair in dictionary)
				{
					dictionary3.Add(keyValuePair.Key, keyValuePair.Value);
				}
				foreach (KeyValuePair<string, string> keyValuePair2 in dictionary2)
				{
					dictionary3.Add(keyValuePair2.Key, keyValuePair2.Value);
				}
				AnalyticsSignal.AnalyticsItem item = new AnalyticsSignal.AnalyticsItem
				{
					Id = dictionary2["AttachmentId"],
					Properties = SharePointSignalRestDataProvider.CreateSignalProperties(dictionary3)
				};
				AnalyticsSignal item2 = new AnalyticsSignal
				{
					Actor = actor,
					Action = action,
					Item = item,
					Source = "Exchange Office Graph Delivery Agent"
				};
				list.Add(item2);
			}
			Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
			dictionary4["signals"] = list;
			return new JavaScriptSerializer().Serialize(dictionary4);
		}

		internal static string GetCoversationIdBase64(MessageItem messageItem)
		{
			ConversationId conversationId = (ConversationId)OfficeGraphAgentUtils.GetPropertyFromMessage(messageItem, ItemSchema.ConversationId, string.Empty);
			return conversationId.ToBase64String();
		}

		internal static object GetPropertyFromMessage(MessageItem messageItem, StorePropertyDefinition storePropertyDefinition, object defaultValue)
		{
			object result;
			try
			{
				object obj = messageItem.TryGetProperty(storePropertyDefinition);
				if (obj == null || obj is PropertyError)
				{
					result = defaultValue;
				}
				else
				{
					result = obj;
				}
			}
			catch (Exception)
			{
				result = defaultValue;
			}
			return result;
		}
	}
}
