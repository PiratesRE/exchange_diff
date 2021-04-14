using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Mapi;

namespace Microsoft.Exchange.OAB
{
	internal abstract class PropTypeHandler
	{
		public static PropTypeHandler GetHandler(PropType propType)
		{
			if (propType <= PropType.Binary)
			{
				if (propType <= PropType.String)
				{
					if (propType == PropType.Int)
					{
						return PropTypeHandler.intHandler;
					}
					if (propType == PropType.Boolean)
					{
						return PropTypeHandler.booleanHandler;
					}
					switch (propType)
					{
					case PropType.AnsiString:
						return PropTypeHandler.ansiStringHandler;
					case PropType.String:
						return PropTypeHandler.stringHandler;
					}
				}
				else
				{
					if (propType == PropType.SysTime)
					{
						return PropTypeHandler.sysTimeHandler;
					}
					if (propType == PropType.Guid)
					{
						return PropTypeHandler.guidHandler;
					}
					if (propType == PropType.Binary)
					{
						return PropTypeHandler.binaryHandler;
					}
				}
			}
			else if (propType <= PropType.StringArray)
			{
				if (propType == PropType.IntArray)
				{
					return PropTypeHandler.intArrayHandler;
				}
				if (propType == (PropType)4107)
				{
					return PropTypeHandler.booleanArrayHandler;
				}
				switch (propType)
				{
				case PropType.AnsiStringArray:
					return PropTypeHandler.ansiStringArrayHandler;
				case PropType.StringArray:
					return PropTypeHandler.stringArrayHandler;
				}
			}
			else
			{
				if (propType == PropType.SysTimeArray)
				{
					return PropTypeHandler.sysTimeArrayHandler;
				}
				if (propType == PropType.GuidArray)
				{
					return PropTypeHandler.guidArrayHandler;
				}
				if (propType == PropType.BinaryArray)
				{
					return PropTypeHandler.binaryArrayHandler;
				}
			}
			return PropTypeHandler.none;
		}

		public abstract object ReadFrom(BinaryReader reader, string elementName);

		public abstract void WriteTo(BinaryWriter writer, object value);

		public abstract void AppendText(StringBuilder text, object value);

		public virtual bool IsWritable
		{
			get
			{
				return true;
			}
		}

		protected static void WriteBytes(BinaryWriter writer, byte[] bytes)
		{
			CompressedUIntReaderWriter.WriteTo(writer, (uint)bytes.Length);
			writer.Write(bytes);
		}

		protected static byte[] ReadStringBytes(BinaryReader reader, string elementName)
		{
			List<byte> list = new List<byte>(200);
			for (;;)
			{
				byte b = reader.ReadByte(elementName);
				if (b == 0)
				{
					break;
				}
				list.Add(b);
			}
			return list.ToArray();
		}

		protected static byte[] ReadBytes(BinaryReader reader, string elementName)
		{
			int count = (int)CompressedUIntReaderWriter.ReadFrom(reader, elementName + ".count");
			return reader.ReadBytes(count, elementName);
		}

		private static readonly PropTypeHandler ansiStringHandler = new PropTypeHandler.AnsiStringHandler();

		private static readonly PropTypeHandler ansiStringArrayHandler = new PropTypeHandler.ArrayHandler(PropTypeHandler.ansiStringHandler);

		private static readonly PropTypeHandler stringHandler = new PropTypeHandler.StringHandler();

		private static readonly PropTypeHandler stringArrayHandler = new PropTypeHandler.ArrayHandler(PropTypeHandler.stringHandler);

		private static readonly PropTypeHandler booleanHandler = new PropTypeHandler.BooleanHandler();

		private static readonly PropTypeHandler booleanArrayHandler = new PropTypeHandler.ArrayHandler(PropTypeHandler.booleanHandler);

		private static readonly PropTypeHandler intHandler = new PropTypeHandler.IntHandler();

		private static readonly PropTypeHandler intArrayHandler = new PropTypeHandler.ArrayHandler(PropTypeHandler.intHandler);

		private static readonly PropTypeHandler guidHandler = new PropTypeHandler.GuidHandler();

		private static readonly PropTypeHandler guidArrayHandler = new PropTypeHandler.ArrayHandler(PropTypeHandler.guidHandler);

		private static readonly PropTypeHandler binaryHandler = new PropTypeHandler.BinaryHandler();

		private static readonly PropTypeHandler binaryArrayHandler = new PropTypeHandler.ArrayHandler(PropTypeHandler.binaryHandler);

		private static readonly PropTypeHandler sysTimeHandler = new PropTypeHandler.SysTimeHandler();

		private static readonly PropTypeHandler sysTimeArrayHandler = new PropTypeHandler.ArrayHandler(PropTypeHandler.sysTimeHandler);

		private static readonly PropTypeHandler none = new PropTypeHandler.NoHandler();

		private sealed class ArrayHandler : PropTypeHandler
		{
			public ArrayHandler(PropTypeHandler singleValueHandler)
			{
				this.singleValueHandler = singleValueHandler;
			}

