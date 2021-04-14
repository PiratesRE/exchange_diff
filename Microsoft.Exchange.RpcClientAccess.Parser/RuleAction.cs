using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal abstract class RuleAction
	{
		protected RuleAction(RuleActionType type, uint flavor, uint userFlags)
		{
			this.ActionType = type;
			this.Flavor = flavor;
			this.UserFlags = userFlags;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{").Append(this.ActionType);
			stringBuilder.Append(", flavor=").Append(this.Flavor);
			stringBuilder.Append(", flags=0x").AppendFormat("{0:X}", this.UserFlags);
			string value = this.InternalToString();
			if (!string.IsNullOrEmpty(value))
			{
				stringBuilder.Append(", ").Append(value);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		protected static string ByteArrayToString(byte[] buffer)
		{
			if (buffer != null)
			{
				StringBuilder stringBuilder = new StringBuilder(buffer.Length * 2);
				foreach (byte b in buffer)
				{
					stringBuilder.Append(RuleAction.HexDigits[(int)(b / 16)]);
					stringBuilder.Append(RuleAction.HexDigits[(int)(b % 16)]);
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		protected virtual string InternalToString()
		{
			return null;
		}

		internal static RuleAction[] Parse(Reader reader)
		{
			RuleAction.currentDepth += 1U;
			RuleAction[] result;
			try
			{
				if (256U < RuleAction.currentDepth)
				{
					throw new BufferParseException("Action depth is greater than the maximum depth allowed.");
				}
				uint num = (uint)reader.ReadUInt16();
				reader.CheckBoundary(num, 11U);
				RuleAction[] array = new RuleAction[num];
				for (uint num2 = 0U; num2 < num; num2 += 1U)
				{
					array[(int)((UIntPtr)num2)] = RuleAction.ParseOneAction(reader);
				}
				result = array;
			}
			finally
			{
				RuleAction.currentDepth -= 1U;
			}
			return result;
		}

		internal virtual void Serialize(Writer writer, Encoding string8Encoding)
		{
			writer.WriteByte((byte)this.ActionType);
			writer.WriteUInt32(this.Flavor);
			writer.WriteUInt32(this.UserFlags);
		}

		internal virtual void ResolveString8Values(Encoding string8Encoding)
		{
		}

		private static RuleAction ParseOneAction(Reader reader)
		{
			RuleAction.currentDepth += 1U;
			RuleAction result;
			try
			{
				if (256U < RuleAction.currentDepth)
				{
					throw new BufferParseException("Action depth is greater than the maximum depth allowed.");
				}
				reader.CheckBoundary(1U, 11U);
				uint num = (uint)reader.ReadUInt16();
				reader.CheckBoundary(num, 1U);
				RuleActionType ruleActionType = (RuleActionType)reader.ReadByte();
				uint flavor = reader.ReadUInt32();
				uint flags = reader.ReadUInt32();
				switch (ruleActionType)
				{
				case RuleActionType.Move:
					result = RuleAction.MoveAction.Parse(reader, flavor, flags);
					break;
				case RuleActionType.Copy:
					result = RuleAction.CopyAction.Parse(reader, flavor, flags);
					break;
				case RuleActionType.Reply:
					result = RuleAction.ReplyAction.Parse(reader, flavor, flags);
					break;
				case RuleActionType.OutOfOfficeReply:
					result = RuleAction.OutOfOfficeReplyAction.Parse(reader, flavor, flags);
					break;
				case RuleActionType.DeferAction:
					result = RuleAction.DeferAction.Parse(reader, flavor, flags, (ulong)(num - 9U));
					break;
				case RuleActionType.Bounce:
					result = RuleAction.BounceAction.Parse(reader, flavor, flags);
					break;
				case RuleActionType.Forward:
					result = RuleAction.ForwardAction.Parse(reader, flavor, flags);
					break;
				case RuleActionType.Delegate:
					result = RuleAction.DelegateAction.Parse(reader, flavor, flags);
					break;
				case RuleActionType.Tag:
					result = RuleAction.TagAction.Parse(reader, flavor, flags);
					break;
				case RuleActionType.Delete:
					result = RuleAction.DeleteAction.Parse(reader, flavor, flags);
					break;
				case RuleActionType.MarkAsRead:
					result = RuleAction.MarkAsReadAction.Parse(reader, flavor, flags);
					break;
				default:
					throw new BufferParseException(string.Format("Invalid action type {0}.", (byte)ruleActionType));
				}
			}
			finally
			{
				RuleAction.currentDepth -= 1U;
			}
			return result;
		}

		private const int MaximumDepth = 256;

		private const uint MinimumActionSize = 11U;

		private static readonly char[] HexDigits = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};

		[ThreadStatic]
		private static uint currentDepth = 0U;

		public readonly RuleActionType ActionType;

		public readonly uint Flavor;

		public readonly uint UserFlags;

		internal abstract class MoveCopyActionBase : RuleAction
		{
			protected MoveCopyActionBase(RuleActionType type, uint flavor, uint flags, bool folderIsInThisStore, byte[] storeEntryId, byte[] folderId) : base(type, flavor, flags)
			{
				if (!folderIsInThisStore)
				{
					Util.ThrowOnNullArgument(storeEntryId, "storeEntryId");
				}
				Util.ThrowOnNullArgument(folderId, "folderId");
				this.FolderIsInThisStore = folderIsInThisStore;
				this.DestinationStoreEntryId = (folderIsInThisStore ? null : storeEntryId);
				this.DestinationFolderId = folderId;
			}

			internal override void Serialize(Writer writer, Encoding string8Encoding)
			{
				base.Serialize(writer, string8Encoding);
				writer.WriteBool(this.FolderIsInThisStore);
				writer.WriteSizedBytes(this.FolderIsInThisStore ? RuleAction.MoveCopyActionBase.fakeStoreEntryId : this.DestinationStoreEntryId);
				writer.WriteSizedBytes(this.DestinationFolderId);
			}

			protected static void ParseMoveCopyData(Reader reader, out bool folderIsInThisStore, out byte[] storeEntryId, out byte[] folderId)
			{
				folderIsInThisStore = reader.ReadBool();
				storeEntryId = reader.ReadSizeAndByteArray();
				folderId = reader.ReadSizeAndByteArray();
			}

			protected override string InternalToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("folderInThisStore=").Append(this.FolderIsInThisStore);
				stringBuilder.Append(", storeEntryId={").Append(RuleAction.ByteArrayToString(this.DestinationStoreEntryId)).Append("}");
				stringBuilder.Append(", folderId={").Append(RuleAction.ByteArrayToString(this.DestinationFolderId)).Append("}");
				return stringBuilder.ToString();
			}

			public readonly bool FolderIsInThisStore;

			public readonly byte[] DestinationStoreEntryId;

			public readonly byte[] DestinationFolderId;

			private static byte[] fakeStoreEntryId = new byte[1];
		}

		internal sealed class MoveAction : RuleAction.MoveCopyActionBase
		{
			internal MoveAction(uint flavor, uint flags, bool folderIsInThisStore, byte[] storeEntryId, byte[] folderId) : base(RuleActionType.Move, flavor, flags, folderIsInThisStore, storeEntryId, folderId)
			{
			}

			internal static RuleAction.MoveAction Parse(Reader reader, uint flavor, uint flags)
			{
				bool folderIsInThisStore;
				byte[] storeEntryId;
				byte[] folderId;
				RuleAction.MoveCopyActionBase.ParseMoveCopyData(reader, out folderIsInThisStore, out storeEntryId, out folderId);
				return new RuleAction.MoveAction(flavor, flags, folderIsInThisStore, storeEntryId, folderId);
			}
		}

		internal sealed class CopyAction : RuleAction.MoveCopyActionBase
		{
			internal CopyAction(uint flavor, uint flags, bool folderIsInThisStore, byte[] storeEntryId, byte[] folderId) : base(RuleActionType.Copy, flavor, flags, folderIsInThisStore, storeEntryId, folderId)
			{
			}

			internal static RuleAction.CopyAction Parse(Reader reader, uint flavor, uint flags)
			{
				bool folderIsInThisStore;
				byte[] storeEntryId;
				byte[] folderId;
				RuleAction.MoveCopyActionBase.ParseMoveCopyData(reader, out folderIsInThisStore, out storeEntryId, out folderId);
				return new RuleAction.CopyAction(flavor, flags, folderIsInThisStore, storeEntryId, folderId);
			}
		}

		internal abstract class ReplyActionBase : RuleAction
		{
			internal ReplyActionBase(RuleActionType type, uint flavor, uint flags, StoreId replyTemplateFolderId, StoreId replyTemplateMessageId, Guid replyTemplateGuid) : base(type, flavor, flags)
			{
				this.ReplyTemplateFolderId = replyTemplateFolderId;
				this.ReplyTemplateMessageId = replyTemplateMessageId;
				this.ReplyTemplateGuid = replyTemplateGuid;
			}

			protected static void ParseReplyData(Reader reader, out StoreId replyTemplateFolderId, out StoreId replyTemplateMessageId, out Guid replyTemplateGuid)
			{
				replyTemplateFolderId = StoreId.Parse(reader);
				replyTemplateMessageId = StoreId.Parse(reader);
				replyTemplateGuid = reader.ReadGuid();
			}

			internal override void Serialize(Writer writer, Encoding string8Encoding)
			{
				base.Serialize(writer, string8Encoding);
				this.ReplyTemplateFolderId.Serialize(writer);
				this.ReplyTemplateMessageId.Serialize(writer);
				writer.WriteGuid(this.ReplyTemplateGuid);
			}

			protected override string InternalToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("TemplateFID=").Append(this.ReplyTemplateFolderId);
				stringBuilder.Append(", TemplateMID=").Append(this.ReplyTemplateMessageId);
				stringBuilder.Append(", TemplateGuid=").Append(this.ReplyTemplateGuid);
				return stringBuilder.ToString();
			}

			public readonly StoreId ReplyTemplateFolderId;

			public readonly StoreId ReplyTemplateMessageId;

			public readonly Guid ReplyTemplateGuid;
		}

		internal sealed class ReplyAction : RuleAction.ReplyActionBase
		{
			internal ReplyAction(uint flavor, uint flags, StoreId replyTemplateFolderId, StoreId replyTemplateMessageId, Guid replyTemplateGuid) : base(RuleActionType.Reply, flavor, flags, replyTemplateFolderId, replyTemplateMessageId, replyTemplateGuid)
			{
			}

			internal static RuleAction.ReplyAction Parse(Reader reader, uint flavor, uint flags)
			{
				StoreId replyTemplateFolderId;
				StoreId replyTemplateMessageId;
				Guid replyTemplateGuid;
				RuleAction.ReplyActionBase.ParseReplyData(reader, out replyTemplateFolderId, out replyTemplateMessageId, out replyTemplateGuid);
				return new RuleAction.ReplyAction(flavor, flags, replyTemplateFolderId, replyTemplateMessageId, replyTemplateGuid);
			}

			[Flags]
			public enum ReplyFlags
			{
				None = 0,
				DoNotSendToOriginator = 1,
				UseStockReplyTemplate = 2
			}
		}

		internal sealed class OutOfOfficeReplyAction : RuleAction.ReplyActionBase
		{
			internal OutOfOfficeReplyAction(uint flavor, uint flags, StoreId replyTemplateFolderId, StoreId replyTemplateMessageId, Guid replyTemplateGuid) : base(RuleActionType.OutOfOfficeReply, flavor, flags, replyTemplateFolderId, replyTemplateMessageId, replyTemplateGuid)
			{
			}

			internal static RuleAction.OutOfOfficeReplyAction Parse(Reader reader, uint flavor, uint flags)
			{
				StoreId replyTemplateFolderId;
				StoreId replyTemplateMessageId;
				Guid replyTemplateGuid;
				RuleAction.ReplyActionBase.ParseReplyData(reader, out replyTemplateFolderId, out replyTemplateMessageId, out replyTemplateGuid);
				return new RuleAction.OutOfOfficeReplyAction(flavor, flags, replyTemplateFolderId, replyTemplateMessageId, replyTemplateGuid);
			}
		}

		internal sealed class DeferAction : RuleAction
		{
			internal DeferAction(uint flavor, uint flags, byte[] data) : base(RuleActionType.DeferAction, flavor, flags)
			{
				this.Data = data;
			}

			internal static RuleAction.DeferAction Parse(Reader reader, uint flavor, uint flags, ulong length)
			{
				return new RuleAction.DeferAction(flavor, flags, reader.ReadBytes((uint)length));
			}

			internal override void Serialize(Writer writer, Encoding string8Encoding)
			{
				base.Serialize(writer, string8Encoding);
				if (this.Data.Length > 0)
				{
					writer.WriteBytes(this.Data);
				}
			}

			protected override string InternalToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this.Data != null && 0 < this.Data.Length)
				{
					stringBuilder.Append("data={").Append(RuleAction.ByteArrayToString(this.Data)).Append("}");
				}
				return stringBuilder.ToString();
			}

			public readonly byte[] Data;
		}

		internal sealed class BounceAction : RuleAction
		{
			internal BounceAction(uint flavor, uint flags, uint bounceCode) : base(RuleActionType.Bounce, flavor, flags)
			{
				this.BounceCode = bounceCode;
			}

			internal static RuleAction.BounceAction Parse(Reader reader, uint flavor, uint flags)
			{
				return new RuleAction.BounceAction(flavor, flags, reader.ReadUInt32());
			}

			internal override void Serialize(Writer writer, Encoding string8Encoding)
			{
				base.Serialize(writer, string8Encoding);
				writer.WriteUInt32(this.BounceCode);
			}

			protected override string InternalToString()
			{
				return string.Format("bounceCode=0x{0:X}", this.BounceCode);
			}

			public readonly uint BounceCode;
		}

		internal abstract class ForwardActionBase : RuleAction
		{
			protected ForwardActionBase(RuleActionType type, uint flavor, uint flags, RuleAction.ForwardActionBase.ActionRecipient[] recipients) : base(type, flavor, flags)
			{
				if (recipients == null)
				{
					throw new ArgumentNullException("recipients");
				}
				this.Recipients = recipients;
			}

			protected static void ParseForwardDelegateData(Reader reader, out RuleAction.ForwardActionBase.ActionRecipient[] recipients)
			{
				ushort num = reader.ReadUInt16();
				reader.CheckBoundary((uint)num, 3U);
				recipients = new RuleAction.ForwardActionBase.ActionRecipient[(int)num];
				for (ushort num2 = 0; num2 < num; num2 += 1)
				{
					recipients[(int)num2] = new RuleAction.ForwardActionBase.ActionRecipient(reader.ReadByte(), reader.ReadCountAndPropertyValueList(WireFormatStyle.Rop));
				}
			}

			internal override void Serialize(Writer writer, Encoding string8Encoding)
			{
				base.Serialize(writer, string8Encoding);
				writer.WriteUInt16((ushort)this.Recipients.Length);
				for (int i = 0; i < this.Recipients.Length; i++)
				{
					writer.WriteByte(this.Recipients[i].Reserved);
					writer.WriteCountAndPropertyValueList(this.Recipients[i].Properties, string8Encoding, WireFormatStyle.Rop);
				}
			}

			internal override void ResolveString8Values(Encoding string8Encoding)
			{
				for (int i = 0; i < this.Recipients.Length; i++)
				{
					for (int j = 0; j < this.Recipients[i].Properties.Length; j++)
					{
						this.Recipients[i].Properties[j].ResolveString8Values(string8Encoding);
					}
				}
			}

			protected override string InternalToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (0 < this.Recipients.Length)
				{
					stringBuilder.Append("recipients={");
					foreach (RuleAction.ForwardActionBase.ActionRecipient actionRecipient in this.Recipients)
					{
						stringBuilder.Append("{").Append(actionRecipient).Append("}");
					}
					stringBuilder.Append("}");
				}
				return stringBuilder.ToString();
			}

			public readonly RuleAction.ForwardActionBase.ActionRecipient[] Recipients;

			internal struct ActionRecipient
			{
				public ActionRecipient(byte reserved, PropertyValue[] properties)
				{
					if (properties == null)
					{
						throw new ArgumentNullException("properties");
					}
					this.Reserved = reserved;
					this.Properties = properties;
				}

				public readonly byte Reserved;

				public readonly PropertyValue[] Properties;
			}
		}

		internal sealed class ForwardAction : RuleAction.ForwardActionBase
		{
			internal ForwardAction(uint flavor, uint flags, RuleAction.ForwardActionBase.ActionRecipient[] recipients) : base(RuleActionType.Forward, flavor, flags, recipients)
			{
			}

			internal static RuleAction.ForwardAction Parse(Reader reader, uint flavor, uint flags)
			{
				RuleAction.ForwardActionBase.ActionRecipient[] recipients;
				RuleAction.ForwardActionBase.ParseForwardDelegateData(reader, out recipients);
				return new RuleAction.ForwardAction(flavor, flags, recipients);
			}

			[Flags]
			internal enum ForwardFlags
			{
				None = 0,
				PreserveSender = 1,
				DoNotChangeMessage = 2,
				ForwardAsAttachment = 4,
				SendSmsAlert = 8
			}
		}

		internal sealed class DelegateAction : RuleAction.ForwardActionBase
		{
			internal DelegateAction(uint flavor, uint flags, RuleAction.ForwardActionBase.ActionRecipient[] recipients) : base(RuleActionType.Delegate, flavor, flags, recipients)
			{
			}

			internal static RuleAction.DelegateAction Parse(Reader reader, uint flavor, uint flags)
			{
				RuleAction.ForwardActionBase.ActionRecipient[] recipients;
				RuleAction.ForwardActionBase.ParseForwardDelegateData(reader, out recipients);
				return new RuleAction.DelegateAction(flavor, flags, recipients);
			}
		}

		internal sealed class TagAction : RuleAction
		{
			internal TagAction(uint flavor, uint flags, PropertyValue propertyValue) : base(RuleActionType.Tag, flavor, flags)
			{
				this.PropertyValue = propertyValue;
			}

			internal static RuleAction.TagAction Parse(Reader reader, uint flavor, uint flags)
			{
				PropertyValue propertyValue = reader.ReadPropertyValue(WireFormatStyle.Rop);
				return new RuleAction.TagAction(flavor, flags, propertyValue);
			}

			internal override void Serialize(Writer writer, Encoding string8Encoding)
			{
				base.Serialize(writer, string8Encoding);
				writer.WritePropertyValue(this.PropertyValue, string8Encoding, WireFormatStyle.Rop);
			}

			internal override void ResolveString8Values(Encoding string8Encoding)
			{
				this.PropertyValue.ResolveString8Values(string8Encoding);
			}

			protected override string InternalToString()
			{
				return string.Format("property={0}", this.PropertyValue);
			}

			public readonly PropertyValue PropertyValue;
		}

		internal sealed class DeleteAction : RuleAction
		{
			internal DeleteAction(uint flavor, uint flags) : base(RuleActionType.Delete, flavor, flags)
			{
			}

			internal static RuleAction.DeleteAction Parse(Reader reader, uint flavor, uint flags)
			{
				return new RuleAction.DeleteAction(flavor, flags);
			}
		}

		internal sealed class MarkAsReadAction : RuleAction
		{
			internal MarkAsReadAction(uint flavor, uint flags) : base(RuleActionType.MarkAsRead, flavor, flags)
			{
			}

			internal static RuleAction.MarkAsReadAction Parse(Reader reader, uint flavor, uint flags)
			{
				return new RuleAction.MarkAsReadAction(flavor, flags);
			}
		}
	}
}
