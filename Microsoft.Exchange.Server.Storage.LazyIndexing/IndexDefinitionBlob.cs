using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public struct IndexDefinitionBlob
	{
		public IndexDefinitionBlob(int columnType, int maxLength, bool fixedLength, bool ascending)
		{
			this.columnType = columnType;
			this.maxLength = maxLength;
			this.fixedLength = fixedLength;
			this.ascending = ascending;
		}

		public int ColumnType
		{
			get
			{
				return this.columnType;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
		}

		public bool FixedLength
		{
			get
			{
				return this.fixedLength;
			}
		}

		public bool Ascending
		{
			get
			{
				return this.ascending;
			}
		}

		internal static byte[] Serialize(int keyColumnCount, int lcid, short identityColumnIndex, IList<IndexDefinitionBlob> arrayOfItems)
		{
			int count = arrayOfItems.Count;
			int num = 0;
			num += SerializedValue.SerializeInt32(1, null, 0);
			num += SerializedValue.SerializeInt32(count, null, 0);
			num += SerializedValue.SerializeInt32(keyColumnCount, null, 0);
			num += SerializedValue.SerializeInt32(lcid, null, 0);
			num += SerializedValue.SerializeInt16(identityColumnIndex, null, 0);
			for (int i = 0; i < count; i++)
			{
				num += SerializedValue.SerializeInt32(arrayOfItems[i].columnType, null, 0);
				num += SerializedValue.SerializeInt32(arrayOfItems[i].maxLength, null, 0);
				num += SerializedValue.SerializeBoolean(arrayOfItems[i].fixedLength, null, 0);
				num += SerializedValue.SerializeBoolean(arrayOfItems[i].ascending, null, 0);
			}
			byte[] array = new byte[num];
			int num2 = 0;
			num2 += SerializedValue.SerializeInt32(1, array, num2);
			num2 += SerializedValue.SerializeInt32(count, array, num2);
			num2 += SerializedValue.SerializeInt32(keyColumnCount, array, num2);
			num2 += SerializedValue.SerializeInt32(lcid, array, num2);
			num2 += SerializedValue.SerializeInt16(identityColumnIndex, array, num2);
			for (int j = 0; j < count; j++)
			{
				num2 += SerializedValue.SerializeInt32(arrayOfItems[j].columnType, array, num2);
				num2 += SerializedValue.SerializeInt32(arrayOfItems[j].maxLength, array, num2);
				num2 += SerializedValue.SerializeBoolean(arrayOfItems[j].fixedLength, array, num2);
				num2 += SerializedValue.SerializeBoolean(arrayOfItems[j].ascending, array, num2);
			}
			return array;
		}

		internal static IndexDefinitionBlob[] Deserialize(out int keyColumnCount, out int lcid, out short identityColumnIndex, byte[] theBlob)
		{
			int num = 0;
			int num2 = SerializedValue.ParseInt32(theBlob, ref num);
			if (num2 != 1)
			{
				throw new InvalidSerializedFormatException("Wrong version.");
			}
			int num3 = SerializedValue.ParseInt32(theBlob, ref num);
			keyColumnCount = SerializedValue.ParseInt32(theBlob, ref num);
			lcid = SerializedValue.ParseInt32(theBlob, ref num);
			identityColumnIndex = SerializedValue.ParseInt16(theBlob, ref num);
			if ((theBlob.Length - num) / 4 < num3)
			{
				throw new InvalidSerializedFormatException("Invalid number of elements.");
			}
			if (num3 < 0)
			{
				throw new InvalidSerializedFormatException("Invalid number of elements.");
			}
			IndexDefinitionBlob[] array = new IndexDefinitionBlob[num3];
			for (int i = 0; i < num3; i++)
			{
				int num4 = SerializedValue.ParseInt32(theBlob, ref num);
				int num5 = SerializedValue.ParseInt32(theBlob, ref num);
				bool flag = SerializedValue.ParseBoolean(theBlob, ref num);
				bool flag2 = SerializedValue.ParseBoolean(theBlob, ref num);
				array[i] = new IndexDefinitionBlob(num4, num5, flag, flag2);
			}
			return array;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("columnType:[");
			stringBuilder.AppendAsString(this.columnType);
			stringBuilder.Append("] maxLength:[");
			stringBuilder.AppendAsString(this.maxLength);
			stringBuilder.Append("] fixedLength:[");
			stringBuilder.AppendAsString(this.fixedLength);
			stringBuilder.Append("] ascending:[");
			stringBuilder.AppendAsString(this.ascending);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public static string IndexDefinitionBlobAsString(byte[] theBlob)
		{
			int value;
			int value2;
			short value3;
			IndexDefinitionBlob[] array = IndexDefinitionBlob.Deserialize(out value, out value2, out value3, theBlob);
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("IndexDefinitionBlob:[");
			stringBuilder.Append("keyColumnCount=[");
			stringBuilder.Append(value);
			stringBuilder.Append("], ");
			stringBuilder.Append("lcid=[");
			stringBuilder.Append(value2);
			stringBuilder.Append("], ");
			stringBuilder.Append("identityColumnIndex=[");
			stringBuilder.Append(value3);
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

		private int maxLength;

		private bool fixedLength;

		private bool ascending;
	}
}
