using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.E4E;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal static class EncryptionConfigurationHelper
	{
		private static bool NewMessageItem(MailboxSession mailboxSession, string xml)
		{
			StoreId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			bool result;
			using (MessageItem messageItem = MessageItem.Create(mailboxSession, defaultFolderId))
			{
				messageItem.Subject = "Encryption Configuration";
				using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextPlain))
				{
					textWriter.Write(xml);
				}
				messageItem.IsDraft = false;
				messageItem[ItemSchema.ReceivedTime] = DateTime.UtcNow;
				messageItem[StoreObjectSchema.ItemClass] = "Encryption Configuration";
				ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
				if (conflictResolutionResult.SaveStatus == SaveResult.Success || conflictResolutionResult.SaveStatus == SaveResult.SuccessWithConflictResolution)
				{
					result = true;
				}
				else
				{
					EncryptionConfigurationHelper.Tracer.TraceError<SaveResult>(0L, "In NewMessageItem, messageItem.Save failed. Status: {0}", conflictResolutionResult.SaveStatus);
					result = false;
				}
			}
			return result;
		}

		private static MessageItem GetMessageItem(MailboxSession mailboxSession)
		{
			StoreId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(defaultFolderId);
			MessageItem result;
			using (Folder folder = Folder.Bind(mailboxSession, storeObjectId))
			{
				ComparisonFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "Encryption Configuration");
				PropertyDefinition[] dataColumns = new PropertyDefinition[]
				{
					ItemSchema.Id
				};
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, queryFilter, null, dataColumns))
				{
					object[][] rows = queryResult.GetRows(2);
					if (rows.Length != 1)
					{
						if (rows.Length == 0)
						{
							EncryptionConfigurationHelper.Tracer.TraceDebug<string>(0L, "Retrieved zero messages with item class: {0}.", "Encryption Configuration");
						}
						else
						{
							EncryptionConfigurationHelper.Tracer.TraceError<int, string>(0L, "Retrieved {0} messages with item class: {1}. Only one expected.", rows.Length, "Encryption Configuration");
						}
						result = null;
					}
					else
					{
						StoreObjectId objectId = ((VersionedId)rows[0][0]).ObjectId;
						MessageItem messageItem = Item.BindAsMessage(mailboxSession, objectId);
						PropertyDefinition[] properties = new PropertyDefinition[]
						{
							ItemSchema.TextBody
						};
						messageItem.Load(properties);
						result = messageItem;
					}
				}
			}
			return result;
		}

		private static string GetExceptionMessages(Exception e)
		{
			StringBuilder stringBuilder = new StringBuilder(2 * e.Message.Length);
			do
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append(e.Message);
				e = e.InnerException;
			}
			while (e != null);
			return stringBuilder.ToString();
		}

		internal static EncryptionConfigurationData GetEncryptionConfigurationData(MailboxSession mailboxSession)
		{
			MessageItem messageItem = EncryptionConfigurationHelper.GetMessageItem(mailboxSession);
			if (messageItem != null)
			{
				using (TextReader textReader = messageItem.Body.OpenTextReader(BodyFormat.TextPlain))
				{
					string serializedXML = textReader.ReadToEnd();
					messageItem.Dispose();
					return EncryptionConfigurationData.Deserialize(serializedXML);
				}
			}
			return new EncryptionConfigurationData();
		}

		internal static bool SetMessageItem(MailboxSession mailboxSession, string xml)
		{
			bool result;
			using (MessageItem messageItem = EncryptionConfigurationHelper.GetMessageItem(mailboxSession))
			{
				if (messageItem == null)
				{
					result = EncryptionConfigurationHelper.NewMessageItem(mailboxSession, xml);
				}
				else
				{
					messageItem.OpenAsReadWrite();
					using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextPlain))
					{
						textWriter.Write(xml);
					}
					messageItem.IsDraft = false;
					messageItem[ItemSchema.ReceivedTime] = DateTime.UtcNow;
					messageItem[StoreObjectSchema.ItemClass] = "Encryption Configuration";
					ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
					if (conflictResolutionResult.SaveStatus == SaveResult.Success || conflictResolutionResult.SaveStatus == SaveResult.SuccessWithConflictResolution)
					{
						result = true;
					}
					else
					{
						EncryptionConfigurationHelper.Tracer.TraceError<SaveResult>(0L, "In SetMessageItem, messageItem.Save failed. Status: {0}", conflictResolutionResult.SaveStatus);
						result = false;
					}
				}
			}
			return result;
		}

		internal static ServiceError GetServiceError(Exception e)
		{
			ResponseCodeType messageKey = (e is TransientException) ? ResponseCodeType.ErrorInternalServerTransientError : ResponseCodeType.ErrorInternalServerError;
			string exceptionMessages = EncryptionConfigurationHelper.GetExceptionMessages(e);
			return new ServiceError(exceptionMessages, messageKey, 0, ExchangeVersion.Exchange2012);
		}

		private const string MessageSubject = "Encryption Configuration";

		private const string ItemClass = "Encryption Configuration";

		private static readonly Trace Tracer = ExTraceGlobals.E4ETracer;
	}
}
