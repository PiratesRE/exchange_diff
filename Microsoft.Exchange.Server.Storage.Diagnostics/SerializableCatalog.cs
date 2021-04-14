using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public struct SerializableCatalog
	{
		public SerializableCatalog(string tableName, string tableType, string partitionKey, string parameterTypes, string visibility)
		{
			this.tableName = tableName;
			this.tableType = tableType;
			this.partitionKey = partitionKey;
			this.parameterTypes = parameterTypes;
			this.visibility = visibility;
		}

		public string TableName
		{
			get
			{
				return this.tableName;
			}
		}

		public string TableType
		{
			get
			{
				return this.tableType;
			}
		}

		public string PartitionKey
		{
			get
			{
				return this.partitionKey;
			}
		}

		public string ParameterTypes
		{
			get
			{
				return this.parameterTypes;
			}
		}

		public string Visibility
		{
			get
			{
				return this.visibility;
			}
		}

		internal static byte[] Serialize(IList<SerializableCatalog> arrayOfItems)
		{
			int count = arrayOfItems.Count;
			int num = 0;
			num += SerializedValue.SerializeInt32(1, null, 0);
			num += SerializedValue.SerializeInt32(count, null, 0);
			for (int i = 0; i < count; i++)
			{
				num += SerializedValue.SerializeString(arrayOfItems[i].tableName, null, 0);
				num += SerializedValue.SerializeString(arrayOfItems[i].tableType, null, 0);
				num += SerializedValue.SerializeString(arrayOfItems[i].partitionKey, null, 0);
				num += SerializedValue.SerializeString(arrayOfItems[i].parameterTypes, null, 0);
				num += SerializedValue.SerializeString(arrayOfItems[i].visibility, null, 0);
			}
			byte[] array = new byte[num];
			int num2 = 0;
			num2 += SerializedValue.SerializeInt32(1, array, num2);
			num2 += SerializedValue.SerializeInt32(count, array, num2);
			for (int j = 0; j < count; j++)
			{
				num2 += SerializedValue.SerializeString(arrayOfItems[j].tableName, array, num2);
				num2 += SerializedValue.SerializeString(arrayOfItems[j].tableType, array, num2);
				num2 += SerializedValue.SerializeString(arrayOfItems[j].partitionKey, array, num2);
				num2 += SerializedValue.SerializeString(arrayOfItems[j].parameterTypes, array, num2);
				num2 += SerializedValue.SerializeString(arrayOfItems[j].visibility, array, num2);
			}
			return array;
		}

		internal static SerializableCatalog[] Deserialize(byte[] theBlob)
		{
			int num = 0;
			int num2 = SerializedValue.ParseInt32(theBlob, ref num);
			if (num2 != 1)
			{
				throw new InvalidSerializedFormatException("Wrong version.");
			}
			int num3 = SerializedValue.ParseInt32(theBlob, ref num);
			if ((theBlob.Length - num) / 5 < num3)
			{
				throw new InvalidSerializedFormatException("Invalid number of elements.");
			}
			if (num3 < 0)
			{
				throw new InvalidSerializedFormatException("Invalid number of elements.");
			}
			SerializableCatalog[] array = new SerializableCatalog[num3];
			for (int i = 0; i < num3; i++)
			{
				string text = SerializedValue.ParseString(theBlob, ref num);
				string text2 = SerializedValue.ParseString(theBlob, ref num);
				string text3 = SerializedValue.ParseString(theBlob, ref num);
				string text4 = SerializedValue.ParseString(theBlob, ref num);
				string text5 = SerializedValue.ParseString(theBlob, ref num);
				array[i] = new SerializableCatalog(text, text2, text3, text4, text5);
			}
			return array;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("TableName:[");
			stringBuilder.AppendAsString(this.TableName);
			stringBuilder.Append("] TableType:[");
			stringBuilder.AppendAsString(this.TableType);
			stringBuilder.Append("] PartitionKey:[");
			stringBuilder.AppendAsString(this.PartitionKey);
			stringBuilder.Append("] ParameterTypes:[");
			stringBuilder.AppendAsString(this.ParameterTypes);
			stringBuilder.Append("] Visibility:[");
			stringBuilder.AppendAsString(this.Visibility);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public static string SerializableCatalogAsString(byte[] theBlob)
		{
			SerializableCatalog[] array = SerializableCatalog.Deserialize(theBlob);
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("SerializableCatalog:[");
			for (int i = 0; i < array.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("[");
				stringBuilder.Append(array[i].ToString());
				stringBuilder.Append("]");
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public const int BlobVersion = 1;

		private string tableName;

		private string tableType;

		private string partitionKey;

		private string parameterTypes;

		private string visibility;
	}
}
