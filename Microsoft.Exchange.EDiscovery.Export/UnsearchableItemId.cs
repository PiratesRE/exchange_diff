using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class UnsearchableItemId : ItemId
	{
		public string ErrorCode { get; set; }

		public string ErrorDescription { get; set; }

		public DateTime LastAttemptTime { get; set; }

		public string AdditionalInformation { get; set; }

		public static void WriteDummyToStream(Stream fs)
		{
			ItemId.WriteString(string.Empty, fs, Encoding.ASCII);
			ItemId.WriteString(string.Empty, fs, Encoding.Unicode);
			fs.Write(BitConverter.GetBytes(DateTime.MinValue.Ticks), 0, 8);
			ItemId.WriteString(string.Empty, fs, Encoding.Unicode);
		}

		public override void WriteToStream(Stream fs)
		{
			base.WriteToStream(fs);
			ItemId.WriteString(this.ErrorCode, fs, Encoding.ASCII);
			ItemId.WriteString(this.ErrorDescription, fs, Encoding.Unicode);
			fs.Write(BitConverter.GetBytes(this.LastAttemptTime.Ticks), 0, 8);
			ItemId.WriteString(this.AdditionalInformation, fs, Encoding.Unicode);
		}

		public override void ReadFromStream(Stream fs, string sourceId)
		{
			base.ReadFromStream(fs, sourceId);
			this.ErrorCode = ItemId.ReadString(fs, sourceId, Encoding.ASCII);
			this.ErrorDescription = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
			byte[] array = new byte[8];
			ItemId.SafeRead(fs, array, 0, 8, sourceId);
			this.LastAttemptTime = new DateTime(BitConverter.ToInt64(array, 0), DateTimeKind.Utc);
			this.AdditionalInformation = ItemId.ReadString(fs, sourceId, Encoding.Unicode);
		}
	}
}
