using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxSyncWatermark : ISyncWatermark, ICustomSerializableBuilder, ICustomSerializable, IComparable, ICloneable, ICustomClonable
	{
		public MailboxSyncWatermark()
		{
		}

		protected MailboxSyncWatermark(int changeNumber)
		{
			this.changeNumber = changeNumber;
		}

		public int ChangeNumber
		{
			get
			{
				return this.changeNumber;
			}
			set
			{
				this.changeNumber = value;
			}
		}

		public int RawChangeNumber
		{
			get
			{
				return this.changeNumber & int.MaxValue;
			}
		}

		public byte[] IcsState
		{
			get
			{
				return this.icsState;
			}
			set
			{
				this.icsState = value;
			}
		}

		public bool IsNew
		{
			get
			{
				return 0 == this.changeNumber;
			}
		}

		public ushort TypeId
		{
			get
			{
				return MailboxSyncWatermark.typeId;
			}
			set
			{
				MailboxSyncWatermark.typeId = value;
			}
		}

		public static MailboxSyncWatermark Create()
		{
			return new MailboxSyncWatermark();
		}

		public static MailboxSyncWatermark CreateForSingleItem()
		{
			return new MailboxSyncWatermark();
		}

		public static MailboxSyncWatermark CreateWithChangeNumber(int changeNumber)
		{
			return new MailboxSyncWatermark(changeNumber);
		}

		public virtual ICustomSerializable BuildObject()
		{
			return new MailboxSyncWatermark();
		}

		public virtual object Clone()
		{
			MailboxSyncWatermark mailboxSyncWatermark = MailboxSyncWatermark.CreateWithChangeNumber(this.ChangeNumber);
			if (this.icsState != null)
			{
				mailboxSyncWatermark.IcsState = (byte[])this.icsState.Clone();
			}
			return mailboxSyncWatermark;
		}

		public virtual object CustomClone()
		{
			return MailboxSyncWatermark.CreateWithChangeNumber(this.ChangeNumber);
		}

		public int CompareTo(object thatObject)
		{
			MailboxSyncWatermark mailboxSyncWatermark = (MailboxSyncWatermark)thatObject;
			int rawChangeNumber = this.RawChangeNumber;
			int rawChangeNumber2 = mailboxSyncWatermark.RawChangeNumber;
			if (rawChangeNumber < rawChangeNumber2)
			{
				return -1;
			}
			if (rawChangeNumber > rawChangeNumber2)
			{
				return 1;
			}
			return 0;
		}

		public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			ByteArrayData byteArrayInstance = componentDataPool.GetByteArrayInstance();
			byteArrayInstance.DeserializeData(reader, componentDataPool);
			this.icsState = byteArrayInstance.Data;
			this.changeNumber = reader.ReadInt32();
		}

		public override bool Equals(object thatObject)
		{
			if (thatObject == null)
			{
				return false;
			}
			MailboxSyncWatermark mailboxSyncWatermark = thatObject as MailboxSyncWatermark;
			return mailboxSyncWatermark != null && this.ChangeNumber == mailboxSyncWatermark.ChangeNumber;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException("MailboxSyncWatermark.GetHashCode()");
		}

		public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			componentDataPool.GetByteArrayInstance().Bind(this.icsState).SerializeData(writer, componentDataPool);
			writer.Write(this.changeNumber);
		}

		public void UpdateWithChangeNumber(int changeNumber, bool read)
		{
			this.changeNumber = changeNumber;
			if (read)
			{
				this.changeNumber |= int.MinValue;
				return;
			}
			this.changeNumber &= int.MaxValue;
		}

		public override string ToString()
		{
			return string.Format("CN: {0}", this.changeNumber);
		}

		private static ushort typeId;

		private int changeNumber;

		private byte[] icsState;
	}
}
