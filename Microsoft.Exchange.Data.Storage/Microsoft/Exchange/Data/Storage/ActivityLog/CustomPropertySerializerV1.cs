using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Storage.Clutter;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CustomPropertySerializerV1 : AbstractCustomPropertySerializer
	{
		public CustomPropertySerializerV1() : base(1, 1)
		{
		}

		public override byte[] Serialize(IDictionary<string, string> dictionary, out bool isTruncatedResult)
		{
			ArgumentValidator.ThrowIfNull("dictionary", dictionary);
			bool flag = false;
			int num = 1021;
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream(2048))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8))
				{
					long length = memoryStream.Length;
					foreach (KeyValuePair<string, string> keyValuePair in dictionary)
					{
						if (!string.IsNullOrEmpty(keyValuePair.Key))
						{
							string text = keyValuePair.Value ?? string.Empty;
							if (memoryStream.Length + (long)keyValuePair.Key.Length + (long)text.Length > (long)num)
							{
								flag = true;
								break;
							}
							binaryWriter.Write(keyValuePair.Key);
							binaryWriter.Write(text);
							if (memoryStream.Length > (long)num)
							{
								flag = true;
								break;
							}
							length = memoryStream.Length;
						}
					}
					array = new byte[3L + length];
					Array.Copy(memoryStream.GetBuffer(), 0L, array, 3L, length);
				}
			}
			array[0] = (byte)this.Version;
			array[1] = (byte)this.MinSupportedDeserializerVersion;
			byte[] array2 = array;
			int num2 = 2;
			array2[num2] |= (flag ? 1 : 0);
			isTruncatedResult = flag;
			return array;
		}

		public override Dictionary<string, string> Deserialize(byte[] byteArray, out bool isTruncatedResult)
		{
			ArgumentValidator.ThrowIfNull("byteArray", byteArray);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			isTruncatedResult = false;
			if (byteArray.Length > 1024)
			{
				InferenceDiagnosticsLog.Log("CustomPropertySerializerV1.Deserialize", string.Format("Byte Array length '{0}' is greater than ByteLimit '{1}'. Possible data corruption.", byteArray.Length, 1024));
				return dictionary;
			}
			using (MemoryStream memoryStream = new MemoryStream(byteArray))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
				{
					try
					{
						long length = memoryStream.Length;
						if (length < 3L)
						{
							InferenceDiagnosticsLog.Log("CustomPropertySerializerV1.Deserialize", "Stream length cannot be lesser than header length");
							return dictionary;
						}
						binaryReader.ReadByte();
						int num = (int)binaryReader.ReadByte();
						if (this.Version < num)
						{
							InferenceDiagnosticsLog.Log("CustomPropertySerializerV1.Deserialize", string.Format("Serializer version '{0}' cannot be lesser than min supported version '{1}'", this.Version, num));
							return dictionary;
						}
						byte b = binaryReader.ReadByte();
						isTruncatedResult = ((b & 1) == 1);
						string text = string.Empty;
						string value = string.Empty;
						while (memoryStream.Position < length)
						{
							text = binaryReader.ReadString();
							value = binaryReader.ReadString();
							if (!dictionary.ContainsKey(text))
							{
								dictionary.Add(text, value);
							}
							else
							{
								InferenceDiagnosticsLog.Log("CustomPropertySerializerV1.Deserialize", string.Format("Found duplicate key '{0}'", text));
							}
						}
					}
					catch (EndOfStreamException ex)
					{
						InferenceDiagnosticsLog.Log("CustomPropertySerializerV1.Deserialize", ex.ToString());
					}
				}
			}
			return dictionary;
		}
	}
}
