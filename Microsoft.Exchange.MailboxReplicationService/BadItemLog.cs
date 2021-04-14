using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class BadItemLog : ObjectLog<BadItemData>
	{
		private BadItemLog() : base(new BadItemLog.BadItemLogSchema(), new SimpleObjectLogConfiguration("BadItem", "BadItemLogEnabled", "BadItemLogMaxDirSize", "BadItemLogMaxFileSize"))
		{
		}

		public static void Write(Guid requestGuid, BadMessageRec item)
		{
			BadItemData objectToLog = new BadItemData(requestGuid, item);
			BadItemLog.instance.LogObject(objectToLog);
		}

		private static BadItemLog instance = new BadItemLog();

		private class BadItemLogSchema : ObjectLogSchema
		{
			public override string Software
			{
				get
				{
					return "Microsoft Exchange Mailbox Replication Service";
				}
			}

			public override string LogType
			{
				get
				{
					return "BadItem Log";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> RequestGuid = new ObjectLogSimplePropertyDefinition<BadItemData>("RequestGuid", (BadItemData d) => d.RequestGuid);

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> BadItemKind = new ObjectLogSimplePropertyDefinition<BadItemData>("BadItemKind", (BadItemData d) => d.Kind.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> EntryId = new ObjectLogSimplePropertyDefinition<BadItemData>("EntryId", (BadItemData d) => TraceUtils.DumpEntryId(d.EntryId));

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> FolderId = new ObjectLogSimplePropertyDefinition<BadItemData>("FolderId", (BadItemData d) => TraceUtils.DumpEntryId(d.EntryId));

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> WKFType = new ObjectLogSimplePropertyDefinition<BadItemData>("WKFType", (BadItemData d) => d.WKFType.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> MessageClass = new ObjectLogSimplePropertyDefinition<BadItemData>("MessageClass", (BadItemData d) => d.MessageClass);

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> MessageSize = new ObjectLogSimplePropertyDefinition<BadItemData>("MessageSize", delegate(BadItemData d)
			{
				if (d.MessageSize == null)
				{
					return string.Empty;
				}
				return d.MessageSize.Value.ToString();
			});

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> DateSent = new ObjectLogSimplePropertyDefinition<BadItemData>("DateSent", delegate(BadItemData d)
			{
				if (d.DateSent == null)
				{
					return string.Empty;
				}
				return d.DateSent.Value.ToString();
			});

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> DateReceived = new ObjectLogSimplePropertyDefinition<BadItemData>("DateReceived", delegate(BadItemData d)
			{
				if (d.DateReceived == null)
				{
					return string.Empty;
				}
				return d.DateReceived.Value.ToString();
			});

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> FailureMessage = new ObjectLogSimplePropertyDefinition<BadItemData>("FailureMessage", delegate(BadItemData d)
			{
				if (d.FailureMessage == null)
				{
					return string.Empty;
				}
				return CommonUtils.FullExceptionMessage(d.FailureMessage).ToString();
			});

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> Category = new ObjectLogSimplePropertyDefinition<BadItemData>("Category", (BadItemData d) => d.Category);

			public static readonly ObjectLogSimplePropertyDefinition<BadItemData> CallStackHash = new ObjectLogSimplePropertyDefinition<BadItemData>("CallStackHash", (BadItemData d) => d.CallStackHash);
		}
	}
}
