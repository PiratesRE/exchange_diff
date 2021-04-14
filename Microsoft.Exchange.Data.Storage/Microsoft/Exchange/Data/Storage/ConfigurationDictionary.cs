using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConfigurationDictionary : DictionaryBase, IXmlSerializable
	{
		internal ConfigurationDictionary()
		{
			this.isDirty = false;
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		private static XmlReader GetNode(XmlReader reader, string nodeName)
		{
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.Name == nodeName)
				{
					return reader;
				}
				reader.Read();
			}
			return null;
		}

		private static XmlReader GetAttribute(XmlReader reader, string attrName)
		{
			if (reader.HasAttributes)
			{
				reader.MoveToFirstAttribute();
				while (!(reader.Name == attrName))
				{
					if (!reader.MoveToNextAttribute())
					{
						goto IL_27;
					}
				}
				return reader;
			}
			IL_27:
			return null;
		}

		private static bool CheckElementSupportedType(object o, bool allowNull)
		{
			bool result = false;
			if (o == null)
			{
				if (allowNull)
				{
					result = true;
				}
			}
			else if (!(o is Enum))
			{
				if (o is ExDateTime)
				{
					result = true;
				}
				else
				{
					TypeCode typeCode = Type.GetTypeCode(o.GetType());
					for (int i = 0; i < ConfigurationDictionary.simpleSerializableTypes.Length; i++)
					{
						if (ConfigurationDictionary.simpleSerializableTypes[i] == typeCode)
						{
							result = true;
							break;
						}
					}
				}
			}
			return result;
		}

		private static string EncodeArray(Array objects)
		{
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array = objects as byte[];
			if (array != null)
			{
				stringBuilder.Append((int)Type.GetTypeCode(typeof(byte)));
				stringBuilder.Append('-');
				stringBuilder.Append(Convert.ToBase64String(array));
			}
			else
			{
				string[] array2 = objects as string[];
				if (array2 != null)
				{
					ConfigurationDictionary.EncodeStringArray(stringBuilder, array2);
				}
			}
			return stringBuilder.ToString();
		}

		private static Array DecodeArray(string propValue)
		{
			if (propValue == null)
			{
				throw new FormatException();
			}
			int num = propValue.IndexOf('-');
			if (num < 0)
			{
				throw new FormatException();
			}
			int num2 = int.Parse(propValue.Substring(0, num));
			Array result;
			if (num2 == (int)Type.GetTypeCode(typeof(byte)))
			{
				result = Convert.FromBase64String(propValue.Substring(num + 1));
			}
			else
			{
				if (num2 != (int)Type.GetTypeCode(typeof(string)))
				{
					throw new FormatException();
				}
				result = ConfigurationDictionary.DecodeStringArray(propValue);
			}
			return result;
		}

		private static void EncodeStringArray(StringBuilder databuilder, string[] strings)
		{
			databuilder.Append((int)Type.GetTypeCode(typeof(string)));
			databuilder.Append('-');
			databuilder.Append(strings.Length);
			for (int i = 0; i < strings.Length; i++)
			{
				databuilder.Append('-');
				databuilder.Append(strings[i].Length);
				databuilder.Append('-');
				databuilder.Append(strings[i]);
			}
		}

		private static string[] DecodeStringArray(string propValue)
		{
			int num = propValue.IndexOf('-');
			if (num < 0)
			{
				throw new FormatException();
			}
			num++;
			int num2 = propValue.Substring(num).IndexOf('-');
			if (num2 < 0)
			{
				throw new FormatException();
			}
			int num3 = int.Parse(propValue.Substring(num, num2));
			if (num3 <= 0)
			{
				throw new FormatException();
			}
			num += num2;
			string[] array = new string[num3];
			for (int i = 0; i < num3; i++)
			{
				if (num >= propValue.Length || propValue[num] != '-')
				{
					throw new FormatException();
				}
				num++;
				num2 = propValue.Substring(num).IndexOf('-');
				if (num2 < 0)
				{
					throw new FormatException();
				}
				int num4 = int.Parse(propValue.Substring(num, num2));
				if (num4 < 0)
				{
					throw new FormatException();
				}
				num += num2 + 1;
				if (num + num4 > propValue.Length)
				{
					throw new FormatException();
				}
				array[i] = propValue.Substring(num, num4);
				num += num4;
			}
			if (num != propValue.Length)
			{
				throw new FormatException();
			}
			return array;
		}

		private static string SerializeObject(object o)
		{
			TypeCode typeCode = TypeCode.Empty;
			string arg = string.Empty;
			Array array = o as Array;
			if (o == null)
			{
				arg = "0";
			}
			else if (array != null)
			{
				typeCode = TypeCode.Object;
				arg = ConfigurationDictionary.EncodeArray(array);
			}
			else if (o is ExDateTime)
			{
				typeCode = TypeCode.DateTime;
				arg = ((ExDateTime)o).ToBinary().ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				typeCode = Type.GetTypeCode(o.GetType());
				int i;
				for (i = 0; i < ConfigurationDictionary.simpleSerializableTypes.Length; i++)
				{
					if (ConfigurationDictionary.simpleSerializableTypes[i] == typeCode)
					{
						IFormattable formattable = o as IFormattable;
						arg = ((formattable != null) ? formattable.ToString(null, CultureInfo.InvariantCulture) : o.ToString());
						break;
					}
				}
				if (i == ConfigurationDictionary.simpleSerializableTypes.Length)
				{
					ExTraceGlobals.StorageTracer.TraceError<Type>(0L, "ConfigurationDictionary::SerializeObject. The type '{0}' is not supported by user configuration for serialization.", o.GetType());
					throw new NotSupportedException(ServerStrings.ExTypeSerializationNotSupported(o.GetType()));
				}
			}
			return string.Format("{0}{1}{2}", (int)typeCode, '-', arg);
		}

		private static object ConstructObject(string value, TypeCode typeCode)
		{
			object result = null;
			if (typeCode == TypeCode.Empty)
			{
				if (value != "0")
				{
					throw new FormatException(ServerStrings.ExBadValueForTypeCode0);
				}
			}
			else if (typeCode == TypeCode.DateTime)
			{
				result = ExDateTime.FromBinary(long.Parse(value, CultureInfo.InvariantCulture));
			}
			else if (typeCode == TypeCode.Int16 || typeCode == TypeCode.Int32)
			{
				result = int.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
			}
			else if (typeCode == TypeCode.UInt16 || typeCode == TypeCode.UInt32)
			{
				result = uint.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
			}
			else if (typeCode == TypeCode.Int64)
			{
				result = long.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
			}
			else if (typeCode == TypeCode.UInt64)
			{
				result = ulong.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
			}
			else if (typeCode == TypeCode.Boolean)
			{
				result = bool.Parse(value);
			}
			else if (typeCode == TypeCode.Byte)
			{
				result = byte.Parse(value, CultureInfo.InvariantCulture);
			}
			else if (typeCode == TypeCode.String)
			{
				result = value;
			}
			else
			{
				if (typeCode != TypeCode.Object)
				{
					throw new FormatException(ServerStrings.ExBadValueForTypeCode0);
				}
				result = ConfigurationDictionary.DecodeArray(value);
			}
			return result;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement("UserConfiguration");
			if (ConfigurationDictionary.GetNode(reader, "Info") == null)
			{
				this.ThrowOnCorruptedData("ReadXml", ServerStrings.ExDictionaryDataCorruptedNoField, "Info");
			}
			if (ConfigurationDictionary.GetAttribute(reader, "version") == null)
			{
				this.ThrowOnCorruptedData("ReadXml", ServerStrings.ExDictionaryDataCorruptedNoField, "version");
			}
			if (reader.Value != "Exchange.12")
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "The version is not supported. Version = {0}.", reader.Value);
			}
			if (ConfigurationDictionary.GetNode(reader, "Data") == null)
			{
				this.ThrowOnCorruptedData("ReadXml", ServerStrings.ExDictionaryDataCorruptedNoField, "Data");
			}
			reader.Read();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.Name == "e" && reader.HasAttributes)
				{
					reader.MoveToFirstAttribute();
					object obj = null;
					object obj2 = null;
					bool flag = true;
					do
					{
						if (reader.Name == "k")
						{
							if (obj == null)
							{
								obj = this.DeserializeObject(reader.Value);
							}
						}
						else if (reader.Name == "v")
						{
							flag = false;
							if (obj2 == null)
							{
								obj2 = this.DeserializeObject(reader.Value);
							}
						}
					}
					while (reader.MoveToNextAttribute());
					if (obj == null)
					{
						this.ThrowOnCorruptedData("ReadXml", ServerStrings.ExDictionaryDataCorruptedNullKey, "k");
					}
					if (flag)
					{
						this.ThrowOnCorruptedData("ReadXml", ServerStrings.ExDictionaryDataCorruptedNoField, "v");
					}
					if (base.Dictionary.Contains(obj))
					{
						this.ThrowOnCorruptedData("ReadXml", ServerStrings.ExDictionaryDataCorruptedDuplicateKey(obj.ToString()), obj.ToString());
					}
					else
					{
						base.Dictionary.Add(obj, obj2);
					}
				}
				reader.Read();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("UserConfiguration");
			writer.WriteStartElement("Info");
			writer.WriteAttributeString("version", "Exchange.12");
			writer.WriteEndElement();
			writer.WriteStartElement("Data");
			foreach (object obj in base.Dictionary)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				writer.WriteStartElement("e");
				string value = ConfigurationDictionary.SerializeObject(dictionaryEntry.Key);
				string value2 = ConfigurationDictionary.SerializeObject(dictionaryEntry.Value);
				writer.WriteAttributeString("k", value);
				writer.WriteAttributeString("v", value2);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteEndDocument();
		}

		protected override void OnClearComplete()
		{
			this.isDirty = true;
		}

		protected override void OnInsertComplete(object key, object value)
		{
			this.isDirty = true;
		}

		protected override void OnRemoveComplete(object key, object value)
		{
			this.isDirty = true;
		}

		protected override void OnSetComplete(object key, object oldValue, object newValue)
		{
			this.isDirty = true;
		}

		protected override void OnInsert(object key, object value)
		{
			this.CheckSupportedType(key, false);
			this.CheckSupportedType(value, true);
		}

		protected override void OnSet(object key, object oldValue, object newValue)
		{
			this.CheckSupportedType(key, false);
			this.CheckSupportedType(newValue, true);
		}

		private void ThrowOnCorruptedData(string method, string message, string field)
		{
			ExTraceGlobals.StorageTracer.TraceError<string, string, string>((long)this.GetHashCode(), "Ex12UserConfigurationDictionary::{0}. {1}. Field = {2}.", method, message, field);
			throw new CorruptDataException(ServerStrings.ExConfigDataCorrupted(field));
		}

		private void CheckSupportedType(object o, bool allowNull)
		{
			bool flag = false;
			Array array = o as Array;
			if (array != null)
			{
				if (array is byte[] && array.Length > 0)
				{
					flag = true;
					goto IL_71;
				}
				if (!(array is string[]) || array.Length <= 0)
				{
					goto IL_71;
				}
				flag = true;
				using (IEnumerator enumerator = array.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == null)
						{
							flag = false;
							break;
						}
					}
					goto IL_71;
				}
			}
			flag = ConfigurationDictionary.CheckElementSupportedType(o, allowNull);
			IL_71:
			if (!flag)
			{
				ExTraceGlobals.StorageTracer.TraceError<Type>((long)this.GetHashCode(), "ConfigurationDictionary::CheckSupportedType. The type is not supported. Type = {0}.", o.GetType());
				throw new NotSupportedException(ServerStrings.ExTypeSerializationNotSupported(o.GetType()));
			}
		}

		private object DeserializeObject(string compoundValue)
		{
			int num = compoundValue.IndexOf('-');
			if (num < 0)
			{
				this.ThrowOnCorruptedData("DeserializeObject", ServerStrings.ExSeparatorNotFoundOnCompoundValue, compoundValue);
			}
			string text = compoundValue.Substring(0, num);
			int num2;
			if (!int.TryParse(text, out num2))
			{
				this.ThrowOnCorruptedData("DeserializeObject", ServerStrings.ExBadObjectType, text);
			}
			TypeCode typeCode = (TypeCode)num2;
			object result = null;
			try
			{
				string value = compoundValue.Substring(num + 1);
				result = ConfigurationDictionary.ConstructObject(value, typeCode);
			}
			catch (ArgumentException)
			{
				this.ThrowOnCorruptedData("DeserializeObject", ServerStrings.ExCannotParseValue, compoundValue);
			}
			catch (OverflowException)
			{
				this.ThrowOnCorruptedData("DeserializeObject", ServerStrings.ExCannotParseValue, compoundValue);
			}
			catch (FormatException)
			{
				this.ThrowOnCorruptedData("DeserializeObject", ServerStrings.ExCannotParseValue, compoundValue);
			}
			return result;
		}

		internal bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
		}

		private const char Separator = '-';

		private const string UserConfiguration = "UserConfiguration";

		private const string InfoNode = "Info";

		private const string CurrentXmlVersion = "1.0";

		private const string VersionName = "version";

		private const string CurrentVersion = "Exchange.12";

		private const string DataNode = "Data";

		private const string Entry = "e";

		private const string Key = "k";

		private const string Value = "v";

		private bool isDirty;

		private static readonly TypeCode[] simpleSerializableTypes = new TypeCode[]
		{
			TypeCode.Int16,
			TypeCode.Int32,
			TypeCode.Int64,
			TypeCode.UInt16,
			TypeCode.UInt32,
			TypeCode.UInt64,
			TypeCode.Boolean,
			TypeCode.String,
			TypeCode.Byte
		};
	}
}
