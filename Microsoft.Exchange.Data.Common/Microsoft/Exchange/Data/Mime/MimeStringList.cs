using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	internal struct MimeStringList
	{
		public MimeStringList(byte[] data)
		{
			this.first = new MimeString(data, 0, data.Length);
			this.overflow = null;
		}

		public MimeStringList(byte[] data, int offset, int count)
		{
			this.first = new MimeString(data, offset, count);
			this.overflow = null;
		}

		public MimeStringList(MimeString str)
		{
			this.first = str;
			this.overflow = null;
		}

		public int Count
		{
			get
			{
				if (this.overflow != null)
				{
					return this.overflow[0].HeaderCount;
				}
				if (this.first.Data == null)
				{
					return 0;
				}
				return 1;
			}
		}

		public int Length
		{
			get
			{
				if (this.overflow == null)
				{
					return this.first.Length;
				}
				return this.overflow[0].HeaderTotalLength;
			}
		}

		public MimeString this[int index]
		{
			get
			{
				if (index == 0)
				{
					return this.first;
				}
				if (index < 4096)
				{
					return this.overflow[index].Str;
				}
				return this.overflow[4096 + index / 4096 - 1].Secondary[index % 4096];
			}
			set
			{
				if (this.overflow != null)
				{
					MimeStringList.ListEntry[] array = this.overflow;
					int num = 0;
					array[num].HeaderTotalLength = array[num].HeaderTotalLength + (value.Length - this[index].Length);
				}
				if (index == 0)
				{
					this.first = value;
					return;
				}
				if (index < 4096)
				{
					this.overflow[index].Str = value;
					return;
				}
				this.overflow[4096 + index / 4096 - 1].Secondary[index % 4096] = value;
			}
		}

		public void Append(MimeString value)
		{
			if (value.Length == 0)
			{
				return;
			}
			int count = this.Count;
			if (count == 0)
			{
				this.first = value;
				if (this.overflow != null)
				{
					MimeStringList.ListEntry[] array = this.overflow;
					int num = 0;
					array[num].HeaderCount = array[num].HeaderCount + 1;
					MimeStringList.ListEntry[] array2 = this.overflow;
					int num2 = 0;
					array2[num2].HeaderTotalLength = array2[num2].HeaderTotalLength + value.Length;
					return;
				}
			}
			else
			{
				if (count < 4096)
				{
					if (this.overflow == null)
					{
						this.overflow = new MimeStringList.ListEntry[8];
						this.overflow[0].HeaderCount = 1;
						this.overflow[0].HeaderTotalLength = this.first.Length;
					}
					else if (count == this.overflow.Length)
					{
						int num3 = count * 2;
						if (num3 >= 4096)
						{
							num3 = 4128;
						}
						MimeStringList.ListEntry[] destinationArray = new MimeStringList.ListEntry[num3];
						Array.Copy(this.overflow, 0, destinationArray, 0, this.overflow.Length);
						this.overflow = destinationArray;
					}
					this.overflow[count].Str = value;
					MimeStringList.ListEntry[] array3 = this.overflow;
					int num4 = 0;
					array3[num4].HeaderCount = array3[num4].HeaderCount + 1;
					MimeStringList.ListEntry[] array4 = this.overflow;
					int num5 = 0;
					array4[num5].HeaderTotalLength = array4[num5].HeaderTotalLength + value.Length;
					return;
				}
				int num6 = 4096 + count / 4096 - 1;
				int num7 = count % 4096;
				if (num6 >= this.overflow.Length)
				{
					throw new MimeException("MIME is too complex (header value is too long)");
				}
				if (this.overflow[num6].Secondary == null)
				{
					this.overflow[num6].Secondary = new MimeString[4096];
				}
				this.overflow[num6].Secondary[num7] = value;
				MimeStringList.ListEntry[] array5 = this.overflow;
				int num8 = 0;
				array5[num8].HeaderCount = array5[num8].HeaderCount + 1;
				MimeStringList.ListEntry[] array6 = this.overflow;
				int num9 = 0;
				array6[num9].HeaderTotalLength = array6[num9].HeaderTotalLength + value.Length;
			}
		}

		public void TakeOver(ref MimeStringList list)
		{
			this.TakeOver(ref list, 4026531840U);
		}

		public void TakeOver(ref MimeStringList list, uint mask)
		{
			if (mask == 4026531840U)
			{
				this.first = list.first;
				this.overflow = list.overflow;
				list.first = default(MimeString);
				list.overflow = null;
				return;
			}
			this.Reset();
			this.TakeOverAppend(ref list, mask);
		}

		public void TakeOverAppend(ref MimeStringList list)
		{
			this.TakeOverAppend(ref list, 4026531840U);
		}

		public void TakeOverAppend(ref MimeStringList list, uint mask)
		{
			if (this.Count == 0 && mask == 4026531840U)
			{
				this.TakeOver(ref list, mask);
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				MimeString refLine = list[i];
				if (mask == 4026531840U || (refLine.Mask & mask) != 0U)
				{
					this.AppendFragment(refLine);
				}
			}
			list.Reset();
		}

		public void AppendFragment(MimeString refLine)
		{
			int num = this.Count;
			if (num != 0)
			{
				num--;
				if (num == 0)
				{
					if (this.first.MergeIfAdjacent(refLine))
					{
						if (this.overflow != null)
						{
							MimeStringList.ListEntry[] array = this.overflow;
							int num2 = 0;
							array[num2].HeaderTotalLength = array[num2].HeaderTotalLength + refLine.Length;
						}
						return;
					}
				}
				else if (num < 4096)
				{
					if (this.overflow[num].StrMergeIfAdjacent(refLine))
					{
						MimeStringList.ListEntry[] array2 = this.overflow;
						int num3 = 0;
						array2[num3].HeaderTotalLength = array2[num3].HeaderTotalLength + refLine.Length;
						return;
					}
				}
				else if (this.overflow[4096 + num / 4096 - 1].Secondary[num % 4096].MergeIfAdjacent(refLine))
				{
					MimeStringList.ListEntry[] array3 = this.overflow;
					int num4 = 0;
					array3[num4].HeaderTotalLength = array3[num4].HeaderTotalLength + refLine.Length;
					return;
				}
			}
			this.Append(refLine);
		}

		public int GetLength(uint mask)
		{
			if (mask == 4026531840U)
			{
				return this.Length;
			}
			int num = 0;
			for (int i = 0; i < this.Count; i++)
			{
				MimeString mimeString = this[i];
				if ((mimeString.Mask & mask) != 0U)
				{
					num += mimeString.Length;
				}
			}
			return num;
		}

		public byte[] GetSz()
		{
			int count = this.Count;
			if (count <= 1)
			{
				return this.first.GetSz();
			}
			byte[] array = new byte[this.Length];
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				num += this[i].CopyTo(array, num);
			}
			return array;
		}

		public byte[] GetSz(uint mask)
		{
			if (mask == 4026531840U)
			{
				return this.GetSz();
			}
			int count = this.Count;
			if (count == 0)
			{
				return null;
			}
			if (count != 1)
			{
				byte[] array = new byte[this.GetLength(mask)];
				int num = 0;
				for (int i = 0; i < count; i++)
				{
					MimeString mimeString = this[i];
					if ((mimeString.Mask & mask) != 0U)
					{
						num += mimeString.CopyTo(array, num);
					}
				}
				return array;
			}
			if ((this.first.Mask & mask) == 0U)
			{
				return MimeString.EmptyByteArray;
			}
			return this.first.GetSz();
		}

		public void WriteTo(Stream stream)
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].WriteTo(stream);
			}
		}

		public override string ToString()
		{
			int count = this.Count;
			if (count <= 1)
			{
				return this.first.ToString();
			}
			StringBuilder stringBuilder = ScratchPad.GetStringBuilder(this.Length);
			for (int i = 0; i < count; i++)
			{
				MimeString mimeString = this[i];
				string value = ByteString.BytesToString(mimeString.Data, mimeString.Offset, mimeString.Length, true);
				stringBuilder.Append(value);
			}
			ScratchPad.ReleaseStringBuilder();
			return stringBuilder.ToString();
		}

		public MimeStringList Clone()
		{
			MimeStringList result = default(MimeStringList);
			result.first = this.first;
			if (this.overflow != null && this.overflow[0].HeaderCount > 1)
			{
				result.overflow = new MimeStringList.ListEntry[this.overflow.Length];
				int length = Math.Min(this.Count, 4096);
				Array.Copy(this.overflow, 0, result.overflow, 0, length);
				if (this.Count > 4096)
				{
					int num = 4096;
					int i = 4096;
					while (i < this.Count)
					{
						result.overflow[num].Secondary = new MimeString[4096];
						length = Math.Min(this.Count - i, 4096);
						Array.Copy(this.overflow[num].Secondary, 0, result.overflow[num].Secondary, 0, length);
						i += 4096;
						num++;
					}
				}
			}
			return result;
		}

		public void Reset()
		{
			this.first = default(MimeString);
			if (this.overflow != null)
			{
				this.overflow[0].Reset();
			}
		}

		public const uint MaskAny = 4026531840U;

		public const uint MaskRawOnly = 268435456U;

		public const uint MaskJis = 536870912U;

		private MimeString first;

		private MimeStringList.ListEntry[] overflow;

		public static readonly MimeStringList Empty = default(MimeStringList);

		private struct ListEntry
		{
			public int HeaderCount
			{
				get
				{
					return this.int1;
				}
				set
				{
					this.int1 = value;
				}
			}

			public int HeaderTotalLength
			{
				get
				{
					return this.int2;
				}
				set
				{
					this.int2 = value;
				}
			}

			public MimeString Str
			{
				get
				{
					return new MimeString((byte[])this.obj, this.int1, (uint)this.int2);
				}
				set
				{
					this.obj = value.Data;
					this.int1 = value.Offset;
					this.int2 = (int)value.LengthAndMask;
				}
			}

			public MimeString[] Secondary
			{
				get
				{
					return (MimeString[])this.obj;
				}
				set
				{
					this.obj = value;
					this.int1 = 0;
					this.int2 = 0;
				}
			}

			public void Reset()
			{
				this.obj = null;
				this.int1 = 0;
				this.int2 = 0;
			}

			public bool StrMergeIfAdjacent(MimeString refLine)
			{
				MimeString str = this.Str;
				if (!str.MergeIfAdjacent(refLine))
				{
					return false;
				}
				this.Str = str;
				return true;
			}

			private object obj;

			private int int1;

			private int int2;
		}
	}
}
