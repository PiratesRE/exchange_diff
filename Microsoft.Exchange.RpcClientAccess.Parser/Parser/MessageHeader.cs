using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class MessageHeader
	{
		public MessageHeader(bool hasNamedProperties, bool useUnicode, string subjectPrefix, string normalizedSubject, ushort messageRecipientsCount)
		{
			this.useUnicode = useUnicode;
			this.hasNamedProperties = hasNamedProperties;
			this.subjectPrefix = subjectPrefix;
			this.normalizedSubject = normalizedSubject;
			this.messageRecipientsCount = messageRecipientsCount;
		}

		internal MessageHeader(Reader reader, Encoding string8Encoding)
		{
			this.hasNamedProperties = reader.ReadBool();
			StringFormatType stringFormatType = (StringFormatType)reader.PeekByte(0L);
			if (stringFormatType != StringFormatType.String8)
			{
				this.useUnicode = true;
			}
			this.subjectPrefix = reader.ReadFormattedString(string8Encoding);
			this.normalizedSubject = reader.ReadFormattedString(string8Encoding);
			this.messageRecipientsCount = reader.ReadUInt16();
		}

		internal bool HasNamedProperties
		{
			get
			{
				return this.hasNamedProperties;
			}
		}

		internal string SubjectPrefix
		{
			get
			{
				return this.subjectPrefix;
			}
		}

		internal string NormalizedSubject
		{
			get
			{
				return this.normalizedSubject;
			}
		}

		internal ushort MessageRecipientsCount
		{
			get
			{
				return this.messageRecipientsCount;
			}
		}

		internal void Serialize(Writer writer, Encoding string8Encoding)
		{
			writer.WriteBool(this.hasNamedProperties, 1);
			writer.WriteFormattedString(this.subjectPrefix, this.useUnicode, string8Encoding);
			writer.WriteFormattedString(this.normalizedSubject, this.useUnicode, string8Encoding);
			writer.WriteUInt16(this.messageRecipientsCount);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		internal void AppendToString(StringBuilder stringBuilder)
		{
			stringBuilder.Append("[NamedProps=").Append(this.hasNamedProperties);
			stringBuilder.Append(" SubjectPrefix=[").Append(this.subjectPrefix).Append("]");
			stringBuilder.Append(" NormalizedSubject=[").Append(this.normalizedSubject).Append("]");
			stringBuilder.Append(" TotalRecipients=").Append(this.messageRecipientsCount);
			stringBuilder.Append("]");
		}

		private readonly bool hasNamedProperties;

		private readonly bool useUnicode;

		private readonly string subjectPrefix;

		private readonly string normalizedSubject;

		private readonly ushort messageRecipientsCount;
	}
}
