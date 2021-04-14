using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public class CategorizationInfo
	{
		public CategorizationInfo(int baseMessageViewLogicalIndexNumber, bool baseMessageViewInReverseOrder, int categoryCount, CategoryHeaderSortOverride[] categoryHeaderSortOverrides)
		{
			this.baseMessageViewLogicalIndexNumber = baseMessageViewLogicalIndexNumber;
			this.baseMessageViewInReverseOrder = baseMessageViewInReverseOrder;
			this.categoryCount = categoryCount;
			this.categoryHeaderSortOverrides = categoryHeaderSortOverrides;
		}

		public int BaseMessageViewLogicalIndexNumber
		{
			get
			{
				return this.baseMessageViewLogicalIndexNumber;
			}
		}

		public bool BaseMessageViewInReverseOrder
		{
			get
			{
				return this.baseMessageViewInReverseOrder;
			}
		}

		public int CategoryCount
		{
			get
			{
				return this.categoryCount;
			}
		}

		public CategoryHeaderSortOverride[] CategoryHeaderSortOverrides
		{
			get
			{
				return this.categoryHeaderSortOverrides;
			}
		}

		public static CategorizationInfo Deserialize(byte[] buffer, Func<int, string, Column> convertToColumn)
		{
			int num = 0;
			int num2 = SerializedValue.ParseInt32(buffer, ref num);
			if (num2 != 1)
			{
				throw new InvalidSerializedFormatException("Invalid version for the serialized CategorizationInfo blob.");
			}
			int num3 = SerializedValue.ParseInt32(buffer, ref num);
			bool flag = SerializedValue.ParseBoolean(buffer, ref num);
			int num4 = SerializedValue.ParseInt32(buffer, ref num);
			int num5 = SerializedValue.ParseInt32(buffer, ref num);
			CategoryHeaderSortOverride[] array = new CategoryHeaderSortOverride[num4];
			for (int i = 0; i < num5; i++)
			{
				int num6 = SerializedValue.ParseInt32(buffer, ref num);
				array[num6] = CategoryHeaderSortOverride.Deserialize(buffer, ref num, convertToColumn);
			}
			return new CategorizationInfo(num3, flag, num4, array);
		}

		public byte[] Serialize()
		{
			int num = this.Serialize(null);
			byte[] array = new byte[num];
			this.Serialize(array);
			return array;
		}

		public bool IsMatching(CategorizationInfo candidateCategorizationInfo)
		{
			bool flag = this.baseMessageViewLogicalIndexNumber == candidateCategorizationInfo.baseMessageViewLogicalIndexNumber && this.baseMessageViewInReverseOrder == candidateCategorizationInfo.baseMessageViewInReverseOrder && this.categoryCount == candidateCategorizationInfo.categoryCount;
			if (flag)
			{
				for (int i = 0; i < this.categoryCount; i++)
				{
					CategoryHeaderSortOverride categoryHeaderSortOverride = this.categoryHeaderSortOverrides[i];
					CategoryHeaderSortOverride categoryHeaderSortOverride2 = candidateCategorizationInfo.categoryHeaderSortOverrides[i];
					if (categoryHeaderSortOverride == null)
					{
						if (categoryHeaderSortOverride2 != null)
						{
							flag = false;
							break;
						}
					}
					else
					{
						if (categoryHeaderSortOverride2 == null)
						{
							flag = false;
							break;
						}
						if (categoryHeaderSortOverride.Column != categoryHeaderSortOverride2.Column || categoryHeaderSortOverride.Ascending != categoryHeaderSortOverride2.Ascending || categoryHeaderSortOverride.AggregateByMaxValue != categoryHeaderSortOverride2.AggregateByMaxValue)
						{
							flag = false;
							break;
						}
					}
				}
			}
			return flag;
		}

		private int Serialize(byte[] buffer)
		{
			int num = 0;
			num += SerializedValue.SerializeInt32(1, buffer, num);
			num += SerializedValue.SerializeInt32(this.baseMessageViewLogicalIndexNumber, buffer, num);
			num += SerializedValue.SerializeBoolean(this.baseMessageViewInReverseOrder, buffer, num);
			num += SerializedValue.SerializeInt32(this.categoryCount, buffer, num);
			num += SerializedValue.SerializeInt32(CategoryHeaderSortOverride.NumberOfOverrides(this.categoryHeaderSortOverrides), buffer, num);
			for (int i = 0; i < this.categoryCount; i++)
			{
				if (this.categoryHeaderSortOverrides[i] != null)
				{
					num += SerializedValue.SerializeInt32(i, buffer, num);
					num += this.categoryHeaderSortOverrides[i].Serialize(buffer, num);
				}
			}
			return num;
		}

		private const int BlobVersion = 1;

		private readonly int baseMessageViewLogicalIndexNumber;

		private readonly bool baseMessageViewInReverseOrder;

		private readonly int categoryCount;

		private readonly CategoryHeaderSortOverride[] categoryHeaderSortOverrides;
	}
}
