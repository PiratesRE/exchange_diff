using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public static class ObjectCopyOnWrite
	{
		public static long CopyAttachment(Context context, Mailbox mailbox, long inidSource)
		{
			AttachmentTable attachmentTable = DatabaseSchema.AttachmentTable(mailbox.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				inidSource
			});
			List<Column> list = new List<Column>(attachmentTable.Table.Columns.Count);
			list.Add(attachmentTable.MailboxPartitionNumber);
			list.Add(attachmentTable.AttachmentId);
			list.Add(attachmentTable.AttachmentMethod);
			list.Add(attachmentTable.RenderingPosition);
			list.Add(attachmentTable.CreationTime);
			list.Add(attachmentTable.LastModificationTime);
			list.Add(attachmentTable.Size);
			list.Add(attachmentTable.Name);
			list.Add(attachmentTable.ContentId);
			list.Add(attachmentTable.ContentType);
			list.Add(attachmentTable.Content);
			list.Add(attachmentTable.MessageFlagsActual);
			list.Add(attachmentTable.MailFlags);
			list.Add(attachmentTable.RecipientList);
			list.Add(attachmentTable.SubobjectsBlob);
			list.Add(attachmentTable.PropertyBlob);
			list.Add(attachmentTable.LargePropertyValueBlob);
			list.Add(attachmentTable.IsEmbeddedMessage);
			list.Add(attachmentTable.ExtensionBlob);
			list.Add(attachmentTable.FullTextType);
			list.Add(attachmentTable.Language);
			if (attachmentTable.MailboxNumber != null)
			{
				list.Add(attachmentTable.MailboxNumber);
			}
			long result;
			using (InsertOperator insertOperator = Factory.CreateInsertOperator(context.Culture, context, attachmentTable.Table, Factory.CreateTableOperator(context.Culture, context, attachmentTable.Table, attachmentTable.Table.PrimaryKeyIndex, list, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true), list, null, attachmentTable.Inid, true))
			{
				result = (long)insertOperator.ExecuteScalar();
			}
			return result;
		}
	}
}
