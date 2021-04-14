using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulReadRecipientsResult : RopResult
	{
		internal SuccessfulReadRecipientsResult(RecipientCollector recipientCollector) : base(RopId.ReadRecipients, ErrorCode.None, null)
		{
			if (recipientCollector == null)
			{
				throw new ArgumentNullException("recipientCollector");
			}
			this.extraPropertyTags = recipientCollector.ExtraPropertyTags;
			this.recipientRows = recipientCollector.RecipientRows;
		}

		internal SuccessfulReadRecipientsResult(Reader reader, PropertyTag[] extraPropertyTags) : base(reader)
		{
			this.extraPropertyTags = extraPropertyTags;
			byte b = reader.ReadByte();
			this.recipientRows = new RecipientRow[(int)b];
			for (int i = 0; i < (int)b; i++)
			{
				this.recipientRows[i] = new RecipientRow(reader, this.extraPropertyTags, RecipientSerializationFlags.RecipientRowId | RecipientSerializationFlags.ExtraUnicodeProperties | RecipientSerializationFlags.CodePageId);
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

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte((byte)this.recipientRows.Length);
			foreach (RecipientRow recipientRow in this.recipientRows)
			{
				recipientRow.Serialize(writer, this.extraPropertyTags, RecipientSerializationFlags.RecipientRowId | RecipientSerializationFlags.ExtraUnicodeProperties | RecipientSerializationFlags.CodePageId);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Recipients=[");
			Util.AppendToString<RecipientRow>(stringBuilder, this.RecipientRows);
			stringBuilder.Append("]");
		}

		private readonly PropertyTag[] extraPropertyTags;

		private readonly RecipientRow[] recipientRows;
	}
}
