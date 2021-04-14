using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RecipientRow
	{
		public RecipientRow(uint recipientRowId, RecipientType recipientType, Encoding string8Encoding, RecipientAddress recipientAddress, RecipientFlags recipientFlags, bool useUnicode, string emailAddress, string displayName, string simpleDisplayName, string transmittableDisplayName, PropertyValue[] extraPropertyValues, PropertyValue[] extraUnicodePropertyValues) : this(recipientRowId, recipientType, new ushort?((ushort)CodePageMap.GetCodePage(string8Encoding)), recipientAddress, recipientFlags, useUnicode, emailAddress, displayName, simpleDisplayName, transmittableDisplayName, extraPropertyValues, extraUnicodePropertyValues)
		{
		}

		internal RecipientRow(uint recipientRowId, RecipientType recipientType, ushort? codePageId, RecipientAddress recipientAddress, RecipientFlags recipientFlags, bool useUnicode, string emailAddress, string displayName, string simpleDisplayName, string transmittableDisplayName, PropertyValue[] extraPropertyValues, PropertyValue[] extraUnicodePropertyValues)
		{
			Util.ThrowOnNullArgument(recipientAddress, "recipientAddress");
			Util.ThrowOnNullArgument(extraPropertyValues, "extraPropertyValues");
			Util.ThrowOnNullArgument(extraUnicodePropertyValues, "extraUnicodePropertyValues");
			this.recipientRowId = recipientRowId;
			this.codePageId = codePageId;
			this.recipientType = recipientType;
			this.recipientData = new RecipientRow.RecipientData(recipientAddress, recipientFlags, useUnicode, String8.Create(emailAddress), String8.Create(displayName), String8.Create(simpleDisplayName), String8.Create(transmittableDisplayName), extraPropertyValues, extraUnicodePropertyValues);
		}

		internal RecipientRow(uint recipientRowId, RecipientType recipientType)
		{
			this.recipientRowId = recipientRowId;
			this.recipientType = recipientType;
			this.recipientData = null;
		}

		internal RecipientRow(Reader reader, PropertyTag[] extraPropertyTags, RecipientSerializationFlags serializationFlags)
		{
			ushort num = 0;
			if ((serializationFlags & RecipientSerializationFlags.RecipientRowId) != (RecipientSerializationFlags)0)
			{
				this.recipientRowId = reader.ReadUInt32();
			}
			this.recipientType = (RecipientType)reader.ReadByte();
			if ((serializationFlags & RecipientSerializationFlags.CodePageId) != (RecipientSerializationFlags)0)
			{
				this.codePageId = new ushort?(reader.ReadUInt16());
			}
			if ((serializationFlags & RecipientSerializationFlags.ExtraUnicodeProperties) != (RecipientSerializationFlags)0)
			{
				num = reader.ReadUInt16();
			}
			ushort num2 = reader.ReadUInt16();
			long position = reader.Position;
			if (num2 > 0)
			{
				ushort num3 = reader.ReadUInt16();
				bool useUnicode = (num3 & 512) != 0;
				RecipientFlags recipientFlags = (RecipientFlags)(num3 & 448);
				RecipientAddressType recipientAddressType = (RecipientAddressType)(num3 & 32775);
				RecipientAddress recipientAddress = RecipientAddress.Parse(reader, recipientAddressType);
				String8 emailAddress = null;
				if ((num3 & 8) != 0)
				{
					emailAddress = String8.Parse(reader, useUnicode, StringFlags.IncludeNull);
				}
				String8 displayName = null;
				if ((num3 & 16) != 0)
				{
					displayName = String8.Parse(reader, useUnicode, StringFlags.IncludeNull);
				}
				String8 simpleDisplayName = null;
				if ((num3 & 1024) != 0)
				{
					simpleDisplayName = String8.Parse(reader, useUnicode, StringFlags.IncludeNull);
				}
				String8 transmittableDisplayName = null;
				if ((num3 & 32) != 0)
				{
					transmittableDisplayName = String8.Parse(reader, useUnicode, StringFlags.IncludeNull);
				}
				ushort num4 = reader.ReadUInt16();
				if ((int)num4 > extraPropertyTags.Length)
				{
					string message = string.Format("Recipient expects more extra properties than available: Expected = {0}; Available = {1}", num4, extraPropertyTags.Length);
					throw new BufferParseException(message);
				}
				PropertyTag[] array = new PropertyTag[(int)num4];
				Array.ConstrainedCopy(extraPropertyTags, 0, array, 0, (int)num4);
				PropertyValue[] propertyValues = PropertyRow.Parse(reader, array, WireFormatStyle.Rop).PropertyValues;
				List<PropertyValue> list = new List<PropertyValue>((int)num);
				int num5 = 0;
				while (num5 < (int)num && reader.Position < position + (long)((ulong)num2))
				{
					list.Add(reader.ReadPropertyValue(WireFormatStyle.Rop));
					num5++;
				}
				PropertyValue[] extraUnicodePropertyValues = list.ToArray();
				if ((ulong)num2 != (ulong)(reader.Position - position))
				{
					string message2 = string.Format("Did not read entire recipient buffer:  Size={0} Read={1}", num2, reader.Position - position);
					throw new BufferParseException(message2);
				}
				this.recipientData = new RecipientRow.RecipientData(recipientAddress, recipientFlags, useUnicode, emailAddress, displayName, simpleDisplayName, transmittableDisplayName, propertyValues, extraUnicodePropertyValues);
				if (this.String8Encoding != null)
				{
					this.recipientData.ResolveString8Values(this.String8Encoding);
				}
			}
		}

		private RecipientRow.RecipientData Data
		{
			get
			{
				return this.recipientData;
			}
		}

		internal bool IsEmpty
		{
			get
			{
				return this.recipientData == null;
			}
		}

		internal RecipientFlags Flags
		{
			get
			{
				if (!this.IsEmpty)
				{
					return this.Data.RecipientFlags;
				}
				return RecipientFlags.None;
			}
		}

		internal ushort? CodePageId
		{
			get
			{
				return this.codePageId;
			}
		}

		internal Encoding String8Encoding
		{
			get
			{
				if (this.codePageId == null)
				{
					return null;
				}
				return CodePageMap.GetEncoding((int)this.codePageId.Value);
			}
		}

		internal string DisplayName
		{
			get
			{
				if (!this.IsEmpty)
				{
					return this.Data.DisplayName;
				}
				return null;
			}
		}

		internal bool UseUnicode
		{
			get
			{
				return this.IsEmpty || this.Data.UseUnicode;
			}
		}

		internal string EmailAddress
		{
			get
			{
				if (!this.IsEmpty)
				{
					return this.Data.EmailAddress;
				}
				return null;
			}
		}

		internal string SimpleDisplayName
		{
			get
			{
				if (!this.IsEmpty)
				{
					return this.Data.SimpleDisplayName;
				}
				return null;
			}
		}

		internal string TransmittableDisplayName
		{
			get
			{
				if (!this.IsEmpty)
				{
					return this.Data.TransmittableDisplayName;
				}
				return null;
			}
		}

		internal RecipientAddress RecipientAddress
		{
			get
			{
				if (!this.IsEmpty)
				{
					return this.Data.RecipientAddress;
				}
				return null;
			}
		}

		internal uint RecipientRowId
		{
			get
			{
				return this.recipientRowId;
			}
		}

		internal RecipientType RecipientType
		{
			get
			{
				return this.recipientType;
			}
		}

		internal PropertyValue[] ExtraPropertyValues
		{
			get
			{
				if (!this.IsEmpty)
				{
					return this.Data.ExtraPropertyValues;
				}
				return Array<PropertyValue>.Empty;
			}
		}

		internal PropertyValue[] ExtraUnicodePropertyValues
		{
			get
			{
				if (!this.IsEmpty)
				{
					return this.Data.ExtraUnicodePropertyValues;
				}
				return Array<PropertyValue>.Empty;
			}
		}

		internal void ResolveString8Values(Encoding string8Encoding)
		{
			if (this.Data != null)
			{
				this.Data.ResolveString8Values(string8Encoding);
			}
		}

		private ushort CombinedRecipientPropertyFlags
		{
			get
			{
				ushort num = 0;
				if (!this.IsEmpty)
				{
					num = (ushort)this.Flags;
					num |= (ushort)this.RecipientAddress.RecipientAddressType;
					if (this.UseUnicode)
					{
						num |= 512;
					}
					if (this.EmailAddress != null)
					{
						num |= 8;
					}
					if (this.DisplayName != null)
					{
						num |= 16;
					}
					if (this.SimpleDisplayName != null)
					{
						num |= 1024;
					}
					if (this.TransmittableDisplayName != null)
					{
						num |= 32;
					}
				}
				return num;
			}
		}

		internal void Serialize(Writer writer, PropertyTag[] extraPropertyTags, RecipientSerializationFlags serializationFlags)
		{
			if (this.String8Encoding == null)
			{
				throw new InvalidOperationException("Cannot use this method without a code page");
			}
			this.Serialize(writer, extraPropertyTags, serializationFlags, this.String8Encoding);
		}

		internal void Serialize(Writer writer, PropertyTag[] extraPropertyTags, RecipientSerializationFlags serializationFlags, Encoding string8Encoding)
		{
			if ((serializationFlags & RecipientSerializationFlags.RecipientRowId) != (RecipientSerializationFlags)0)
			{
				writer.WriteUInt32(this.recipientRowId);
			}
			writer.WriteByte((byte)this.recipientType);
			if ((serializationFlags & RecipientSerializationFlags.CodePageId) != (RecipientSerializationFlags)0)
			{
				writer.WriteUInt16(this.CodePageId.Value);
			}
			if ((serializationFlags & RecipientSerializationFlags.ExtraUnicodeProperties) != (RecipientSerializationFlags)0)
			{
				writer.WriteUInt16((ushort)this.ExtraUnicodePropertyValues.Length);
			}
			long position = writer.Position;
			writer.WriteUInt16(0);
			long position2 = writer.Position;
			if (!this.IsEmpty)
			{
				writer.WriteUInt16(this.CombinedRecipientPropertyFlags);
				this.RecipientAddress.Serialize(writer);
				RecipientRow.SerializeStringIfNonNull(writer, this.EmailAddress, this.UseUnicode, string8Encoding);
				RecipientRow.SerializeStringIfNonNull(writer, this.DisplayName, this.UseUnicode, string8Encoding);
				RecipientRow.SerializeStringIfNonNull(writer, this.SimpleDisplayName, this.UseUnicode, string8Encoding);
				RecipientRow.SerializeStringIfNonNull(writer, this.TransmittableDisplayName, this.UseUnicode, string8Encoding);
				PropertyTag[] array = new PropertyTag[this.ExtraPropertyValues.Length];
				Array.ConstrainedCopy(extraPropertyTags, 0, array, 0, this.ExtraPropertyValues.Length);
				PropertyRow propertyRow = new PropertyRow(array, this.ExtraPropertyValues);
				writer.WriteUInt16((ushort)this.ExtraPropertyValues.Length);
				propertyRow.Serialize(writer, string8Encoding, WireFormatStyle.Rop);
				if ((serializationFlags & RecipientSerializationFlags.ExtraUnicodeProperties) != (RecipientSerializationFlags)0)
				{
					foreach (PropertyValue value in this.ExtraUnicodePropertyValues)
					{
						writer.WritePropertyValue(value, string8Encoding, WireFormatStyle.Rop);
					}
				}
				long position3 = writer.Position;
				writer.Position = position;
				writer.WriteUInt16((ushort)(position3 - position2));
				writer.Position = position3;
			}
		}

		private static void SerializeStringIfNonNull(Writer writer, string stringValue, bool useUnicode, Encoding string8Encoding)
		{
			if (stringValue != null)
			{
				if (useUnicode)
				{
					writer.WriteUnicodeString(stringValue, StringFlags.IncludeNull);
					return;
				}
				writer.WriteString8(stringValue, string8Encoding, StringFlags.IncludeNull);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		internal void AppendToString(StringBuilder stringBuilder)
		{
			stringBuilder.Append("[RowId=").Append(this.RecipientRowId);
			stringBuilder.Append(" Type=").Append(this.RecipientType);
			stringBuilder.Append(" CodePage=").Append(this.CodePageId);
			if (this.Data != null)
			{
				stringBuilder.Append(" DisplayName=[").Append(this.Data.DisplayNameAsString8).Append("]");
			}
			stringBuilder.Append("]");
		}

		private readonly uint recipientRowId;

		private readonly ushort? codePageId;

		private readonly RecipientType recipientType;

		private readonly RecipientRow.RecipientData recipientData;

		private class RecipientData
		{
			internal RecipientData(RecipientAddress recipientAddress, RecipientFlags recipientFlags, bool useUnicode, String8 emailAddress, String8 displayName, String8 simpleDisplayName, String8 transmittableDisplayName, PropertyValue[] extraPropertyValues, PropertyValue[] extraUnicodePropertyValues)
			{
				EnumValidator.ThrowIfInvalid<RecipientFlags>(recipientFlags, "recipientFlags");
				this.recipientAddress = recipientAddress;
				this.recipientFlags = recipientFlags;
				this.useUnicode = useUnicode;
				this.emailAddress = emailAddress;
				this.displayName = displayName;
				this.simpleDisplayName = simpleDisplayName;
				this.transmittableDisplayName = transmittableDisplayName;
				this.extraPropertyValues = extraPropertyValues;
				this.extraUnicodePropertyValues = extraUnicodePropertyValues;
			}

			internal RecipientAddress RecipientAddress
			{
				get
				{
					return this.recipientAddress;
				}
			}

			internal RecipientFlags RecipientFlags
			{
				get
				{
					return this.recipientFlags;
				}
			}

			internal bool UseUnicode
			{
				get
				{
					return this.useUnicode;
				}
			}

			internal string EmailAddress
			{
				get
				{
					if (this.emailAddress != null)
					{
						return this.emailAddress.StringValue;
					}
					return null;
				}
			}

			internal string DisplayName
			{
				get
				{
					if (this.displayName != null)
					{
						return this.displayName.StringValue;
					}
					return null;
				}
			}

			internal String8 DisplayNameAsString8
			{
				get
				{
					return this.displayName;
				}
			}

			internal string SimpleDisplayName
			{
				get
				{
					if (this.simpleDisplayName != null)
					{
						return this.simpleDisplayName.StringValue;
					}
					return null;
				}
			}

			internal string TransmittableDisplayName
			{
				get
				{
					if (this.transmittableDisplayName != null)
					{
						return this.transmittableDisplayName.StringValue;
					}
					return null;
				}
			}

			internal PropertyValue[] ExtraPropertyValues
			{
				get
				{
					return this.extraPropertyValues;
				}
			}

			internal PropertyValue[] ExtraUnicodePropertyValues
			{
				get
				{
					return this.extraUnicodePropertyValues;
				}
			}

			internal void ResolveString8Values(Encoding string8Encoding)
			{
				RecipientRow.RecipientData.ResolveString8IfPresent(this.emailAddress, string8Encoding);
				RecipientRow.RecipientData.ResolveString8IfPresent(this.displayName, string8Encoding);
				RecipientRow.RecipientData.ResolveString8IfPresent(this.simpleDisplayName, string8Encoding);
				RecipientRow.RecipientData.ResolveString8IfPresent(this.transmittableDisplayName, string8Encoding);
				foreach (PropertyValue propertyValue in this.extraPropertyValues)
				{
					propertyValue.ResolveString8Values(string8Encoding);
				}
			}

			private static void ResolveString8IfPresent(String8 string8, Encoding encoding)
			{
				if (string8 != null)
				{
					string8.ResolveString8Values(encoding);
				}
			}

			private readonly RecipientAddress recipientAddress;

			private readonly RecipientFlags recipientFlags;

			private readonly bool useUnicode;

			private readonly String8 emailAddress;

			private readonly String8 displayName;

			private readonly String8 simpleDisplayName;

			private readonly String8 transmittableDisplayName;

			private readonly PropertyValue[] extraPropertyValues;

			private readonly PropertyValue[] extraUnicodePropertyValues;
		}
	}
}
