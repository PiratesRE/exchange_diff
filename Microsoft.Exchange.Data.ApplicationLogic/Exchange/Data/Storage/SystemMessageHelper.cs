using System;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	internal static class SystemMessageHelper
	{
		internal static void PostMessage(MailboxSession session, string subject, string body)
		{
			SystemMessageHelper.InternalPostMessage(session, subject, body, null, null, null);
		}

		internal static void PostMessage(MailboxSession session, string subject, string body, string className, Importance importance)
		{
			SystemMessageHelper.InternalPostMessage(session, subject, body, null, className, new Importance?(importance));
		}

		internal static void PostUniqueMessage(MailboxSession session, string subject, string body, string messageId)
		{
			if (string.IsNullOrEmpty(messageId))
			{
				throw new ArgumentNullException("messageId");
			}
			SystemMessageHelper.InternalPostMessage(session, subject, body, messageId, null, null);
		}

		private static void InternalPostMessage(MailboxSession session, string subject, string body, string messageId, string className, Importance? importance)
		{
			if (session == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (string.IsNullOrEmpty(subject))
			{
				throw new ArgumentNullException("subject");
			}
			if (string.IsNullOrEmpty(body))
			{
				throw new ArgumentNullException("body");
			}
			using (Folder folder = Folder.Bind(session, session.GetDefaultFolderId(DefaultFolderType.Inbox)))
			{
				if (!string.IsNullOrEmpty(messageId))
				{
					QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.InternetMessageId, messageId);
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, SystemMessageHelper.PropList))
					{
						queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter);
						object[][] rows = queryResult.GetRows(1);
						if (rows.Length > 0)
						{
							return;
						}
					}
				}
				using (MessageItem messageItem = MessageItem.Create(session, folder.Id))
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(session.MailboxOwner.MailboxInfo.OrganizationId), 123, "InternalPostMessage", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\SystemMessageHelper\\SystemMessageHelper.cs");
					if (tenantOrTopologyConfigurationSession.SessionSettings != null)
					{
						tenantOrTopologyConfigurationSession.SessionSettings.AccountingObject = session.AccountingObject;
					}
					MicrosoftExchangeRecipient microsoftExchangeRecipient = tenantOrTopologyConfigurationSession.FindMicrosoftExchangeRecipient();
					if (microsoftExchangeRecipient != null)
					{
						messageItem.From = new Participant(microsoftExchangeRecipient);
					}
					messageItem.Subject = subject;
					messageItem.Recipients.Add(new Participant(session.MailboxOwner), RecipientItemType.To);
					messageItem[MessageItemSchema.IsDraft] = false;
					messageItem[MessageItemSchema.IsRead] = false;
					using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextHtml))
					{
						textWriter.Write(body);
					}
					if (!string.IsNullOrEmpty(messageId))
					{
						messageItem.InternetMessageId = messageId;
					}
					if (!string.IsNullOrEmpty(className))
					{
						messageItem.ClassName = className;
					}
					if (importance != null)
					{
						messageItem.Importance = importance.Value;
					}
					messageItem.Save(SaveMode.NoConflictResolution);
				}
			}
		}

		private static PropertyDefinition[] PropList = new PropertyDefinition[]
		{
			ItemSchema.InternetMessageId
		};
	}
}
