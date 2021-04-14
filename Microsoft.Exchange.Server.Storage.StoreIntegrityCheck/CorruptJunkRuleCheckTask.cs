using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class CorruptJunkRuleCheckTask : IntegrityCheckTaskBase
	{
		public CorruptJunkRuleCheckTask(IJobExecutionTracker tracker) : base(tracker)
		{
		}

		public override string TaskName
		{
			get
			{
				return "CorruptJunkRule";
			}
		}

		public override ErrorCode ExecuteOneFolder(Mailbox mailbox, MailboxEntry mailboxEntry, FolderEntry folderEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<string, string>(0L, "Execute task {0} on folder {1}", this.TaskName, folderEntry.ToString());
			}
			this.currentMailbox = mailboxEntry;
			this.currentFolder = folderEntry;
			List<MessageEntry> list = null;
			ErrorCode first = this.CollectJunkRules(mailbox, mailboxEntry, folderEntry, out list, shouldContinue);
			if (first != ErrorCode.NoError)
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError<string>(0L, "Unexpected error while detecting corruption on folder {0}", folderEntry.ToString());
				}
				return first.Propagate((LID)50236U);
			}
			if (list.Count == 0)
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "No junk rules detected");
				}
				return ErrorCode.NoError;
			}
			if (!shouldContinue())
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
				}
				return ErrorCode.CreateExiting((LID)47164U);
			}
			MessageEntry messageEntry = null;
			int num = -1;
			foreach (MessageEntry messageEntry2 in list)
			{
				using (TopMessage topMessage = TopMessage.OpenMessage(mailbox.CurrentOperationContext, mailbox, messageEntry2.DocumentId))
				{
					if (topMessage == null)
					{
						if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.OnlineIsintegTracer.TraceDebug<int>(0L, "Message '{0}' gone, no fix needed", messageEntry2.DocumentId);
						}
					}
					else
					{
						byte[] array = (byte[])topMessage.GetPropertyValue(mailbox.CurrentOperationContext, PropTag.Message.ExtendedRuleCondition);
						int num2 = (array == null) ? 0 : array.Length;
						if (num2 > num)
						{
							num = num2;
							messageEntry = messageEntry2;
						}
						if (!"IPM.ExtendedRule.Message".Equals(topMessage.GetMessageClass(mailbox.CurrentOperationContext), StringComparison.OrdinalIgnoreCase))
						{
							bool problemFixed = false;
							if (!detectOnly)
							{
								topMessage.SetMessageClass(mailbox.CurrentOperationContext, "IPM.ExtendedRule.Message");
								topMessage.SaveChanges(mailbox.CurrentOperationContext, SaveMessageChangesFlags.SkipQuotaCheck);
								problemFixed = true;
							}
							base.ReportCorruption("Incorrect rule message class", this.currentMailbox, this.currentFolder, messageEntry2, CorruptionType.IncorrectRuleMessageClass, problemFixed);
						}
					}
				}
			}
			if (list.Count > 1)
			{
				foreach (MessageEntry messageEntry3 in list)
				{
					if (messageEntry3 != messageEntry)
					{
						bool problemFixed2 = false;
						if (!detectOnly)
						{
							using (TopMessage topMessage2 = TopMessage.OpenMessage(mailbox.CurrentOperationContext, mailbox, messageEntry3.DocumentId))
							{
								if (topMessage2 == null)
								{
									if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
									{
										ExTraceGlobals.OnlineIsintegTracer.TraceDebug<int>(0L, "Message '{0}' gone, no fix needed", messageEntry3.DocumentId);
									}
									continue;
								}
								topMessage2.SetMessageClass(mailbox.CurrentOperationContext, "IPM.DiscardedExtendedRule");
								topMessage2.SaveChanges(mailbox.CurrentOperationContext, SaveMessageChangesFlags.SkipQuotaCheck);
								problemFixed2 = true;
							}
						}
						base.ReportCorruption("Extra junk rule", this.currentMailbox, this.currentFolder, messageEntry3, CorruptionType.ExtraJunkmailRule, problemFixed2);
					}
				}
			}
			return ErrorCode.NoError;
		}

		protected internal override bool IgnoreFolder(Context context, Mailbox mailbox, ExchangeId folderId, ExchangeId parentFolderId, bool isSearchFolder, short specialFolderNumber)
		{
			return isSearchFolder || specialFolderNumber != 10;
		}

		private ErrorCode CollectJunkRules(Mailbox mailbox, MailboxEntry mailboxEntry, FolderEntry folderEntry, out List<MessageEntry> junkRules, Func<bool> shouldContinue)
		{
			MessagePropValueGetter messagePropValueGetter = new MessagePropValueGetter(mailbox.CurrentOperationContext, mailboxEntry.MailboxPartitionNumber, folderEntry.FolderId);
			List<MessageEntry> messageEntries = new List<MessageEntry>();
			Column ruleProviderColumn = PropertySchema.MapToColumn(mailbox.CurrentOperationContext.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message, PropTag.Message.RuleMsgProvider);
			MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(mailbox.CurrentOperationContext.Database);
			ErrorCode result = messagePropValueGetter.Execute(true, null, new Column[]
			{
				messageTable.MessageDocumentId,
				messageTable.MessageId,
				ruleProviderColumn,
				messageTable.MessageClass
			}, delegate(Reader reader)
			{
				int @int = reader.GetInt32(messageTable.MessageDocumentId);
				string @string = reader.GetString(messageTable.MessageClass);
				if (!"IPM.ExtendedRule.Message".Equals(@string, StringComparison.OrdinalIgnoreCase) && !"IPM.Rule.Version2.Message".Equals(@string, StringComparison.OrdinalIgnoreCase))
				{
					return ErrorCode.NoError;
				}
				string string2 = reader.GetString(ruleProviderColumn);
				if (!"JunkEmailRule".Equals(string2, StringComparison.OrdinalIgnoreCase))
				{
					return ErrorCode.NoError;
				}
				byte[] binary = reader.GetBinary(messageTable.MessageId);
				ExchangeId messageId = ExchangeId.CreateFrom26ByteArray(mailbox.CurrentOperationContext, mailbox.ReplidGuidMap, binary);
				messageEntries.Add(new MessageEntry(@int, messageId));
				return ErrorCode.NoError;
			}, shouldContinue);
			junkRules = messageEntries;
			return result;
		}

		internal const string DiscardedExtendedRuleMessageClass = "IPM.DiscardedExtendedRule";

		private const string JunkRuleProvider = "JunkEmailRule";

		private MailboxEntry currentMailbox;

		private FolderEntry currentFolder;
	}
}
