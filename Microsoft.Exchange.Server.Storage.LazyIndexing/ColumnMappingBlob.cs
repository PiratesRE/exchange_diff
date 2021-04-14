using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public struct ColumnMappingBlob
	{
		public ColumnMappingBlob(int columnType, bool fixedLength, int columnLength, string propName, int propId)
		{
			this.columnType = columnType;
			this.fixedLength = fixedLength;
			this.columnLength = columnLength;
			this.propName = propName;
			this.propId = propId;
		}

		public int ColumnType
		{
			get
			{
				return this.columnType;
			}
		}

		public bool FixedLength
		{
			get
			{
				return this.fixedLength;
			}
		}

		public int ColumnLength
		{
			get
			{
				return this.columnLength;
			}
		}

		public string PropName
		{
			get
			{
				return this.propName;
			}
		}

		public int PropId
		{
			get
			{
				return this.propId;
			}
		}

		internal static byte[] Serialize(int keyColumnCount, IList<ColumnMappingBlob> arrayOfItems)
		{
			int count = arrayOfItems.Count;
			int num = 0;
			num += SerializedValue.SerializeInt32(1, null, 0);
			num += SerializedValue.SerializeInt32(count, null, 0);
			num += SerializedValue.SerializeInt32(keyColumnCount, null, 0);
			for (int i = 0; i < count; i++)
			{
				num += SerializedValue.SerializeInt32(arrayOfItems[i].columnType, null, 0);
				num += SerializedValue.SerializeBoolean(arrayOfItems[i].fixedLength, null, 0);
				num += SerializedValue.SerializeInt32(arrayOfItems[i].columnLength, null, 0);
				num += SerializedValue.SerializeString(arrayOfItems[i].propName, null, 0);
				num += SerializedValue.SerializeInt32(arrayOfItems[i].propId, null, 0);
			}
			byte[] array = new byte[num];
			int num2 = 0;
			num2 += SerializedValue.SerializeInt32(1, array, num2);
			num2 += SerializedValue.SerializeInt32(count, array, num2);
			num2 += SerializedValue.SerializeInt32(keyColumnCount, array, num2);
			for (int j = 0; j < count; j++)
			{
				num2 += SerializedValue.SerializeInt32(arrayOfItems[j].columnType, array, num2);
				num2 += SerializedValue.SerializeBoolean(arrayOfItems[j].fixedLength, array, num2);
				num2 += SerializedValue.SerializeInt32(arrayOfItems[j].columnLength, array, num2);
				num2 += SerializedValue.SerializeString(arrayOfItems[j].propName, array, num2);
				num2 += SerializedValue.SerializeInt32(arrayOfItems[j].propId, array, num2);
			}
			return array;
		}

		internal static ColumnMappingBlob[] Deserialize(out int keyColumnCount, byte[] theBlob)
		{
			int num = 0;
			int num2 = SerializedValue.ParseInt32(theBlob, ref num);
			if (num2 != 1)
			{
				throw new InvalidSerializedFormatException("Wrong version.");
			}
			int num3 = SerializedValue.ParseInt32(theBlob, ref num);
			keyColumnCount = SerializedValue.ParseInt32(theBlob, ref num);
			if ((theBlob.Length - num) / 5 < num3)
			{
				throw new InvalidSerializedFormatException("Invalid number of elements.");
			}
			if (num3 < 0)
			{
				throw new InvalidSerializedFormatException("Invalid number of elements.");
			}
			ColumnMappingBlob[] array = new ColumnMappingBlob[num3];
			for (int i = 0; i < num3; i++)
			{
				int num4 = SerializedValue.ParseInt32(theBlob, ref num);
				bool flag = SerializedValue.ParseBoolean(theBlob, ref num);
				int num5 = SerializedValue.ParseInt32(theBlob, ref num);
				string text = SerializedValue.ParseString(theBlob, ref num);
				int num6 = SerializedValue.ParseInt32(theBlob, ref num);
				array[i] = new ColumnMappingBlob(num4, flag, num5, text, num6);
			}
			return array;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("columnType:[");
			stringBuilder.AppendAsString(this.columnType);
			stringBuilder.Append("] fixedLength:[");
			stringBuilder.AppendAsString(this.fixedLength);
			stringBuilder.Append("] columnLength:[");
			stringBuilder.AppendAsString(this.columnLength);
			stringBuilder.Append("] propName:[");
			stringBuilder.AppendAsString(this.propName);
			stringBuilder.Append("] propId:[");
			stringBuilder.AppendAsString(this.propId);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public static string ColumnMappingBlobAsString(byte[] theBlob)
		{
			int value;
			ColumnMappingBlob[] array = ColumnMappingBlob.Deserialize(out value, theBlob);
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("ColumnMappingBlob:[");
			stringBuilder.Append("keyColumnCount=[");
			stringBuilder.Append(value);
			stringBuilder.Append("], ");
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

		private int columnType;

		private bool fixedLength;

		private int columnLength;

		private string propName;

		private int propId;
	}
}
