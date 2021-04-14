using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public struct PropGroupChangeInfo
	{
		public PropGroupChangeInfo(ExchangeId initialCn)
		{
			this.mappingId = MessagePropGroups.CurrentGroupMappingId;
			this.groupCns = new ExchangeId[MessagePropGroups.NumberedGroupLists.Length + 1];
			for (int i = 0; i < this.groupCns.Length; i++)
			{
				this.groupCns[i] = initialCn;
			}
		}

		private PropGroupChangeInfo(ExchangeId[] groupCns)
		{
			this.mappingId = MessagePropGroups.CurrentGroupMappingId;
			this.groupCns = groupCns;
		}

		public bool IsValid
		{
			get
			{
				return this.groupCns != null && this.MappingId == MessagePropGroups.CurrentGroupMappingId;
			}
		}

		public int MappingId
		{
			get
			{
				return this.mappingId;
			}
		}

		public int Count
		{
			get
			{
				if (this.groupCns != null)
				{
					return this.groupCns.Length - 1;
				}
				return 0;
			}
		}

		public ExchangeId this[int groupIndex]
		{
			get
			{
				return this.groupCns[groupIndex];
			}
			set
			{
				this.groupCns[groupIndex] = value;
			}
		}

		public ExchangeId Other
		{
			get
			{
				if (this.groupCns != null)
				{
					return this.groupCns[MessagePropGroups.NumberedGroupLists.Length];
				}
				return ExchangeId.Zero;
			}
			set
			{
				this.groupCns[MessagePropGroups.NumberedGroupLists.Length] = value;
			}
		}

		public static PropGroupChangeInfo Deserialize(Context context, IReplidGuidMap replidGuidMap, byte[] value)
		{
			if (value == null || value.Length <= 16)
			{
				return default(PropGroupChangeInfo);
			}
			int num = 0;
			int num2 = ParseSerialize.ParseInt32(value, num);
			num += 4;
			if (num2 != MessagePropGroups.CurrentGroupMappingId)
			{
				return default(PropGroupChangeInfo);
			}
			int num3 = ParseSerialize.ParseInt32(value, num);
			num += 4;
			if (num3 == 0)
			{
				return default(PropGroupChangeInfo);
			}
			ExchangeId[] array = new ExchangeId[MessagePropGroups.NumberedGroupLists.Length + 1];
			long num4 = ParseSerialize.ParseInt64(value, num);
			num += 8;
			ExchangeId exchangeId = ExchangeId.CreateFromInt64(context, replidGuidMap, num4);
			int i = 0;
			while (i < MessagePropGroups.NumberedGroupLists.Length + 1)
			{
				if ((num3 & 1) == 0)
				{
					array[i] = exchangeId;
				}
				else
				{
					if (num + 1 > value.Length)
					{
						return default(PropGroupChangeInfo);
					}
					byte b = value[num++];
					if (b == 255)
					{
						if (num + 8 > value.Length)
						{
							return default(PropGroupChangeInfo);
						}
						num4 = ParseSerialize.ParseInt64(value, num);
						num += 8;
						array[i] = ExchangeId.CreateFromInt64(context, replidGuidMap, num4);
					}
					else
					{
						int num5 = (int)(b & 31);
						int num6 = (b >> 5) + 1;
						ExchangeId exchangeId2;
						if (num5 == 31)
						{
							exchangeId2 = exchangeId;
						}
						else
						{
							if (num5 >= i)
							{
								return default(PropGroupChangeInfo);
							}
							exchangeId2 = array[num5];
						}
						if (num6 == 8)
						{
							array[i] = exchangeId2;
						}
						else
						{
							if (num + (8 - num6) > value.Length)
							{
								return default(PropGroupChangeInfo);
							}
							num4 = exchangeId2.ToLong() << (8 - num6) * 8;
							for (int j = 0; j < 8 - num6; j++)
							{
								num4 = (long)((ulong)num4 >> 8);
								num4 |= (long)((long)((ulong)value[num++]) << 56);
							}
							array[i] = ExchangeId.CreateFromInt64(context, replidGuidMap, num4);
						}
					}
				}
				i++;
				num3 >>= 1;
			}
			if (num != value.Length)
			{
				return default(PropGroupChangeInfo);
			}
			return new PropGroupChangeInfo(array);
		}

		public byte[] Serialize()
		{
			if (this.Count == 0)
			{
				return null;
			}
			long[] array = new long[MessagePropGroups.NumberedGroupLists.Length + 1];
			byte[] array2 = new byte[MessagePropGroups.NumberedGroupLists.Length + 1];
			for (int i = 0; i < MessagePropGroups.NumberedGroupLists.Length + 1; i++)
			{
				array[i] = this.groupCns[i].ToLong();
			}
			Dictionary<long, int> dictionary = new Dictionary<long, int>((MessagePropGroups.NumberedGroupLists.Length + 1) / 2);
			int num = 0;
			int num2 = 0;
			for (int j = 0; j < MessagePropGroups.NumberedGroupLists.Length + 1; j++)
			{
				long key = array[j];
				int num3;
				if (!dictionary.TryGetValue(key, out num3))
				{
					num3 = (dictionary[key] = j);
				}
				byte[] array3 = array2;
				int num4 = num3;
				if ((int)(array3[num4] += 1) > num)
				{
					num2 = num3;
					num = (int)array2[num3];
					if (num >= (MessagePropGroups.NumberedGroupLists.Length + 1 + 1) / 2)
					{
						break;
					}
				}
			}
			long num5 = array[num2];
			int num6 = 0;
			int num7 = 16;
			Array.Clear(array2, 0, array2.Length);
			for (int k = MessagePropGroups.NumberedGroupLists.Length; k >= 0; k--)
			{
				num6 <<= 1;
				if (array[k] != num5)
				{
					num6 |= 1;
					int num8 = 31;
					int num9 = this.ComputeCommonPrefixLength(array[k], num5);
					int num10 = 0;
					while (num10 < k && num9 < 8)
					{
						if (array[num10] != num5)
						{
							int num11 = this.ComputeCommonPrefixLength(array[k], array[num10]);
							if (num11 > num9)
							{
								num8 = num10;
								num9 = num11;
							}
						}
						num10++;
					}
					if (num9 == 0)
					{
						array2[k] = byte.MaxValue;
					}
					else
					{
						array2[k] = (byte)(num9 - 1 << 5 | num8);
					}
					num7 += 1 + (8 - num9);
				}
			}
			byte[] array4 = new byte[num7];
			int num12 = 0;
			ParseSerialize.SerializeInt32(MessagePropGroups.CurrentGroupMappingId, array4, num12);
			num12 += 4;
			ParseSerialize.SerializeInt32(num6, array4, num12);
			num12 += 4;
			ParseSerialize.SerializeInt64(num5, array4, num12);
			num12 += 8;
			int l = 0;
			while (l < MessagePropGroups.NumberedGroupLists.Length + 1)
			{
				if ((num6 & 1) != 0)
				{
					byte b = array2[l];
					array4[num12++] = b;
					if (b == 255)
					{
						ParseSerialize.SerializeInt64(array[l], array4, num12);
						num12 += 8;
					}
					else
					{
						int num13 = (b >> 5) + 1;
						long num14 = (long)((ulong)array[l] >> 8 * num13);
						int m = 0;
						while (m < 8 - num13)
						{
							array4[num12++] = (byte)num14;
							m++;
							num14 >>= 8;
						}
					}
				}
				l++;
				num6 >>= 1;
			}
			return array4;
		}

		public uint ComputeChangeMask(IdSet cnsetSeen)
		{
			if (!this.IsValid)
			{
				return uint.MaxValue;
			}
			uint num = 1U;
			uint num2 = 0U;
			for (int i = 0; i < this.Count; i++)
			{
				if (!cnsetSeen.Contains(this[i]))
				{
					num2 |= num;
				}
				num <<= 1;
			}
			if (!cnsetSeen.Contains(this.Other))
			{
				num2 |= 2147483648U;
			}
			return num2;
		}

		private int ComputeCommonPrefixLength(long cn, long otherCn)
		{
			cn ^= otherCn;
			int i = 0;
			while (i < 8)
			{
				if (0L != (cn & 255L))
				{
					return i;
				}
				i++;
				cn >>= 8;
			}
			return 8;
		}

		private int mappingId;

		private ExchangeId[] groupCns;
	}
}
