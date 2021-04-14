using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class FirstTimeSyncWatermark : ISyncWatermark, ICustomSerializableBuilder, ICustomSerializable, IComparable, ICloneable, ICustomClonable
	{
		public ExDateTime ReceivedDateUtc { get; set; }

		public int ChangeNumber { get; set; }

		public int RawChangeNumber
		{
			get
			{
				return this.ChangeNumber & int.MaxValue;
			}
		}

		public byte[] IcsState { get; set; }

		public bool IsNew
		{
			get
			{
				return (DateTime)this.ReceivedDateUtc == DateTime.MinValue;
			}
		}

		public ushort TypeId
		{
			get
			{
				return FirstTimeSyncWatermark.typeId;
			}
			set
			{
				FirstTimeSyncWatermark.typeId = value;
			}
		}

		public static FirstTimeSyncWatermark CreateNew()
		{
			return FirstTimeSyncWatermark.Create(ExDateTime.MinValue, -1);
		}

		public static FirstTimeSyncWatermark Create(ExDateTime receivedDate, int changeNumber)
		{
			return new FirstTimeSyncWatermark
			{
				ReceivedDateUtc = receivedDate,
				ChangeNumber = changeNumber
			};
		}

		public static FirstTimeSyncWatermark Create(ExDateTime receivedDate, int changeNumber, bool read)
		{
			FirstTimeSyncWatermark firstTimeSyncWatermark = new FirstTimeSyncWatermark();
			firstTimeSyncWatermark.Update(changeNumber, read, receivedDate);
			return firstTimeSyncWatermark;
		}

		public ICustomSerializable BuildObject()
		{
			return new FirstTimeSyncWatermark();
		}

		public object Clone()
		{
			FirstTimeSyncWatermark firstTimeSyncWatermark = (FirstTimeSyncWatermark)this.CustomClone();
			if (this.IcsState != null)
			{
				firstTimeSyncWatermark.IcsState = (byte[])this.IcsState.Clone();
			}
			return firstTimeSyncWatermark;
		}

		public object CustomClone()
		{
			return new FirstTimeSyncWatermark
			{
				ChangeNumber = this.ChangeNumber,
				ReceivedDateUtc = this.ReceivedDateUtc
			};
		}

		public int CompareTo(object thatObject)
		{
			FirstTimeSyncWatermark firstTimeSyncWatermark = thatObject as FirstTimeSyncWatermark;
			return ExDateTime.Compare(this.ReceivedDateUtc, (firstTimeSyncWatermark == null) ? ExDateTime.MinValue : firstTimeSyncWatermark.ReceivedDateUtc);
		}

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			DateTimeData dateTimeDataInstance = componentDataPool.GetDateTimeDataInstance();
			dateTimeDataInstance.DeserializeData(reader, componentDataPool);
			this.ReceivedDateUtc = dateTimeDataInstance.Data;
			ByteArrayData byteArrayInstance = componentDataPool.GetByteArrayInstance();
			byteArrayInstance.DeserializeData(reader, componentDataPool);
			this.IcsState = byteArrayInstance.Data;
			this.ChangeNumber = reader.ReadInt32();
		}

		public override bool Equals(object thatObject)
		{
			FirstTimeSyncWatermark firstTimeSyncWatermark = thatObject as FirstTimeSyncWatermark;
			return firstTimeSyncWatermark != null && this.ReceivedDateUtc == firstTimeSyncWatermark.ReceivedDateUtc;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException("FirstTimeSyncWatermark.GetHashCode()");
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			componentDataPool.GetDateTimeDataInstance().Bind(this.ReceivedDateUtc).SerializeData(writer, componentDataPool);
			componentDataPool.GetByteArrayInstance().Bind(this.IcsState).SerializeData(writer, componentDataPool);
			writer.Write(this.ChangeNumber);
		}

		public void Update(int changeNumber, bool read, ExDateTime receivedDate)
		{
			this.ChangeNumber = changeNumber;
			if (read)
			{
				this.ChangeNumber |= int.MinValue;
			}
			else
			{
				this.ChangeNumber &= int.MaxValue;
			}
			this.ReceivedDateUtc = receivedDate;
		}

		private static ushort typeId;
	}
}
