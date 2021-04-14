using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Mapi;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal sealed class ReplyTemplate : IDisposable
	{
		public static ReplyTemplate Find(MailboxSession session, RuleAction.OOFReply ruleAction)
		{
			ReplyTemplate result;
			try
			{
				if (ruleAction.ReplyTemplateMessageEntryID == null)
				{
					result = ReplyTemplate.FindByTemplateGuid(session, ruleAction);
				}
				else
				{
					StoreObjectId messageId = StoreObjectId.FromProviderSpecificId(ruleAction.ReplyTemplateMessageEntryID);
					MessageItem messageItem = MessageItem.Bind(session, messageId, new PropertyDefinition[]
					{
						ItemSchema.ReplyTemplateId
					});
					messageItem.OpenAsReadWrite();
					ReplyTemplate.Tracer.TraceDebug<IExchangePrincipal, ByteArray>(0L, "Mailbox:{0}: Found reply template by entry Id. Entry id={1}", session.MailboxOwner, new ByteArray(ruleAction.ReplyTemplateMessageEntryID));
					ReplyTemplate.TracerPfd.TracePfd<int, IExchangePrincipal, ByteArray>(0L, "PFD IWO {0} Mailbox:{1}: Found reply template by entry Id. Entry id={2}", 31639, session.MailboxOwner, new ByteArray(ruleAction.ReplyTemplateMessageEntryID));
					result = new ReplyTemplate(messageItem);
				}
			}
			catch (ObjectNotFoundException)
			{
				ReplyTemplate.Tracer.TraceDebug<IExchangePrincipal, ByteArray>(0L, "Mailbox:{0}: Found no reply template by entry Id. Entry id={1}", session.MailboxOwner, new ByteArray(ruleAction.ReplyTemplateMessageEntryID));
				result = ReplyTemplate.FindByTemplateGuid(session, ruleAction);
			}
			return result;
		}

		public static ReplyTemplate Create(MailboxSession session, Guid templateGuid, string messageClass, OofReplyType oofReplyType)
		{
			ReplyTemplate.Tracer.TraceDebug<IExchangePrincipal, Guid, string>(0L, "Mailbox:{0}: Creating new reply template. GUID={1}, MessageClass={2}", session.MailboxOwner, templateGuid, messageClass);
			ReplyTemplate.TracerPfd.TracePfd(0L, "PFD IWO {0} Mailbox:{1}: Created new reply template. GUID={2}, MessageClass={3}", new object[]
			{
				23447,
				session.MailboxOwner,
				templateGuid,
				messageClass
			});
			return new ReplyTemplate(session, templateGuid, messageClass, oofReplyType);
		}

		public string PlainTextBody
		{
			get
			{
				string result;
				using (TextReader textReader = this.messageItem.Body.OpenTextReader(BodyFormat.TextPlain))
				{
					result = textReader.ReadToEnd();
				}
				return result;
			}
			set
			{
				using (TextWriter textWriter = this.messageItem.Body.OpenTextWriter(ReplyTemplate.ReplyTemplateBodyConfigurationPlain))
				{
					textWriter.Write(value);
				}
			}
		}

		public string ClassName
		{
			get
			{
				return this.messageItem.ClassName;
			}
			set
			{
				this.messageItem.ClassName = value;
			}
		}

		public OofReplyType OofReplyType
		{
			get
			{
				return (OofReplyType)this.messageItem[MessageItemSchema.OofReplyType];
			}
			set
			{
				this.messageItem[MessageItemSchema.OofReplyType] = value;
			}
		}

		public string CharSet
		{
			get
			{
				return this.charSet;
			}
			set
			{
				this.charSet = value;
			}
		}

		public string HtmlBody
		{
			get
			{
				string result;
				using (TextReader textReader = this.messageItem.Body.OpenTextReader(BodyFormat.TextHtml))
				{
					result = textReader.ReadToEnd();
				}
				return result;
			}
			set
			{
				using (TextWriter textWriter = this.messageItem.Body.OpenTextWriter(ReplyTemplate.ReplyTemplateBodyConfigurationHtml))
				{
					if (value != null)
					{
						textWriter.Write(value);
					}
				}
			}
		}

		public byte[] EntryId
		{
			get
			{
				return this.messageItem.Id.ObjectId.ProviderLevelItemId;
			}
		}

		public void Dispose()
		{
			if (this.messageItem != null)
			{
				this.messageItem.Dispose();
				this.messageItem = null;
			}
		}

		public void SaveChanges()
		{
			ConflictResolutionResult conflictResolutionResult = this.messageItem.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(this.messageItem.Id), conflictResolutionResult);
			}
			this.messageItem.Load();
		}

		public void Load()
		{
			this.messageItem.Load();
		}

		private ReplyTemplate(MailboxSession session, Guid templateGuid, string messageClass, OofReplyType oofReplyType)
		{
			using (Folder folder = Folder.Bind(session, DefaultFolderType.Inbox))
			{
				this.messageItem = MessageItem.CreateAssociated(session, folder.Id);
				using (this.messageItem.Body.OpenWriteStream(ReplyTemplate.ReplyTemplateBodyConfigurationPlain))
				{
				}
				object[] propertyValues = new object[]
				{
					templateGuid.ToByteArray(),
					oofReplyType
				};
				this.messageItem.SetProperties(ReplyTemplate.NewPropsArray, propertyValues);
				this.messageItem.ClassName = messageClass;
			}
		}

		private static BodyWriteConfiguration CreateBodyWriteConfiguration(BodyFormat bf)
		{
			BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(bf, "utf-8");
			bodyWriteConfiguration.SetTargetFormat(bf, "utf-8", BodyCharsetFlags.DisableCharsetDetection);
			return bodyWriteConfiguration;
		}

		private ReplyTemplate(MessageItem messageItem)
		{
			this.messageItem = messageItem;
		}

		private static ReplyTemplate FindByTemplateGuid(MailboxSession session, RuleAction.OOFReply ruleAction)
		{
			MessageItem messageItem = null;
			try
			{
				using (Folder folder = Folder.Bind(session, DefaultFolderType.Inbox))
				{
					byte[] replyTemplateEntryIdFromTemplateGuid = ReplyTemplate.GetReplyTemplateEntryIdFromTemplateGuid(folder, ruleAction.ReplyTemplateGuid);
					if (replyTemplateEntryIdFromTemplateGuid != null)
					{
						StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(replyTemplateEntryIdFromTemplateGuid);
						messageItem = MessageItem.Bind(session, storeObjectId, new PropertyDefinition[]
						{
							ItemSchema.ReplyTemplateId
						});
						messageItem.OpenAsReadWrite();
						ReplyTemplate.Tracer.TraceDebug<IExchangePrincipal, Guid, byte[]>(0L, "Mailbox:{0}: Found reply template by GUID. GUID={1}, Entry Id={2}", session.MailboxOwner, ruleAction.ReplyTemplateGuid, storeObjectId.GetBytes());
					}
					else
					{
						ReplyTemplate.Tracer.TraceDebug<IExchangePrincipal, Guid>(0L, "Mailbox:{0}: Found no reply template by GUID. GUID={1}", session.MailboxOwner, ruleAction.ReplyTemplateGuid);
						messageItem = null;
					}
				}
			}
			catch (ObjectNotFoundException)
			{
				return null;
			}
			if (messageItem != null)
			{
				return new ReplyTemplate(messageItem);
			}
			return null;
		}

		private static byte[] GetReplyTemplateEntryIdFromTemplateGuid(Folder folder, Guid replyTemplateGuid)
		{
			using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, null, null, ReplyTemplate.ReplyTemplateProperties))
			{
				queryResult.SeekToOffset(SeekReference.OriginBeginning, 0);
				for (;;)
				{
					object[][] rows = queryResult.GetRows(10);
					if (rows.GetLength(0) == 0)
					{
						goto IL_6E;
					}
					foreach (object[] array2 in rows)
					{
						byte[] array3 = array2[1] as byte[];
						if (array3 != null && replyTemplateGuid == new Guid(array3))
						{
							goto Block_5;
						}
					}
				}
				Block_5:
				object[] array2;
				return array2[0] as byte[];
				IL_6E:;
			}
			return null;
		}

		private MessageItem messageItem;

		private string charSet;

		private static readonly PropertyDefinition[] ReplyTemplateProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.ReplyTemplateId
		};

		private static readonly PropertyDefinition[] NewPropsArray = new PropertyDefinition[]
		{
			ItemSchema.ReplyTemplateId,
			MessageItemSchema.OofReplyType
		};

		private static readonly Trace Tracer = ExTraceGlobals.OOFTracer;

		private static readonly Trace TracerPfd = ExTraceGlobals.PFDTracer;

		private static readonly BodyWriteConfiguration ReplyTemplateBodyConfigurationPlain = ReplyTemplate.CreateBodyWriteConfiguration(BodyFormat.TextPlain);

		private static readonly BodyWriteConfiguration ReplyTemplateBodyConfigurationHtml = ReplyTemplate.CreateBodyWriteConfiguration(BodyFormat.TextHtml);

		private enum ReplyTemplatePropertyIndex
		{
			EntryId,
			ReplyTemplateId
		}
	}
}
