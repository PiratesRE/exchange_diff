using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OutlookBlobWriter
	{
		internal OutlookBlobWriter(BinaryWriter writer, Encoding encoding)
		{
			this.writer = writer;
			this.encoding = encoding;
		}

		internal void WriteShort(short value)
		{
			this.writer.Write(value);
		}

		internal void WriteInt(int value)
		{
			this.writer.Write(value);
		}

		internal void WriteUint(uint value)
		{
			this.writer.Write(value);
		}

		internal void WriteBool(bool value)
		{
			this.writer.Write(value ? -1 : 0);
		}

		internal void WriteString(string value)
		{
			if (value.Length < 255)
			{
				this.writer.Write((byte)value.Length);
			}
			else
			{
				this.writer.Write(byte.MaxValue);
				this.writer.Write((short)value.Length);
			}
			this.writer.Write(value.ToCharArray());
		}

		internal void WriteByteArray(byte[] value)
		{
			this.writer.Write(value);
		}

		internal void WriteSkipBlock()
		{
			this.WriteInt(0);
		}

		internal void WriteEntryIdList(StoreId[] entryList)
		{
			byte[][] array = StoreId.StoreIdsToEntryIds(entryList);
			int num = 8 + array.Length * 8;
			foreach (byte[] array3 in array)
			{
				num += array3.Length;
			}
			this.WriteInt(num);
			this.WriteInt(array.Length);
			this.WriteInt(0);
			foreach (byte[] array5 in array)
			{
				this.WriteInt(array5.Length);
				this.WriteInt(0);
			}
			foreach (byte[] value in array)
			{
				this.WriteByteArray(value);
			}
		}

		internal void WriteRestriction(Restriction restriction)
		{
			if (restriction is Restriction.BitMaskRestriction)
			{
				Restriction.BitMaskRestriction bitMaskRestriction = (Restriction.BitMaskRestriction)restriction;
				this.WriteInt(6);
				this.WriteInt((int)bitMaskRestriction.Bmr);
				this.WriteInt((int)bitMaskRestriction.Tag);
				this.WriteInt(bitMaskRestriction.Mask);
				return;
			}
			if (restriction is Restriction.AndRestriction)
			{
				this.WriteInt(0);
				Restriction.AndRestriction andRestriction = (Restriction.AndRestriction)restriction;
				this.WriteInt(andRestriction.Restrictions.Length);
				foreach (Restriction restriction2 in andRestriction.Restrictions)
				{
					this.WriteRestriction(restriction2);
				}
				return;
			}
			if (restriction is Restriction.OrRestriction)
			{
				this.WriteInt(1);
				Restriction.OrRestriction orRestriction = (Restriction.OrRestriction)restriction;
				this.WriteInt(orRestriction.Restrictions.Length);
				foreach (Restriction restriction3 in orRestriction.Restrictions)
				{
					this.WriteRestriction(restriction3);
				}
				return;
			}
			if (restriction is Restriction.NotRestriction)
			{
				Restriction.NotRestriction notRestriction = (Restriction.NotRestriction)restriction;
				this.WriteInt(2);
				this.WriteRestriction(notRestriction.Restriction);
				return;
			}
			if (restriction is Restriction.ContentRestriction)
			{
				Restriction.ContentRestriction contentRestriction = (Restriction.ContentRestriction)restriction;
				this.WriteInt(3);
				this.WriteInt((int)contentRestriction.Flags);
				this.WriteInt((int)contentRestriction.PropTag);
				this.WritePropValue(contentRestriction.PropValue.PropTag, contentRestriction.PropValue.Value);
				return;
			}
			if (restriction is Restriction.PropertyRestriction)
			{
				Restriction.PropertyRestriction propertyRestriction = (Restriction.PropertyRestriction)restriction;
				this.WriteInt(4);
				this.WriteInt((int)propertyRestriction.Op);
				this.WriteInt((int)propertyRestriction.PropTag);
				this.WritePropValue(propertyRestriction.PropValue.PropTag, propertyRestriction.PropValue.Value);
				return;
			}
			if (restriction is Restriction.ComparePropertyRestriction)
			{
				Restriction.ComparePropertyRestriction comparePropertyRestriction = (Restriction.ComparePropertyRestriction)restriction;
				this.WriteInt(5);
				this.WriteInt((int)comparePropertyRestriction.Op);
				this.WriteInt((int)comparePropertyRestriction.TagLeft);
				this.WriteInt((int)comparePropertyRestriction.TagRight);
				return;
			}
			if (restriction is Restriction.ExistRestriction)
			{
				Restriction.ExistRestriction existRestriction = (Restriction.ExistRestriction)restriction;
				this.WriteInt(8);
				this.WriteInt((int)existRestriction.Tag);
				return;
			}
			if (restriction is Restriction.AttachmentRestriction)
			{
				Restriction.AttachmentRestriction attachmentRestriction = (Restriction.AttachmentRestriction)restriction;
				this.WriteInt(9);
				this.WriteInt(236126221);
				this.WriteRestriction(attachmentRestriction.Restriction);
				return;
			}
			if (restriction is Restriction.RecipientRestriction)
			{
				Restriction.RecipientRestriction recipientRestriction = (Restriction.RecipientRestriction)restriction;
				this.WriteInt(9);
				this.WriteInt(236060685);
				this.WriteRestriction(recipientRestriction.Restriction);
				return;
			}
			throw new ArgumentException(ServerStrings.ExUnknownRestrictionType, "restriction");
		}

		internal void WritePropValue(PropTag propertyTag, object value)
		{
			this.WriteInt((int)propertyTag);
			if (value is int)
			{
				this.WriteInt((int)value);
				return;
			}
			if (value is bool)
			{
				this.WritePropValueBool((bool)value);
				return;
			}
			if (value is DateTime)
			{
				ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(false, ExTimeZoneHelperForMigrationOnly.ValidationLevel.High, "OutlookBlobWriter.WritePropValue is being passed a DateTime argument.", new object[0]);
				this.WritePropValueDateTime((DateTime)value);
				return;
			}
			if (value is DateTime[])
			{
				ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(false, ExTimeZoneHelperForMigrationOnly.ValidationLevel.High, "OutlookBlobWriter.WritePropValue is being passed a DateTime[] argument.", new object[0]);
				DateTime[] array = (DateTime[])value;
				this.WriteInt(array.Length);
				foreach (DateTime dateTime in array)
				{
					this.WritePropValueDateTime(dateTime);
				}
				return;
			}
			if (value is ExDateTime)
			{
				this.WritePropValueDateTime((DateTime)((ExDateTime)value));
				return;
			}
			if (value is ExDateTime[])
			{
				ExDateTime[] array3 = (ExDateTime[])value;
				this.WriteInt(array3.Length);
				foreach (ExDateTime exDateTime in array3)
				{
					this.WritePropValueDateTime((DateTime)exDateTime);
				}
				return;
			}
			if (value is string)
			{
				this.WritePropValueString((string)value, propertyTag.ValueType());
				return;
			}
			if (value is string[])
			{
				string[] array5 = (string[])value;
				this.WriteInt(array5.Length);
				foreach (string value2 in array5)
				{
					this.WritePropValueString(value2, propertyTag.ValueType());
				}
				return;
			}
			if (value is byte[])
			{
				byte[] array7 = (byte[])value;
				this.WriteShort((short)array7.Length);
				this.WriteByteArray(array7);
				return;
			}
			if (value is byte[][])
			{
				byte[][] array8 = (byte[][])value;
				this.WriteInt(array8.Length);
				foreach (byte[] array10 in array8)
				{
					this.WriteInt(array10.Length);
					this.WriteByteArray(array10);
				}
				return;
			}
			throw new NotSupportedException();
		}

		private void WritePropValueBool(bool value)
		{
			this.writer.Write(value ? ushort.MaxValue : 0);
		}

		private void WritePropValueString(string value, PropType type)
		{
			switch (type)
			{
			case PropType.AnsiString:
				break;
			case PropType.String:
				goto IL_31;
			default:
				switch (type)
				{
				case PropType.AnsiStringArray:
					break;
				case PropType.StringArray:
					goto IL_31;
				default:
					throw new InvalidOperationException();
				}
				break;
			}
			this.WritePropValueAnsiString(value);
			return;
			IL_31:
			this.WritePropValueString(value);
		}

		private void WritePropValueString(string value)
		{
			this.writer.Write((short)((value.Length + 1) * 2));
			this.writer.Write(value.ToCharArray());
			this.writer.Write('\0');
		}

		private void WritePropValueAnsiString(string value)
		{
			byte[] bytes = this.encoding.GetBytes(value);
			this.writer.Write((short)(bytes.Length + 1));
			this.writer.Write(bytes);
			this.writer.Write(0);
		}

		private void WritePropValueDateTime(DateTime dateTime)
		{
			long num = dateTime.ToFileTimeUtc();
			uint value = (uint)(num & (long)((ulong)-1));
			uint value2 = (uint)(num >> 32);
			this.WriteUint(value);
			this.WriteUint(value2);
		}

		private const int SizeOfSBinaryArray = 8;

		private const int SizeOfSBinary = 8;

		private readonly BinaryWriter writer;

		private readonly Encoding encoding;

		internal enum RestrictionType
		{
			And,
			Or,
			Not,
			Content,
			Property,
			PropertyComparison,
			Bitmask,
			Size,
			Exist,
			Subfilter
		}
	}
}
