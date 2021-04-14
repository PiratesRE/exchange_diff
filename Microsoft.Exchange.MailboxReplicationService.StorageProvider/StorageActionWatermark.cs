using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class StorageActionWatermark : XMLSerializableBase, IActionWatermark, IComparable, IComparable<StorageActionWatermark>
	{
		public StorageActionWatermark()
		{
		}

		internal StorageActionWatermark(Activity activity)
		{
			ArgumentValidator.ThrowIfNull("activity", activity);
			this.TimeStamp = (DateTime)activity.TimeStamp;
			this.SequenceNumber = activity.SequenceNumber;
		}

		[XmlElement(ElementName = "TimeStamp")]
		public DateTime TimeStamp { get; set; }

		[XmlElement(ElementName = "SequenceNumber")]
		public long SequenceNumber { get; set; }

		public static StorageActionWatermark Deserialize(string data)
		{
			return XMLSerializableBase.Deserialize<StorageActionWatermark>(data, true);
		}

		string IActionWatermark.SerializeToString()
		{
			return base.Serialize(false);
		}

		int IComparable.CompareTo(object obj)
		{
			StorageActionWatermark storageActionWatermark = obj as StorageActionWatermark;
			if (storageActionWatermark == null)
			{
				throw new ArgumentException();
			}
			return ((IComparable<StorageActionWatermark>)this).CompareTo(storageActionWatermark);
		}

		int IComparable<StorageActionWatermark>.CompareTo(StorageActionWatermark other)
		{
			int num = this.TimeStamp.CompareTo(other.TimeStamp);
			if (num == 0)
			{
				num = this.SequenceNumber.CompareTo(other.SequenceNumber);
			}
			return num;
		}
	}
}
