using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class ItemId : BasicItemId
	{
		public uint Size { get; set; }

		public string ParentFolder { get; set; }

		public string PrimaryItemId { get; set; }

		public string UniqueHash { get; set; }

		public string InternetMessageId { get; set; }

		public string Subject { get; set; }

		public string Sender { get; set; }

		public string SenderSmtpAddress { get; set; }

		public DateTime SentTime { get; set; }

		public DateTime ReceivedTime { get; set; }

		public string BodyPreview { get; set; }

		public string Importance { get; set; }

		public bool IsRead { get; set; }

		public bool HasAttachment { get; set; }

		public string ToRecipients { get; set; }

		public string CcRecipients { get; set; }

		public string BccRecipients { get; set; }

		public int DocumentId { get; set; }

		public string IdMarkerDocumentId
		{
			get
			{
				return string.Format("{0}{1}{2}", base.Id, "4887312c-8b40-4fec-a252-f2749065c0e5", this.DocumentId);
			}
		}

		public string ToGroupExpansionRecipients { get; set; }

		public string CcGroupExpansionRecipients { get; set; }

		public string BccGroupExpansionRecipients { get; set; }

		public DistributionGroupExpansionError? DGGroupExpansionError { get; set; }

		public bool NeedsDGExpansion { get; set; }

		public string ReportingPath
		{
			get
			{
				return base.SourceId + this.ParentFolder;
			}
		}

		public bool IsDuplicate
		{
			get
			{
				return !string.IsNullOrEmpty(this.PrimaryItemId);
			}
		}

		public virtual void WriteToStream(Stream fs)
		{
			ItemId.WriteString(base.Id, fs, Encoding.ASCII);
			ItemId.WriteString(this.ParentFolder, fs, Encoding.Unicode);
			ItemId.WriteString(this.PrimaryItemId, fs, Encoding.ASCII);
			ItemId.WriteString(this.Subject, fs, Encoding.Unicode);
			ItemId.WriteString(this.Sender, fs, Encoding.Unicode);
			fs.Write(BitConverter.GetBytes(this.SentTime.Ticks), 0, 8);
			fs.Write(BitConverter.GetBytes(this.ReceivedTime.Ticks), 0, 8);
			ItemId.WriteString(this.BodyPreview, fs, Encoding.Unicode);
			ItemId.WriteString(this.Importance, fs, Encoding.Unicode);
			fs.Write(BitConverter.GetBytes(this.IsRead), 0, 1);
			fs.Write(BitConverter.GetBytes(this.HasAttachment), 0, 1);
			ItemId.WriteString(this.ToRecipients, fs, Encoding.Unicode);
			ItemId.WriteString(this.CcRecipients, fs, Encoding.Unicode);
			ItemId.WriteString(this.BccRecipients, fs, Encoding.Unicode);
			ItemId.WriteString(this.ToGroupExpansionRecipients, fs, Encoding.Unicode);
			ItemId.WriteString(this.CcGroupExpansionRecipients, fs, Encoding.Unicode);
			ItemId.WriteString(this.BccGroupExpansionRecipients, fs, Encoding.Unicode);
			ItemId.WriteString((this.DGGroupExpansionError == null) ? null : this.DGGroupExpansionError.ToString(), fs, Encoding.Unicode);
			byte[] bytes = BitConverter.GetBytes(this.Size);
			fs.Write(bytes, 0, 4);
			if (!this.IsDuplicate)
			{
				ItemId.WriteString(this.UniqueHash, fs, Encoding.Unicode);
				string value = string.Empty;
				if (!string.IsNullOrEmpty(this.UniqueHash))
				{
					UniqueItemHash uniqueItemHash = UniqueItemHash.Parse(this.UniqueHash);
					value = uniqueItemHash.InternetMessageId;
				}
				ItemId.WriteString(value, fs, Encoding.Unicode);
			}
			byte[] bytes2 = BitConverter.GetBytes(this.DocumentId);
			fs.Write(bytes2, 0, 4);
			fs.Write(new byte[]
			{
				this.NeedsDGExpansion ? 1 : 0
			}, 0, 1);
			ItemId.WriteString(this.SenderSmtpAddress, fs, Encoding.Unicode);
		}

		public virtual void ReadFromStream(Stream fs, string sourceId)
		{
			base.Id = ItemId.ReadString(fs, sourceId, Encoding.ASCII);
			this.ParentFolder = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			this.PrimaryItemId = ItemId.ReadString(fs, sourceId, Encoding.ASCII);
			this.Subject = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			this.Sender = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			byte[] array = new byte[8];
			ItemId.SafeRead(fs, array, 0, 8, sourceId);
			this.SentTime = new DateTime(BitConverter.ToInt64(array, 0), DateTimeKind.Utc);
			ItemId.SafeRead(fs, array, 0, 8, sourceId);
			this.ReceivedTime = new DateTime(BitConverter.ToInt64(array, 0), DateTimeKind.Utc);
			this.BodyPreview = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			this.Importance = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			ItemId.SafeRead(fs, array, 0, 1, sourceId);
			this.IsRead = BitConverter.ToBoolean(array, 0);
			ItemId.SafeRead(fs, array, 0, 1, sourceId);
			this.HasAttachment = BitConverter.ToBoolean(array, 0);
			this.ToRecipients = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			this.CcRecipients = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			this.BccRecipients = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			this.ToGroupExpansionRecipients = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			this.CcGroupExpansionRecipients = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			this.BccGroupExpansionRecipients = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			string value = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			this.DGGroupExpansionError = null;
			if (!string.IsNullOrEmpty(value))
			{
				this.DGGroupExpansionError = new DistributionGroupExpansionError?((DistributionGroupExpansionError)Enum.Parse(typeof(DistributionGroupExpansionError), value, true));
			}
			base.SourceId = sourceId;
			byte[] array2 = new byte[4];
			ItemId.SafeRead(fs, array2, 0, array2.Length, sourceId);
			this.Size = BitConverter.ToUInt32(array2, 0);
			if (!this.IsDuplicate)
			{
				this.UniqueHash = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
				if (fs.Position + 1L < fs.Length)
				{
					this.InternetMessageId = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
				}
			}
			if (fs.Position + 4L <= fs.Length)
			{
				byte[] array3 = new byte[4];
				ItemId.SafeRead(fs, array3, 0, array3.Length, sourceId);
				this.DocumentId = BitConverter.ToInt32(array3, 0);
			}
			if (fs.Position + 1L <= fs.Length)
			{
				byte[] array4 = new byte[1];
				ItemId.SafeRead(fs, array4, 0, array4.Length, sourceId);
				this.NeedsDGExpansion = (array4[0] != 0);
			}
			if (fs.Position + 1L < fs.Length)
			{
				this.SenderSmtpAddress = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			}
		}

		protected static void WriteString(string value, Stream stream, Encoding encoding)
		{
			byte[] array = string.IsNullOrEmpty(value) ? new byte[0] : encoding.GetBytes(value);
			stream.Write(BitConverter.GetBytes(array.Length), 0, 4);
			stream.Write(array, 0, array.Length);
		}

		protected static string ReadString(Stream stream, string sourceId, Encoding encoding)
		{
			byte[] array = new byte[4];
			stream.Read(array, 0, array.Length);
			int num = BitConverter.ToInt32(array, 0);
			if (num > 0)
			{
				byte[] array2 = new byte[num];
				ItemId.SafeRead(stream, array2, 0, num, sourceId);
				return encoding.GetString(array2);
			}
			return null;
		}

		protected static void SafeRead(Stream stream, byte[] buffer, int offset, int count, string sourceId)
		{
			int num = stream.Read(buffer, offset, count);
			if (num != count)
			{
				throw new ExportException(ExportErrorType.ItemIdListCorrupted, string.Format(CultureInfo.CurrentCulture, "Item id list for source '{0}' is corrupted.", new object[]
				{
					sourceId
				}));
			}
		}

		public const string DocumentIdMarker = "4887312c-8b40-4fec-a252-f2749065c0e5";
	}
}
