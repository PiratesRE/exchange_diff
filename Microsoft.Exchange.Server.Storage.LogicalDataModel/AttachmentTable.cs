using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class AttachmentTable
	{
		internal AttachmentTable()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxPartitionNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.inid = Factory.CreatePhysicalColumn("Inid", "Inid", typeof(long), false, true, false, false, false, Visibility.Public, 0, 8, 8);
			this.attachmentId = Factory.CreatePhysicalColumn("AttachmentId", "AttachmentId", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.attachmentMethod = Factory.CreatePhysicalColumn("AttachmentMethod", "AttachmentMethod", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.renderingPosition = Factory.CreatePhysicalColumn("RenderingPosition", "RenderingPosition", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.creationTime = Factory.CreatePhysicalColumn("CreationTime", "CreationTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.lastModificationTime = Factory.CreatePhysicalColumn("LastModificationTime", "LastModificationTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.size = Factory.CreatePhysicalColumn("Size", "Size", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.name = Factory.CreatePhysicalColumn("Name", "Name", typeof(string), true, false, false, false, false, Visibility.Public, 512, 0, 512);
			this.contentId = Factory.CreatePhysicalColumn("ContentId", "ContentId", typeof(string), true, false, false, false, false, Visibility.Public, 128, 0, 128);
			this.contentType = Factory.CreatePhysicalColumn("ContentType", "ContentType", typeof(string), true, false, false, false, false, Visibility.Public, 128, 0, 128);
			this.content = Factory.CreatePhysicalColumn("Content", "Content", typeof(byte[]), true, false, true, true, false, Visibility.Private, 1073741824, 0, 256);
			this.messageFlagsActual = Factory.CreatePhysicalColumn("MessageFlagsActual", "MessageFlagsActual", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.mailFlags = Factory.CreatePhysicalColumn("MailFlags", "MailFlags", typeof(short), false, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.recipientList = Factory.CreatePhysicalColumn("RecipientList", "RecipientList", typeof(byte[][]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.subobjectsBlob = Factory.CreatePhysicalColumn("SubobjectsBlob", "SubobjectsBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.propertyBlob = Factory.CreatePhysicalColumn("PropertyBlob", "PropertyBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.largePropertyValueBlob = Factory.CreatePhysicalColumn("LargePropertyValueBlob", "LargePropertyValueBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.isEmbeddedMessage = Factory.CreatePhysicalColumn("IsEmbeddedMessage", "IsEmbeddedMessage", typeof(bool), true, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.fullTextType = Factory.CreatePhysicalColumn("FullTextType", "FullTextType", typeof(short), true, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.language = Factory.CreatePhysicalColumn("Language", "Language", typeof(string), true, false, false, false, false, Visibility.Public, 4, 0, 4);
			this.propertyBag = Factory.CreatePhysicalColumn("PropertyBag", "PropertyBag", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 4, 0, 4);
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), true, false, false, false, true, Visibility.Public, 0, 4, 4);
			string text = "AttachmentPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			this.attachmentPK = new Index(text, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.Inid
			});
			Index[] indexes = new Index[]
			{
				this.AttachmentPK
			};
			SpecialColumns specialCols = new SpecialColumns(this.PropertyBlob, null, this.PropertyBag, 1);
			PhysicalColumn[] computedColumns = new PhysicalColumn[]
			{
				this.PropertyBag
			};
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.Inid,
				this.AttachmentId,
				this.AttachmentMethod,
				this.RenderingPosition,
				this.CreationTime,
				this.LastModificationTime,
				this.Size,
				this.Name,
				this.ContentId,
				this.ContentType,
				this.Content,
				this.MessageFlagsActual,
				this.MailFlags,
				this.RecipientList,
				this.SubobjectsBlob,
				this.PropertyBlob,
				this.LargePropertyValueBlob,
				this.IsEmbeddedMessage,
				this.ExtensionBlob,
				this.FullTextType,
				this.Language,
				this.MailboxNumber
			};
			this.table = Factory.CreateTable("Attachment", TableClass.Attachment, CultureHelper.DefaultCultureInfo, false, TableAccessHints.None, false, Visibility.Redacted, false, specialCols, indexes, computedColumns, columns);
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public PhysicalColumn MailboxPartitionNumber
		{
			get
			{
				return this.mailboxPartitionNumber;
			}
		}

		public PhysicalColumn Inid
		{
			get
			{
				return this.inid;
			}
		}

		public PhysicalColumn AttachmentId
		{
			get
			{
				return this.attachmentId;
			}
		}

		public PhysicalColumn AttachmentMethod
		{
			get
			{
				return this.attachmentMethod;
			}
		}

		public PhysicalColumn RenderingPosition
		{
			get
			{
				return this.renderingPosition;
			}
		}

		public PhysicalColumn CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		public PhysicalColumn LastModificationTime
		{
			get
			{
				return this.lastModificationTime;
			}
		}

		public PhysicalColumn Size
		{
			get
			{
				return this.size;
			}
		}

		public PhysicalColumn Name
		{
			get
			{
				return this.name;
			}
		}

		public PhysicalColumn ContentId
		{
			get
			{
				return this.contentId;
			}
		}

		public PhysicalColumn ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public PhysicalColumn Content
		{
			get
			{
				return this.content;
			}
		}

		public PhysicalColumn MessageFlagsActual
		{
			get
			{
				return this.messageFlagsActual;
			}
		}

		public PhysicalColumn MailFlags
		{
			get
			{
				return this.mailFlags;
			}
		}

		public PhysicalColumn RecipientList
		{
			get
			{
				return this.recipientList;
			}
		}

		public PhysicalColumn SubobjectsBlob
		{
			get
			{
				return this.subobjectsBlob;
			}
		}

		public PhysicalColumn PropertyBlob
		{
			get
			{
				return this.propertyBlob;
			}
		}

		public PhysicalColumn LargePropertyValueBlob
		{
			get
			{
				return this.largePropertyValueBlob;
			}
		}

		public PhysicalColumn IsEmbeddedMessage
		{
			get
			{
				return this.isEmbeddedMessage;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public PhysicalColumn FullTextType
		{
			get
			{
				return this.fullTextType;
			}
		}

		public PhysicalColumn Language
		{
			get
			{
				return this.language;
			}
		}

		public PhysicalColumn PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public PhysicalColumn MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public Index AttachmentPK
		{
			get
			{
				return this.attachmentPK;
			}
		}

		internal void PostMountInitialize(ComponentVersion databaseVersion)
		{
			PhysicalColumn physicalColumn = this.mailboxPartitionNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxPartitionNumber = null;
			}
			physicalColumn = this.inid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.inid = null;
			}
			physicalColumn = this.attachmentId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.attachmentId = null;
			}
			physicalColumn = this.attachmentMethod;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.attachmentMethod = null;
			}
			physicalColumn = this.renderingPosition;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.renderingPosition = null;
			}
			physicalColumn = this.creationTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.creationTime = null;
			}
			physicalColumn = this.lastModificationTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastModificationTime = null;
			}
			physicalColumn = this.size;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.size = null;
			}
			physicalColumn = this.name;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.name = null;
			}
			physicalColumn = this.contentId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.contentId = null;
			}
			physicalColumn = this.contentType;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.contentType = null;
			}
			physicalColumn = this.content;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.content = null;
			}
			physicalColumn = this.messageFlagsActual;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageFlagsActual = null;
			}
			physicalColumn = this.mailFlags;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailFlags = null;
			}
			physicalColumn = this.recipientList;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.recipientList = null;
			}
			physicalColumn = this.subobjectsBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.subobjectsBlob = null;
			}
			physicalColumn = this.propertyBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propertyBlob = null;
			}
			physicalColumn = this.largePropertyValueBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.largePropertyValueBlob = null;
			}
			physicalColumn = this.isEmbeddedMessage;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.isEmbeddedMessage = null;
			}
			physicalColumn = this.extensionBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.extensionBlob = null;
			}
			physicalColumn = this.fullTextType;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.fullTextType = null;
			}
			physicalColumn = this.language;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.language = null;
			}
			physicalColumn = this.propertyBag;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propertyBag = null;
			}
			physicalColumn = this.mailboxNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxNumber = null;
			}
			for (int i = this.Table.Columns.Count - 1; i >= 0; i--)
			{
				this.Table.Columns[i].Index = i;
			}
			Index index = this.attachmentPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.attachmentPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string InidName = "Inid";

		public const string AttachmentIdName = "AttachmentId";

		public const string AttachmentMethodName = "AttachmentMethod";

		public const string RenderingPositionName = "RenderingPosition";

		public const string CreationTimeName = "CreationTime";

		public const string LastModificationTimeName = "LastModificationTime";

		public const string SizeName = "Size";

		public const string NameName = "Name";

		public const string ContentIdName = "ContentId";

		public const string ContentTypeName = "ContentType";

		public const string ContentName = "Content";

		public const string MessageFlagsActualName = "MessageFlagsActual";

		public const string MailFlagsName = "MailFlags";

		public const string RecipientListName = "RecipientList";

		public const string SubobjectsBlobName = "SubobjectsBlob";

		public const string PropertyBlobName = "PropertyBlob";

		public const string LargePropertyValueBlobName = "LargePropertyValueBlob";

		public const string IsEmbeddedMessageName = "IsEmbeddedMessage";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string FullTextTypeName = "FullTextType";

		public const string LanguageName = "Language";

		public const string PropertyBagName = "PropertyBag";

		public const string MailboxNumberName = "MailboxNumber";

		public const string PhysicalTableName = "Attachment";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn inid;

		private PhysicalColumn attachmentId;

		private PhysicalColumn attachmentMethod;

		private PhysicalColumn renderingPosition;

		private PhysicalColumn creationTime;

		private PhysicalColumn lastModificationTime;

		private PhysicalColumn size;

		private PhysicalColumn name;

		private PhysicalColumn contentId;

		private PhysicalColumn contentType;

		private PhysicalColumn content;

		private PhysicalColumn messageFlagsActual;

		private PhysicalColumn mailFlags;

		private PhysicalColumn recipientList;

		private PhysicalColumn subobjectsBlob;

		private PhysicalColumn propertyBlob;

		private PhysicalColumn largePropertyValueBlob;

		private PhysicalColumn isEmbeddedMessage;

		private PhysicalColumn extensionBlob;

		private PhysicalColumn fullTextType;

		private PhysicalColumn language;

		private PhysicalColumn propertyBag;

		private PhysicalColumn mailboxNumber;

		private Index attachmentPK;

		private Table table;
	}
}
