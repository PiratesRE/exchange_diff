using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class GlobCountSet : IEquatable<GlobCountSet>, IEnumerable<GlobCountRange>, IEnumerable
	{
		public bool IsEmpty
		{
			get
			{
				return this.ranges.Count == 0 && this.singles == null;
			}
		}

		public int CountRanges
		{
			get
			{
				this.MergeSingles();
				return this.ranges.Count;
			}
		}

		public ulong CountIds
		{
			get
			{
				this.MergeSingles();
				ulong num = 0UL;
				foreach (GlobCountRange globCountRange in this.ranges)
				{
					num += globCountRange.HighBound - globCountRange.LowBound + 1UL;
				}
				return num;
			}
		}

		internal GlobCountSet(ICollection<GlobCountRange> ranges)
		{
			this.ranges = new GlobCountSet.SegmentedList<GlobCountRange>(4096, ranges.Count);
			foreach (GlobCountRange range in ranges)
			{
				this.InsertRangeImpl(range);
			}
		}

		internal GlobCountSet()
		{
			this.ranges = new GlobCountSet.SegmentedList<GlobCountRange>(4096);
		}

		public static GlobCountSet Parse(Reader reader)
		{
			GlobCountSet globCountSet = new GlobCountSet();
			globCountSet.InternalParse(reader);
			return globCountSet;
		}

		public static GlobCountSet Intersect(GlobCountSet first, GlobCountSet second)
		{
			first.MergeSingles();
			second.MergeSingles();
			GlobCountSet globCountSet = null;
			int num = 0;
			int num2 = 0;
			while (num < first.ranges.Count && num2 < second.ranges.Count)
			{
				GlobCountRange globCountRange = first.ranges[num];
				GlobCountRange globCountRange2 = second.ranges[num2];
				if (globCountRange.HighBound < globCountRange2.LowBound)
				{
					num++;
				}
				else if (globCountRange.LowBound > globCountRange2.HighBound)
				{
					num2++;
				}
				else
				{
					GlobCountRange item = new GlobCountRange(Math.Max(globCountRange.LowBound, globCountRange2.LowBound), Math.Min(globCountRange.HighBound, globCountRange2.HighBound));
					if (globCountSet == null)
					{
						globCountSet = new GlobCountSet();
					}
					globCountSet.ranges.Add(item);
					if (globCountRange.HighBound < globCountRange2.HighBound)
					{
						num++;
					}
					else
					{
						num2++;
					}
				}
			}
			return globCountSet;
		}

		public bool Insert(GlobCountSet set)
		{
			this.MergeSingles();
			set.MergeSingles();
			bool flag = false;
			foreach (GlobCountRange range in set.ranges)
			{
				flag |= this.InsertRangeImpl(range);
			}
			return flag;
		}

		public bool Insert(ulong globCount)
		{
			if (this.singles == null && (this.ranges.Count < 10 || this.ranges[this.ranges.Count - 1].LowBound <= globCount))
			{
				return this.InsertRangeImpl(new GlobCountRange(globCount, globCount));
			}
			if (this.singles == null)
			{
				if (this.Contains(globCount))
				{
					return false;
				}
				this.singles = new GlobCountSet.SegmentedList<ulong>(8192);
			}
			this.singles.Add(globCount);
			return true;
		}

		public bool Insert(GlobCountRange range)
		{
			this.MergeSingles();
			return this.InsertRangeImpl(range);
		}

		public bool Remove(GlobCountSet set)
		{
			this.MergeSingles();
			set.MergeSingles();
			bool flag = false;
			foreach (GlobCountRange range in set.ranges)
			{
				flag |= this.RemoveRangeImpl(range);
			}
			return flag;
		}

		public bool Remove(ulong globCount)
		{
			return this.Remove(new GlobCountRange(globCount, globCount));
		}

		public bool Remove(GlobCountRange range)
		{
			this.MergeSingles();
			return this.RemoveRangeImpl(range);
		}

		public bool IdealPack()
		{
			if (this.IsEmpty)
			{
				return false;
			}
			this.MergeSingles();
			GlobCountRange globCountRange = this.ranges[this.ranges.Count - 1];
			if (globCountRange.LowBound == 1UL && this.ranges.Count == 1)
			{
				return false;
			}
			this.ranges = new GlobCountSet.SegmentedList<GlobCountRange>(4096);
			this.ranges.Add(new GlobCountRange(1UL, globCountRange.HighBound));
			return true;
		}

		public bool Contains(ulong element)
		{
			GlobCountSet.VerifyGlobCountArgument(element, "element");
			this.MergeSingles();
			if (this.ranges.Count < 10)
			{
				for (int i = 0; i < this.ranges.Count; i++)
				{
					if (this.ranges[i].Contains(element))
					{
						return true;
					}
				}
				return false;
			}
			int num = this.ranges.BinarySearch(new GlobCountRange(element, element), GlobCountSet.GlobCountRangeLowBoundComparer.Instance);
			if (num >= 0)
			{
				return true;
			}
			int num2 = ~num - 1;
			return num2 >= 0 && element <= this.ranges[num2].HighBound;
		}

		public GlobCountSet Clone()
		{
			this.MergeSingles();
			return new GlobCountSet(this.ranges);
		}

		public override string ToString()
		{
			this.MergeSingles();
			StringBuilder stringBuilder = new StringBuilder("{");
			for (int i = 0; i < this.ranges.Count; i++)
			{
				stringBuilder.Append(this.ranges[i]);
				if (i < this.ranges.Count - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public void Serialize(Writer writer)
		{
			this.MergeSingles();
			this.InternalSerialize(writer, 0, this.ranges.Count, 0);
			writer.WriteByte(0);
		}

		public static GlobCountSet Union(GlobCountSet first, GlobCountSet second)
		{
			first.MergeSingles();
			second.MergeSingles();
			GlobCountSet globCountSet = new GlobCountSet();
			int num = 0;
			int num2 = 0;
			GlobCountRange item;
			if (first.ranges[0].LowBound <= second.ranges[0].LowBound)
			{
				item = first.ranges[0];
				num++;
			}
			else
			{
				item = second.ranges[0];
				num2++;
			}
			while (num < first.ranges.Count || num2 < second.ranges.Count)
			{
				if (num < first.ranges.Count && item.HighBound + 1UL >= first.ranges[num].LowBound)
				{
					item = new GlobCountRange(item.LowBound, Math.Max(item.HighBound, first.ranges[num].HighBound));
					num++;
				}
				else if (num2 < second.ranges.Count && item.HighBound + 1UL >= second.ranges[num2].LowBound)
				{
					item = new GlobCountRange(item.LowBound, Math.Max(item.HighBound, second.ranges[num2].HighBound));
					num2++;
				}
				else
				{
					globCountSet.ranges.Add(item);
					if (num2 >= second.ranges.Count || (num < first.ranges.Count && first.ranges[num].LowBound <= second.ranges[num2].LowBound))
					{
						item = first.ranges[num];
						num++;
					}
					else
					{
						item = second.ranges[num2];
						num2++;
					}
				}
			}
			globCountSet.ranges.Add(item);
			return globCountSet;
		}

		public static GlobCountSet Subtract(GlobCountSet first, GlobCountSet second)
		{
			first.MergeSingles();
			second.MergeSingles();
			GlobCountSet globCountSet = null;
			int i = 0;
			int num = 0;
			GlobCountRange item = first.ranges[0];
			while (i < first.ranges.Count)
			{
				if (num < second.ranges.Count)
				{
					GlobCountRange globCountRange2;
					GlobCountRange globCountRange = globCountRange2 = second.ranges[num];
					if (globCountRange2.LowBound <= item.HighBound)
					{
						if (globCountRange.HighBound < item.LowBound)
						{
							num++;
							continue;
						}
						if (globCountRange.LowBound > item.LowBound)
						{
							GlobCountRange item2 = new GlobCountRange(item.LowBound, globCountRange.LowBound - 1UL);
							if (globCountSet == null)
							{
								globCountSet = new GlobCountSet();
							}
							globCountSet.ranges.Add(item2);
						}
						if (globCountRange.HighBound < item.HighBound)
						{
							item = new GlobCountRange(globCountRange.HighBound + 1UL, item.HighBound);
							num++;
							continue;
						}
						i++;
						if (i < first.ranges.Count)
						{
							item = first.ranges[i];
							continue;
						}
						continue;
					}
				}
				if (globCountSet == null)
				{
					globCountSet = new GlobCountSet();
				}
				globCountSet.ranges.Add(item);
				i++;
				if (i < first.ranges.Count)
				{
					item = first.ranges[i];
				}
			}
			return globCountSet;
		}

		public IEnumerator<GlobCountRange> GetEnumerator()
		{
			this.MergeSingles();
			return this.ranges.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool Equals(GlobCountSet other)
		{
			if (other == null || this.CountRanges != other.CountRanges)
			{
				return false;
			}
			for (int i = 0; i < this.ranges.Count; i++)
			{
				if (!this.ranges[i].Equals(other.ranges[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			GlobCountSet other = obj as GlobCountSet;
			return this.Equals(other);
		}

		public override int GetHashCode()
		{
			this.MergeSingles();
			int num = this.ranges.Count.GetHashCode();
			for (int i = 0; i < this.ranges.Count; i++)
			{
				num ^= this.ranges[i].GetHashCode();
			}
			return num;
		}

		internal static void VerifyGlobCountArgument(ulong globCount, string argumentName)
		{
			if (globCount == 0UL || globCount > 281474976710655UL)
			{
				throw new ArgumentOutOfRangeException(argumentName);
			}
		}

		private static bool TrySerializeRangeAsSingle(Writer writer, GlobCountRange range, int previousBytesInCommon)
		{
			if (range.LowBound == range.HighBound)
			{
				writer.WriteByte((byte)(6 - previousBytesInCommon));
				GlobCountSet.WriteBytes(writer, range.LowBound, previousBytesInCommon, 6 - previousBytesInCommon);
				return true;
			}
			return false;
		}

		private static void SerializeRange(Writer writer, GlobCountRange range, int previousBytesInCommon)
		{
			writer.WriteByte(82);
			GlobCountSet.WriteBytes(writer, range.LowBound, previousBytesInCommon, 6 - previousBytesInCommon);
			GlobCountSet.WriteBytes(writer, range.HighBound, previousBytesInCommon, 6 - previousBytesInCommon);
		}

		private static void EncodeRangeIntoBitmap(ref byte bitmap, GlobCountRange range, byte bitmapFirstElement)
		{
			byte b = (byte)(range.LowBound & 255UL);
			byte b2 = (byte)(range.HighBound & 255UL);
			for (int i = (int)b; i <= (int)b2; i++)
			{
				if (i != (int)bitmapFirstElement)
				{
					byte b3 = (byte)(1 << i - (int)bitmapFirstElement - 1);
					bitmap |= b3;
				}
			}
		}

		private static int GetCountOfBytesInCommon(ulong left, ulong right, int previousBytesInCommon)
		{
			int result = previousBytesInCommon;
			int num = previousBytesInCommon;
			while (num < 6 && GlobCountSet.HasByteInCommon(num, left, right))
			{
				result = num + 1;
				num++;
			}
			return result;
		}

		private static bool HasByteInCommon(int byteIndex, ulong left, ulong right)
		{
			return (left & GlobCountSet.Masks[byteIndex]) == (right & GlobCountSet.Masks[byteIndex]);
		}

		private static void WriteBytes(Writer writer, ulong globCount, int firstByteIndex, int byteCount)
		{
			for (int i = firstByteIndex; i < firstByteIndex + byteCount; i++)
			{
				byte value = (byte)(globCount >> (5 - i) * 8 & 255UL);
				writer.WriteByte(value);
			}
		}

		private static GlobCountRange CreateGlobCountRangeFromBuffer(ulong lowBound, ulong highBound)
		{
			GlobCountRange result;
			try
			{
				result = new GlobCountRange(lowBound, highBound);
			}
			catch (ArgumentException ex)
			{
				throw new BufferParseException(ex.Message);
			}
			return result;
		}

		private static ulong ReadBytes(Reader reader, ulong bytesInCommon, int countOfBytesInCommon, int bytesToRead)
		{
			ulong num = 0UL;
			for (int i = 0; i < bytesToRead; i++)
			{
				num <<= 8;
				num += (ulong)reader.ReadByte();
			}
			num <<= (6 - countOfBytesInCommon - bytesToRead) * 8;
			return num | bytesInCommon;
		}

		private static ulong ClearBytesInCommon(ulong bytesInCommon, int countOfBytesInCommon, int bytesToClear)
		{
			ulong num = bytesInCommon;
			for (int i = countOfBytesInCommon - bytesToClear; i < countOfBytesInCommon; i++)
			{
				num &= ~GlobCountSet.Masks[i];
			}
			return num;
		}

		private void InternalSerialize(Writer writer, int startRangeIndex, int endRangeIndex, int previousBytesInCommon)
		{
			int i = startRangeIndex;
			while (i < endRangeIndex)
			{
				int num = 0;
				if (this.TrySerializeRangesWithBytesInCommon(writer, i, endRangeIndex, previousBytesInCommon, out num))
				{
					i += num;
				}
				else if (this.TrySerializeRangesAsBitmap(writer, i, endRangeIndex, previousBytesInCommon, out num))
				{
					i += num;
				}
				else
				{
					GlobCountRange range = this.ranges[i];
					if (!GlobCountSet.TrySerializeRangeAsSingle(writer, range, previousBytesInCommon))
					{
						GlobCountSet.SerializeRange(writer, range, previousBytesInCommon);
					}
					i++;
				}
			}
		}

		private bool TrySerializeRangesWithBytesInCommon(Writer writer, int currentRangeIndex, int endRangeIndex, int previousBytesInCommon, out int countOfSerializedRanges)
		{
			countOfSerializedRanges = 0;
			if (previousBytesInCommon < 5 && currentRangeIndex + 1 < endRangeIndex)
			{
				int num = currentRangeIndex;
				ulong lowBound = this.ranges[currentRangeIndex].LowBound;
				int num2 = currentRangeIndex + 1;
				while (num2 < endRangeIndex && GlobCountSet.HasByteInCommon(previousBytesInCommon, lowBound, this.ranges[num2].HighBound))
				{
					num = num2;
					num2++;
				}
				if (num > currentRangeIndex)
				{
					int countOfBytesInCommon = GlobCountSet.GetCountOfBytesInCommon(lowBound, this.ranges[num].HighBound, previousBytesInCommon);
					writer.WriteByte((byte)(countOfBytesInCommon - previousBytesInCommon));
					GlobCountSet.WriteBytes(writer, lowBound, previousBytesInCommon, countOfBytesInCommon - previousBytesInCommon);
					this.InternalSerialize(writer, currentRangeIndex, num + 1, countOfBytesInCommon);
					writer.WriteByte(80);
					countOfSerializedRanges = num - currentRangeIndex + 1;
				}
			}
			return countOfSerializedRanges > 0;
		}

		private bool TrySerializeRangesAsBitmap(Writer writer, int currentRangeIndex, int endRangeIndex, int previousBytesInCommon, out int countOfSerializedRanges)
		{
			countOfSerializedRanges = 0;
			if (previousBytesInCommon == 5 && currentRangeIndex + 1 < endRangeIndex)
			{
				int num = currentRangeIndex;
				ulong lowBound = this.ranges[currentRangeIndex].LowBound;
				int num2 = currentRangeIndex + 1;
				while (num2 < endRangeIndex && this.ranges[num2].HighBound - lowBound <= 8UL)
				{
					num = num2;
					num2++;
				}
				if (num > currentRangeIndex)
				{
					byte b = (byte)(lowBound & 255UL);
					byte value = 0;
					for (int i = currentRangeIndex; i <= num; i++)
					{
						GlobCountSet.EncodeRangeIntoBitmap(ref value, this.ranges[i], b);
					}
					writer.WriteByte(66);
					writer.WriteByte(b);
					writer.WriteByte(value);
					countOfSerializedRanges = num - currentRangeIndex + 1;
				}
			}
			return countOfSerializedRanges > 0;
		}

		private void InternalParse(Reader reader)
		{
			ulong bytesInCommon = 0UL;
			int num = 0;
			Stack<int> stack = new Stack<int>(5);
			byte b;
			for (;;)
			{
				b = reader.ReadByte();
				byte b2 = b;
				switch (b2)
				{
				case 0:
					goto IL_5D;
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
					if ((int)b + num > 6)
					{
						goto Block_5;
					}
					if ((int)b + num < 6)
					{
						bytesInCommon = GlobCountSet.ReadBytes(reader, bytesInCommon, num, (int)b);
						stack.Push((int)b);
						num += (int)b;
					}
					else
					{
						ulong num2 = GlobCountSet.ReadBytes(reader, bytesInCommon, num, (int)b);
						GlobCountRange range = GlobCountSet.CreateGlobCountRangeFromBuffer(num2, num2);
						this.InsertRangeImpl(range);
					}
					break;
				default:
				{
					if (b2 != 66)
					{
						switch (b2)
						{
						case 80:
						{
							if (stack.Count < 1)
							{
								goto Block_10;
							}
							int num3 = stack.Pop();
							bytesInCommon = GlobCountSet.ClearBytesInCommon(bytesInCommon, num, num3);
							num -= num3;
							continue;
						}
						case 82:
						{
							ulong lowBound = GlobCountSet.ReadBytes(reader, bytesInCommon, num, 6 - num);
							ulong highBound = GlobCountSet.ReadBytes(reader, bytesInCommon, num, 6 - num);
							GlobCountRange range2 = GlobCountSet.CreateGlobCountRangeFromBuffer(lowBound, highBound);
							this.InsertRangeImpl(range2);
							continue;
						}
						}
						goto Block_3;
					}
					if (num != 5)
					{
						goto Block_7;
					}
					ulong num4 = GlobCountSet.ReadBytes(reader, bytesInCommon, num, 1);
					this.InsertRangeImpl(GlobCountSet.CreateGlobCountRangeFromBuffer(num4, num4));
					byte b3 = reader.ReadByte();
					for (int i = 0; i < 8; i++)
					{
						byte b4 = (byte)(1 << i);
						if ((b3 & b4) != 0)
						{
							ulong num5 = num4 + (ulong)((long)i) + 1UL;
							this.InsertRangeImpl(GlobCountSet.CreateGlobCountRangeFromBuffer(num5, num5));
						}
					}
					break;
				}
				}
			}
			Block_3:
			goto IL_1A8;
			IL_5D:
			if (num == 0)
			{
				return;
			}
			throw new BufferParseException("End of GlobCountSet found prematurely, while there are still bytes in common that have not been 'popped'.");
			Block_5:
			throw new BufferParseException(string.Format("Too many bytes specified. We already have {0} bytes in common, and we found {1} more bytes being specified.", num, b));
			Block_7:
			throw new BufferParseException("Found a bitmap before we have 5 bytes in common.");
			Block_10:
			throw new BufferParseException("Found a pop instruction, and there is no bytes in common pushed.");
			IL_1A8:
			throw new BufferParseException(string.Format("Expected: 0 to 6, 'B', 'P' or 'R'. Found: {0}.", b));
		}

		private bool InsertRangeImpl(GlobCountRange range)
		{
			int num = this.ranges.Count - 1;
			if (this.ranges.Count >= 10 && range.HighBound < this.ranges[this.ranges.Count - 1].LowBound)
			{
				int num2 = this.ranges.BinarySearch(range, GlobCountSet.GlobCountRangeHighBoundComparer.Instance);
				if (num2 >= 0)
				{
					num = num2;
				}
				else
				{
					num = ~num2;
					if (num == this.ranges.Count)
					{
						num--;
					}
				}
			}
			int i = num;
			while (i >= 0)
			{
				GlobCountRange globCountRange = this.ranges[i];
				if (globCountRange.HighBound + 1UL < range.LowBound)
				{
					this.ranges.Insert(i + 1, range);
					return true;
				}
				if (globCountRange.LowBound <= range.LowBound)
				{
					if (globCountRange.HighBound < range.HighBound)
					{
						this.ranges[i] = new GlobCountRange(globCountRange.LowBound, range.HighBound);
						return true;
					}
					return false;
				}
				else if (globCountRange.LowBound > range.HighBound + 1UL)
				{
					i--;
				}
				else
				{
					ulong lowBound = Math.Min(globCountRange.LowBound, range.LowBound);
					ulong highBound = Math.Max(globCountRange.HighBound, range.HighBound);
					range = new GlobCountRange(lowBound, highBound);
					this.ranges.RemoveAt(i);
					i--;
				}
			}
			this.ranges.Insert(0, range);
			return true;
		}

		private bool RemoveRangeImpl(GlobCountRange range)
		{
			bool result = false;
			int num = 0;
			if (this.ranges.Count >= 10 && range.LowBound > this.ranges[0].HighBound)
			{
				int num2 = this.ranges.BinarySearch(range, GlobCountSet.GlobCountRangeLowBoundComparer.Instance);
				if (num2 >= 0)
				{
					num = num2;
				}
				else
				{
					num = ~num2 - 1;
					if (num < 0)
					{
						num++;
					}
				}
			}
			int i = num;
			while (i < this.ranges.Count)
			{
				GlobCountRange globCountRange = this.ranges[i];
				if (globCountRange.HighBound < range.LowBound)
				{
					i++;
				}
				else
				{
					if (globCountRange.LowBound > range.HighBound)
					{
						break;
					}
					result = true;
					if (range.HighBound >= globCountRange.HighBound)
					{
						if (range.LowBound <= globCountRange.LowBound)
						{
							this.ranges.RemoveAt(i);
						}
						else
						{
							this.ranges[i] = new GlobCountRange(globCountRange.LowBound, range.LowBound - 1UL);
							i++;
						}
					}
					else
					{
						if (range.LowBound <= globCountRange.LowBound)
						{
							this.ranges[i] = new GlobCountRange(range.HighBound + 1UL, globCountRange.HighBound);
							break;
						}
						this.ranges[i] = new GlobCountRange(range.HighBound + 1UL, globCountRange.HighBound);
						this.ranges.Insert(i, new GlobCountRange(globCountRange.LowBound, range.LowBound - 1UL));
						break;
					}
				}
			}
			return result;
		}

		private void MergeSingles()
		{
			if (this.singles != null)
			{
				this.DoMergeSingles();
			}
		}

		private void DoMergeSingles()
		{
			if (this.singles.Count <= 1)
			{
				foreach (ulong num in this.singles)
				{
					this.InsertRangeImpl(new GlobCountRange(num, num));
				}
				this.singles = null;
				return;
			}
			this.singles.Sort();
			if (this.ranges.Count == 0 || this.ranges[this.ranges.Count - 1].LowBound <= this.singles[0])
			{
				using (GlobCountSet.SegmentedList<ulong>.Enumerator enumerator2 = this.singles.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ulong num2 = enumerator2.Current;
						this.InsertRangeImpl(new GlobCountRange(num2, num2));
					}
					goto IL_212;
				}
			}
			GlobCountSet.SegmentedList<GlobCountRange> segmentedList = this.ranges;
			this.ranges = new GlobCountSet.SegmentedList<GlobCountRange>(4096);
			int num3 = 0;
			int i = 0;
			while (num3 < segmentedList.Count && i < this.singles.Count)
			{
				ulong num4 = this.singles[i];
				GlobCountRange globCountRange = new GlobCountRange(num4, num4);
				int num5 = segmentedList.BinarySearch(globCountRange, GlobCountSet.GlobCountRangeLowBoundComparer.Instance);
				if (num5 >= 0)
				{
					i++;
				}
				else
				{
					num5 = ~num5;
					if (num3 < num5)
					{
						if (this.ranges.Count != 0)
						{
							this.InsertRangeImpl(segmentedList[num3]);
							num3++;
						}
						if (num3 < num5)
						{
							this.ranges.AppendFrom(segmentedList, num3, num5 - num3);
							num3 = num5;
						}
					}
					this.InsertRangeImpl(globCountRange);
					i++;
				}
			}
			if (num3 < segmentedList.Count)
			{
				this.InsertRangeImpl(segmentedList[num3]);
				num3++;
				if (num3 < segmentedList.Count)
				{
					this.ranges.AppendFrom(segmentedList, num3, segmentedList.Count - num3);
				}
			}
			else
			{
				while (i < this.singles.Count)
				{
					ulong num6 = this.singles[i];
					GlobCountRange range = new GlobCountRange(num6, num6);
					this.InsertRangeImpl(range);
					i++;
				}
			}
			IL_212:
			this.singles = null;
		}

		private const int GlobCountSize = 6;

		private const int RangesSegmentSize = 4096;

		private const int SinglesSegmentSize = 8192;

		private const ulong MaxGlobCount = 281474976710655UL;

		private static readonly ulong[] Masks = new ulong[]
		{
			280375465082880UL,
			1095216660480UL,
			4278190080UL,
			16711680UL,
			65280UL,
			255UL
		};

		private GlobCountSet.SegmentedList<GlobCountRange> ranges;

		private GlobCountSet.SegmentedList<ulong> singles;

		private class SegmentedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable
		{
			public SegmentedList(int segmentSize) : this(segmentSize, 0)
			{
			}

			public SegmentedList(int segmentSize, int initialCapacity)
			{
				this.segmentSize = segmentSize;
				this.offsetMask = segmentSize - 1;
				this.segmentShift = 0;
				while ((segmentSize >>= 1) != 0)
				{
					this.segmentShift++;
				}
				if (initialCapacity > 0)
				{
					this.items = new T[initialCapacity + this.segmentSize - 1 >> this.segmentShift][];
					initialCapacity = Math.Min(this.segmentSize, initialCapacity);
					this.items[0] = new T[initialCapacity];
					this.capacity = initialCapacity;
				}
			}

			public int Count
			{
				get
				{
					return this.count;
				}
			}

			bool ICollection<!0>.IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public T this[int index]
			{
				get
				{
					return this.items[index >> this.segmentShift][index & this.offsetMask];
				}
				set
				{
					this.items[index >> this.segmentShift][index & this.offsetMask] = value;
				}
			}

			public void Add(T item)
			{
				if (this.count == this.capacity)
				{
					this.EnsureCapacity(this.count + 1);
				}
				this.items[this.count >> this.segmentShift][this.count & this.offsetMask] = item;
				this.count++;
			}

			public void Insert(int index, T item)
			{
				if (this.count == this.capacity)
				{
					this.EnsureCapacity(this.count + 1);
				}
				if (index < this.count)
				{
					this.AddRoomForElement(index);
				}
				this.count++;
				this.items[index >> this.segmentShift][index & this.offsetMask] = item;
			}

			public void RemoveAt(int index)
			{
				if (index < this.count)
				{
					this.RemoveRoomForElement(index);
				}
				this.count--;
			}

			public int BinarySearch(T item, IComparer<T> comparer)
			{
				int i = 0;
				int num = this.count - 1;
				while (i <= num)
				{
					int num2 = i + (num - i >> 1);
					int num3 = comparer.Compare(this.items[num2 >> this.segmentShift][num2 & this.offsetMask], item);
					if (num3 == 0)
					{
						return num2;
					}
					if (num3 < 0)
					{
						i = num2 + 1;
					}
					else
					{
						num = num2 - 1;
					}
				}
				return ~i;
			}

			public void Sort()
			{
				this.QuickSort(0, this.count - 1, Comparer<T>.Default);
			}

			public void AppendFrom(GlobCountSet.SegmentedList<T> from, int index, int count)
			{
				if (count > 0)
				{
					int num = this.count + count;
					if (this.capacity < num)
					{
						this.EnsureCapacity(num);
					}
					do
					{
						int num2 = index / from.segmentSize;
						int num3 = index % from.segmentSize;
						int val = from.segmentSize - num3;
						int num4 = this.count >> this.segmentShift;
						int num5 = this.count & this.offsetMask;
						int val2 = this.segmentSize - num5;
						int num6 = Math.Min(count, Math.Min(val, val2));
						Array.Copy(from.items[num2], num3, this.items[num4], num5, num6);
						index += num6;
						count -= num6;
						this.count += num6;
					}
					while (count != 0);
				}
			}

			public GlobCountSet.SegmentedList<T>.Enumerator GetEnumerator()
			{
				return new GlobCountSet.SegmentedList<T>.Enumerator(this);
			}

			IEnumerator<T> IEnumerable<!0>.GetEnumerator()
			{
				return new GlobCountSet.SegmentedList<T>.Enumerator(this);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new GlobCountSet.SegmentedList<T>.Enumerator(this);
			}

			void ICollection<!0>.Clear()
			{
				throw new NotImplementedException("This method of ICollection is not implemented");
			}

			bool ICollection<!0>.Contains(T item)
			{
				throw new NotImplementedException("This method of ICollection is not implemented");
			}

			void ICollection<!0>.CopyTo(T[] array, int arrayIndex)
			{
				throw new NotImplementedException("This method of ICollection is not implemented");
			}

			bool ICollection<!0>.Remove(T item)
			{
				throw new NotImplementedException("This method of ICollection is not implemented");
			}

			private void AddRoomForElement(int index)
			{
				int num = index >> this.segmentShift;
				int num2 = this.count >> this.segmentShift;
				int num3 = index & this.offsetMask;
				int num4 = this.count & this.offsetMask;
				if (num == num2)
				{
					Array.Copy(this.items[num], num3, this.items[num], num3 + 1, num4 - num3);
					return;
				}
				T t = this.items[num][this.segmentSize - 1];
				Array.Copy(this.items[num], num3, this.items[num], num3 + 1, this.segmentSize - num3 - 1);
				for (int i = num + 1; i < num2; i++)
				{
					T t2 = this.items[i][this.segmentSize - 1];
					Array.Copy(this.items[i], 0, this.items[i], 1, this.segmentSize - 1);
					this.items[i][0] = t;
					t = t2;
				}
				Array.Copy(this.items[num2], 0, this.items[num2], 1, num4);
				this.items[num2][0] = t;
			}

			private void RemoveRoomForElement(int index)
			{
				int num = index >> this.segmentShift;
				int num2 = this.count - 1 >> this.segmentShift;
				int num3 = index & this.offsetMask;
				int num4 = this.count - 1 & this.offsetMask;
				if (num == num2)
				{
					Array.Copy(this.items[num], num3 + 1, this.items[num], num3, num4 - num3);
					return;
				}
				Array.Copy(this.items[num], num3 + 1, this.items[num], num3, this.segmentSize - num3 - 1);
				for (int i = num + 1; i < num2; i++)
				{
					this.items[i - 1][this.segmentSize - 1] = this.items[i][0];
					Array.Copy(this.items[i], 1, this.items[i], 0, this.segmentSize - 1);
				}
				this.items[num2 - 1][this.segmentSize - 1] = this.items[num2][0];
				Array.Copy(this.items[num2], 1, this.items[num2], 0, num4);
			}

			private void EnsureCapacity(int minCapacity)
			{
				if (this.capacity < this.segmentSize)
				{
					if (this.items == null)
					{
						this.items = new T[minCapacity + this.segmentSize - 1 >> this.segmentShift][];
					}
					int i = this.segmentSize;
					if (minCapacity < this.segmentSize)
					{
						for (i = ((this.capacity == 0) ? 2 : (this.capacity * 2)); i < minCapacity; i *= 2)
						{
						}
						i = Math.Min(i, this.segmentSize);
					}
					T[] array = new T[i];
					if (this.count > 0)
					{
						Array.Copy(this.items[0], 0, array, 0, this.count);
					}
					this.items[0] = array;
					this.capacity = array.Length;
				}
				if (this.capacity < minCapacity)
				{
					int num = this.capacity >> this.segmentShift;
					int num2 = minCapacity + this.segmentSize - 1 >> this.segmentShift;
					if (num2 > this.items.Length)
					{
						int j;
						for (j = this.items.Length * 2; j < num2; j *= 2)
						{
						}
						T[][] destinationArray = new T[j][];
						Array.Copy(this.items, 0, destinationArray, 0, num);
						this.items = destinationArray;
					}
					for (int k = num; k < num2; k++)
					{
						this.items[k] = new T[this.segmentSize];
						this.capacity += this.segmentSize;
					}
				}
			}

			private void SwapIfGreaterWithItems(IComparer<T> comparer, int a, int b)
			{
				if (a != b && comparer.Compare(this.items[a >> this.segmentShift][a & this.offsetMask], this.items[b >> this.segmentShift][b & this.offsetMask]) > 0)
				{
					T t = this.items[a >> this.segmentShift][a & this.offsetMask];
					this.items[a >> this.segmentShift][a & this.offsetMask] = this.items[b >> this.segmentShift][b & this.offsetMask];
					this.items[b >> this.segmentShift][b & this.offsetMask] = t;
				}
			}

			private void QuickSort(int left, int right, IComparer<T> comparer)
			{
				do
				{
					int num = left;
					int num2 = right;
					int num3 = num + (num2 - num >> 1);
					this.SwapIfGreaterWithItems(comparer, num, num3);
					this.SwapIfGreaterWithItems(comparer, num, num2);
					this.SwapIfGreaterWithItems(comparer, num3, num2);
					T t = this.items[num3 >> this.segmentShift][num3 & this.offsetMask];
					for (;;)
					{
						if (comparer.Compare(this.items[num >> this.segmentShift][num & this.offsetMask], t) >= 0)
						{
							while (comparer.Compare(t, this.items[num2 >> this.segmentShift][num2 & this.offsetMask]) < 0)
							{
								num2--;
							}
							if (num > num2)
							{
								break;
							}
							if (num < num2)
							{
								T t2 = this.items[num >> this.segmentShift][num & this.offsetMask];
								this.items[num >> this.segmentShift][num & this.offsetMask] = this.items[num2 >> this.segmentShift][num2 & this.offsetMask];
								this.items[num2 >> this.segmentShift][num2 & this.offsetMask] = t2;
							}
							num++;
							num2--;
							if (num > num2)
							{
								break;
							}
						}
						else
						{
							num++;
						}
					}
					if (num2 - left <= right - num)
					{
						if (left < num2)
						{
							this.QuickSort(left, num2, comparer);
						}
						left = num;
					}
					else
					{
						if (num < right)
						{
							this.QuickSort(num, right, comparer);
						}
						right = num2;
					}
				}
				while (left < right);
			}

			private readonly int segmentSize;

			private readonly int segmentShift;

			private readonly int offsetMask;

			private int capacity;

			private int count;

			private T[][] items;

			public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
			{
				internal Enumerator(GlobCountSet.SegmentedList<T> list)
				{
					this.list = list;
					this.index = -1;
				}

				public void Dispose()
				{
				}

				public bool MoveNext()
				{
					if (this.index < this.list.count - 1)
					{
						this.index++;
						return true;
					}
					this.index = -1;
					return false;
				}

				public T Current
				{
					get
					{
						return this.list[this.index];
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return this.Current;
					}
				}

				void IEnumerator.Reset()
				{
					this.index = -1;
				}

				private GlobCountSet.SegmentedList<T> list;

				private int index;
			}
		}

		private class GlobCountRangeLowBoundComparer : IComparer<GlobCountRange>
		{
			public int Compare(GlobCountRange x, GlobCountRange y)
			{
				return x.LowBound.CompareTo(y.LowBound);
			}

			public static readonly GlobCountSet.GlobCountRangeLowBoundComparer Instance = new GlobCountSet.GlobCountRangeLowBoundComparer();
		}

		private class GlobCountRangeHighBoundComparer : IComparer<GlobCountRange>
		{
			public int Compare(GlobCountRange x, GlobCountRange y)
			{
				return x.HighBound.CompareTo(y.HighBound);
			}

			public static readonly GlobCountSet.GlobCountRangeHighBoundComparer Instance = new GlobCountSet.GlobCountRangeHighBoundComparer();
		}
	}
}
