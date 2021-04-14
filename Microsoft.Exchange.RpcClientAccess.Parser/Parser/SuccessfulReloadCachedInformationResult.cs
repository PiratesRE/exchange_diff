using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulReloadCachedInformationResult : RopResult
	{
		internal SuccessfulReloadCachedInformationResult(MessageHeader messageHeader, RecipientCollector recipientCollector) : base(RopId.ReloadCachedInformation, ErrorCode.None, null)
		{
			if (messageHeader == null)
			{
				throw new ArgumentNullException("messageHeader");
			}
			if (recipientCollector == null)
			{
				throw new ArgumentNullException("recipientCollector");
			}
			this.messageHeader = messageHeader;
			this.extraPropertyTags = recipientCollector.ExtraPropertyTags;
			this.recipientRows = recipientCollector.RecipientRows;
		}

		internal SuccessfulReloadCachedInformationResult(Reader reader, Encoding string8Encoding) : base(reader)
		{
			this.messageHeader = new MessageHeader(reader, string8Encoding);
			this.extraPropertyTags = reader.ReadCountAndPropertyTagArray(FieldLength.WordSize);
			byte b = reader.ReadByte();
			this.recipientRows = new RecipientRow[(int)b];
			for (int i = 0; i < (int)b; i++)
			{
				this.recipientRows[i] = new RecipientRow(reader, this.extraPropertyTags, RecipientSerializationFlags.ExtraUnicodeProperties | RecipientSerializationFlags.CodePageId);
			}
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

		internal static SuccessfulReloadCachedInformationResult Parse(Reader reader, Encoding string8Encoding)
		{
			return new SuccessfulReloadCachedInformationResult(reader, string8Encoding);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.messageHeader.Serialize(writer, base.String8Encoding);
			writer.WriteCountAndPropertyTagArray(this.extraPropertyTags, FieldLength.WordSize);
			writer.WriteByte((byte)this.recipientRows.Length);
			foreach (RecipientRow recipientRow in this.recipientRows)
			{
				recipientRow.Serialize(writer, this.extraPropertyTags, RecipientSerializationFlags.ExtraUnicodeProperties | RecipientSerializationFlags.CodePageId);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
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

		private readonly MessageHeader messageHeader;

		private readonly PropertyTag[] extraPropertyTags;

		private readonly RecipientRow[] recipientRows;
	}
}
