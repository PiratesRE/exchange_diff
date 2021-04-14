using System;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class RecurrenceFormatException : CorruptDataException
	{
		internal RecurrenceFormatException(LocalizedString message, Stream stream) : this(message, stream, null)
		{
		}

		internal RecurrenceFormatException(LocalizedString message, Stream stream, Exception innerException) : base(message, innerException)
		{
			this.position = (int)stream.Position;
			MemoryStream memoryStream = stream as MemoryStream;
			if (memoryStream != null)
			{
				this.blob = memoryStream.ToArray();
				return;
			}
			if (stream.CanSeek)
			{
				stream.Seek(0L, SeekOrigin.Begin);
				this.blob = new byte[stream.Length];
				stream.Read(this.blob, 0, this.blob.Length);
				stream.Seek((long)this.position, SeekOrigin.Begin);
			}
		}

		protected RecurrenceFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.blob = (byte[])info.GetValue("blob", typeof(byte[]));
			this.position = (int)info.GetValue("position", typeof(int));
		}

		public byte[] Blob
		{
			get
			{
				return this.blob;
			}
		}

		public int Position
		{
			get
			{
				return this.position;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("blob", this.blob);
			info.AddValue("position", this.position);
		}

		private const string BlobLabel = "blob";

		private const string PositionLabel = "position";

		private byte[] blob;

		private int position;
	}
}
