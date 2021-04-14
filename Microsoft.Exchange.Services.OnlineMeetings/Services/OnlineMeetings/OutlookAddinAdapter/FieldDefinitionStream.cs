using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	internal class FieldDefinitionStream
	{
		internal FieldDefinitionStream(string fieldName)
		{
			this.NmidName = fieldName;
			this.NmidNameLength = (ushort)fieldName.Length;
			this.NameAnsi = fieldName;
			this.SkipBlocks.Add(fieldName);
		}

		private FieldDefinitionStream()
		{
		}

		internal FieldDefinitionStreamFlags Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		internal VarEnum Vt
		{
			get
			{
				return this.vt;
			}
			set
			{
				this.vt = value;
			}
		}

		internal uint DispId
		{
			get
			{
				return this.dispId;
			}
			set
			{
				this.dispId = value;
			}
		}

		internal ushort NmidNameLength
		{
			get
			{
				return this.nmidNameLength;
			}
			set
			{
				this.nmidNameLength = value;
			}
		}

		internal string NmidName
		{
			get
			{
				return this.nmidName;
			}
			set
			{
				this.nmidName = value;
			}
		}

		internal string NameAnsi
		{
			get
			{
				return this.nameAnsi;
			}
			set
			{
				this.nameAnsi = value;
			}
		}

		internal string FormulaAnsi
		{
			get
			{
				return this.formulaAnsi;
			}
			set
			{
				this.formulaAnsi = value;
			}
		}

		internal string ValidationRuleAnsi
		{
			get
			{
				return this.validationRuleAnsi;
			}
			set
			{
				this.validationRuleAnsi = value;
			}
		}

		internal string ValidationTextAnsi
		{
			get
			{
				return this.validationTextAnsi;
			}
			set
			{
				this.validationTextAnsi = value;
			}
		}

		internal string ErrorAnsi
		{
			get
			{
				return this.errorAnsi;
			}
			set
			{
				this.errorAnsi = value;
			}
		}

		internal int InternalType
		{
			get
			{
				return this.internalType;
			}
			set
			{
				this.internalType = value;
			}
		}

		internal List<string> SkipBlocks
		{
			get
			{
				return this.skipBlocks;
			}
			private set
			{
				this.skipBlocks = value;
			}
		}

		internal static FieldDefinitionStream Read(BinaryReader reader, PropertyDefinitionStreamVersion version)
		{
			if (version != PropertyDefinitionStreamVersion.PropDefV1)
			{
				return FieldDefinitionStream.ReadVersion103(reader);
			}
			return FieldDefinitionStream.ReadVersion102(reader);
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write((uint)this.Flags);
			writer.Write((ushort)this.Vt);
			writer.Write(this.DispId);
			writer.Write(this.NmidNameLength);
			writer.Write(FieldDefinitionStream.UnicodeEncoder.GetBytes(this.NmidName));
			FieldDefinitionStream.WriteString(writer, this.NameAnsi, false);
			FieldDefinitionStream.WriteString(writer, this.FormulaAnsi, false);
			FieldDefinitionStream.WriteString(writer, this.ValidationRuleAnsi, false);
			FieldDefinitionStream.WriteString(writer, this.ValidationTextAnsi, false);
			FieldDefinitionStream.WriteString(writer, this.ErrorAnsi, false);
			writer.Write(this.InternalType);
			FieldDefinitionStream.WriteSkipBlocks(writer, this.SkipBlocks);
		}

		private static FieldDefinitionStream ReadVersion102(BinaryReader reader)
		{
			FieldDefinitionStream fieldDefinitionStream = new FieldDefinitionStream();
			fieldDefinitionStream.Flags = (FieldDefinitionStreamFlags)reader.ReadUInt32();
			fieldDefinitionStream.Vt = (VarEnum)reader.ReadInt16();
			fieldDefinitionStream.DispId = reader.ReadUInt32();
			fieldDefinitionStream.NmidNameLength = reader.ReadUInt16();
			byte[] array = reader.ReadBytes((int)(fieldDefinitionStream.NmidNameLength * 2));
			fieldDefinitionStream.NmidName = FieldDefinitionStream.UnicodeEncoder.GetString(array, 0, array.Length);
			fieldDefinitionStream.NameAnsi = FieldDefinitionStream.ReadString(reader, false);
			fieldDefinitionStream.FormulaAnsi = FieldDefinitionStream.ReadString(reader, false);
			fieldDefinitionStream.ValidationRuleAnsi = FieldDefinitionStream.ReadString(reader, false);
			fieldDefinitionStream.ValidationTextAnsi = FieldDefinitionStream.ReadString(reader, false);
			fieldDefinitionStream.ErrorAnsi = FieldDefinitionStream.ReadString(reader, false);
			return fieldDefinitionStream;
		}

		private static FieldDefinitionStream ReadVersion103(BinaryReader reader)
		{
			FieldDefinitionStream fieldDefinitionStream = FieldDefinitionStream.ReadVersion102(reader);
			fieldDefinitionStream.InternalType = reader.ReadInt32();
			fieldDefinitionStream.SkipBlocks = FieldDefinitionStream.ReadSkipBlocks(reader);
			return fieldDefinitionStream;
		}

		private static string ReadString(BinaryReader reader, bool isUnicode)
		{
			ushort num = (ushort)reader.ReadByte();
			if (num == 255)
			{
				num = reader.ReadUInt16();
			}
			if (isUnicode)
			{
				byte[] array = reader.ReadBytes((int)(num * 2));
				return FieldDefinitionStream.UnicodeEncoder.GetString(array, 0, array.Length);
			}
			byte[] array2 = reader.ReadBytes((int)num);
			return FieldDefinitionStream.AsciiEncoder.GetString(array2, 0, array2.Length);
		}

		private static void WriteString(BinaryWriter writer, string str, bool isUnicode)
		{
			if (string.IsNullOrEmpty(str))
			{
				writer.Write(0);
				return;
			}
			if (str.Length >= 255)
			{
				writer.Write(byte.MaxValue);
				writer.Write((ushort)str.Length);
			}
			else
			{
				writer.Write((byte)str.Length);
			}
			if (isUnicode)
			{
				writer.Write(FieldDefinitionStream.UnicodeEncoder.GetBytes(str));
				return;
			}
			writer.Write(FieldDefinitionStream.AsciiEncoder.GetBytes(str));
		}

		private static List<string> ReadSkipBlocks(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			List<string> list = new List<string>();
			while (num != 0)
			{
				list.Add(FieldDefinitionStream.ReadString(reader, true));
				num = reader.ReadInt32();
			}
			return list;
		}

		private static void WriteSkipBlocks(BinaryWriter writer, List<string> skipBlocks)
		{
			foreach (string text in skipBlocks)
			{
				int value = text.Length * 2 + ((text.Length > 255) ? 2 : 1);
				writer.Write(value);
				FieldDefinitionStream.WriteString(writer, text, true);
			}
			writer.Write(0);
		}

		private static readonly UnicodeEncoding UnicodeEncoder = new UnicodeEncoding();

		private static readonly Encoding AsciiEncoder = CTSGlobals.AsciiEncoding;

		private FieldDefinitionStreamFlags flags;

		private VarEnum vt;

		private uint dispId;

		private ushort nmidNameLength;

		private string nmidName = string.Empty;

		private string nameAnsi = string.Empty;

		private string formulaAnsi = string.Empty;

		private string validationRuleAnsi = string.Empty;

		private string validationTextAnsi = string.Empty;

		private string errorAnsi = string.Empty;

		private int internalType;

		private List<string> skipBlocks = new List<string>();
	}
}