			public override object ReadFrom(BinaryReader reader, string elementName)
			{
				int num = (int)CompressedUIntReaderWriter.ReadFrom(reader, elementName);
				object[] array = new object[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.singleValueHandler.ReadFrom(reader, elementName);
				}
				return array;
			}

			public override void WriteTo(BinaryWriter writer, object value)
			{
				Array array = (Array)value;
				CompressedUIntReaderWriter.WriteTo(writer, (uint)array.Length);
				foreach (object value2 in array)
				{
					this.singleValueHandler.WriteTo(writer, value2);
				}
			}

			public override void AppendText(StringBuilder text, object value)
			{
				text.Append("{");
				bool flag = true;
				Array array = (Array)value;
				foreach (object value2 in array)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						text.Append(",");
					}
					this.singleValueHandler.AppendText(text, value2);
				}
				text.Append("}");
			}

			private PropTypeHandler singleValueHandler;
		}

		private sealed class AnsiStringHandler : PropTypeHandler
		{
			public override object ReadFrom(BinaryReader reader, string elementName)
			{
				return PropTypeHandler.ReadStringBytes(reader, elementName);
			}

			public override void WriteTo(BinaryWriter writer, object value)
			{
				writer.Write((byte[])value);
				writer.Write(0);
			}

			public override void AppendText(StringBuilder text, object value)
			{
				text.Append("'");
				text.Append(Encoding.ASCII.GetString((byte[])value));
				text.Append("'");
			}
		}

		private sealed class StringHandler : PropTypeHandler
		{
			public override object ReadFrom(BinaryReader reader, string elementName)
			{
				return Encoding.UTF8.GetString(PropTypeHandler.ReadStringBytes(reader, elementName));
			}

			public override void WriteTo(BinaryWriter writer, object value)
			{
				writer.Write(Encoding.UTF8.GetBytes((string)value));
				writer.Write(0);
			}

			public override void AppendText(StringBuilder text, object value)
			{
				text.Append("'");
				text.Append((string)value);
				text.Append("'");
			}
		}

		private sealed class IntHandler : PropTypeHandler
		{
			public override object ReadFrom(BinaryReader reader, string elementName)
			{
				return (int)CompressedUIntReaderWriter.ReadFrom(reader, elementName);
			}

			public override void WriteTo(BinaryWriter writer, object value)
			{
				CompressedUIntReaderWriter.WriteTo(writer, (uint)((int)value));
			}

			public override void AppendText(StringBuilder text, object value)
			{
				text.Append(((int)value).ToString());
			}
		}

		private sealed class BooleanHandler : PropTypeHandler
		{
			public override object ReadFrom(BinaryReader reader, string elementName)
			{
				return reader.ReadByte(elementName) != 0;
			}

			public override void WriteTo(BinaryWriter writer, object value)
			{
				writer.Write(((bool)value) ? 1 : 0);
			}

			public override void AppendText(StringBuilder text, object value)
			{
				text.Append(((bool)value).ToString());
			}
		}

		private sealed class BinaryHandler : PropTypeHandler
		{
			public override object ReadFrom(BinaryReader reader, string elementName)
			{
				return PropTypeHandler.ReadBytes(reader, elementName);
			}

			public override void WriteTo(BinaryWriter writer, object value)
			{
				PropTypeHandler.WriteBytes(writer, (byte[])value);
			}

			public override void AppendText(StringBuilder text, object value)
			{
				text.Append(BitConverter.ToString((byte[])value).Replace("-", string.Empty));
			}
		}

		private sealed class GuidHandler : PropTypeHandler
		{
			public override object ReadFrom(BinaryReader reader, string elementName)
			{
				long position = reader.BaseStream.Position;
				byte[] array = PropTypeHandler.ReadBytes(reader, elementName);
				if (array.Length != 16)
				{
					throw new InvalidDataException(string.Format("Unable to read element '{0}' from position {1} because byte array is different size than expected for a GUID (16 bytes). Size read was: {2}", elementName, position, array.Length));
				}
				return new Guid(array);
			}

			public override void WriteTo(BinaryWriter writer, object value)
			{
				PropTypeHandler.WriteBytes(writer, ((Guid)value).ToByteArray());
			}

			public override void AppendText(StringBuilder text, object value)
			{
				text.Append(((Guid)value).ToString());
			}
		}

		private sealed class SysTimeHandler : PropTypeHandler
		{
			public override object ReadFrom(BinaryReader reader, string elementName)
			{
				return DateTime.FromFileTimeUtc((long)reader.ReadUInt64(elementName));
			}

			public override void WriteTo(BinaryWriter writer, object value)
			{
				writer.Write((ulong)((DateTime)value).ToFileTimeUtc());
			}

			public override void AppendText(StringBuilder text, object value)
			{
				text.Append(((DateTime)value).ToString());
			}
		}

		private sealed class NoHandler : PropTypeHandler
		{
			public override bool IsWritable
			{
				get
				{
					return false;
				}
			}

			public override object ReadFrom(BinaryReader reader, string elementName)
			{
				return null;
			}

			public override void WriteTo(BinaryWriter writer, object value)
			{
			}

			public override void AppendText(StringBuilder text, object value)
			{
				text.Append("unknown");
			}
		}
	}
}
