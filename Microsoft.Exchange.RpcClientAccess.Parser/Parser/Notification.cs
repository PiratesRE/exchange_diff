using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class Notification
	{
		protected Notification(Notification.NotificationModifiers notificationType)
		{
			if ((ushort)(notificationType & ~(Notification.NotificationModifiers.CriticalError | Notification.NotificationModifiers.NewMail | Notification.NotificationModifiers.ObjectCreated | Notification.NotificationModifiers.ObjectDeleted | Notification.NotificationModifiers.ObjectModified | Notification.NotificationModifiers.ObjectMoved | Notification.NotificationModifiers.ObjectCopied | Notification.NotificationModifiers.SearchComplete | Notification.NotificationModifiers.TableModified | Notification.NotificationModifiers.StatusObject | Notification.NotificationModifiers.Extended)) != 0)
			{
				throw new ArgumentException("Invalid flag value set", "notificationType");
			}
			this.notificationType = notificationType;
		}

		public static Notification Parse(Reader reader, PropertyTag[] originalPropertyTags, Encoding string8Encoding)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			Notification.NotificationModifiers notificationModifiers = (Notification.NotificationModifiers)reader.ReadUInt16();
			Notification.NotificationModifiers notificationModifiers2 = notificationModifiers & Notification.NotificationModifiers.NotificationTypeMask;
			if (notificationModifiers2 <= Notification.NotificationModifiers.ObjectModified)
			{
				switch (notificationModifiers2)
				{
				case Notification.NotificationModifiers.NewMail:
					return new Notification.NewMailNotification(reader, notificationModifiers);
				case Notification.NotificationModifiers.CriticalError | Notification.NotificationModifiers.NewMail:
					break;
				case Notification.NotificationModifiers.ObjectCreated:
					return new Notification.ObjectCreatedNotification(reader, notificationModifiers);
				default:
					if (notificationModifiers2 == Notification.NotificationModifiers.ObjectDeleted)
					{
						return new Notification.ObjectDeletedNotification(reader, notificationModifiers);
					}
					if (notificationModifiers2 == Notification.NotificationModifiers.ObjectModified)
					{
						return new Notification.ObjectModifiedNotification(reader, notificationModifiers);
					}
					break;
				}
			}
			else if (notificationModifiers2 <= Notification.NotificationModifiers.ObjectCopied)
			{
				if (notificationModifiers2 == Notification.NotificationModifiers.ObjectMoved)
				{
					return new Notification.ObjectMovedNotification(reader, notificationModifiers);
				}
				if (notificationModifiers2 == Notification.NotificationModifiers.ObjectCopied)
				{
					return new Notification.ObjectCopiedNotification(reader, notificationModifiers);
				}
			}
			else
			{
				if (notificationModifiers2 == Notification.NotificationModifiers.SearchComplete)
				{
					return new Notification.SearchCompleteNotification(reader, notificationModifiers);
				}
				if (notificationModifiers2 == Notification.NotificationModifiers.TableModified)
				{
					return Notification.TableModifiedNotificationFactory.Create(reader, notificationModifiers, originalPropertyTags, string8Encoding);
				}
			}
			throw new BufferParseException(string.Format("Notification type not supported: {0}", notificationModifiers & Notification.NotificationModifiers.NotificationTypeMask));
		}

		public void Serialize(Writer writer, Encoding string8Encoding)
		{
			Notification.NotificationModifiers modifiers = this.GetModifiers();
			writer.WriteUInt16((ushort)modifiers);
			this.InternalSerialize(writer, modifiers, string8Encoding);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			stringBuilder.Append("[");
			this.AppendToString(stringBuilder);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		protected virtual Notification.NotificationModifiers GetModifiers()
		{
			return this.notificationType;
		}

		protected virtual void AppendToString(StringBuilder sb)
		{
			sb.AppendFormat("Modifiers [{0}]", this.GetModifiers().ToString());
		}

		protected abstract void InternalSerialize(Writer writer, Notification.NotificationModifiers modifiers, Encoding string8Encoding);

		private readonly Notification.NotificationModifiers notificationType;

		[Flags]
		internal enum NotificationModifiers : ushort
		{
			CriticalError = 1,
			NewMail = 2,
			ObjectCreated = 4,
			ObjectDeleted = 8,
			ObjectModified = 16,
			ObjectMoved = 32,
			ObjectCopied = 64,
			SearchComplete = 128,
			TableModified = 256,
			StatusObject = 512,
			Extended = 1024,
			TotalItemsChanged = 4096,
			UnreadItemsChanged = 8192,
			SearchFolder = 16384,
			Message = 32768,
			NotificationTypeMask = 2047
		}

		internal enum TableModifiedNotificationType : ushort
		{
			TableChanged = 1,
			TableError,
			TableRowAdded,
			TableRowDeleted,
			TableRowModified,
			TableSortDone,
			TableRestrictDone,
			TableSetColumnDone,
			TableReload,
			TableRowDeletedExtended
		}

		internal static class TableModifiedNotificationFactory
		{
			public static Notification.TableModifiedNotification Create(Reader reader, Notification.NotificationModifiers modifiers, PropertyTag[] originalPropertyTags, Encoding string8Encoding)
			{
				Notification.TableModifiedNotificationType tableModifiedNotificationType = (Notification.TableModifiedNotificationType)reader.ReadUInt16();
				switch (tableModifiedNotificationType)
				{
				case Notification.TableModifiedNotificationType.TableChanged:
				case Notification.TableModifiedNotificationType.TableRestrictDone:
				case Notification.TableModifiedNotificationType.TableReload:
					return new Notification.TableModifiedNotification(reader, modifiers, tableModifiedNotificationType);
				case Notification.TableModifiedNotificationType.TableRowAdded:
				case Notification.TableModifiedNotificationType.TableRowModified:
					return new Notification.TableModifiedNotification.TableRowAddModifiedNotification(reader, modifiers, tableModifiedNotificationType, originalPropertyTags, string8Encoding);
				case Notification.TableModifiedNotificationType.TableRowDeleted:
					return new Notification.TableModifiedNotification.TableRowDeletedModifiedNotification(reader, modifiers, tableModifiedNotificationType);
				case Notification.TableModifiedNotificationType.TableRowDeletedExtended:
					return new Notification.TableModifiedNotification.TableRowDeletedExtendedNotification(reader, modifiers, tableModifiedNotificationType, string8Encoding);
				}
				throw new BufferParseException(string.Format("TableModifiedNotificationType not supported: {0}", tableModifiedNotificationType));
			}
		}

		internal class TableModifiedNotification : Notification
		{
			public TableModifiedNotification(Notification.TableModifiedNotificationType tableModifiedNotificationType) : base(Notification.NotificationModifiers.TableModified)
			{
				this.tableModifiedNotificationType = tableModifiedNotificationType;
			}

			internal TableModifiedNotification(Reader reader, Notification.NotificationModifiers modifiers, Notification.TableModifiedNotificationType tableModifiedNotificationType) : base(modifiers & Notification.NotificationModifiers.NotificationTypeMask)
			{
				if ((ushort)(modifiers & Notification.NotificationModifiers.NotificationTypeMask) != 256)
				{
					throw new ArgumentException("Invalid notification type.", "modifiers");
				}
				this.tableModifiedNotificationType = tableModifiedNotificationType;
			}

			protected override void InternalSerialize(Writer writer, Notification.NotificationModifiers modifiers, Encoding string8Encoding)
			{
				writer.WriteUInt16((ushort)this.tableModifiedNotificationType);
			}

			protected override void AppendToString(StringBuilder sb)
			{
				base.AppendToString(sb);
				sb.AppendFormat(" Type [{0}]", this.tableModifiedNotificationType);
			}

			public Notification.TableModifiedNotificationType NotificationType
			{
				get
				{
					return this.tableModifiedNotificationType;
				}
			}

			private Notification.TableModifiedNotificationType tableModifiedNotificationType;

			private class TableModifiedNotificationContext
			{
				internal TableModifiedNotificationContext(StoreId folderId)
				{
					this.folderId = folderId;
				}

				internal TableModifiedNotificationContext(StoreId folderId, StoreId messageId, uint instance) : this(folderId)
				{
					this.messageId = new StoreId?(messageId);
					this.instance = new uint?(instance);
				}

				internal TableModifiedNotificationContext(Reader reader, Notification.NotificationModifiers modifiers)
				{
					this.folderId = StoreId.Parse(reader);
					if ((ushort)(modifiers & Notification.NotificationModifiers.Message) != 0)
					{
						this.messageId = new StoreId?(StoreId.Parse(reader));
						this.instance = new uint?(reader.ReadUInt32());
					}
				}

				internal void Serialize(Writer writer)
				{
					this.folderId.Serialize(writer);
					if (this.messageId != null)
					{
						this.messageId.Value.Serialize(writer);
						writer.WriteUInt32(this.instance.Value);
					}
				}

				internal Notification.NotificationModifiers GetModifiers(Notification.NotificationModifiers modifiers, bool isInSearchFolder)
				{
					if (this.MessageId != null)
					{
						modifiers |= Notification.NotificationModifiers.Message;
					}
					if (isInSearchFolder)
					{
						modifiers |= Notification.NotificationModifiers.SearchFolder;
					}
					return modifiers;
				}

				internal void AppendToString(StringBuilder sb)
				{
					sb.AppendFormat(" [FID [{0}]", this.FolderId);
					if (this.MessageId != null)
					{
						sb.AppendFormat(" MID [{0}] Inst [{1}]", this.MessageId.Value, this.Instance);
					}
					sb.Append("]");
				}

				public StoreId FolderId
				{
					get
					{
						return this.folderId;
					}
				}

				public StoreId? MessageId
				{
					get
					{
						return this.messageId;
					}
				}

				public uint? Instance
				{
					get
					{
						return this.instance;
					}
				}

				private readonly StoreId folderId;

				private readonly StoreId? messageId;

				private readonly uint? instance;
			}

			internal class TableRowDeletedModifiedNotification : Notification.TableModifiedNotification
			{
				internal TableRowDeletedModifiedNotification(Notification.TableModifiedNotificationType tableModifiedNotificationType, StoreId folderId, StoreId messageId, uint instance, bool isInSearchFolder) : base(tableModifiedNotificationType)
				{
					this.tableModifiedNotificationContext = new Notification.TableModifiedNotification.TableModifiedNotificationContext(folderId, messageId, instance);
					this.IsInSearchFolder = isInSearchFolder;
				}

				internal TableRowDeletedModifiedNotification(Notification.TableModifiedNotificationType tableModifiedNotificationType, StoreId folderId) : base(tableModifiedNotificationType)
				{
					this.tableModifiedNotificationContext = new Notification.TableModifiedNotification.TableModifiedNotificationContext(folderId);
				}

				internal TableRowDeletedModifiedNotification(Reader reader, Notification.NotificationModifiers modifiers, Notification.TableModifiedNotificationType tableModifiedNotificationType) : base(reader, modifiers, tableModifiedNotificationType)
				{
					this.tableModifiedNotificationContext = new Notification.TableModifiedNotification.TableModifiedNotificationContext(reader, modifiers);
					this.IsInSearchFolder = ((ushort)(modifiers & Notification.NotificationModifiers.SearchFolder) != 0);
				}

				public StoreId FolderId
				{
					get
					{
						return this.tableModifiedNotificationContext.FolderId;
					}
				}

				public StoreId? MessageId
				{
					get
					{
						return this.tableModifiedNotificationContext.MessageId;
					}
				}

				internal bool IsInSearchFolder { get; set; }

				protected override void InternalSerialize(Writer writer, Notification.NotificationModifiers modifiers, Encoding string8Encoding)
				{
					base.InternalSerialize(writer, modifiers, string8Encoding);
					this.tableModifiedNotificationContext.Serialize(writer);
				}

				protected override Notification.NotificationModifiers GetModifiers()
				{
					return this.tableModifiedNotificationContext.GetModifiers(base.GetModifiers(), this.IsInSearchFolder);
				}

				protected override void AppendToString(StringBuilder sb)
				{
					base.AppendToString(sb);
					sb.Append(" Row");
					this.tableModifiedNotificationContext.AppendToString(sb);
				}

				private readonly Notification.TableModifiedNotification.TableModifiedNotificationContext tableModifiedNotificationContext;
			}

			internal sealed class TableRowDeletedExtendedNotification : Notification.TableModifiedNotification.TableRowDeletedModifiedNotification
			{
				internal TableRowDeletedExtendedNotification(Notification.TableModifiedNotificationType tableModifiedNotificationType, StoreId folderId, StoreId messageId, uint instance, PropertyValue[] propertyValues, bool isInSearchFolder) : base(tableModifiedNotificationType, folderId, messageId, instance, isInSearchFolder)
				{
					this.propertyValues = propertyValues;
				}

				internal TableRowDeletedExtendedNotification(Reader reader, Notification.NotificationModifiers modifiers, Notification.TableModifiedNotificationType tableModifiedNotificationType, Encoding string8Encoding) : base(reader, modifiers, tableModifiedNotificationType)
				{
					this.propertyValues = reader.ReadCountAndPropertyValueList(string8Encoding, WireFormatStyle.Rop);
				}

				internal PropertyValue[] PropertyValues
				{
					get
					{
						return this.propertyValues;
					}
				}

				protected override void InternalSerialize(Writer writer, Notification.NotificationModifiers modifiers, Encoding string8Encoding)
				{
					base.InternalSerialize(writer, modifiers, string8Encoding);
					writer.WriteCountAndPropertyValueList(this.propertyValues, string8Encoding, WireFormatStyle.Rop);
				}

				protected override void AppendToString(StringBuilder sb)
				{
					base.AppendToString(sb);
					sb.Append(" Properties: ");
					for (int i = 0; i < this.propertyValues.Length; i++)
					{
						if (i != 0)
						{
							sb.Append(" ");
						}
						sb.Append("[");
						this.propertyValues[i].AppendToString(sb);
						sb.Append("]");
					}
				}

				private PropertyValue[] propertyValues;
			}

			internal sealed class TableRowAddModifiedNotification : Notification.TableModifiedNotification
			{
				public TableRowAddModifiedNotification(Notification.TableModifiedNotificationType tableModifiedNotificationType, StoreId folderId, StoreId messageId, uint instance, StoreId insertAfterFolderId, StoreId insertAfterMessageId, uint insertAfterInstance, PropertyTag[] originalPropertyTags, PropertyValue[] propertyValues, bool isInSearchFolder) : base(tableModifiedNotificationType)
				{
					this.tableModifiedNotificationContext = new Notification.TableModifiedNotification.TableModifiedNotificationContext(folderId, messageId, instance);
					this.insertAfterTableModifiedNotificationContext = new Notification.TableModifiedNotification.TableModifiedNotificationContext(insertAfterFolderId, insertAfterMessageId, insertAfterInstance);
					this.SetPropertyRow(originalPropertyTags, propertyValues);
					this.IsInSearchFolder = isInSearchFolder;
				}

				public TableRowAddModifiedNotification(Notification.TableModifiedNotificationType tableModifiedNotificationType, StoreId folderId, StoreId insertAfterFolderId, PropertyTag[] originalPropertyTags, PropertyValue[] propertyValues) : base(tableModifiedNotificationType)
				{
					this.tableModifiedNotificationContext = new Notification.TableModifiedNotification.TableModifiedNotificationContext(folderId);
					this.insertAfterTableModifiedNotificationContext = new Notification.TableModifiedNotification.TableModifiedNotificationContext(insertAfterFolderId);
					this.SetPropertyRow(originalPropertyTags, propertyValues);
				}

				private void SetPropertyRow(PropertyTag[] originalPropertyTags, PropertyValue[] propertyValues)
				{
					if (originalPropertyTags == null)
					{
						throw new ArgumentNullException("originalPropertyTags cannot be null.");
					}
					if (propertyValues == null)
					{
						throw new ArgumentNullException("propertyValues cannot be null.");
					}
					this.propertyTags = originalPropertyTags;
					this.propertyRow = new PropertyRow(originalPropertyTags, propertyValues);
				}

				internal TableRowAddModifiedNotification(Reader reader, Notification.NotificationModifiers modifiers, Notification.TableModifiedNotificationType tableModifiedNotificationType, PropertyTag[] propertyTags, Encoding string8Encoding) : base(reader, modifiers, tableModifiedNotificationType)
				{
					this.tableModifiedNotificationContext = new Notification.TableModifiedNotification.TableModifiedNotificationContext(reader, modifiers);
					this.insertAfterTableModifiedNotificationContext = new Notification.TableModifiedNotification.TableModifiedNotificationContext(reader, modifiers);
					ushort count = reader.ReadUInt16();
					this.IsInSearchFolder = ((ushort)(modifiers & Notification.NotificationModifiers.SearchFolder) != 0);
					this.propertyTags = propertyTags;
					if (this.propertyTags == null)
					{
						reader.ReadArraySegment((uint)count);
						this.propertyRow = PropertyRow.Empty;
						return;
					}
					this.propertyRow = PropertyRow.Parse(reader, propertyTags, WireFormatStyle.Rop);
					this.propertyRow.ResolveString8Values(string8Encoding);
				}

				internal PropertyTag[] PropertyTags
				{
					get
					{
						return this.propertyTags;
					}
				}

				internal PropertyRow PropertyRow
				{
					get
					{
						return this.propertyRow;
					}
				}

				internal bool IsInSearchFolder { get; set; }

				protected override void InternalSerialize(Writer writer, Notification.NotificationModifiers modifiers, Encoding string8Encoding)
				{
					base.InternalSerialize(writer, modifiers, string8Encoding);
					this.tableModifiedNotificationContext.Serialize(writer);
					this.insertAfterTableModifiedNotificationContext.Serialize(writer);
					long position = writer.Position;
					writer.WriteUInt16(0);
					long position2 = writer.Position;
					this.propertyRow.Serialize(writer, string8Encoding, WireFormatStyle.Rop);
					long position3 = writer.Position;
					writer.Position = position;
					writer.WriteUInt16((ushort)(position3 - position2));
					writer.Position = position3;
				}

				protected override Notification.NotificationModifiers GetModifiers()
				{
					return this.tableModifiedNotificationContext.GetModifiers(base.GetModifiers(), this.IsInSearchFolder);
				}

				protected override void AppendToString(StringBuilder sb)
				{
					base.AppendToString(sb);
					sb.Append(" Row");
					this.tableModifiedNotificationContext.AppendToString(sb);
					sb.Append(" Prev Row");
					this.insertAfterTableModifiedNotificationContext.AppendToString(sb);
					sb.AppendFormat(" Properties [{0}]", this.propertyRow.ToString());
				}

				private readonly Notification.TableModifiedNotification.TableModifiedNotificationContext tableModifiedNotificationContext;

				private readonly Notification.TableModifiedNotification.TableModifiedNotificationContext insertAfterTableModifiedNotificationContext;

				private PropertyTag[] propertyTags;

				private PropertyRow propertyRow;
			}
		}

		internal abstract class ObjectNotification : Notification
		{
			protected ObjectNotification(Notification.NotificationModifiers notificationType, StoreId folderId, StoreId? messageId) : base(notificationType)
			{
				this.folderId = folderId;
				this.messageId = messageId;
			}

			protected ObjectNotification(Reader reader, Notification.NotificationModifiers modifiers) : base(modifiers & Notification.NotificationModifiers.NotificationTypeMask)
			{
				if (reader == null)
				{
					throw new ArgumentNullException("reader");
				}
				this.folderId = StoreId.Parse(reader);
				if ((ushort)(modifiers & Notification.NotificationModifiers.Message) != 0)
				{
					this.messageId = new StoreId?(StoreId.Parse(reader));
				}
			}

			protected override void InternalSerialize(Writer writer, Notification.NotificationModifiers modifiers, Encoding string8Encoding)
			{
				this.folderId.Serialize(writer);
				if ((ushort)(modifiers & Notification.NotificationModifiers.Message) != 0)
				{
					this.messageId.Value.Serialize(writer);
				}
			}

			protected override Notification.NotificationModifiers GetModifiers()
			{
				Notification.NotificationModifiers notificationModifiers = base.GetModifiers();
				if (this.messageId != null)
				{
					notificationModifiers |= Notification.NotificationModifiers.Message;
				}
				return notificationModifiers;
			}

			protected override void AppendToString(StringBuilder sb)
			{
				base.AppendToString(sb);
				sb.AppendFormat(" FID [{0}]", this.FolderId);
				if (this.MessageId != null)
				{
					sb.AppendFormat(" MID [{0}]", this.MessageId);
				}
			}

			public StoreId FolderId
			{
				get
				{
					return this.folderId;
				}
			}

			public StoreId? MessageId
			{
				get
				{
					return this.messageId;
				}
			}

			private readonly StoreId folderId;

			private readonly StoreId? messageId;
		}

		internal sealed class SearchCompleteNotification : Notification.ObjectNotification
		{
			public SearchCompleteNotification(StoreId folderId) : base(Notification.NotificationModifiers.SearchComplete, folderId, null)
			{
			}

			internal SearchCompleteNotification(Reader reader, Notification.NotificationModifiers modifiers) : base(reader, modifiers)
			{
				if ((ushort)(modifiers & Notification.NotificationModifiers.NotificationTypeMask) != 128)
				{
					throw new ArgumentException("Invalid notification type.", "modifiers");
				}
			}
		}

		internal sealed class NewMailNotification : Notification.ObjectNotification
		{
			public NewMailNotification(StoreId folderId, StoreId messageId, uint messageFlags, string messageClass) : base(Notification.NotificationModifiers.NewMail, folderId, new StoreId?(messageId))
			{
				if (messageClass == null)
				{
					throw new ArgumentNullException("Message class cannot be null.", "messageClass");
				}
				this.messageFlags = messageFlags;
				this.messageClass = messageClass;
			}

			internal NewMailNotification(Reader reader, Notification.NotificationModifiers modifiers) : base(reader, modifiers)
			{
				if ((ushort)(modifiers & Notification.NotificationModifiers.NotificationTypeMask) != 2)
				{
					throw new ArgumentException("Invalid notification type.", "modifiers");
				}
				if ((ushort)(modifiers & Notification.NotificationModifiers.Message) == 0 || base.MessageId == null)
				{
					throw new BufferParseException("Message ID is not present in NewMail notification.");
				}
				this.messageFlags = reader.ReadUInt32();
				bool flag = reader.ReadBool();
				if (flag)
				{
					this.messageClass = reader.ReadUnicodeString(StringFlags.IncludeNull);
					return;
				}
				this.messageClass = reader.ReadAsciiString(StringFlags.IncludeNull);
			}

			protected override Notification.NotificationModifiers GetModifiers()
			{
				return base.GetModifiers() | Notification.NotificationModifiers.Message;
			}

			protected override void InternalSerialize(Writer writer, Notification.NotificationModifiers modifiers, Encoding string8Encoding)
			{
				base.InternalSerialize(writer, modifiers, string8Encoding);
				writer.WriteUInt32(this.messageFlags);
				writer.WriteBool(false);
				writer.WriteAsciiString(this.messageClass, StringFlags.IncludeNull);
			}

			protected override void AppendToString(StringBuilder sb)
			{
				base.AppendToString(sb);
				sb.AppendFormat(" MsgFlags [{0}] MsgClass [{1}]", this.messageFlags, this.messageClass);
			}

			public string MessageClass
			{
				get
				{
					return this.messageClass;
				}
			}

			public MessageFlags MessageFlags
			{
				get
				{
					return (MessageFlags)this.messageFlags;
				}
			}

			private readonly uint messageFlags;

			private readonly string messageClass;
		}

		internal abstract class WithParentFolderNotification : Notification.ObjectNotification
		{
			protected WithParentFolderNotification(Notification.NotificationModifiers notificationType, StoreId folderId, StoreId? messageId, StoreId parentFolderId) : base(notificationType, folderId, messageId)
			{
				this.parentFolderId = parentFolderId;
			}

			protected WithParentFolderNotification(Reader reader, Notification.NotificationModifiers modifiers) : base(reader, modifiers)
			{
				if ((ushort)(modifiers & Notification.NotificationModifiers.Message) == 0 || (ushort)(modifiers & Notification.NotificationModifiers.SearchFolder) != 0)
				{
					this.parentFolderId = StoreId.Parse(reader);
					return;
				}
				this.parentFolderId = base.FolderId;
			}

			protected override void InternalSerialize(Writer writer, Notification.NotificationModifiers modifiers, Encoding string8Encoding)
			{
				base.InternalSerialize(writer, modifiers, string8Encoding);
				if ((ushort)(modifiers & Notification.NotificationModifiers.Message) == 0 || (ushort)(modifiers & Notification.NotificationModifiers.SearchFolder) != 0)
				{
					this.parentFolderId.Serialize(writer);
				}
			}

			protected override Notification.NotificationModifiers GetModifiers()
			{
				Notification.NotificationModifiers notificationModifiers = base.GetModifiers();
				if ((ushort)(notificationModifiers & Notification.NotificationModifiers.Message) != 0 && !this.parentFolderId.Equals(base.FolderId))
				{
					notificationModifiers |= Notification.NotificationModifiers.SearchFolder;
				}
				return notificationModifiers;
			}

			protected override void AppendToString(StringBuilder sb)
			{
				Notification.NotificationModifiers modifiers = this.GetModifiers();
				base.AppendToString(sb);
				if ((ushort)(modifiers & Notification.NotificationModifiers.Message) == 0 || (ushort)(modifiers & Notification.NotificationModifiers.SearchFolder) != 0)
				{
					sb.AppendFormat(" FID [{0}]", this.parentFolderId);
				}
			}

			private readonly StoreId parentFolderId;
		}

		internal sealed class ObjectCreatedNotification : Notification.WithParentFolderNotification
		{
			public ObjectCreatedNotification(StoreId folderId, StoreId? messageId, StoreId parentFolderId, PropertyTag[] propertyTags) : base(Notification.NotificationModifiers.ObjectCreated, folderId, messageId, parentFolderId)
			{
				if (propertyTags == null)
				{
					throw new ArgumentNullException("propertyTags");
				}
				this.propertyTags = propertyTags;
			}

			internal ObjectCreatedNotification(Reader reader, Notification.NotificationModifiers modifiers) : base(reader, modifiers)
			{
				if ((ushort)(modifiers & Notification.NotificationModifiers.NotificationTypeMask) != 4)
				{
					throw new ArgumentException("Invalid notification type.", "modifiers");
				}
				this.propertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
			}

			protected override void InternalSerialize(Writer writer, Notification.NotificationModifiers modifiers, Encoding string8Encoding)
			{
				base.InternalSerialize(writer, modifiers, string8Encoding);
				writer.WriteCountAndPropertyTagArray(this.propertyTags, FieldLength.WordSize);
			}

			protected override void AppendToString(StringBuilder sb)
			{
				base.AppendToString(sb);
				sb.Append(" PropTags [");
				for (int i = 0; i < this.propertyTags.Length; i++)
				{
					sb.AppendFormat(" {0}", this.propertyTags[i].ToString());
				}
				sb.Append(" ]");
			}

			internal PropertyTag[] PropertyTags
			{
				get
				{
					return this.propertyTags;
				}
			}

			private readonly PropertyTag[] propertyTags;
		}

		internal sealed class ObjectDeletedNotification : Notification.WithParentFolderNotification
		{
			public ObjectDeletedNotification(StoreId folderId, StoreId? messageId, StoreId parentFolderId) : base(Notification.NotificationModifiers.ObjectDeleted, folderId, messageId, parentFolderId)
			{
			}

			internal ObjectDeletedNotification(Reader reader, Notification.NotificationModifiers modifiers) : base(reader, modifiers)
			{
				if ((ushort)(modifiers & Notification.NotificationModifiers.NotificationTypeMask) != 8)
				{
					throw new ArgumentException("Invalid notification type.", "modifiers");
				}
			}
		}

		internal sealed class ObjectModifiedNotification : Notification.ObjectNotification
		{
			public ObjectModifiedNotification(StoreId folderId, StoreId? messageId, PropertyTag[] propertyTags, int totalItemsChanged, int unreadItemsChanged) : base(Notification.NotificationModifiers.ObjectModified, folderId, messageId)
			{
				if (propertyTags == null)
				{
					throw new ArgumentNullException("propertyTags");
				}
				if (messageId != null && totalItemsChanged != -1)
				{
					throw new ArgumentException("The value of totalItemsChanged must be -1 for message notifications", "totalItemsChanged");
				}
				if (messageId != null && unreadItemsChanged != -1)
				{
					throw new ArgumentException("The value of unreadItemsChanged must be -1 for message notifications", "unreadItemsChanged");
				}
				this.propertyTags = propertyTags;
				this.totalItemsChanged = totalItemsChanged;
				this.unreadItemsChanged = unreadItemsChanged;
			}

			internal ObjectModifiedNotification(Reader reader, Notification.NotificationModifiers modifiers) : base(reader, modifiers)
			{
				if ((ushort)(modifiers & Notification.NotificationModifiers.NotificationTypeMask) != 16)
				{
					throw new ArgumentException("Invalid notification type.", "modifiers");
				}
				this.propertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
				this.totalItemsChanged = (((ushort)(modifiers & Notification.NotificationModifiers.TotalItemsChanged) != 0) ? reader.ReadInt32() : -1);
				this.unreadItemsChanged = (((ushort)(modifiers & Notification.NotificationModifiers.UnreadItemsChanged) != 0) ? reader.ReadInt32() : -1);
			}

			protected override void InternalSerialize(Writer writer, Notification.NotificationModifiers modifiers, Encoding string8Encoding)
			{
				base.InternalSerialize(writer, modifiers, string8Encoding);
				writer.WriteCountAndPropertyTagArray(this.propertyTags, FieldLength.WordSize);
				if ((ushort)(modifiers & Notification.NotificationModifiers.TotalItemsChanged) != 0)
				{
					writer.WriteInt32(this.totalItemsChanged);
				}
				if ((ushort)(modifiers & Notification.NotificationModifiers.UnreadItemsChanged) != 0)
				{
					writer.WriteInt32(this.unreadItemsChanged);
				}
			}

			protected override Notification.NotificationModifiers GetModifiers()
			{
				Notification.NotificationModifiers notificationModifiers = base.GetModifiers();
				if (this.totalItemsChanged != -1)
				{
					notificationModifiers |= Notification.NotificationModifiers.TotalItemsChanged;
				}
				if (this.unreadItemsChanged != -1)
				{
					notificationModifiers |= Notification.NotificationModifiers.UnreadItemsChanged;
				}
				return notificationModifiers;
			}

			protected override void AppendToString(StringBuilder sb)
			{
				base.AppendToString(sb);
				sb.Append(" PropTags [");
				for (int i = 0; i < this.propertyTags.Length; i++)
				{
					sb.AppendFormat(" {0}", this.propertyTags[i].ToString());
				}
				sb.Append(" ]");
				if (this.totalItemsChanged != -1)
				{
					sb.AppendFormat(" Changed [{0}]", this.totalItemsChanged);
				}
				if (this.unreadItemsChanged != -1)
				{
					sb.AppendFormat(" Unread [{0}]", this.unreadItemsChanged);
				}
			}

			public PropertyTag[] PropertyTags
			{
				get
				{
					return this.propertyTags;
				}
			}

			private readonly PropertyTag[] propertyTags;

			private readonly int totalItemsChanged;

			private readonly int unreadItemsChanged;
		}

		internal abstract class WithOldItemIdNotification : Notification.WithParentFolderNotification
		{
			protected WithOldItemIdNotification(Notification.NotificationModifiers notificationType, StoreId folderId, StoreId? messageId, StoreId parentFolderId, StoreId oldFolderId, StoreId? oldMessageId, StoreId oldParentFolderId) : base(notificationType, folderId, messageId, parentFolderId)
			{
				if (messageId != null != (oldMessageId != null))
				{
					throw new ArgumentException("Message ID or old message ID parameters are invalid. The parameters must be either both null or both non-null.");
				}
				if (oldMessageId != null && !oldParentFolderId.Equals(oldFolderId))
				{
					throw new ArgumentException("The value of oldParentFolderId must be the same as the value of oldFolderId.", "oldParentFolderId");
				}
				this.oldFolderId = oldFolderId;
				this.oldMessageId = oldMessageId;
				this.oldParentFolderId = oldParentFolderId;
			}

			protected WithOldItemIdNotification(Reader reader, Notification.NotificationModifiers modifiers) : base(reader, modifiers)
			{
				this.oldFolderId = StoreId.Parse(reader);
				if ((ushort)(modifiers & Notification.NotificationModifiers.Message) != 0)
				{
					this.oldMessageId = new StoreId?(StoreId.Parse(reader));
					this.oldParentFolderId = this.oldFolderId;
					return;
				}
				this.oldParentFolderId = StoreId.Parse(reader);
			}

			public StoreId OldFolderId
			{
				get
				{
					return this.oldFolderId;
				}
			}

			public StoreId? OldMessageId
			{
				get
				{
					return this.oldMessageId;
				}
			}

			protected override void InternalSerialize(Writer writer, Notification.NotificationModifiers modifiers, Encoding string8Encoding)
			{
				base.InternalSerialize(writer, modifiers, string8Encoding);
				this.oldFolderId.Serialize(writer);
				if ((ushort)(modifiers & Notification.NotificationModifiers.Message) != 0)
				{
					this.oldMessageId.Value.Serialize(writer);
					return;
				}
				this.oldParentFolderId.Serialize(writer);
			}

			protected override void AppendToString(StringBuilder sb)
			{
				base.AppendToString(sb);
				sb.AppendFormat(" Old FID [{0}]", this.oldFolderId);
				if (this.oldMessageId != null)
				{
					sb.AppendFormat(" Old MID [{0}]", this.oldMessageId.Value);
					return;
				}
				sb.AppendFormat(" Old Parent FID [{0}]", this.oldParentFolderId.ToString());
			}

			private readonly StoreId oldFolderId;

			private readonly StoreId? oldMessageId;

			private readonly StoreId oldParentFolderId;
		}

		internal sealed class ObjectMovedNotification : Notification.WithOldItemIdNotification
		{
			public ObjectMovedNotification(StoreId folderId, StoreId? messageId, StoreId parentFolderId, StoreId oldFolderId, StoreId? oldMessageId, StoreId oldParentFolderId) : base(Notification.NotificationModifiers.ObjectMoved, folderId, messageId, parentFolderId, oldFolderId, oldMessageId, oldParentFolderId)
			{
			}

			internal ObjectMovedNotification(Reader reader, Notification.NotificationModifiers modifiers) : base(reader, modifiers)
			{
				if ((ushort)(modifiers & Notification.NotificationModifiers.NotificationTypeMask) != 32)
				{
					throw new ArgumentException("Invalid notification type.", "modifiers");
				}
			}
		}

		internal sealed class ObjectCopiedNotification : Notification.WithOldItemIdNotification
		{
			public ObjectCopiedNotification(StoreId folderId, StoreId? messageId, StoreId parentFolderId, StoreId oldFolderId, StoreId? oldMessageId, StoreId oldParentFolderId) : base(Notification.NotificationModifiers.ObjectCopied, folderId, messageId, parentFolderId, oldFolderId, oldMessageId, oldParentFolderId)
			{
			}

			internal ObjectCopiedNotification(Reader reader, Notification.NotificationModifiers modifiers) : base(reader, modifiers)
			{
				if ((ushort)(modifiers & Notification.NotificationModifiers.NotificationTypeMask) != 64)
				{
					throw new ArgumentException("Invalid notification type.", "modifiers");
				}
			}
		}
	}
}
