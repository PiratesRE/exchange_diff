using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class OccurrenceStoreObjectId : StoreObjectId, IEquatable<OccurrenceStoreObjectId>, IComparable<OccurrenceStoreObjectId>
	{
		public OccurrenceStoreObjectId(byte[] entryId, ExDateTime date) : base(entryId, StoreObjectType.CalendarItemOccurrence)
		{
			this.occurrenceDate = date.Date;
		}

		public OccurrenceStoreObjectId(byte[] byteArray, int startingIndex) : base(byteArray, startingIndex + 8 + 1)
		{
			if (byteArray == null)
			{
				throw new ArgumentNullException("byteArray", ServerStrings.ExInvalidIdFormat);
			}
			if (byteArray[startingIndex] != 8)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidIdFormat);
			}
			long num = 0L;
			for (int i = 0; i < 8; i++)
			{
				num <<= 8;
				num |= (long)((ulong)byteArray[startingIndex + 1 + i]);
			}
			this.occurrenceDate = ExDateTime.FromBinary(num);
		}

		public OccurrenceStoreObjectId(byte dateSize, byte[] dateBytes, byte[] storeObjectIdByteArray) : base(storeObjectIdByteArray, 0)
		{
			if (dateBytes == null)
			{
				throw new ArgumentNullException("dateBytes", ServerStrings.ExInvalidIdFormat);
			}
			if (storeObjectIdByteArray == null)
			{
				throw new ArgumentNullException("storeObjectIdByteArray", ServerStrings.ExInvalidIdFormat);
			}
			if (dateSize != 8)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidIdFormat);
			}
			long num = 0L;
			for (int i = 0; i < 8; i++)
			{
				num <<= 8;
				num |= (long)((ulong)dateBytes[i]);
			}
			this.occurrenceDate = ExDateTime.FromBinary(num);
		}

		public override byte[] GetBytes()
		{
			byte[] bytes = base.GetBytes();
			byte[] array = new byte[bytes.Length + 8 + 1];
			array[0] = 8;
			bytes.CopyTo(array, 9);
			long num = this.occurrenceDate.ToBinary();
			long num2 = 8L;
			for (;;)
			{
				long num3 = num2;
				num2 = num3 - 1L;
				if (num3 <= 0L)
				{
					break;
				}
				array[(int)(checked((IntPtr)(unchecked(1L + num2))))] = (byte)(num & 255L);
				num >>= 8;
			}
			return array;
		}

		public override bool Equals(object id)
		{
			OccurrenceStoreObjectId id2 = id as OccurrenceStoreObjectId;
			return this.Equals(id2);
		}

		public override bool Equals(StoreObjectId id)
		{
			OccurrenceStoreObjectId id2 = id as OccurrenceStoreObjectId;
			return this.Equals(id2);
		}

		public bool Equals(OccurrenceStoreObjectId id)
		{
			return id != null && base.Equals(id) && this.occurrenceDate.Equals(id.occurrenceDate);
		}

		public override int CompareTo(object o)
		{
			if (o == null)
			{
				return 1;
			}
			if (!base.GetType().Equals(o.GetType()))
			{
				throw new ArgumentException();
			}
			OccurrenceStoreObjectId o2 = (OccurrenceStoreObjectId)o;
			return this.CompareTo(o2);
		}

		public int CompareTo(OccurrenceStoreObjectId o)
		{
			if (o == null)
			{
				return 1;
			}
			int num = base.CompareTo(o);
			if (num != 0)
			{
				return num;
			}
			return this.occurrenceDate.CompareTo(o.occurrenceDate);
		}

		public override StoreObjectId Clone()
		{
			return new OccurrenceStoreObjectId(this.EntryId, this.occurrenceDate);
		}

		public ExDateTime OccurrenceId
		{
			get
			{
				return this.occurrenceDate;
			}
		}

		internal StoreObjectId GetMasterStoreObjectId()
		{
			return StoreObjectId.FromProviderSpecificId(base.ProviderLevelItemId, StoreObjectType.CalendarItem);
		}

		public override void UpdateItemType(StoreObjectType newItemType)
		{
			if (newItemType != StoreObjectType.CalendarItemOccurrence)
			{
				throw new EnumArgumentException("OccurrenceStoreObjectId should always have type of CalendarItemOccurrence", "newItemType");
			}
		}

		public override int GetByteArrayLength()
		{
			return 9 + base.GetByteArrayLength();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.occurrenceDate.GetHashCode();
		}

		protected override void WriteBytes(BinaryWriter writer)
		{
			writer.Write(8);
			long num = this.occurrenceDate.ToBinary();
			long num2 = 8L;
			for (;;)
			{
				long num3 = num2;
				num2 = num3 - 1L;
				if (num3 <= 0L)
				{
					break;
				}
				writer.Write((byte)(num >> (int)(8L * num2) & 255L));
			}
			base.WriteBytes(writer);
		}

		private readonly ExDateTime occurrenceDate;
	}
}
