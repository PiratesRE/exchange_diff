using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulOpenEmbeddedMessageResult : RopResult
	{
		internal SuccessfulOpenEmbeddedMessageResult(IServerObject serverObject, bool isNew, StoreId messageId, MessageHeader messageHeader, RecipientCollector recipientCollector) : base(RopId.OpenEmbeddedMessage, ErrorCode.None, serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
			if (isNew)
			{
				if (messageHeader != null)
				{
					throw new ArgumentException("messageHeader should be null for new messages", "messageHeader");
				}
				if (recipientCollector != null)
				{
					throw new ArgumentException("recipientCollector should be null for new messages", "recipientCollector");
				}
			}
			else
			{
				if (messageHeader == null)
				{
					throw new ArgumentNullException("messageHeader");
				}
				if (recipientCollector == null)
				{
					throw new ArgumentNullException("recipientCollector");
				}
			}
			this.isNew = isNew;
			this.messageId = messageId;
			this.messageHeader = messageHeader;
			if (recipientCollector != null)
			{
				this.extraPropertyTags = recipientCollector.ExtraPropertyTags;
				this.recipientRows = recipientCollector.RecipientRows;
				return;
			}
			this.extraPropertyTags = null;
			this.recipientRows = null;
		}

		internal SuccessfulOpenEmbeddedMessageResult(Reader reader, Encoding string8Encoding) : base(reader)
		{
			this.isNew = reader.ReadBool();
			this.messageId = StoreId.Parse(reader);
			if (!this.isNew)
			{
				this.messageHeader = new MessageHeader(reader, string8Encoding);
				this.extraPropertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
				byte b = reader.ReadByte();
				this.recipientRows = new RecipientRow[(int)b];
				for (int i = 0; i < (int)b; i++)
				{
					this.recipientRows[i] = new RecipientRow(reader, this.extraPropertyTags, RecipientSerializationFlags.ExtraUnicodeProperties | RecipientSerializationFlags.CodePageId);
				}
				return;
			}
			this.messageHeader = null;
			this.extraPropertyTags = null;
			this.recipientRows = null;
		}

		internal bool HasNamedProperties
		{
			get
			{
				return this.messageHeader.HasNamedProperties;
			}
		}

		internal string SubjectPrefix
		{
			get
			{
				return this.messageHeader.SubjectPrefix;
			}
		}

		internal string NormalizedSubject
		{
			get
			{
				return this.messageHeader.NormalizedSubject;
			}
		}

		internal ushort MessageRecipientsCount
		{
			get
			{
				return this.messageHeader.MessageRecipientsCount;
			}
		}

		internal PropertyTag[] ExtraPropertyTags
		{
			get
			{
				return this.extraPropertyTags;
			}
		}

		internal RecipientRow[] RecipientRows
		{
			get
			{
				return this.recipientRows;
			}
		}

		internal bool IsNew
		{
			get
			{
				return this.isNew;
			}
		}

		internal static SuccessfulOpenEmbeddedMessageResult Parse(Reader reader, Encoding string8Encoding)
		{
			return new SuccessfulOpenEmbeddedMessageResult(reader, string8Encoding);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.isNew, 1);
			this.messageId.Serialize(writer);
			if (!this.isNew)
			{
				this.messageHeader.Serialize(writer, base.String8Encoding);
				writer.WriteCountAndPropertyTagArray(this.extraPropertyTags, FieldLength.WordSize);
				writer.WriteByte((byte)this.recipientRows.Length);
				foreach (RecipientRow recipientRow in this.recipientRows)
				{
					recipientRow.Serialize(writer, this.extraPropertyTags, RecipientSerializationFlags.ExtraUnicodeProperties | RecipientSerializationFlags.CodePageId);
				}
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" MID=").Append(this.messageId.ToString());
			if (this.messageHeader != null)
			{
				stringBuilder.Append(" Header=");
				this.messageHeader.AppendToString(stringBuilder);
			}
			stringBuilder.Append(" ExtraTags=[");
			Util.AppendToString<PropertyTag>(stringBuilder, this.extraPropertyTags);
			stringBuilder.Append("]");
			stringBuilder.Append(" Recipients=[");
			Util.AppendToString<RecipientRow>(stringBuilder, this.recipientRows);
			stringBuilder.Append("]");
		}

		private readonly bool isNew;

		private readonly StoreId messageId;

		private readonly MessageHeader messageHeader;

		private readonly PropertyTag[] extraPropertyTags;

		private readonly RecipientRow[] recipientRows;
	}
}
